using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfHealScript : SkillScript
{
    float HealAmount;

    void Start()
    {
        HealAmount = 20f;
        addtionRate = 2f;
    }

    public override void Init(bool isSetCool = true)
    {
        if (!PV.IsMine)
            return;

        if (skill != null)
        {
            StopCoroutine(skill);
            skill = null;
        }

        if(isActive)
        {
            isActive = false;
            skillActive.SetActive(false);
            player.isSkillActive = false;
            player.healParticle.SetActive(false);
            if (isSetCool)
                SetCoolTime();
        }
    }

    protected override void Update()
    {
        if (player.PV.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!isActive && player.curHp != player.maxHp && !player.isSkillActive && !player.Gun.isReload && player.isControll && !player.isCC && !player.isResistance && !player.isDead)
                {
                    if (player.level >= skillUsedLevel)
                    {
                        if (curCool == maxCool)
                        {
                            PV.RPC("GetQSound", RpcTarget.AllViaServer, 0, 7f, transform.position, true);

                            if (skill == null)
                                skill = Heal();
                            StartCoroutine(skill);
                        }
                    }
                }
            }
            CoolTime();
        }

    }

    public IEnumerator Heal()
    {
        if (player.pCScript != null)
        {
            if (player.isZoom)
                player.Gun.ZoomCancle();

            player.pCScript.Cancel();
        }

        isActive = true;
        skillActive.SetActive(true);
        player.isSkillActive = true;
        player.healParticle.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        if (player.maxHp <= player.curHp + (player.level - skillUsedLevel <= 0 ? HealAmount : HealAmount + (player.level - skillUsedLevel) * addtionRate))
            player.curHpProperti = player.maxHp;
        else
            player.curHpProperti += HealAmount + (player.level - skillUsedLevel) * addtionRate;

        player.HpBar.fillAmount = player.curHp / player.maxHp;

        skill = null;
        Init();
    }
}
