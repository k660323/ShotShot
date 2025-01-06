using Photon.Pun;
using UnityEngine;

public class DetectScript : MonoBehaviour
{
    public string TagName;
    public PhotonView PV;
    public BoxCollider2D ParentCollider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag(TagName))
            return;
        if (collision.GetComponent<PhotonView>().IsMine != PV.IsMine)
            return;

            ParentCollider.enabled = true;
            gameObject.SetActive(false);
    }
}
