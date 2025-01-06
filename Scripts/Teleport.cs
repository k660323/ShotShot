using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Teleport : MonoBehaviour
{
    [SerializeField]
    private Teleport targetTelport;

    bool isTelporting;
    [SerializeField]
    Image TeleportGauge;
    [SerializeField]
    float curTel;
    [SerializeField]
    float maxTel;

    bool isCoolTime;
    [SerializeField]
    Image CoolTimeGauge;
    [SerializeField]
    float curCool;
    [SerializeField]
    float maxCool;

    PhotonView PV;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CharacterScript.Instance == null)
            return;

        if (isCoolTime)
        {
            curCool -= Time.deltaTime;
            CoolTimeGauge.fillAmount = curCool / maxCool;
            if(curCool <= 0f)
            {
                isCoolTime = false;
                CoolTimeGauge.fillAmount = 0f;
            }
            return;
        }

        if(isTelporting)
        {
            if (CharacterScript.Instance.isDead)
            {
                TelCancel();
                return;
            }

            curTel += Time.deltaTime;
            TeleportGauge.fillAmount = curTel / maxTel;


            if(curTel >= maxTel)
            {
                isTelporting = false;
                CharacterScript.Instance.Teleport(targetTelport.transform.position);
                SetCoolTime();
                targetTelport.SetCoolTime();
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (PhotonNetwork.IsMasterClient)
        {
            Player targetActorNum = collision.GetComponent<PhotonView>().Owner;
            PV.RPC("TelStart", targetActorNum);
        }
    }

    [PunRPC]
    void TelStart()
    {
        if (isCoolTime)
            return;
        isTelporting = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (PhotonNetwork.IsMasterClient)
        {
            Player targetActorNum = collision.GetComponent<PhotonView>().Owner;
            PV.RPC("TelCancel", targetActorNum);
        }
    }

    [PunRPC]
    void TelCancel()
    {
        isTelporting = false;
        curTel = 0f;
        TeleportGauge.fillAmount = 0f;
    }

    void SetCoolTime()
    {
        curCool = maxCool;
        isCoolTime = true;
    }
}
