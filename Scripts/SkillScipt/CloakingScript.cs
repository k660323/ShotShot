using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CloakingScript : SkillScript
{
    IEnumerator CloakingCoroutine;

    public bool isCloakingUse;

    [SerializeField]
    Text NickNameText;
    [SerializeField]
    Outline[] outline;
    [SerializeField]
    Image hpBar;

    [SerializeField]
    SpriteRenderer[] SRG;

    public float clockingTime;
    bool isCoroutine;

    private void Start()
    {
        CloakingCoroutine = CloakingState();

        PV = GetComponent<PhotonView>();  
        
        NickNameText = player.NickNameText;
        outline = player.NickNameText.GetComponents<Outline>();
        hpBar = player.HpBar;

        SRG = new SpriteRenderer[2 + player.WeaponPos.transform.childCount];
        SRG[0] = player.SR;
        SRG[1] = player.IconImage;
        for (int i = 0; i < player.WeaponPos.transform.childCount; i++)
        {
            SRG[i + 2] = player.WeaponPos.transform.GetChild(i).GetComponentInChildren<SpriteRenderer>();
        }
    }

    public void ResetWeaponArray()
    {
        SRG = new SpriteRenderer[2 + player.WeaponPos.transform.childCount - 1];
        SRG[0] = player.SR;
        SRG[1] = player.IconImage;
        for (int i = 1; i < player.WeaponPos.transform.childCount; i++)
        {
            SRG[i + 2 - 1] = player.WeaponPos.transform.GetChild(i).GetComponentInChildren<SpriteRenderer>();
        }
    }

    protected override void Update()
    {
        if (player.PV.IsMine)
        {
            ClockingCode();
            CoolTime();
        }

    }

    void ClockingCode()
    {
        if (isCloakingUse)
            if (player.h != 0 || player.v != 0 || !EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButton(0) || player.isSkillActive || player.isCC || player.isResistance || player.isDead)
            {
                Cancel();
            }
            else if (player.h == 0 && player.v == 0 && !isCoroutine && player.isControll && !player.isCC && !player.isResistance && player.tag != "Invincibility" && !player.isDead)
            {
                if (player.level >= skillUsedLevel)
                {
                    if (curCool == maxCool)
                    {
                        CloakingCoroutine = CloakingState();
                        StartCoroutine(CloakingCoroutine);
                    }
                }
            }
    }

    public void Cancel()
    {
        if (isCoroutine)
        {
            isCoroutine = false;
            if (CloakingCoroutine != null)
                StopCoroutine(CloakingCoroutine);
        }
        if (isActive)
        {
            isActive = false;
            gameObject.layer = 7;
            SetCoolTime();
            PV.RPC("Clocking", RpcTarget.AllViaServer, false);
        }
    }

    IEnumerator CloakingState()
    {
        isCoroutine = true;
        yield return new WaitForSeconds(clockingTime);
        isActive = true;
        gameObject.layer = 14;     
        PV.RPC("Clocking", RpcTarget.AllViaServer,true);
    }

    // 닉네임,아이콘,무기류 알베도 값 
    [PunRPC]
    void Clocking(bool isActive)
    {
        if (isActive)
        {
            NickNameText.color = new Color(NickNameText.color.r, NickNameText.color.g, NickNameText.color.b, 50f / 255f);
            for (int i = 0; i < outline.Length; i++)
            {
                outline[i].effectColor = new Color(0, 0, 0, 50 / 255f);
            }
            hpBar.color = new Color(hpBar.color.r, hpBar.color.g, hpBar.color.b, 50f / 255f);

            for (int i = 0; i < SRG.Length; i++)
            {
                if (SRG[i].gameObject.name != "Icon")
                {
                    SRG[i].color = new Color(SRG[i].color.r, SRG[i].color.g, SRG[i].color.b, 50f / 255f);
                }
                else
                {
                    if (PV.IsMine)
                        SRG[i].color = new Color(SRG[i].color.r, SRG[i].color.g, SRG[i].color.b, 1f);
                    else
                        SRG[i].color = new Color(SRG[i].color.r, SRG[i].color.g, SRG[i].color.b, 0f);
                }                  
            }
        }
        else
        {
            NickNameText.color = new Color(NickNameText.color.r, NickNameText.color.g, NickNameText.color.b, 1f);
            for (int i = 0; i < outline.Length; i++)
            {
                outline[i].effectColor = new Color(0, 0, 0, 1f);
            }
            hpBar.color = new Color(hpBar.color.r, hpBar.color.g, hpBar.color.b, 1f);
            for (int i = 0; i < SRG.Length; i++)
            {
                SRG[i].color = new Color(SRG[i].color.r, SRG[i].color.g, SRG[i].color.b, 1f);
            }
        }
    }

    
}
