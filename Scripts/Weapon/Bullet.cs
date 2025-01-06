using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Bullet : MonoBehaviourPunCallbacks
{
    protected PhotonView PV;
    protected short myTeam;
    protected int DSkin;
    [HideInInspector]
    public Vector3 dir;
    public float speed;
    public float defaultLiftTime;
    public float liftTime;

    [SerializeField]
    protected float damage;

    public bool ReverseKB;
    public bool knockback;
    public float kbTime;
    public bool kbResistance;
    public float kbPower;

    public bool Slow;
    public float sTime;
    public float sSpeed;

    public bool CC;
    public float ccTime;

    public bool penetrate;

    protected void Awake() 
    {
        PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime); // 초당 해당 방향의 속도로 이동
    }

    // 총알이 트리거로 설정시 작동
    protected virtual void OnTriggerEnter2D(Collider2D collision) // 트리거 인 총알이 콜라이더랑 부딫혔을때
    {
        bool isTeam = false;
        if (collision.gameObject.CompareTag("Block") && !penetrate)
        {
            CancelRegisterInsertQueue();
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.GetComponent<PhotonView>().IsMine && !PV.IsMine) // 남이 총쏜 총알 -> 자신이 맞았을때
            {
                isTeam = SessionInfo.Instance.TeamCheck(myTeam, collision.GetComponent<CharacterScript>().myTeam);

                if (!isTeam)
                {
                    if (knockback)
                    {
                        collision.GetComponent<CharacterScript>().PhysicsState(kbTime, kbResistance);
                        collision.GetComponent<Rigidbody2D>().AddForce((ReverseKB ? -dir : dir) * kbPower, ForceMode2D.Impulse);
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

    public override void OnEnable()
    {
        StopCoroutine(RegisterInsertQueue());
        StartCoroutine(RegisterInsertQueue());
    }

    protected IEnumerator RegisterInsertQueue()
    {
        yield return new WaitForSeconds(liftTime);
        ObjectPoolingManager.Instance.InsertBulletQueue(gameObject);
    }

    protected virtual void CancelRegisterInsertQueue()
    {
        StopCoroutine(RegisterInsertQueue());
        ObjectPoolingManager.Instance.InsertBulletQueue(gameObject);
    }

    [PunRPC]
    public void BulletAttribute(bool _knockback, float _kbTime, bool _kbResistance, float _kbPower, bool _slow, float _sTime, float _SSpeed, bool _cc, float _ccTime,bool _penetrate)
    {
        knockback = _knockback;
        kbTime = _kbTime;
        kbPower = _kbPower;
        kbResistance = _kbResistance;

        Slow = _slow;
        sTime = _sTime;

        CC = _cc;
        ccTime = _ccTime;

        penetrate = _penetrate;
    }

    [PunRPC]
    protected void DDRPC(Vector3 startPos, Vector2 targetDir, float _bulletDamage, float _addtionDamage, int DamegeSkinIndex,short _teamNum,float critical=1.0f)
    {
        DSkin = DamegeSkinIndex;
        transform.position = startPos;
        damage = (_bulletDamage + _addtionDamage) * critical;
        dir = targetDir;
        liftTime = defaultLiftTime;
        myTeam = _teamNum;
        // 자식 객체 만들어서 스프라이트 충돌체 다 넣는다.
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

        gameObject.SetActive(true);
    }
    
    [PunRPC]
    protected void DDRPC(Vector3 startPos, Vector2 targetDir, float _bulletDamage, float _addtionDamage, int DamegeSkinIndex, short _teamNum, float chargePercent, float critical = 1.0f)
    {
        DSkin = DamegeSkinIndex;
        transform.position = startPos;
        damage = (_bulletDamage + _addtionDamage) * critical;
        dir = targetDir;
        liftTime = defaultLiftTime;
        liftTime = (liftTime * chargePercent) < 0.5f ? 0.5f : liftTime * chargePercent;
        myTeam = _teamNum;
        // 자식 객체 만들어서 스프라이트 충돌체 다 넣는다.
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

        gameObject.SetActive(true);
    }
}
