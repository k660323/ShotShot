using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TasersSmashingScript : SkillScript
{
    public string weaponName;
    public override void Init(bool isSetCool = true)
    {
        if (!PV.IsMine)
            return;
        if (isActive)
        {
            isActive = false;
            skillActive.SetActive(false);
            player.isSkillReady = false;
            player.curSkill = null;
            player.Gun.SwapCool();
            if (PreviousObject != null)
                PreviousObject.SetActive(true);
            weaponObject.SetActive(false);
        }

        if (isSetCool)
            SetCoolTime();
    }

    private void Start()
    {
        for (int i = 0; i < player.WeaponPos.transform.childCount; i++)
        {
            if (weaponName == player.WeaponPos.transform.GetChild(i).name)
            {
                weaponObject = player.WeaponPos.transform.GetChild(i).gameObject;
                break;
            }
        }

        weaponObject.GetComponent<ThunderScript>().tsScript = this;
    }

    protected override void Update()
    {
        if (player.PV.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!player.isSkillActive && !player.Gun.isReload && player.isControll && !player.isCC && !player.isDead)
                {
                    if (!isActive)
                    {
                        if (player.curSkill == null)
                        {
                            if (player.level >= skillUsedLevel)
                                if (curCool == maxCool)
                                    ReadyTasers();
                        }
                        else // 들고 있는 무기가 있을시 스왑
                        {
                            if (player.level >= skillUsedLevel)
                                if (curCool == maxCool)
                                    for (int i = 0; i < swapSkillUse.Length; i++)
                                    {
                                        if (player.curSkill == swapSkillUse[i])
                                        {
                                            swapSkillUse[i].isActive = false;
                                            swapSkillUse[i].weaponObject.SetActive(false);
                                            swapSkillUse[i].skillActive.SetActive(false);
                                            swapSkillUse[i].PreviousObject.SetActive(true);
                                            swapSkillUse[i].PreviousObject = null;
                                            player.curSkill = null;
                                            player.isSkillReady = false;

                                            ReadyTasers();
                                            break;
                                        }
                                    }
                        }

                    }
                    else if (isActive && !player.isDead)
                        Init(false);
                }
            }
            else if (isActive)
            {
                if (Input.GetMouseButtonDown(0))
                    if (player.isControll && !player.isCC && !player.isDead)
                    {
                        float totalDamage = skillDamage + ((player.level <= skillUsedLevel) ? 0 : (player.level - skillUsedLevel) * addtionRate);
                        weaponObject.GetComponent<PhotonView>().RPC("ActiveThunder", RpcTarget.AllBufferedViaServer, totalDamage, player.myTeam, player.DSkinIndex);
                    }
            }
            CoolTime();
        }
    }

    public void ReadyTasers()
    {
        if (player.pCScript != null)
        {
            if (player.isZoom)
                player.Gun.ZoomCancle();
        }

        isActive = true;
        skillActive.SetActive(true);
        player.isSkillReady = true;
        player.curSkill = this;

        for (int i = 0; i < player.WeaponPos.transform.childCount; i++)
        {
            if (player.WeaponPos.transform.GetChild(i).gameObject.activeSelf)
            {
                if (player.WeaponPos.transform.GetChild(i).name != weaponObject.name)
                {
                    PreviousObject = player.WeaponPos.transform.GetChild(i).gameObject;
                    PreviousObject.SetActive(false);
                    break;
                }
            }
        }
        weaponObject.SetActive(true);
    }
}
