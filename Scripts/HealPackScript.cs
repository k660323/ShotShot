using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Threading;

public class HealPackScript : MonoBehaviourPunCallbacks
{
    [SerializeField]
    AudioClip[] audioClips;

    public float healAmount;
    PhotonView PV;
    public volatile int locknum;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void OnEnable()
    {
        locknum = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (collision.CompareTag("Player"))
            {
                CharacterScript player = collision.GetComponent<CharacterScript>();
                if (player.maxHpProperti > player.curHpProperti)
                {
                    if(Interlocked.CompareExchange(ref locknum,1,0) == 0)
                    {
                        PV.RPC("HPUp", RpcTarget.AllViaServer, player.PV.OwnerActorNr);
                    }                
                }
            }
        }
       
    }

    [PunRPC]
    void HPUp(int actorNr)
    {
        CharacterScript player = CharacterScript.Instance;
        if (player.PV.OwnerActorNr == actorNr)
        {
            if (player.maxHpProperti > player.curHpProperti + healAmount)
            {
                player.curHpProperti += healAmount;
            }
            else
            {
                player.curHpProperti = player.maxHpProperti;
            }
            GetQSound(0, 5, player.transform.position, false);
        }
        gameObject.SetActive(false);
    }

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
