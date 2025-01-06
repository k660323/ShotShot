using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using ExitGames.Client.Photon;

public class NetworkManager : MonoBehaviourPunCallbacks,IPunObservable
{
    [SerializeField]
    PhotonView pv;
    [SerializeField]
    InputField inputText;
    [SerializeField]
    GameObject matchMakingBg;
    [SerializeField]
    GameObject modeSelect;
    [SerializeField]
    GameObject error;
    [SerializeField]
    MapManager mapManager;
    [SerializeField]
    Text manualText;
    [SerializeField]
    GameObject COJObject;
    [SerializeField]
    GameObject optionGObject;

    string state;

    float timer;
    
    string mode;
    bool isReadyLoad;
    public string curGameVersion;
    #region 커스텀 타입 
    public static readonly byte[] memColor = new byte[4 * 4];
    private static short SerializeColor(StreamBuffer outStream, object customobject)
    {
        Color co = (Color)customobject;
        lock (memColor)
        {
            byte[] bytes = memColor;
            int index = 0;
            Protocol.Serialize(co.r, bytes, ref index);
            Protocol.Serialize(co.g, bytes, ref index);
            Protocol.Serialize(co.b, bytes, ref index);
            Protocol.Serialize(co.a, bytes, ref index);
            outStream.Write(bytes, 0, 4 * 4);
        }

        return 4 * 4;
    }


    private static object DeserializeColor(StreamBuffer inStream, short length)
    {
        Color co = new Color();
        lock (memColor)
        {
            inStream.Read(memColor, 0, 4 * 4);
            int index = 0;
            Protocol.Deserialize(out co.r, memColor, ref index);
            Protocol.Deserialize(out co.g, memColor, ref index);
            Protocol.Deserialize(out co.b, memColor, ref index);
            Protocol.Deserialize(out co.a, memColor, ref index);
        }

