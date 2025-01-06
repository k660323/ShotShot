using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderScript : MonoBehaviour,IPunObservable
{
    [SerializeField]
    AudioClip[] audioClips;
    [SerializeField]
    PhotonView PV;
    Animator AN;
    PolygonCollider2D Polycollider;

    CharacterScript player;
    public TasersSmashingScript tsScript;

    [SerializeField]
    float damage;
    short myTeam;
    public int DSkin;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Polycollider.enabled);
        }
        else
        {
            Polycollider.enabled = (bool)stream.ReceiveNext();
        }
    }

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        Polycollider = GetComponent<PolygonCollider2D>();
        AN = GetComponentInChildren<Animator>();

        if (PV.IsMine)
            player = CharacterScript.Instance;
    }

    [PunRPC]
    void ActiveThunder(float _damage, short _myTeam, int _DSkinIndex)
    {
        damage = _damage;
        myTeam = _myTeam;
        DSkin = _DSkinIndex;
        if (PV.IsMine)
        {
            StartCoroutine(ActiveTasersThunder());
        }
    }

    IEnumerator ActiveTasersThunder()
    {
        PV.RPC("GetQSound", RpcTarget.AllViaServer, 0, 7f, transform.position, false);

        player.isCC = true;
        AN.SetBool("isAttack", true);
        Polycollider.enabled = true;
        yield return new WaitForSeconds(1.0f);
        Polycollider.enabled = false;
        AN.SetBool("isAttack", false);
        player.isCC = false;
        tsScript.Init();
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
                    collision.GetComponent<CharacterScript>().CCState(3f);
                    collision.GetComponent<CharacterScript>().Hit(PV.Owner, myTeam, damage, DSkin);
                }
            }
        }
        else if (collision.gameObject.CompareTag("SpiderMine"))
        {
            if (collision.GetComponent<PhotonView>().IsMine && !PV.IsMine)
            {
                isTeam = SessionInfo.Instance.TeamCheck(myTeam, collision.GetComponent<SpiderMineScript>().myTeam);
                if (!isTeam)
                {
                    collision.GetComponent<SpiderMineScript>().Hit(damage, DSkin);
                }
            }
        }
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
