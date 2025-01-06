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

    // ���� �濡 �ִ� �÷��̾�
    [SerializeField]
    Text CurPlayerText;

    // ĵ���� �׷�
    [SerializeField]
    GameObject choiceObject;
    public GameObject playerInfoObject;
    public  GameObject GameSetObject;
    [SerializeField]
    GameObject optionGObject;
    // ĳ���� ����â
    [SerializeField]
    CharacterSelection _cSelection;
    [SerializeField]
    Text choiceTimerText;
    bool isReady;

    bool[] readyArray;
    bool[] mapLoadOkay;

    // ���� ����
    public Text gTimerText;

    // Ÿ�̸�
    public float choiceTimer;
    public float gameTimer = -1;

    // �˸� �ؽ�Ʈ
    [SerializeField]
    Text noticeText;
    bool isPending;
    Queue<string> textQueue = new Queue<string>();
    object _lock = new object();

    // ������
    public SpawnManager spawnManager;

    // ų
    public KillLogMananger killLogManager;
    public Text targetKillText;
    // ���
    public Text gameModeText;

    // ��ų �ڸ�Ʈ
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

    // ��ų ������ ��
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

    // ���� ������ źâ
    [SerializeField]
    Image weaponImage;
    [SerializeField]
    Image weaponCImage;
    [SerializeField]
    Text AmmoText;

    // ����ġ
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
        CurPlayerText.text = "������ : " + startPlayerCount + "��";

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

    // ������ Ŭ���̾�Ʈ ����
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
            // �Ѿ� ����ȭ
            AmmoText.text = CharacterScript.Instance.Gun.curAmmo + "/" + CharacterScript.Instance.Gun.maxAmmo;

            //����ġ��
            EXPBar.fillAmount = (CharacterScript.Instance.curEXP) == 0 ? 0f : CharacterScript.Instance.curEXP / CharacterScript.Instance.maxEXP;
            //����ġ �ؽ�Ʈ
            EXPText.text = CharacterScript.Instance.curEXP.ToString() + "/" + CharacterScript.Instance.maxEXP.ToString();
            // ����
            LVText.text = "LV " + CharacterScript.Instance.level.ToString();
        }
        targetKillText.text = "��ǥ ų�� : " + killLogManager.targetKill;
    }

    public void MoveLock()
    {
        if (CharacterScript.Instance != null)
            CharacterScript.Instance.isControll = !optionGObject.activeSelf;
    }

    #region ĳ���� ���� / ���� / ���� ����
    public void Choice()
    {
        isReady = true;
        PV.RPC("Readyup", RpcTarget.MasterClient, spawnManager.myNum, isReady);
    }

    // ������ ����
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
        
        // ��ų �̹��� ĵ������ ���
        SkillSlotSetting();

        // ���� Image ���
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

    #region ų
    [PunRPC]
    public void Kill(float XP)
    {
        killLogManager.KillUP();
        noticeSend("�� ó��!");
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

    #region ���� ���/�˸�
    public void LevelNotice()
    {
        noticeSend("���� UP!");


        for (int i = 0; i < skillLock.Length; i++)
        {
            SkillScript skillScript = CharacterScript.Instance.skillGroup.transform.GetChild(i).GetComponent<SkillScript>();
            if (skillLock[i].activeInHierarchy)
            {
                if(CharacterScript.Instance.level >= skillScript.skillUsedLevel)
                {
                    skillLock[i].SetActive(false); // ��ų��
                    noticeSend("��ų�� �رݵǾ����ϴ�");
                }         
            }
        }
    }

    public void noticeSend(string text)
    {
        lock (_lock) // ���ô� �� �Ǵ�
        {
            textQueue.Enqueue(text);
            if (!isPending)  // �������� ������ Ȯ��
                RegisterNotice();
        }
    }

    void RegisterNotice()
    {
        isPending = true; // ����������
        string text = textQueue.Dequeue();
        StartCoroutine(notice(text)); // ������ ������ �� �� Ǭ��
    }

    IEnumerator notice(string text)
    {
        noticeText.text = text;
        yield return new WaitForSeconds(2f);
        noticeText.text = "";
        yield return new WaitForSeconds(0.5f);
        if (textQueue.Count != 0)
            RegisterNotice(); // �ٽ� ������ text�� �������
        else
            isPending = false;
    }
    #endregion

    #region ��ų �ڸ�Ʈ
    public void visbleSkillComment(int i)
    {        
        SkillScript skillScript = CharacterScript.Instance.skillGroup.transform.GetChild(i).GetComponent<SkillScript>();
        CImage.sprite = skillScript.abilityImage;
        CName.text = skillScript.abilityName;

        CLevelText.text = "��ų����\nLV " + skillScript.skillUsedLevel;
        CTypeText.text = skillScript.Type + "\n" + "����Ű : " + skillScript.shortcutkeys;

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
            skillImage[i].sprite = skillScript.abilityImage; // ��ų �̹���
            skillScript.coolTime = skillCoolTime[i]; // ��ų�� �̹���
            skillScript.coolText = skillCoolText[i]; // ��ų�� �ؽ�Ʈ
            skillCommentText[i].text = skillScript.shortcutkeys; // ��ų ����Ű
            skillScript.skillActive = skillActive[i]; // ��ų Ȱ��ȭ �̹���
            skillLock[i].SetActive(!(CharacterScript.Instance.level >= skillScript.skillUsedLevel)); // ��ų��
        }
    }

    public void WeaponSlotSetting()
    {
        weaponImage.sprite = CharacterScript.Instance.Gun.GetComponentInChildren<SpriteRenderer>().sprite;
        CharacterScript.Instance.Gun.coolTime = weaponCImage;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) // ������ ���� �� ȣ��
    {
        leaveRoomEvent.Invoke(otherPlayer);
      
    }

    void LeaveRoom(Player otherPlayer)
    {
        CurPlayerText.text = "������ : " + PhotonNetwork.CurrentRoom.PlayerCount.ToString() + "��";

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
    public void ShowGameResult(string[] maxKillInfo) // �̱���,����
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
