using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PumpScript : GunWeapon
{
    protected override void Update()
    {
        if (player.PV.IsMine)
        {
            if (player.isControll && !player.isCC &&!player.isDead)
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
                            curCool = 1f / (atkRate + addtionATKRate); // rate +addtionRate 높게 나오면 공속up 정수 // rate + addtionRate 낮게 나오면 공속down 
                            curAmmo -= 1; // rate +addtionRate  3 - 2.8 = 0.2  5초딜 최대한 0에 수렴할수록 공속 느려짐
                            float critical = (player.pCriScript != null) ? player.pCriScript.Critical() : 1f;
                            float damage = player.isZoom ? bulletZoomDamage : bulletDamage;
                            startAngle = defaultAngle;
                            for (int i = 0; i < bulletCount; i++)
                            {
                                qRoate = Quaternion.AngleAxis(startAngle, Vector3.forward);
                                v2Dir = (qRoate * player.dir).normalized;

                                GameObject go = ObjectPoolingManager.Instance.GetBulletQueue();
                                go.GetComponent<PhotonView>().RPC("BulletAttribute", RpcTarget.All, Knockback, kbTime, KbResistacne, KbPower, Slow, STime, SSpeed, CC, ccTime, penetrate);
                                go.GetComponent<PhotonView>().RPC("DDRPC", RpcTarget.All, FirePos.position, v2Dir, damage + (player.level == 1 ? 0 : (player.level - 1) * LvDamagePercent) + player.addtionDamage, addtionDamage, player.DSkinIndex, player.myTeam, critical);

                                startAngle += addAngle;
                            }
                            PV.RPC("GetGESound", RpcTarget.All);
                            PV.RPC("GetQSound", RpcTarget.All, "ShotPart", 0, bulletSoundVol, FirePos.position, false);
                            AfterShoot();
                        }
                        else if (curAmmo == 0)
                        {
                            if (!isReload)
                            {
                                reloadCoroutine = Reloading();
                                StartCoroutine(reloadCoroutine);
                            }
                        }
                    }
                }
            }
            else
            {
                if (!isReload && isFireReady && !player.isSkillReady && !player.isSkillActive)
                {
                    if (Input.GetMouseButtonDown(0) && curAmmo > 0) // 차징
                    {
                        chargeScript.ChargingStart();
                    }
                    else if (Input.GetMouseButton(0) && curAmmo == 0)
                    {
                        reloadCoroutine = Reloading();
                        StartCoroutine(reloadCoroutine);
                    }

                    if (Input.GetMouseButtonUp(0) && chargeScript.isActive) // 샷
                    {
                        chargeScript.ChargingStop();
                        curCool = 1f / (atkRate + addtionATKRate); // rate +addtionRate 높게 나오면 공속up 정수 // rate + addtionRate 낮게 나오면 공속down 
                        curAmmo -= 1; // rate +addtionRate  3 - 2.8 = 0.2  5초딜 최대한 0에 수렴할수록 공속 느려짐
                        startAngle = defaultAngle;
                        float critical = (player.pCriScript != null) ? player.pCriScript.Critical() : 1f;
                        float damage = player.isZoom ? bulletZoomDamage : bulletDamage;

                        float totalAddDamage = (damage * chargeScript.DamageUPRate) * (chargeScript.curChargingGauge / chargeScript.maxChargingGauge);
                        addtionDamage += totalAddDamage;

                        for (int i = 0; i < bulletCount; i++)
                        {
                            qRoate = Quaternion.AngleAxis(startAngle, Vector3.forward);
                            v2Dir = (qRoate * player.dir).normalized;

                            GameObject go = ObjectPoolingManager.Instance.GetBulletQueue();
                            go.GetComponent<PhotonView>().RPC("BulletAttribute", RpcTarget.All, Knockback, kbTime, KbResistacne, KbPower, Slow, STime, SSpeed, CC, ccTime, penetrate);
                            go.GetComponent<PhotonView>().RPC("DDRPC", RpcTarget.All, FirePos.position, v2Dir, damage + (player.level == 1 ? 0 : (player.level - 1) * LvDamagePercent) + player.addtionDamage, addtionDamage, player.DSkinIndex, player.myTeam, critical);

                            startAngle += addAngle;
                        }

                        addtionDamage -= totalAddDamage;
                        PV.RPC("GetGESound", RpcTarget.All);
                        PV.RPC("GetQSound", RpcTarget.All, "ShotPart", 0, bulletSoundVol, FirePos.position, false);
                        AfterShoot();
                    }
                }
            }


            if (Input.GetMouseButtonDown(0)) // 장전중 캔슬 가능
            {
                if (isReload && curAmmo != 0)
                    ReloadingCancel();
            }
        }
    }

    protected override IEnumerator Reloading()
    {
        isReload = true;
        reloadMoveValue = player.moveSpeed * reloadingMoveSpeedRate;
        player.addtionSpeed -= reloadMoveValue;
        player.AN.SetFloat("ReloadSpeed", 1 / (reloadTime + addtionReloadTime));
        while (true)
        {
            if (curAmmo < maxAmmo)
            {
                player.PV.RPC("TriggerAnim", RpcTarget.AllViaServer, "isReload");
                yield return new WaitForSeconds(reloadTime + addtionReloadTime);               
                curAmmo++;
                PV.RPC("GetQSound", RpcTarget.All, "ReloadPart", 0, 8f, transform.position, false);
            }
            else
            {
                yield return new WaitForSeconds(0.2f);
                break;
            }
        }
        player.PV.RPC("TriggerAnim", RpcTarget.AllViaServer, "isReloadOut");
        player.addtionSpeed += reloadMoveValue;
        isReload = false;
    }
}
