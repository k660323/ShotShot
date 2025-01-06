using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ObscurationScript : MonoBehaviourPunCallbacks
{
    SpriteRenderer[] SR;
    private void Awake()
    {
        SR = gameObject.GetComponentsInChildren<SpriteRenderer>(); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PhotonView>() != null)
            if (collision.GetComponent<PhotonView>().IsMine && collision.CompareTag("Player") || collision.GetComponent<PhotonView>().IsMine && collision.CompareTag("Invincibility"))
                for (int i = 0; i < SR.Length; i++)
                {
                    SR[i].color = new Color(1, 1, 1, 120f / 255f);
                }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PhotonView>() != null)
            if (collision.GetComponent<PhotonView>().IsMine && collision.CompareTag("Player") || collision.GetComponent<PhotonView>().IsMine && collision.CompareTag("Invincibility"))
                for (int i = 0; i < SR.Length; i++)
                {
                    SR[i].color = new Color(1, 1, 1, 1);
                }
    }
}
