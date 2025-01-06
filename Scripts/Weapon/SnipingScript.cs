using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;
using UnityEngine.EventSystems;

public class SnipingScript : MonoBehaviourPunCallbacks,IPunObservable
{
    public short myTeam;
    public NeilSnipingScript ns;
    [SerializeField]
    AudioClip[] audioClips;
    AudioListener audioListener;
    PhotonView PV;
    Vector3 curPos;
    Rigidbody2D RB;
    CharacterScript player;
    BoxCollider2D boxCollider;
    float h;
    float v;
    public float damage;
    int DSkin;

    bool isFireReady;
    public float moveSpeed;

    public float atkRate;
    public float addtionATKRate;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext((Vector2)transform.position);
            stream.SendNext(boxCollider.enabled);
        }
        else
        {
            curPos = (Vector2)stream.ReceiveNext();
            boxCollider.enabled = (bool)stream.ReceiveNext();
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        RB = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        if (PV.IsMine)
        {
            player = CharacterScript.Instance;
            audioListener = GetComponent<AudioListener>();
            audioListener.enabled = false;
        }  
    }

    public override void OnEnable()
    {
        if (PV.IsMine)
        {          
            RB.transform.localPosition = Vector3.zero;
            audioListener.enabled = true;
            player.Gun.curCool = 0f;
            player.Gun.curAmmo = 5;
            player.Gun.coolTime.fillAmount = 0f;


            CMManager.Instance().virtualCamera.Follow = transform;
            CMManager.Instance().virtualCamera.LookAt = transform;
        }
    }

    [PunRPC]
    void InitDamage(float _damage,short _teamNum,int DSkinIndex)
    {
        myTeam = _teamNum;
        DSkin = DSkinIndex;
        damage = _damage;
        gameObject.SetActive(true);
    }

    [PunRPC]
    void unActiveObject() 
    {
        gameObject.SetActive(false);
    }


    // Update is called once per frame
     void Update()
    {
        if (PV.IsMine)
        {
            Shot();
        }
        else if (!PV.IsMine)
        {
            if ((transform.position - curPos).sqrMagnitude > 50)
            {
                transform.position = curPos;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, curPos, 5f * Time.deltaTime);
            }
        }

    }

    void Shot()
    {
        if (player.Gun.curCool > 0)
        {
            player.Gun.curCool -= Time.deltaTime;
            player.Gun.coolTime.fillAmount = player.Gun.curCool / (1f / (atkRate + addtionATKRate));
        }
        isFireReady = 0 >= player.Gun.curCool;

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (isFireReady)
                {
                    if(player.Gun.curAmmo > 0)
                    {
                        player.Gun.curCool = 1f / (atkRate + addtionATKRate);
                        player.Gun.curAmmo -= 1;
                        PV.RPC("GetQSound",RpcTarget.AllViaServer, 0, 30f, transform.position);
                        StartCoroutine(activeCollider());
                    }
                    else if(player.Gun.curAmmo != 0)
                    {
                        ns.Init();
                        gameObject.SetActive(false);
                    }
                  
                }
            }
        }
            
    }

    IEnumerator activeCollider()
    {
        boxCollider.enabled = true;
        yield return new WaitForSeconds(0.5f);
        boxCollider.enabled = false;
        if(player.Gun.curAmmo ==0)
        {
            ns.Init();
        }
        
    }

    void FixedUpdate()
    {
        if (PV.IsMine)
        {
            Move();
        }
    }
    void Move()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        Vector2 tempV2 = RB.position;
        tempV2 += new Vector2(h, v).normalized * moveSpeed * Time.deltaTime;

        if (((Vector2)player.transform.position - tempV2).sqrMagnitude <= 325f)
        {
            RB.position += new Vector2(h, v).normalized * moveSpeed * Time.deltaTime;
        }
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
                    collision.GetComponent<CharacterScript>().CCState(1f);
                    collision.GetComponent<CharacterScript>().Hit(PV.Owner, myTeam, damage, DSkin, "Sniping");
                }
            }
        }
        else if (collision.gameObject.CompareTag("LandMine"))
        {
            if (collision.GetComponent<PhotonView>().IsMine && !PV.IsMine)
            {
                isTeam = SessionInfo.Instance.TeamCheck(myTeam, collision.GetComponent<LandMineScript>().myTeam);

                if (!isTeam)
                    collision.GetComponent<LandMineScript>().Hit(damage);
            }
        }
        else if (collision.gameObject.CompareTag("SpiderMine"))
        {
            if (collision.GetComponent<PhotonView>().IsMine && !PV.IsMine)
            {
                isTeam = SessionInfo.Instance.TeamCheck(myTeam, collision.GetComponent<SpiderMineScript>().myTeam);

                if (!isTeam)
                    collision.GetComponent<SpiderMineScript>().Hit(damage);
            }
        }
        else if (collision.gameObject.CompareTag("Scarecrow"))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                collision.GetComponent<ScarecrowScript>().Hit(damage);
            }
        }
    }

    [PunRPC]
    protected void GetQSound(int index, float maxRange, Vector3 startPos)
    {
        GameObject s_Object = ObjectPoolingManager.Instance.GetSQueue();
        s_Object.GetComponent<AudioSource>().clip = audioClips[index];
        s_Object.GetComponent<AudioSource>().maxDistance = maxRange;
        s_Object.transform.position = startPos;
        s_Object.SetActive(true);
    }
}
