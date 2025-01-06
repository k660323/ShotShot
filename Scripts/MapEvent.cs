using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Defined;

public class MapEvent : MonoBehaviour
{
    public gameMode mode;

    static MapEvent instance;
    public static MapEvent Instance
    {
        get
        {
            Init();
            return instance;
        }
    }

    static void Init()
    {
        if (instance == null)
        {
            var obj = FindObjectOfType<MapEvent>();
            if (obj == null)
            {
                Debug.LogError("맵 이벤트 설정되어 있지 않습니다.");
            }
            else
            {
                instance = obj;
            }
        }
    }

    int min;
    int sec;

    void Start()
    {
        Init();
        if (this != instance)
            Destroy(gameObject);
    }

    public void SetEvent()
    {
        switch (mode)
        {
            case gameMode.DeathMatch:
                if(PhotonNetwork.IsMasterClient)
                {
                    float playerPercent = SessionInfo.Instance.RoomPlayer / SessionInfo.Instance.MaxPlayer;
                    SetTimer(playerPercent);
                    GameManager.Instance.killLogManager.SetTargetKill();
                } 
               
                GameManager.Instance.endEvent -= DeathMatchEndEvent;
                GameManager.Instance.endEvent += DeathMatchEndEvent;
                break;
            case gameMode.Siege:

                break;
            case gameMode.BattleRoyal:

                break;
            case gameMode.Occupation:

                break;
            case gameMode.Defense:

                break;

            case gameMode.Raid:

                break;
            default:

                break;
        }

        GameManager.Instance.endEvent -= TimerEvent;
        GameManager.Instance.endEvent += TimerEvent;
    }

    void SetTimer(float percent)
    {
        if (percent == 1f) // 8명
            GameManager.Instance.gameTimer = 900f; // 15 분
        else if (percent >= 0.8f) // 6명 이상
            GameManager.Instance.gameTimer = 900f; // 15 분
        else if (percent >= 0.5f) // 4명
            GameManager.Instance.gameTimer = 720f; // 12 분
        else
            GameManager.Instance.gameTimer = 600f; // 10 분
    }

    void DeathMatchEndEvent()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (GameManager.Instance.gameTimer == 0)
        {
            GameManager.Instance.state = GMState.GameEnd;
            GameManager.Instance.killLogManager.GetComponent<PhotonView>().RPC("LogUPdate", RpcTarget.AllBufferedViaServer);

            GameManager.Instance.PV.RPC("ShowGameResult", RpcTarget.AllBufferedViaServer); 
        }
        else if (GameManager.Instance.killLogManager.targetKill <= GameManager.Instance.killLogManager.userKillCount ||
            GameManager.Instance.killLogManager.targetKill <= GameManager.Instance.killLogManager.Team1KillCount ||
            GameManager.Instance.killLogManager.targetKill <= GameManager.Instance.killLogManager.Team2KillCount)
        {
            GameManager.Instance.state = GMState.GameEnd;
            GameManager.Instance.killLogManager.GetComponent<PhotonView>().RPC("LogUPdate", RpcTarget.AllBufferedViaServer);

            GameManager.Instance.PV.RPC("ShowGameResult", RpcTarget.AllBufferedViaServer);
        }
    }
    void TimerEvent()
    {
        if (GameManager.Instance.gameTimer > 0)
        {
            if (PhotonNetwork.IsMasterClient)
                GameManager.Instance.gameTimer -= Time.deltaTime;
            min = (int)(GameManager.Instance.gameTimer / 60f);
            sec = (int)(GameManager.Instance.gameTimer % 60f);
            GameManager.Instance.gTimerText.text = min + ":" + (sec < 10 ? "0" + sec : sec.ToString());
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
                GameManager.Instance.gameTimer = 0f;
        }
    }
}
