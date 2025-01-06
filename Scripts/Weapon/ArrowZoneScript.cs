using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowZoneScript : MonoBehaviour
{
    PhotonView PV;
    [SerializeField]
    Transform ArrowPos;
    [SerializeField]
    int maxCount = 20;
    [SerializeField]
    GameObject prefab;
    [HideInInspector]
    public float damage;

    [PunRPC]
    private void InitDamage(float _damage)
    {
        damage = _damage;
        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            StartCoroutine(CreateArrows());
        }
    }

    IEnumerator CreateArrows()
    {
        float xMin = transform.GetComponent<SpriteRenderer>().bounds.min.x;
        float xMax = transform.GetComponent<SpriteRenderer>().bounds.max.x;

        for (int i = 0; i < maxCount; i++)
        {
            float RandX = Random.Range(xMin, xMax);
            GameObject go = ObjectPoolingManager.Instance.GetArrowReinQueue();
            go.GetComponent<PhotonView>().RPC("Init", RpcTarget.AllViaServer, new Vector3(RandX, ArrowPos.position.y, 0), damage, CharacterScript.Instance.myTeam,CharacterScript.Instance.DSkinIndex);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1.5f);

        PV.RPC("DestoryObject", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void DestoryObject()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("ArrowRein"))
            return;
        if (collision.gameObject.GetComponent<PhotonView>().IsMine != PV.IsMine)
            return;

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("ArrowRein"))
            return;
        if (collision.gameObject.GetComponent<PhotonView>().IsMine != PV.IsMine)
            return;

        ObjectPoolingManager.Instance.InsertArrowReinQueue(collision.gameObject);
    }
}
