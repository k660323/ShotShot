using UnityEngine;
using Photon.Pun;

public class SightCheck : MonoBehaviour
{
    PhotonView PV;
    SpriteRenderer IconColor;

    // Start is called before the first frame update
    void Awake()
    {
        PV = GetComponentInParent<PhotonView>();
        IconColor = GetComponent<SpriteRenderer>();

        IconColor.color = PV.IsMine ? Color.green : Color.red;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && PV.IsMine)
        {
            Color color = collision.GetComponent<CharacterScript>().IconImage.color;
            color = new Color(color.r, color.g, color.b, 1);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && PV.IsMine)
        {
            Color color = collision.GetComponent<CharacterScript>().IconImage.color;
            color = new Color(color.r, color.g, color.b, 0);
        }
    }
}
