using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using DG.Tweening;
using static Defined;
using UnityEngine.Events;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks,IPunObservable
{
    public GMState state;

    public PhotonView PV;

    [SerializeField]
    AudioSource audioSource;
    [SerializeField]
    AudioClip[] NotcieClips;

    // 현재 방에 있는 플레이어
    [SerializeField]
    Text CurPlayerText;

    // 캔버스 그룹
    [SerializeField]
    GameObject choiceObject;
    public GameObject playerInfoObject;
    public  GameObject GameSetObject;
    [SerializeField]
    GameObject optionGObject;
    // 캐릭터 선택창
    [SerializeField]
    CharacterSelection _cSelection;
    [SerializeField]
    Text choiceTimerText;
    bool isReady;

    bool[] readyArray;
    bool[] mapLoadOkay;

    // 게임 시작
    public Text gTimerText;

    // 타이머
    public float choiceTimer;
    public float gameTimer = -1;

    // 알림 텍스트
    [SerializeField]
    Text noticeText;
    bool isPending;
    Queue<string> textQueue = new Queue<string>();
    object _lock = new object();

    // 리스폰
    public SpawnManager spawnManager;

    // 킬
    public KillLogMananger killLogManager;
    public Text targetKillText;
    // 모드
    public Text gameModeText;

    // 스킬 코맨트
    [SerializeField]
    GameObject commentObject;
    [SerializeField]
    Image CImage;
    [SerializeField]
    Text CName;
    [SerializeField]
    Text CLevelText;
    [SerializeField]
    Text CTypeText;
    [SerializeField]
    Text CText;

    // 스킬 아이콘 쿨
    [SerializeField]
    Image[] skillImage;
    [SerializeField]
    Image[] skillCoolTime;
    [SerializeField]
    Text[] skillCoolText;
    [SerializeField]
    Text[] skillCommentText;
    [SerializeField]
    GameObject[] skillActive;
    [SerializeField]
    GameObject[] skillLock;

    // 무기 아이콘 탄창
    [SerializeField]
    Image weaponImage;
    [SerializeField]
    Image weaponCImage;
    [SerializeField]
    Text AmmoText;

    // 경험치
    [SerializeField]
    Image EXPBar;
    [SerializeField]
    Text EXPText;
    [SerializeField]
    Text LVText;

    public int startPlayerCount;

    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            Init();
            return instance;
        }
    }

    MapInfoManager _mapInfo = new MapInfoManager();
    public static MapInfoManager MapInfo { get { return Instance._mapInfo; } }

    public UnityAction endEvent;
    public UnityAction<Player> leaveRoomEvent;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(choiceTimer);
            stream.SendNext(gameTimer);
            stream.SendNext(state);
            stream.SendNext(readyArray);
            stream.SendNext(killLogManager.targetKill);
        }
        else
        {
            choiceTimer = (float)stream.ReceiveNext();
            gameTimer = (float)stream.ReceiveNext();
            state = (GMState)stream.ReceiveNext();
            readyArray = (bool[])stream.ReceiveNext();
            killLogManager.targetKill = (int)stream.ReceiveNext();
        }
    }

    static void Init()
    {
        if (instance == null)
        {
            var obj = FindObjectOfType<GameManager>();
            if (obj != null)
            {
                instance = obj;
            }
            else
            {
                var newObj = new GameObject().AddComponent<GameManager>();
                instance = newObj;
            }
        }
    }

    void Awake()
    {
        Init();
        if (this != instance)
        {
            Destroy(gameObject);
            return;
        }
        leaveRoomEvent -= LeaveRoom;
        leaveRoomEvent += LeaveRoom;
        startPlayerCount = SessionInfo.Instance.RoomPlayer;
        CurPlayerText.text = "접속자 : " + startPlayerCount + "명";

        if (PhotonNetwork.IsMasterClient)
        {
            state = GMState.Default;
        }
        readyArray = new bool[startPlayerCount];
        mapLoadOkay = new bool[startPlayerCount];

        gameModeText.text = SessionInfo.Instance.mode;

        MapInfo.LoadMap(SessionInfo.Instance.mapName);
    }

    void Start()
    {
        StartCoroutine(mapLoadCheck());
    }

    IEnumerator mapLoadCheck()
    {
        while (FindObjectOfType<Grid>() == null)
        {
            yield return null;
        }
        MapInfo.go = FindObjectOfType<Grid>().gameObject;
        MapInfo.go.name = MapInfo.mapName;
        MapInfo.CurrentGrid = MapInfo.go.GetComponent<Grid>();
        if (spawnManager == null)
            spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        PV.RPC("MapLoadOkay", RpcTarget.MasterClient, spawnManager.myNum);
    }

    // 마스터 클라이언트 전용
    [PunRPC]
    void MapLoadOkay(short id)
    {
        short loadIndex = 0;
        mapLoadOkay[id] = true;
        for (short i = 0; i < readyArray.Length; i++)
        {
            if (mapLoadOkay[i] == true)
                loadIndex++;
        }

        if (loadIndex == SessionInfo.Instance.RoomPlayer)
        {
            state = GMState.Choice;
        }
    }

    void Update()
    {
        if (state == GMState.Choice)
        {
            if (PhotonNetwork.IsMasterClient)
                choiceTimer -= Time.deltaTime;
            choiceTimerText.text = ((int)choiceTimer).ToString();
            
            if (choiceTimer <= 0 && !isReady)
                Choice();
        }

        if (state == GMState.GameStart)
        {
            endEvent?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            optionGObject.SetActive(!optionGObject.activeSelf);
            optionGObject.transform.GetChild(1).gameObject.SetActive(false);
            optionGObject.transform.GetChild(2).gameObject.SetActive(false);
            MoveLock();
        }
    }

    private void LateUpdate()
    {
        if (CharacterScript.Instance != null)
        {
            // 총알 동기화
            AmmoText.text = CharacterScript.Instance.Gun.curAmmo + "/" + CharacterScript.Instance.Gun.maxAmmo;

            //경험치바
            EXPBar.fillAmount = (CharacterScript.Instance.curEXP) == 0 ? 0f : CharacterScript.Instance.curEXP / CharacterScript.Instance.maxEXP;
            //경험치 텍스트
            EXPText.text = CharacterScript.Instance.curEXP.ToString() + "/" + CharacterScript.Instance.maxEXP.ToString();
            // 레벨
            LVText.text = "LV " + CharacterScript.Instance.level.ToString();
        }
        targetKillText.text = "목표 킬수 : " + killLogManager.targetKill;
    }

    public void MoveLock()
    {
        if (CharacterScript.Instance != null)
            CharacterScript.Instance.isControll = !optionGObject.activeSelf;
    }

    #region 캐릭터 선택 / 생성 / 게임 시작
    public void Choice()
    {
        isReady = true;
        PV.RPC("Readyup", RpcTarget.MasterClient, spawnManager.myNum, isReady);
    }

    // 방장이 취합
    [PunRPC]
    public void Readyup(short id,bool _isReady)
    {
        int readyIndex = 0;
        readyArray[id] = _isReady;
        for(int i=0; i < readyArray.Length; i++)
        {
            if(readyArray[i] == true)
                readyIndex++;
        }

        if(readyIndex == SessionInfo.Instance.RoomPlayer)
           PV.RPC("SpawnPlayer",RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    public void SpawnPlayer()
    {
        Camera.main.GetComponent<AudioListener>().enabled = false;
        choiceObject.SetActive(false);
        playerInfoObject.SetActive(true);
     
        PhotonNetwork.Instantiate($"Prefabs/Player/{CharacterInfoManager.Instance._charactorInfoList[_cSelection.index].name}", spawnManager.getSP(spawnManager.spawnArray[spawnManager.myNum]).position, Quaternion.identity);
        
        // 스킬 이미지 캔버스에 등록
        SkillSlotSetting();

        // 무기 Image 등록
        WeaponSlotSetting();

        StartGame();
    }

    void StartGame()
    {
        killLogManager.KillInit();
        MapEvent.Instance.SetEvent();
        BGMManager.Instance.BgSoundPlay(BGMManager.Instance.bgList[4], true);
        state = GMState.GameStart;
    }
    #endregion

    #region 킬
    [PunRPC]
    public void Kill(float XP)
    {
        killLogManager.KillUP();
        noticeSend("적 처리!");
        CharacterScript.Instance.killFunction();
        if (CharacterScript.Instance.level < CharacterScript.Instance.maxLevel)
            StartCoroutine(CharacterScript.Instance.smoothUP(XP));
        StartCoroutine(noticeSound(0));
    }
    #endregion

    public IEnumerator noticeSound(int i)
    {
        while (true)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = NotcieClips[i];
                audioSource.Play();
                yield break;
            }
            yield return null;
        }
    }

    #region 공지 등록/알림
    public void LevelNotice()
    {
        noticeSend("레벨 UP!");


        for (int i = 0; i < skillLock.Length; i++)
        {
            SkillScript skillScript = CharacterScript.Instance.skillGroup.transform.GetChild(i).GetComponent<SkillScript>();
            if (skillLock[i].activeInHierarchy)
            {
                if(CharacterScript.Instance.level >= skillScript.skillUsedLevel)
                {
                    skillLock[i].SetActive(false); // 스킬락
                    noticeSend("스킬이 해금되었습니다");
                }         
            }
        }
    }

    public void noticeSend(string text)
    {
        lock (_lock) // 들어올대 락 건다
        {
            textQueue.Enqueue(text);
            if (!isPending)  // 내보내기 중인지 확인
                RegisterNotice();
        }
    }

    void RegisterNotice()
    {
        isPending = true; // 내보내기중
        string text = textQueue.Dequeue();
        StartCoroutine(notice(text)); // 쓰레드 생성시 후 락 푼다
    }

    IEnumerator notice(string text)
    {
        noticeText.text = text;
        yield return new WaitForSeconds(2f);
        noticeText.text = "";
        yield return new WaitForSeconds(0.5f);
        if (textQueue.Count != 0)
            RegisterNotice(); // 다시 내보낼 text가 있을경우
        else
            isPending = false;
    }
    #endregion

    #region 스킬 코맨트
    public void visbleSkillComment(int i)
    {        
        SkillScript skillScript = CharacterScript.Instance.skillGroup.transform.GetChild(i).GetComponent<SkillScript>();
        CImage.sprite = skillScript.abilityImage;
        CName.text = skillScript.abilityName;

        CLevelText.text = "스킬레벨\nLV " + skillScript.skillUsedLevel;
        CTypeText.text = skillScript.Type + "\n" + "단축키 : " + skillScript.shortcutkeys;

        CText.text = skillScript.abilityText;

        commentObject.SetActive(true);
    }

    public void unVisbleSkillComment()
    {      
        commentObject.SetActive(false);
    }
    #endregion

    public void SkillSlotSetting()
    {
        for (int i = 0; i < skillLock.Length; i++)
        {
            SkillScript skillScript = CharacterScript.Instance.skillGroup.transform.GetChild(i).GetComponent<SkillScript>();
            skillImage[i].sprite = skillScript.abilityImage; // 스킬 이미지
            skillScript.coolTime = skillCoolTime[i]; // 스킬쿨 이미지
            skillScript.coolText = skillCoolText[i]; // 스킬쿨 텍스트
            skillCommentText[i].text = skillScript.shortcutkeys; // 스킬 단축키
            skillScript.skillActive = skillActive[i]; // 스킬 활성화 이미지
            skillLock[i].SetActive(!(CharacterScript.Instance.level >= skillScript.skillUsedLevel)); // 스킬락
        }
    }

    public void WeaponSlotSetting()
    {
        weaponImage.sprite = CharacterScript.Instance.Gun.GetComponentInChildren<SpriteRenderer>().sprite;
        CharacterScript.Instance.Gun.coolTime = weaponCImage;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) // 나간놈 빼고 다 호출
    {
        leaveRoomEvent.Invoke(otherPlayer);
      
    }

    void LeaveRoom(Player otherPlayer)
    {
        CurPlayerText.text = "접속자 : " + PhotonNetwork.CurrentRoom.PlayerCount.ToString() + "명";

        if (Instance.state == GMState.Choice)
        {
            for (short i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (PhotonNetwork.PlayerList[i] == otherPlayer)
                {
                    Instance.Readyup(i, false);
                    break;
                }
            }
        }
    }

    [PunRPC]
    public void ShowGameResult(string[] maxKillInfo) // 이긴팀,정보
    {
        CharacterScript.Instance.isControll = false;
        playerInfoObject.gameObject.SetActive(false);
        GameSetObject.gameObject.SetActive(true);

        List<Player> list = PhotonNetwork.PlayerList.OrderByDescending(p => p.CustomProperties["Kill"].ToString()).ToList();
        short maxKill = short.Parse(list[0].CustomProperties["Kill"].ToString());

        if (SessionInfo.Instance.isTeamMode)
        {
            if (SessionInfo.Instance.myTeam == short.Parse(maxKillInfo[1]))
            {
                BGMManager.Instance.BgSoundPlay(BGMManager.Instance.bgList[0], false);
                GameSetObject.transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                BGMManager.Instance.BgSoundPlay(BGMManager.Instance.bgList[1], false);
                GameSetObject.transform.GetChild(1).gameObject.SetActive(true);
            }
        }
        else
        {
            if (short.Parse(PhotonNetwork.LocalPlayer.CustomProperties["Kill"].ToString()) == maxKill)
            {
                BGMManager.Instance.BgSoundPlay(BGMManager.Instance.bgList[0], false);
                GameSetObject.transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                BGMManager.Instance.BgSoundPlay(BGMManager.Instance.bgList[1], false);
                GameSetObject.transform.GetChild(1).gameObject.SetActive(true);
            }
        }
    }

    public void LevelUPBtn()
    {
        CMManager.Instance().CameraShake(1f, 1f, 5f);
        StartCoroutine(CharacterScript.Instance.smoothUP(CharacterScript.Instance.maxEXP - CharacterScript.Instance.curEXP));
    }

    public void GameExit()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene("LobbyScene");
    }

}
