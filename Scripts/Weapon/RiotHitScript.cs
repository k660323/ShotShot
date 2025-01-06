using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiotHitScript : MonoBehaviourPunCallbacks
{
    public short myTeam;
    PhotonView PV;

    [SerializeField]
    public float damage;

    public int DSkin;

    Transform playerTrans;

    // Start is called before the first frame update
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        playerTrans = GetComponentInParent<RiotScript>().player.transform;
    }

    [PunRPC]
    void Init(float _damage, short _teamNum, int DSkinIndex)
    {
        myTeam = _teamNum;
        DSkin = DSkinIndex;
        damage = _damage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool isTeam = false;
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.GetComponent<PhotonView>().IsMine && !PV.IsMine)
            {
                isTeam = SessionInfo.Instance.TeamCheck(myTeam, collision.GetComponent<CharacterScript>().myTeam);

                if (!isTeam)
                {
                    Vector3 blackHoolDir = (collision.transform.position - playerTrans.position).normalized;
                    collision.GetComponent<CharacterScript>().PhysicsState(0.05f, true);
                    collision.GetComponent<Rigidbody2D>().AddForce(-blackHoolDir * 2f, ForceMode2D.Impulse);
                    
                    collision.GetComponent<CharacterScript>().Hit(PV.Owner, myTeam, damage, DSkin);
                }
            }
        }
        else if (collision.gameObject.CompareTag("LandMine"))
        {
            if (collision.GetComponent<PhotonView>().IsMine && !PV.IsMine)
            {
                isTeam = SessionInfo.Instance.TeamCheck(myTeam, collision.GetComponent<LandMineScript>().myTeam);

                if (!isTeam)
                    collision.GetComponent<LandMineScript>().Hit(damage, DSkin);
            }
        }
        else if (collision.gameObject.CompareTag("SpiderMine"))
        {
            if (collision.GetComponent<PhotonView>().IsMine && !PV.IsMine)
            {
                isTeam = SessionInfo.Instance.TeamCheck(myTeam, collision.GetComponent<SpiderMineScript>().myTeam);

                if (!isTeam)
                    collision.GetComponent<SpiderMineScript>().Hit(damage, DSkin);
            }
        }
        else if (collision.gameObject.CompareTag("Scarecrow"))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                collision.GetComponent<ScarecrowScript>().Hit(damage, DSkin);
            }
        }
    }
}
