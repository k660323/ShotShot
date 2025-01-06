using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TommyScript : GunWeapon
{
    protected override void Update()
    {
        if (player.PV.IsMine)
        {
            if (player.isControll && !player.isCC &&!player.isDead )
            {
                Shot();
                Reload();
            }
        }
    }


    protected override void Shot()
    {
        if (curCool > 0)
        {
            curCool -= Time.deltaTime;
            coolTime.fillAmount = curCool / (1f / (atkRate + addtionATKRate));
        }
        isFireReady = 0 >= curCool;

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (!isChargeMode)
            {
                if (Input.GetMouseButton(0))
                {
                    if (!isReload && isFireReady && !player.isSkillReady && !player.isSkillActive)
                    {
                        if (curAmmo > 0)
                        { 
                            curCool = 1f / (atkRate + addtionATKRate); // rate +addtionRate ���� ������ ����up ���� // rate + addtionRate ���� ������ ����down 
                            curAmmo -= 1; // rate +addtionRate  3 - 2.8 = 0.2  5�ʵ� �ִ��� 0�� �����Ҽ��� ���� ������
                            float critical = (player.pCriScript != null) ? player.pCriScript.Critical() : 1f;
                            GameObject go = ObjectPoolingManager.Instance.GetBulletQueue();
                            go.GetComponent<PhotonView>().RPC("BulletAttribute", RpcTarget.All, Knockback, kbTime, KbResistacne, KbPower, Slow, STime, SSpeed, CC, ccTime, penetrate);
                            go.GetComponent<PhotonView>().RPC("DDRPC", RpcTarget.All, FirePos.position, player.dir, bulletDamage + (player.level == 1 ? 0 : (player.level - 1) * LvDamagePercent) + player.addtionDamage, addtionDamage, player.DSkinIndex, player.myTeam, critical);

                            PV.RPC("GetGESound", RpcTarget.AllBufferedViaServer);
                            PV.RPC("GetQSound", RpcTarget.AllBufferedViaServer, "ShotPart", 0, bulletSoundVol, FirePos.position, false);
                            AfterShoot();
                        }
                        else if (curAmmo == 0)
                        {
                            reloadCoroutine = Reloading();
                            StartCoroutine(reloadCoroutine);
                        }
                    }
                }
            }
            else
            {
                if (!isReload && isFireReady && !player.isSkillReady && !player.isSkillActive)
                {
                    if (Input.GetMouseButtonDown(0) && curAmmo > 0) // ��¡
                    {
                        chargeScript.ChargingStart();
                    }
                    else if (Input.GetMouseButton(0) && curAmmo == 0)
                    {
                        reloadCoroutine = Reloading();
                        StartCoroutine(reloadCoroutine);
                    }

                    if (Input.GetMouseButtonUp(0) && chargeScript.isActive) // ��
                    {
                        curCool = 1f / (atkRate + addtionATKRate); // rate +addtionRate ���� ������ ����up ���� // rate + addtionRate ���� ������ ����down 
                        curAmmo -= 1; // rate +addtionRate  3 - 2.8 = 0.2  5�ʵ� �ִ��� 0�� �����Ҽ��� ���� ������
                        float critical = (player.pCriScript != null) ? player.pCriScript.Critical() : 1f;
                        float damage = player.isZoom ? bulletZoomDamage : bulletDamage;
                        float totalAddDamage = (damage * chargeScript.DamageUPRate) * (chargeScript.curChargingGauge / chargeScript.maxChargingGauge);
                        addtionDamage += totalAddDamage;
                        GameObject go = ObjectPoolingManager.Instance.GetBulletQueue();
                        go.GetComponent<PhotonView>().RPC("BulletAttribute", RpcTarget.All, Knockback, kbTime, KbResistacne, KbPower, Slow, STime, SSpeed, CC, ccTime, penetrate);
                        go.GetComponent<PhotonView>().RPC("DDRPC", RpcTarget.All, FirePos.position, player.dir, damage + (player.level == 1 ? 0 : (player.level - 1) * LvDamagePercent) + player.addtionDamage, addtionDamage, player.DSkinIndex, player.myTeam, critical);
                        addtionDamage -= totalAddDamage;

                        PV.RPC("GetGESound", RpcTarget.AllBufferedViaServer);
                        PV.RPC("GetQSound", RpcTarget.AllBufferedViaServer, "ShotPart", 0, bulletSoundVol, FirePos.position, false);
                        AfterShoot();
                    }
                }
            }
        }
    }
}
