using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeilSnipingScript : SkillScript
{
    public override void Init(bool isSetCool = true)
    {
        if (!PV.IsMine)
            return;

        if (skill != null)
        {
            StopCoroutine(skill);
            skill = null;
        }

        if (isActive)
        {
            isActive = false;
            skillActive.SetActive(false);
            player.Gun.curCool = 0f;
            player.Gun.curAmmo = player.Gun.maxAmmo;
            player.Gun.coolTime.fillAmount = 0f;
            weaponObject.GetComponent<PhotonView>().RPC("unActiveObject", RpcTarget.AllViaServer);
            player.audioListener.enabled = true;
            player.PV.RPC("TriggerAnim", RpcTarget.AllViaServer, "isSitUp");

            CMManager.Instance().virtualCamera.Follow = player.transform;
            CMManager.Instance().virtualCamera.LookAt = player.transform;
            if (isSetCool)
                SetCoolTime();
        }

        if (!player.isDead && !player.isCC)
            player.isControll = true;
    }

    protected override void Update()
    {
        if (player.PV.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (!isActive && !player.isSkillActive && !player.Gun.isReload && player.isControll && !player.isCC && !player.isResistance && !player.isDead)
                {
                    if (player.level >= skillUsedLevel)
                    {
                        if (curCool == maxCool)
                        {
                            SnipingMode();
                        }
                    }
                }
                else
                {
                    Init();
                }
            }
            CoolTime();
        }
    }

    void SnipingMode()
    {
        player.audioListener.enabled = false;
        player.PV.RPC("TriggerAnim", RpcTarget.AllViaServer, "isSitDown");
        player.isControll = false;
        float totalDamage = skillDamage + ((player.level <= skillUsedLevel) ? 0 : (player.level - skillUsedLevel) * addtionRate);
        weaponObject.GetComponent<PhotonView>().RPC("InitDamage", RpcTarget.AllViaServer, totalDamage, player.myTeam, player.DSkinIndex);
        isActive = true;
        skillActive.SetActive(true);

        if (skill == null)
            skill = DurationSnipingMode();
        StartCoroutine(skill);
    }

    IEnumerator DurationSnipingMode()
    {
        yield return new WaitForSeconds(15f);
        skill = null;
        if (isActive)
            Init();
    }
}
