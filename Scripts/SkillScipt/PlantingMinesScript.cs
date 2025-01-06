using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantingMinesScript : SkillScript
{
    public string weaponName;
    public float landSpeed;

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
    }

    public override void Init(bool isSetCool = true)
    {
        if (!PV.IsMine)
            return;
        if (!player.isDead && !player.isCC)
            player.isControll = true;

        if (isActive)
        {
            isActive = false;
            skillActive.SetActive(false);
            player.isSkillReady = false;
            player.curSkill = null;
            weaponObject.SetActive(false);
            if (PreviousObject != null)
                PreviousObject.SetActive(true);
            SetCoolTime();
        }

        if (skill != null)
        {
            StopCoroutine(skill);
            skill = null;
        }
        player.PV.RPC("TriggerAnim", RpcTarget.AllViaServer, "isSitUp");
    }
    protected override void Update()
    {
        if (player.PV.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (!player.isSkillActive && !player.Gun.isReload && player.isControll && !player.isCC && !player.isPhysics && !player.isDead)
                {
                    if (!isActive)
                    {
                        if (player.curSkill == null)
                        {
                            if (player.level >= skillUsedLevel)
                                if (curCool == maxCool)
                                    ActiveLandMine();
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

                                            ActiveLandMine();
                                            break;
                                        }
                                    }
                        }
                    }
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if (isActive && !player.isDead)
                {
                    if (skill == null)
                        skill = PlantingMine();
                    StartCoroutine(skill);
                }
            }

            CoolTime();
        }
    }

    public void ActiveLandMine()
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

    IEnumerator PlantingMine()
    {
        player.isControll = false;
        player.AN.SetFloat("SitSpeed", landSpeed);
        player.PV.RPC("TriggerAnim", RpcTarget.AllViaServer, "isSitDown");
        yield return new WaitForSeconds(1.0f);
        float totalDamage = skillDamage + ((player.level <= skillUsedLevel) ? 0 : (player.level - skillUsedLevel) * addtionRate);
        GameObject go = ObjectPoolingManager.Instance.GetMineQueue();
        go.GetComponent<PhotonView>().RPC("Init", RpcTarget.AllViaServer, transform.position, totalDamage, player.myTeam, player.DSkinIndex);
        player.Gun.SwapCool();
        skill = null;
        Init();
    }
}
