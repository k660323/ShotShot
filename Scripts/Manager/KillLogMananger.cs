using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Linq;

public class KillLogMananger : MonoBehaviourPunCallbacks
{
    [SerializeField]
    PhotonView PV;

    [SerializeField]
    Text[] KillBoardText;
    [SerializeField]
    Animator AN;

    public int targetKill;
    public short userKillCount = 0;
    public short Team1KillCount = 0;
    public short Team2KillCount = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            AN.SetBool("expand", !AN.GetBool("expand"));
        }
    }

    public void KillInit()
    {
        short team = 0;
        string teamText = "";
        List<Player> list = PhotonNetwork.PlayerList.ToList();

        for (int i = 0; i < list.Count; i++)
        {
            if (SessionInfo.Instance.isTeamMode)
            {
                team = (short)list[i].CustomProperties["Team"];
                teamText = "[" + team.ToString() + "ÆÀ]";
            }

            if (PhotonNetwork.PlayerList[i].IsLocal)
            {
                KillBoardText[i].color = Color.green;
            }
            else if (SessionInfo.Instance.isTeamMode && CharacterScript.Instance.myTeam == team)
            {
                KillBoardText[i].color = Color.yellow;
            }
            else
            {
                KillBoardText[i].color = new Color(50 / 255, 50 / 255, 50 / 255);
            }

            KillBoardText[i].text = (i + 1) + "." + teamText + list[i].NickName + " : 0 kill\n";
        }
    }

    public void SetTargetKill( )
    {
        float playerPercent = SessionInfo.Instance.RoomPlayer / SessionInfo.Instance.MaxPlayer;
        if (playerPercent == 1f) // 8¸í
            targetKill = SessionInfo.Instance.isTeamMode ? 35 : 25;
        else if (playerPercent >= 0.8f) // 6¸í ÀÌ»ó
            targetKill = SessionInfo.Instance.isTeamMode ? 30 : 20;
        else if (playerPercent >= 0.5f) // 4¸í
            targetKill = SessionInfo.Instance.isTeamMode ? 20 : 15;
        else
            targetKill = 15;
    }

    public void KillUP()
    {
        Hashtable ht = PhotonNetwork.LocalPlayer.CustomProperties;
        ht["Kill"] = (int)ht["Kill"] + 1;
        PhotonNetwork.LocalPlayer.SetCustomProperties(ht);

        PV.RPC("LogUPdate", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    public void LogUPdate()
    {
        // ³»¸²Â÷¼ø (¼±Çü ¸®½ºÆ®)
        List<Player> list = PhotonNetwork.PlayerList.OrderByDescending(p => p.CustomProperties["Kill"].ToString()).ToList();

        short _1TeamKillCount = 0;
        short _2TeamKillCount = 0;
      
        for (short i = 0; i < list.Count; i++)
        {
            string teamText = "";
            short team = short.Parse(list[i].CustomProperties["Team"].ToString());
            short kill = short.Parse(list[i].CustomProperties["Kill"].ToString());

            if (SessionInfo.Instance.isTeamMode)
            {
                teamText = "[" + team.ToString() + "ÆÀ]";
                if (team == 1) // 1ÆÀ
                {
                    _1TeamKillCount += kill;
                }
                else if (team == 2) // 2ÆÀ
                {
                    _2TeamKillCount += kill;
                }    
            }

            if (PhotonNetwork.LocalPlayer == list[i])
            {
                KillBoardText[i].color = Color.green;
            }
            else
            {
                if (SessionInfo.Instance.isTeamMode && SessionInfo.Instance.TeamCheck(CharacterScript.Instance.myTeam, team))
                {
                    KillBoardText[i].color = Color.yellow;
                }
                else
                {
                    KillBoardText[i].color = new Color(50 / 255, 50 / 255, 50 / 255);
                }
            }

            KillBoardText[i].text = (i + 1) + "." + teamText + list[i].NickName + " : " + kill + "kill\n";
        }

        if (SessionInfo.Instance.isTeamMode)
        {
            Team1KillCount = _1TeamKillCount;
            Team2KillCount = _2TeamKillCount;
        }

        userKillCount = short.Parse(list[0].CustomProperties["Kill"].ToString());
    }
}
