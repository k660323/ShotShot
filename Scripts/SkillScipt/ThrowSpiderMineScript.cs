using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowSpiderMineScript : SkillScript
{

    [SerializeField]
    GameObject spider;
    public string weaponName;
    public float maxRange;
    Vector2 dir;

    [SerializeField]
    float biteDamage;

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
        if (isActive)
        {
            isActive = false;
            skillActive.SetActive(false);
            player.isSkillReady = false;
            player.curSkill = null;

            if (PreviousObject != null)
                PreviousObject.SetActive(true);
            weaponObject.SetActive(false);
            player.Gun.SwapCool();
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
                                    ReadySM();
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

                                            ReadySM();
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
                        PV.RPC("GetQSound", RpcTarget.AllViaServer, 0, 7f, transform.position, false);
                        ThrowSM(Camera.main.ScreenToWorldPoint(Input.mousePosition));

                    }
            }

            CoolTime();
        }

    }

    public void ReadySM()
    {
        if (player.pCScript != null)
        {
            if(player.isZoom)
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

    public void ThrowSM(Vector2 cursorPos)
    {
        float tbiteDamage = biteDamage + ((player.level <= skillUsedLevel) ? 0 : (player.level - skillUsedLevel) * addtionRate);
        float totalDamage = skillDamage + ((player.level <= skillUsedLevel) ? 0 : (player.level - skillUsedLevel) * addtionRate);
        if (spider == null)
            spider = PhotonNetwork.Instantiate("Weapon/SpiderMine", Vector3.zero, Quaternion.identity);
        if (Vector2.Distance(player.transform.position, cursorPos) > maxRange)
        {
            dir = (cursorPos - (Vector2)player.transform.position).normalized;
            cursorPos = (Vector2)player.transform.position + (dir * maxRange);
        }
        spider.GetComponent<PhotonView>().RPC("Init", RpcTarget.AllViaServer, transform.position, cursorPos.x, cursorPos.y, tbiteDamage, totalDamage, player.myTeam, player.DSkinIndex);
        Init();
    }
}
