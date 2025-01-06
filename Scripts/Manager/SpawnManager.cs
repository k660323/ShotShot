using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    public bool isTeamSpawn;
    // 리스폰
    [SerializeField]
    PhotonView PV;
    [SerializeField]
    Transform[] spawnPos;
    [SerializeField]
    Image GaugeBar;
    [SerializeField]
    Text RespawnText;
    public GameObject respawnPanel;
    bool isSpawn;
    float respawnTime;
    short Ran;
    public short[] registerArray;
    public short[] spawnArray;
    object _lock2 = new object();
    bool _pending = false;
    public short myNum;

    void Start()
    {
        GameManager.Instance.leaveRoomEvent -= RegisterCancle;
        GameManager.Instance.leaveRoomEvent += RegisterCancle;
        respawnPanel = GameObject.Find("Canvas").transform.Find("PlayerInfoGroup").Find("RespawnPanel").gameObject;
        RespawnText = respawnPanel.transform.Find("SpawnText").GetComponent<Text>();
        GaugeBar = respawnPanel.transform.Find("GaugeBar").GetComponent<Image>();

        myNum = SessionInfo.Instance.registerIndex;

        PV.RPC("SetRegister", RpcTarget.AllBuffered, myNum);
    }

    void Update()
    {
        if (isSpawn)
        {
            respawnTime -= Time.deltaTime;
            RespawnText.text = ((int)respawnTime).ToString() + "초 뒤 다시 리스폰합니다.";
            GaugeBar.fillAmount += Time.deltaTime / 10;
            if (GaugeBar.fillAmount >= 1)
            {
                isSpawn = false;
                respawnPanel.SetActive(false);
                //TODO
                //여기서 리스폰 할 장소 받고 생성
                GaugeBar.fillAmount = 0f;
                if (spawnArray[myNum] == -1)
                {
                    CharacterScript.Instance.transform.position = getSP(myNum).position;
                }
                else
                {
                    CharacterScript.Instance.transform.position = getSP(spawnArray[myNum]).position;
                }
                PV.RPC("resetSpawn", RpcTarget.AllBuffered, myNum);
                CharacterScript.Instance.Init();            
            }
        }
    }

    #region 리스폰
    public void RespawnPanel(short cp)
    {
        PV.RPC("SetRegister", RpcTarget.AllBuffered, cp);
        respawnPanel.SetActive(true);
        respawnTime = 10;
        isSpawn = true;
    }

    [PunRPC]
    void SetRegister(short index)
    {
        registerArray[index] = index;
        if (PhotonNetwork.IsMasterClient)
            RespawnSetting();
    }
    void RespawnSetting()
    {
        lock (_lock2)
        {
            if (!_pending)
            {
                _pending = true;
                StartCoroutine("RespawnCounter");
            }
        }
    }

    IEnumerator RespawnCounter()
    {
        for (short i = 0; i < registerArray.Length; i++)
        {
            if (registerArray[i] == i)
            {
                if (spawnArray[i] == -1)
                {
                    First:
                    Ran = RandomSpawnIndex(i);
                    for (short j = 0; j < registerArray.Length; j++)
                    {
                        if (spawnArray[j] == Ran)
                        {
                            goto First;
                        }
                    }
                    spawnArray[i] = Ran;
                    short index = i;
                    short number = Ran;
                    PV.RPC("SetSpawn", RpcTarget.OthersBuffered, index, number);
                }
            }
        }
        for (short i = 0; i < registerArray.Length; i++)
        {
            if (registerArray[i] == i && spawnArray[i] == -1)
            {
                RespawnSetting();
                break;
            }
            if (i == registerArray.Length - 1)
            {
                _pending = false;
                break;
            }
        }
        yield return null;
    }

    short RandomSpawnIndex(short _index)
    {
        if (isTeamSpawn)
        {
            Hashtable ht = PhotonNetwork.PlayerList[_index].CustomProperties;
            short team = (short)ht["team"]; 
            if (team % 2 == 0) // 짝수 팀
            {
                return (short)(Random.Range(0, 4) * 2); // 0~7 스폰 0 2 4 6 
            }
            else // 홀수 팀
            {
                return (short)(Random.Range(0, 4) * 2 + 1); // 0~7 스폰 /1 3 5 7
            }
        }
        else // 팀 모드 아님
        {
            return (short)Random.Range(0, 8);
        }
    }

    [PunRPC]
    void SetSpawn(short index, short num)
    {
        spawnArray[index] = num;
    }

    [PunRPC]
    void resetSpawn(short index)
    {
        spawnArray[index] = -1;
        registerArray[index] = -1;
    }
    #endregion

    public Transform getSP(short spawnIndex)
    {
        PV.RPC("resetSpawn", RpcTarget.AllBuffered, myNum);
        return spawnPos[spawnIndex];
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (GameManager.Instance.state != Defined.GMState.GameEnd)
            return;

        if (PhotonNetwork.IsMasterClient)
            RespawnSetting();
    }

    public void RegisterCancle(Player otherPlayer)
    {
        if (GameManager.Instance.state != Defined.GMState.GameEnd)
            return;

        for (short i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList[i] == otherPlayer)
            {
                registerArray[i] = -1;
                spawnArray[i] = -1;
                break;
            }
        }
    }
}
