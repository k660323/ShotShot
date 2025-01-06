using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ScarecrowScript : MonoBehaviourPunCallbacks
{
    [SerializeField]
    AudioClip[] dieClips;

    public Transform SpawnDS;
    PhotonView PV;
    Animator AN;
    BoxCollider2D boxCollider;
    public BoxCollider2D CrashCollider;

    public float curHp;
    public float maxHp;
    bool isDead;
    // Start is called before the first frame update
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        AN = GetComponentInChildren<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        curHp = maxHp;
        gameObject.tag = "Scarecrow";
        AN.SetBool("isAlive", true);
        boxCollider.enabled = true;
        CrashCollider.enabled = true;
    }

    public void Hit(float damage,int DSkinIndex = 0)
    {
        curHp -= damage;
        PV.RPC("ShowDamageSkin", RpcTarget.All, (int)damage, DSkinIndex);
        if (curHp <= 0 && !isDead)
        {
            isDead = true;
            PV.RPC("GetQSound", RpcTarget.AllViaServer, "DiePart", 0, 8f, transform.position, false);
            PV.RPC("Die", RpcTarget.AllViaServer);
        }
    }

    [PunRPC]
    public void ShowDamageSkin(int damage, int DamegeSkinIndex)
    {
        FloatingTextManager.Instance.CreateFloater(SpawnDS, damage, DamegeSkinIndex);
    }

    [PunRPC]
    public void Die()
    {
        gameObject.tag = "Die";
        AN.SetBool("isAlive",false);
        AN.SetTrigger("isDie");
        boxCollider.enabled = false;
        CrashCollider.enabled = false;
    }

    [PunRPC]
    protected void GetQSound(string name, int index, float maxRange, Vector3 startPos, bool isFollow)
    {
        GameObject s_Object = ObjectPoolingManager.Instance.GetSQueue();

        switch (name)
        {
            case "IdlePart":
              //  s_Object.GetComponent<AudioSource>().clip = shotClips[index];
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
