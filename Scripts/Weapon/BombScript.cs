using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BombScript : MonoBehaviourPunCallbacks
{
    public short myTeam;
    [SerializeField]
    AudioClip[] audioClips;

    [SerializeField]
    CircleCollider2D detectRange;
    [SerializeField]
    CircleCollider2D BoomRange;

    PhotonView PV;
    Animator AN;

    Vector2 curPos;
    public float speed;
    public float boomTime;
    public float damage;

    int DSkin;
    bool isActive;

    IEnumerator bombCoroutine;

    private void Awake()
    {
        PV = GetComponentInChildren<PhotonView>();
        AN = GetComponentInChildren<Animator>();
        AN.SetFloat("ReadySpeed", boomTime);
    }

    public override void OnEnable()
    {
        gameObject.tag = "Bomb";
        isActive = false;  
        detectRange.enabled = true;
        AN.SetBool("isReady", false);
        AN.SetBool("isBoom", false);
        AN.SetFloat("ReadySpeed", boomTime);
        bombCoroutine = Boom();
    }

    void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            if((Vector2)transform.position == curPos && !isActive)
            {
                StartCoroutine(bombCoroutine);
                isActive = true;
            }
            else if((Vector2)transform.position != curPos && !isActive)
            {
                transform.position = Vector2.MoveTowards(transform.position, curPos, speed * Time.deltaTime);
            }
        }
    }

    IEnumerator Boom()
    {
        // ≈Õ¡ˆ±‚ ¿¸ æ÷¥œ±Ê¿Ã 3√ 
        AN.SetBool("isReady", true);
        yield return new WaitForSeconds(1f / boomTime);
        // ∆¯∆»
        gameObject.tag = "Boom";
        detectRange.enabled = false;
        AN.SetBool("isBoom", true);
        GetQSound(0, 25f, transform.position);
        BoomRange.enabled = true;
        yield return new WaitForSeconds(0.58f);
        // ∆¯∆» ¿‹«ÿ
        BoomRange.enabled = false;
        yield return new WaitForSeconds(0.42f);
        // ∞‘¿” ø¿∫Í¡ß∆Æ ¡¶∞≈
        ObjectPoolingManager.Instance.InsertBombQueue(gameObject);
    }

    public IEnumerator nBoom()
    {
        isActive = true;
        if (bombCoroutine != null)
            StopCoroutine(bombCoroutine);
        // ∆¯∆»
        gameObject.tag = "Boom";
        detectRange.enabled = false;
        AN.SetBool("isReady", true);
        AN.SetBool("isBoom", true);
        GetQSound(0, 25f, transform.position);
        BoomRange.enabled = true;
        yield return new WaitForSeconds(0.58f);
        // ∆¯∆» ¿‹«ÿ
        BoomRange.enabled = false;
        yield return new WaitForSeconds(0.42f);
        // ∞‘¿” ø¿∫Í¡ß∆Æ ¡¶∞≈
        ObjectPoolingManager.Instance.InsertBombQueue(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (BoomRange.enabled)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (collision.GetComponent<PhotonView>().IsMine)
                {
                    collision.GetComponent<CharacterScript>().Hit(PV.Owner, myTeam, damage, DSkin,"Boom");
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
    public void Init(Vector3 startPos,float x,float y,float _damage, int DSkinIndex)
    {
        transform.position = startPos;
        curPos = new Vector2(x, y);
        damage = _damage;
        DSkin = DSkinIndex;
        gameObject.SetActive(true);
    }

    [PunRPC]
    protected void GetQSound(int index,float maxRange, Vector3 startPos)
    {
        GameObject s_Object = ObjectPoolingManager.Instance.GetSQueue();
        s_Object.GetComponent<AudioSource>().clip = audioClips[index];
        s_Object.GetComponent<AudioSource>().maxDistance = maxRange;
        s_Object.transform.position = startPos;
        s_Object.SetActive(true);
    }
}
