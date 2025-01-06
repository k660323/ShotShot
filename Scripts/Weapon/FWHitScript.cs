using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FWHitScript : MonoBehaviourPunCallbacks
{
    public short myTeam;
    public int DSkin;
    PhotonView PV;

    public float damage;
    [SerializeField]
    float addtionRate;

    public bool isKnockBack;

    // Start is called before the first frame update
    void Awake()
    {
        PV = GetComponentInParent<PhotonView>();
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
                    if (isKnockBack)
                    {
                        collision.GetComponent<CharacterScript>().PhysicsState(3f, false);
                        Vector3 dir = (collision.transform.position - transform.position).normalized;
                        collision.GetComponent<Rigidbody2D>().AddForce(-dir * 10f, ForceMode2D.Impulse);
                    }
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

    [PunRPC]
    void Init(float _damage,short _teamNum, int DSkinIndex)
    {
        myTeam = _teamNum;
        DSkin = DSkinIndex;
        damage = _damage;
    }
}
