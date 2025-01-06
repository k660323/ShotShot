using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMineScript : MonoBehaviourPunCallbacks,IPunObservable
{
    public short myTeam;

    [SerializeField]
    AudioClip[] plantClips;
    [SerializeField]
    AudioClip[] boomClips;
    [SerializeField]
    AudioClip[] dieClips;

    [SerializeField]
    BoxCollider2D deDectedRange;
    [SerializeField]
    CircleCollider2D BoomRange;


    PhotonView PV;   
    SpriteRenderer SR;
    Animator AN;

    public Transform SpawnDS;
    public float boomTime;
    public float damage;
    int DSkin;

    public float curHp;
    public float maxHp;
    bool isDead;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(curHp);
            stream.SendNext(deDectedRange.enabled);
            stream.SendNext(BoomRange.enabled);
            stream.SendNext(gameObject.tag);
        }
        else
        {
            curHp = (float)stream.ReceiveNext();
            deDectedRange.enabled = (bool)stream.ReceiveNext();
            BoomRange.enabled = (bool)stream.ReceiveNext();
            gameObject.tag = (string)stream.ReceiveNext();
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
        isDead = false;
        gameObject.tag = "LandMine";
        BoomRange.enabled = false;
        deDectedRange.enabled = true;
        AN.SetFloat("BoomSpeed", boomTime);
        if (!PV.IsMine)
        {
            if (SessionInfo.Instance.isTeamMode)
            {
                if (!SessionInfo.Instance.TeamCheck(myTeam, CharacterScript.Instance.myTeam))
                {
                    SR.color = new Color(SR.color.r, SR.color.g, SR.color.b, 70f / 255f);
                }
            }
            else
            {
                SR.color = new Color(SR.color.r, SR.color.g, SR.color.b, 70f / 255f);
            }
        }

        GetQSound("PlantPart", 0, 7f, transform.position, false);
    }

    [PunRPC]
    IEnumerator Boom()
    {
        // ∆¯∆»
        SR.color = new Color(1, 1, 1, 1);

        gameObject.tag = "Boom";
        deDectedRange.enabled = false;

        AN.SetTrigger("isBoom");
        GetQSound("BoomPart", 0, 25f, transform.position, false);


        BoomRange.enabled = true;
        yield return new WaitForSeconds(1 / boomTime);
        BoomRange.enabled = false;

        // ∞‘¿” ø¿∫Í¡ß∆Æ ¡¶∞≈
        ObjectPoolingManager.Instance.InsertMineQueue(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool isTeam = false;
        if (deDectedRange.enabled) // ¡÷¿Œª©∞Ì π‡¿∫ ≥∏∏ »£√‚
        {     
            if (collision.gameObject.CompareTag("Player") && collision.GetComponent<PhotonView>().IsMine && !PV.IsMine)
            {
                isTeam = SessionInfo.Instance.TeamCheck(myTeam, collision.GetComponent<CharacterScript>().myTeam);

                if (!isTeam)
                    PV.RPC("Boom", RpcTarget.AllViaServer);
            }       
        }
        if(BoomRange.enabled)// ¡÷¿ŒªÛ∞¸x ≈Õ¡¸
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (collision.GetComponent<PhotonView>().IsMine)
                {
                    collision.GetComponent<CharacterScript>().Hit(PV.Owner, myTeam, damage, DSkin, "LandMine");
                }
            }
            else if (collision.gameObject.CompareTag("LandMine"))
            {
                if (collision.GetComponent<PhotonView>().IsMine)
                {
                    collision.GetComponent<LandMineScript>().Hit(damage, DSkin);
                }
            }
            else if (collision.gameObject.CompareTag("SpiderMine"))
            {
                if (collision.GetComponent<PhotonView>().IsMine)
                {
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

    [PunRPC]
    public void Init(Vector3 startPos, float _damage,short _teamNum, int DSkinIndex)
    {
        myTeam = _teamNum;
        DSkin = DSkinIndex;
        transform.position = startPos;
        damage = _damage;
        gameObject.SetActive(true);
    }

    public void Hit(float damage, int DSkinIndex = 0)
    {
        if (isDead)
            return;

        curHp -= damage;
        PV.RPC("ShowDamageSkin", RpcTarget.All, (int)damage, DSkinIndex);
        if (curHp <= 0 && !isDead)
        {
            isDead = true;
            PV.RPC("GetQSound", RpcTarget.AllViaServer, "DiePart", 0, 7f, transform.position, false);
            PV.RPC("RemoveObject", RpcTarget.AllViaServer);
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
        ObjectPoolingManager.Instance.InsertMineQueue(gameObject);
    }

    [PunRPC]
    protected void GetQSound(string name,int index, float maxRange, Vector3 startPos, bool isFollow)
    {
        GameObject s_Object = ObjectPoolingManager.Instance.GetSQueue();
        switch (name)
        {
            case "PlantPart":
                s_Object.GetComponent<AudioSource>().clip = plantClips[index];
                break;
            case "BoomPart":
                s_Object.GetComponent<AudioSource>().clip = boomClips[index];
                break;
            case "DiePart":
                s_Object.GetComponent<AudioSource>().clip = dieClips[index];
                break;
            default:
                break;
        }
       
        s_Object.GetComponent<AudioSource>().maxDistance = maxRange;
        if (isFollow)
            s_Object.transform.parent = transform;
        s_Object.transform.position = startPos;
        s_Object.SetActive(true);
    }
}