        return co;
    }
    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(timer);
            stream.SendNext(modeSelect.transform.GetChild(1).GetChild(0).GetComponent<Toggle>().isOn);
            stream.SendNext(modeSelect.transform.GetChild(1).GetChild(1).GetComponent<Toggle>().isOn);
            stream.SendNext(modeSelect.transform.GetChild(1).GetChild(2).GetComponent<Toggle>().isOn);
            stream.SendNext(modeSelect.transform.GetChild(1).GetChild(3).GetComponent<Toggle>().isOn);
        }
        else
        {          
            timer = (float)stream.ReceiveNext();
            modeSelect.transform.GetChild(1).GetChild(0).GetComponent<Toggle>().isOn = (bool)stream.ReceiveNext();
            modeSelect.transform.GetChild(1).GetChild(1).GetComponent<Toggle>().isOn = (bool)stream.ReceiveNext();
            modeSelect.transform.GetChild(1).GetChild(2).GetComponent<Toggle>().isOn = (bool)stream.ReceiveNext();
            modeSelect.transform.GetChild(1).GetChild(3).GetComponent<Toggle>().isOn = (bool)stream.ReceiveNext();
        }
    }

   
    //public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    //{
    //    object propsTime;
    //    if (propertiesThatChanged.TryGetValue("StartTime", out propsTime))
    //    {
    //        startTime = (double)propsTime;
    //    }
    //}

    //if (PhotonNetwork.IsMasterClient)
    //{
    //    Hashtable ht = new Hashtable { { "StartTime", PhotonNetwork.Time } };
    //    PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
    //     값바꾸면 무조건 set해야 OnRoomPropertiesUpdate 가 호출되서 다른 클라한테도 적용된다.
    //}
    // PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "ID", PhotonNetwork.CurrentRoom.PlayerCount - 1 } });
    // playerCP = PhotonNetwork.LocalPlayer.CustomProperties;

    // Start is called before the first frame update
    
    void Awake()
    {
        PhotonPeer.RegisterType(typeof(Color), 0, SerializeColor, DeserializeColor);
        PhotonNetwork.GameVersion = curGameVersion;
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 30;
        PhotonNetwork.AutomaticallySyncScene = true;
        // 프레임
        Application.targetFrameRate = 60;
        
        if(PlayerPrefs.HasKey("name"))
        {
            inputText.text = PlayerPrefs.GetString("name");
        }
    }

    void Start()
    {
        Cursor.visible = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            optionGObject.SetActive(!optionGObject.activeSelf);
            optionGObject.transform.GetChild(1).gameObject.SetActive(false);
            optionGObject.transform.GetChild(2).gameObject.SetActive(false);
        }

        if(state == "ModeSelect" && timer > 0)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                timer -= Time.deltaTime;
            }
            modeSelect.transform.GetChild(3).GetComponent<Text>().text = ((int)timer).ToString();
        }
        else if(state == "ModeSelect" && timer <= 0 && !isReadyLoad)
        {
            if (PhotonNetwork.IsMasterClient)
                LoadMap();
        }
    }


    #region 연결
    public void Connect()
    {
        if (inputText.text == null || inputText.text == "")
        {
            error.SetActive(true);
            return;
        }
        PlayerPrefs.SetString("name", inputText.text);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.LocalPlayer.NickName = inputText.text;
        COJObject.SetActive(true);
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 8 }, null);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 8 }, null);
    }

    public override void OnJoinedRoom()
    {
        Init();                     
    }

    void Init()
    {
        pv.RPC("SyncPeople", RpcTarget.AllBufferedViaServer);
        if (PhotonNetwork.IsMasterClient)
        {
            matchMakingBg.transform.GetChild(0).gameObject.GetComponent<Text>().text = "방장";
            matchMakingBg.transform.GetChild(3).GetChild(0).gameObject.SetActive(true);

            for (int i = 0; i < modeSelect.transform.GetChild(1).childCount; i++)
            {
                modeSelect.transform.GetChild(1).GetChild(i).GetComponent<Toggle>().interactable = true;
            }
            modeSelect.transform.GetChild(1).GetChild(0).GetComponent<Toggle>().isOn = true;

            modeSelect.transform.GetChild(2).GetComponent<Button>().interactable = true;
        }
        else
        {
            matchMakingBg.transform.GetChild(0).gameObject.GetComponent<Text>().text = "유저";
            matchMakingBg.transform.GetChild(3).GetChild(0).gameObject.SetActive(false);

            for (int i = 0; i < modeSelect.transform.GetChild(1).childCount; i++)
            {
                modeSelect.transform.GetChild(1).GetChild(i).GetComponent<Toggle>().interactable = false;
            }
            modeSelect.transform.GetChild(1).GetChild(0).GetComponent<Toggle>().isOn = true;
            modeSelect.transform.GetChild(2).GetComponent<Button>().interactable = false;
        }
        state = "MatchMaking";      
        matchMakingBg.SetActive(true);
    }
    #endregion

    #region 방장변경
    public override void OnMasterClientSwitched(Player newMasterClient) // 모든 유저에게 호출
    {
        if (PhotonNetwork.InRoom)
            SetMaster();
    }

    void SetMaster()
    {
        switch (state)
        {
            case "MatchMaking":
                if (PhotonNetwork.IsMasterClient)
                {
                    matchMakingBg.transform.GetChild(0).gameObject.GetComponent<Text>().text = "방장";
                    matchMakingBg.transform.GetChild(3).GetChild(0).gameObject.SetActive(true);

                    for (int i = 0; i < modeSelect.transform.GetChild(1).childCount; i++)
                    {
                        modeSelect.transform.GetChild(1).GetChild(i).GetComponent<Toggle>().interactable = true;
                    }
                    modeSelect.transform.GetChild(1).GetChild(0).GetComponent<Toggle>().isOn = true;
                    modeSelect.transform.GetChild(2).GetComponent<Button>().interactable = true;
                }
                break;
            case "ModeSelect":
                if (PhotonNetwork.IsMasterClient)
                {
                    for (int i = 0; i < modeSelect.transform.GetChild(1).childCount; i++)
                    {
                        modeSelect.transform.GetChild(1).GetChild(i).GetComponent<Toggle>().interactable = true;
                    }
                    modeSelect.transform.GetChild(1).GetChild(0).GetComponent<Toggle>().isOn = true;
                    modeSelect.transform.GetChild(2).GetComponent<Button>().interactable = true;
                }
                break;
            default:
                break;
        }    
    }
    #endregion

    #region 모드 선택
    public void StartGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        pv.RPC("ShowModeWindow", RpcTarget.AllBuffered);     
    }

    [PunRPC]
    void ShowModeWindow()
    {
        if (PhotonNetwork.IsMasterClient)
            timer = 60;
       
        state = "ModeSelect";
        manualText.text = "믿을 사람은 오직 나 \n최후의 1인이 되자!";
        matchMakingBg.gameObject.SetActive(false);
        modeSelect.gameObject.SetActive(true);

    }

    public void ModeManual(int i)
    {
        switch (i)
        {
            case 0: // 데스매치
                manualText.text = "믿을 사람은 오직 나 \n최후의 1인이 되자!";
                break;
            case 1: // 팀 데스매치
                manualText.text = "팀과 협동하여 \n승자가 되자!!";
                break;
            case 2: // 디펜스
                manualText.text = "끝없이 몰려오는 \n적들을 섬멸하라!";
                break;
            case 3: // 레이드
                manualText.text = "강력한 보스에 \n도전하라!";
                break;
        }
    }
    #endregion

    #region 맵 로드
    public void LoadMap()
    {
        isReadyLoad = true;
        for (int i=0; i < modeSelect.transform.GetChild(1).childCount;i++)
        {
            if (modeSelect.transform.GetChild(1).GetChild(i).GetComponent<Toggle>().isOn)
            {
                switch (i)
                {
                    case 0: // 데스매치
                        mode = "데스매치";                     
                        break;
                    case 1: // 팀 데스매치
                        mode = "팀 데스매치";
                        break;
                    case 2: // 디펜스
                        mode = "디펜스";
                        break;
                    case 3: // 레이드
                        mode = "레이드";
                        break;
                }
                break;
            }         
        }
        mapManager._mapList.TryGetValue(mode, out List<map> mList);
        pv.RPC("syncMap", RpcTarget.AllBuffered, mode, Random.Range(0, mList.Count));

        
    }

    [PunRPC]
    void syncMap(string modeName,int n)
    {
        SyncPeople();
        mapManager._mapList.TryGetValue(modeName, out List<map> mList);
        SessionInfo.Instance.mode = modeName;
        SessionInfo.Instance.nowScene = mList[n].sceneName;
        SessionInfo.Instance.mapName = mList[n].mapName;
        SessionInfo.Instance.mapImage = mList[n].mapImage;
        if (SessionInfo.Instance.mode == "팀 데스매치")
            SessionInfo.Instance.isTeamMode = true;
        else
            SessionInfo.Instance.isTeamMode = false;
        SessionInfo.Instance.SetTeam();
        NextSceneManager.SceneLoad();
    }
    #endregion

    #region 네트워크 연결 종료 및 나가기
    public void Disconnted()
    {
        PhotonNetwork.Disconnect();
    }
    
    public void MatchingCancel()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            PhotonNetwork.SetMasterClient(PhotonNetwork.MasterClient.GetNext());
        }
       
        
        PhotonNetwork.LeaveRoom();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        SyncPeople();
    }

    [PunRPC]
    void SyncPeople()
    {
        matchMakingBg.transform.GetChild(1).GetComponent<Text>().text = "현재 인원 : " + PhotonNetwork.CurrentRoom.PlayerCount + "명";
    }

    public override void OnLeftRoom() // 방을 떠나는 유저만 호출
    {
        matchMakingBg.SetActive(false);
    }
    #endregion

    #region 옵션창 활성화
    public void GraphicSetting()
    {
        OptionManager.Instance.gameObject.SetActive(!OptionManager.Instance.gameObject.activeSelf);
    }

    public void AudioSetting()
    {
        OptionManager.Instance.gameObject.SetActive(!OptionManager.Instance.gameObject.activeSelf);
    }

    public void ExitApplication()
    {
        Application.Quit();
    }

    #endregion
}
