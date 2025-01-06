using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderMineScript : MonoBehaviourPunCallbacks,IPunObservable
{
    public short myTeam;
    [SerializeField]
    AudioClip[] audioClips;
    [SerializeField]
    CircleCollider2D detectedCircle;
    [SerializeField]
    CircleCollider2D biteCircle;
    [SerializeField]
    CircleCollider2D boomCircle;

    PhotonView PV;
    SpriteRenderer SR;
    Animator AN;

    public Transform SpawnDS;    
    public float biteDamage;
    public float boomDamage;
    int DSkin;

    public float biteSpeed;

    bool isDead;
    public float curHp;
    public float maxHp;
    Vector2 curPos;
    bool isActive;
    public float speed;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(curHp);
            stream.SendNext(biteSpeed);
        }
        else
        {
            curHp = (float)stream.ReceiveNext();
            biteSpeed = (float)stream.ReceiveNext();
        }
    }

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        SR = GetComponentInChildren<SpriteRenderer>();
        AN = GetComponentInChildren<Animator>();
    }

    public override void OnEnable()
    {
        detectedCircle.enabled = false;
        biteCircle.enabled = false;
        boomCircle.enabled = false;
        gameObject.tag = "SpiderMine";
        curHp = maxHp;
        isActive = false;
        SR.color = new Color(SR.color.r, SR.color.g, SR.color.b, 1f);
        isDead = false;
    }

    void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            if (!isDead)
            {
                if ((Vector2)transform.position == curPos && !isActive)
                {

                    detectedCircle.enabled = true;
                    isActive = true;
                    if (!PV.IsMine)
                    {
                        if(SessionInfo.Instance.isTeamMode)
                        {
                            if(SessionInfo.Instance.TeamCheck(myTeam,CharacterScript.Instance.myTeam))
                            {
                                return;
                            }
                        }

                        SR.color = new Color(SR.color.r, SR.color.g, SR.color.b, 70f / 255f);
                    }
                }
                else
                {
                    transform.position = Vector2.MoveTowards(transform.position, curPos, speed * Time.deltaTime);
                }
            }
           
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
     
        if (detectedCircle.enabled)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (!PV.IsMine && collision.GetComponent<PhotonView>().IsMine) // 주인 빼고 맞은 사람이 처리
                {
                    bool isTeam = SessionInfo.Instance.TeamCheck(myTeam, collision.GetComponent<CharacterScript>().myTeam);
                    if (!isTeam)
                        PV.RPC("Bite", RpcTarget.AllBufferedViaServer);
                }
            }
        }
        else if (biteCircle.enabled)
        {
            if (collision.gameObject.CompareTag("Player")) //주인 상관x 처리
            {
                if (collision.GetComponent<PhotonView>().IsMine)
                {
                    collision.GetComponent<CharacterScript>().CCState(1f);
                    collision.GetComponent<CharacterScript>().Hit(PV.Owner, myTeam, biteDamage, DSkin, "SpiderMine");
                }
            }
        }
        else if (boomCircle.enabled)
        {
            if (collision.gameObject.CompareTag("Player")) // 주인 상관x 처리
            {
                if (collision.GetComponent<PhotonView>().IsMine)
                {
                    collision.GetComponent<CharacterScript>().SH(PV.OwnerActorNr, myTeam, "poison", boomDamage, 5, DSkin);
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (detectedCircle.enabled)
        {
            if (collision.gameObject.CompareTag("Player") && PV.OwnerActorNr != collision.GetComponent<PhotonView>().OwnerActorNr)
            {
                Vector2 dir = ((Vector2)gameObject.transform.position - (Vector2)collision.transform.position).normalized;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                Quaternion angleAxis = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
                Quaternion rotation = Quaternion.Lerp(transform.rotation, angleAxis, 15f * Time.deltaTime);
                transform.rotation = rotation;
            }
        }    
    }

    [PunRPC]
    IEnumerator Bite()
    {
        // 폭팔
        gameObject.tag = "Boom";

        SR.color = new Color(SR.color.r, SR.color.g, SR.color.b, 1f);
        detectedCircle.enabled = false;
        AN.SetFloat("AttackRate", biteSpeed);
        AN.SetTrigger("isAttack");
        yield return new WaitForSeconds(1/ biteSpeed);
        biteCircle.enabled = true;
        yield return new WaitForSeconds(0.5f);
        biteCircle.enabled = false;
        AN.SetBool("isAlive", false);
        AN.SetTrigger("isBoom");
        boomCircle.enabled = true;
        yield return new WaitForSeconds(1f);
        boomCircle.enabled = false;


        // 게임 오브젝트 제거
        gameObject.SetActive(false);
        AN.SetBool("isAlive", true);
    }

    public void Hit(float damage, int DSkinIndex = 0)
    {
        if (!isDead)
        {
            curHp -= damage;
            PV.RPC("ShowDamageSkin", RpcTarget.AllBufferedViaServer, (int)damage, DSkinIndex);
            if (curHp <= 0)
            {
                PV.RPC("RemoveObject", RpcTarget.AllBufferedViaServer);
            }
        }   
    }

    [PunRPC]
    public void ShowDamageSkin(int damage, int DSkinIndex)
    {
        FloatingTextManager.Instance.CreateFloater(SpawnDS, damage, DSkinIndex);
    }

    [PunRPC]
    public void RemoveObject()
    {
        StartCoroutine(die());
    }

    IEnumerator die()
    {
        isDead = true;
        gameObject.tag = "Die";
        detectedCircle.enabled = false;
        biteCircle.enabled = false;
        boomCircle.enabled = false;
        SR.color = new Color(SR.color.r, SR.color.g, SR.color.b, 1f);
        AN.SetTrigger("isDie");
        yield return new WaitForSeconds(1.0f);
        gameObject.SetActive(false);
    }

    [PunRPC]
    public void Init(Vector3 startPos, float x, float y, float _biteDamage, float _boomDamage,short _teamNum, int DSkinIndex)
    {
        myTeam = _teamNum;
        DSkin = DSkinIndex;
        transform.position = startPos;
        curPos = new Vector2(x, y);
        biteDamage = _biteDamage;
        boomDamage = _boomDamage;
        for(short i =0;i<SpawnDS.childCount;i++)
        {
            Destroy(SpawnDS.GetChild(i).gameObject);
        }
        gameObject.SetActive(true);
    }

    [PunRPC]
    protected void GetQSound(int index, float maxRange, Vector3 startPos, bool isFollow)
    {
        GameObject s_Object = ObjectPoolingManager.Instance.GetSQueue();
        s_Object.GetComponent<AudioSource>().clip = audioClips[index];
        s_Object.GetComponent<AudioSource>().maxDistance = maxRange;
        if (isFollow)
            s_Object.transform.parent = transform;
        s_Object.transform.position = startPos;
        s_Object.SetActive(true);
    }
}
