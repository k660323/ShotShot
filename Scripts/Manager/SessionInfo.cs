using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class SessionInfo : MonoBehaviourPunCallbacks
{
    static SessionInfo _sInfo;

    public static SessionInfo Instance
    {
        get
        {
            Init();

            return _sInfo;
        }
    }

    static void Init()
    {
        if (_sInfo == null)
        {
            SessionInfo obj = FindObjectOfType<SessionInfo>();
            if (obj == null)
            {
                GameObject go = new GameObject() { name = "SessionInfo" };
                obj = go.AddComponent<SessionInfo>();
            }
            _sInfo = obj;
        }
    }

    public int RoomPlayer { get { return PhotonNetwork.CurrentRoom.PlayerCount; } }
    public int MaxPlayer { get { return PhotonNetwork.CurrentRoom.MaxPlayers; } }

    public short registerIndex;
    public bool isTeamMode;
    public short myTeam;

    public string mode;
    public string nowScene;
    public string mapName;
    public Sprite mapImage;

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) // SetCustomPropertiesÇØ¾ß È£ÃâµÊ
    {
        targetPlayer.CustomProperties["Team"] = (short)changedProps["Team"];
        targetPlayer.CustomProperties["Kill"] = (int)changedProps["Kill"];
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
        if (this != Instance)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    // ¸Ê ·Îµù Àü¿¡ È£Ãâ
    public void SetTeam()
    {
        for (short index = 0; index < RoomPlayer; index++)
        {
            if (PhotonNetwork.PlayerList[index].IsLocal)
            {
                registerIndex = index;
                if (isTeamMode)
                {
                    // È¦¼ö ·¹µå
                    if ((index + 1) % 2 != 0)
                    {
                        myTeam = 1;
                    }
                    // Â¦¼öÆÀ ºí·ç
                    else
                    {
                        myTeam = 2;
                    }
                }
                else
                {
                    myTeam = 0;
                }
                Hashtable ht = PhotonNetwork.LocalPlayer.CustomProperties;
                ht["Team"] = myTeam;
                ht["Kill"] = 0;
                PhotonNetwork.LocalPlayer.SetCustomProperties(ht);
                break;
            }
        }
    }

    public bool TeamCheck(short _myteamNum , short _TargetTeamNum)
    {
        // ÆÀÀü ¿©ºÎ
        if (!SessionInfo.Instance.isTeamMode)
            return false;

        // °°ÀºÆÀ
        if (_myteamNum == _TargetTeamNum)
            return true;
        // ´Ù¸¥ÆÀ
        else
            return false;
    }
}
