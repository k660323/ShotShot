using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowReinScript : Bullet
{
    [SerializeField]
    GameObject DetectObject;

    [SerializeField]
    BoxCollider2D box2D;

    protected override void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime); // 초당 해당 방향의 속도로 이동
    }

    protected override void OnTriggerEnter2D(Collider2D collision) // 트리거 인 총알이 콜라이더랑 부딫혔을때
    {
        bool isTeam = false;
       
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.GetComponent<PhotonView>().IsMine && !PV.IsMine) // 남이 총쏜 총알 -> 자신이 맞았을때
            {
                isTeam = SessionInfo.Instance.TeamCheck(myTeam, collision.GetComponent<CharacterScript>().myTeam);

                if (!isTeam)
                {
                    if (knockback)
                    {
                        collision.GetComponent<CharacterScript>().PhysicsState(kbTime, kbResistance);
                        collision.GetComponent<Rigidbody2D>().AddForce(dir * kbPower, ForceMode2D.Impulse);
                    }
                    if (Slow)
                    {
                        collision.GetComponent<CharacterScript>().SlowState(sTime, sSpeed);
                    }
                    if (CC)
                    {
                        collision.GetComponent<CharacterScript>().CCState(ccTime);
                    }
                    collision.GetComponent<CharacterScript>().Hit(PV.Owner, myTeam, damage, DSkin);
                }
                CancelRegisterInsertQueue();
            }
            else if (collision.GetComponent<PhotonView>().OwnerActorNr != PV.OwnerActorNr)
            {
                CancelRegisterInsertQueue();
            }
        }
        else if (collision.gameObject.CompareTag("LandMine"))
        {
            isTeam = SessionInfo.Instance.TeamCheck(myTeam, collision.GetComponent<LandMineScript>().myTeam);
            if (collision.GetComponent<PhotonView>().IsMine && !PV.IsMine)
            {
                if (!isTeam)
                {
                    CancelRegisterInsertQueue();
                    collision.GetComponent<LandMineScript>().Hit(damage, DSkin);
                }
            }
            else if (collision.GetComponent<PhotonView>().OwnerActorNr != PV.OwnerActorNr)
            {
                if (!isTeam)
                    CancelRegisterInsertQueue();
            }
        }
        else if (collision.gameObject.CompareTag("SpiderMine"))
        {
            isTeam = SessionInfo.Instance.TeamCheck(myTeam, collision.GetComponent<SpiderMineScript>().myTeam);
            if (collision.GetComponent<PhotonView>().IsMine && !PV.IsMine)
            {
                if (!isTeam)
                {
                    CancelRegisterInsertQueue();
                    collision.GetComponent<SpiderMineScript>().Hit(damage, DSkin);
                }
            }
            else if (collision.GetComponent<PhotonView>().OwnerActorNr != PV.OwnerActorNr)
            {
                if (!isTeam)
                    CancelRegisterInsertQueue();
            }
        }
        else if (collision.gameObject.CompareTag("Scarecrow"))
        {
            CancelRegisterInsertQueue();
            if (PhotonNetwork.IsMasterClient)
            {
                collision.GetComponent<ScarecrowScript>().Hit(damage, DSkin);
            }
        }
        else if (collision.gameObject.CompareTag("Bomb"))
        {
            if (collision.GetComponent<PhotonView>().OwnerActorNr == PV.OwnerActorNr)
            {
                CancelRegisterInsertQueue();
                collision.GetComponent<BombScript>().StartCoroutine(collision.GetComponent<BombScript>().nBoom());
            }
        }
    }

    protected override void CancelRegisterInsertQueue()
    {
        ObjectPoolingManager.Instance.InsertArrowReinQueue(gameObject);
    }

    [PunRPC]
    private void Init(Vector3 startPos, float _damage, short _teamNum,int DamegeSkinIndex)
    {
        DSkin = DamegeSkinIndex;
        myTeam = _teamNum;
        transform.position = startPos;
        damage = _damage;
        box2D.enabled = false;
        gameObject.SetActive(true);
    }

    public override void OnEnable()
    {
        DetectObject.SetActive(true);
    }
}
