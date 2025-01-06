using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class GrenadeScript : SkillScript
{
    public string weaponName;
    public float maxRange;
    Vector2 dir;

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

        if (isActive)
        {
            isActive = false;
            skillActive.SetActive(false);
            player.isSkillReady = false;
            player.curSkill = null;
            player.Gun.SwapCool();
            weaponObject.SetActive(false);
            if (PreviousObject != null)
                PreviousObject.SetActive(true);

            if (isSetCool)
                SetCoolTime();
        }
    }

    protected override void Update()
    {
        if (player.PV.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (!player.isSkillActive && !player.Gun.isReload && player.isControll && !player.isCC && !player.isDead)
                {
                    if (!isActive)
                    {
                        if (player.curSkill == null)
                        {
                            if (player.level >= skillUsedLevel)
                                if (curCool == maxCool)
                                    ReadyBomb();
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

                                            ReadyBomb();
                                            break;
                                        }
                                    }
                        }

                    }
                    else if (isActive && !player.isDead)
                        Init(false);
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if (player.isControll && !player.isCC && !player.isDead)
                {
                    if (isActive)
                    {
                        PV.RPC("GetQSound", RpcTarget.AllViaServer, 0, 7f, transform.position, false);
                        ThrowGrenade(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    }
                }
               
            }

            CoolTime();
        }
     
    }

    public void ReadyBomb()
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

        for(int i=0; i < player.WeaponPos.transform.childCount; i++)
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

    public void ThrowGrenade(Vector2 cursorPos)
    {
        GameObject go = ObjectPoolingManager.Instance.GetBombQueue();
        float totalDamage = skillDamage + ((player.level <= skillUsedLevel) ? 0 : (player.level - skillUsedLevel) * addtionRate);
        if (Vector2.Distance(player.transform.position, cursorPos) <= maxRange)
        {
            go.GetComponent<PhotonView>().RPC("Init", RpcTarget.AllBuffered, transform.position, cursorPos.x, cursorPos.y, totalDamage, player.DSkinIndex);
        }
        else
        {
            dir = (cursorPos - (Vector2)player.transform.position).normalized;
            cursorPos = (Vector2)player.transform.position + (dir * maxRange);
            go.GetComponent<PhotonView>().RPC("Init", RpcTarget.AllBuffered,  transform.position, cursorPos.x, cursorPos.y, totalDamage, player.DSkinIndex);
        }
        Init();
    }
}
