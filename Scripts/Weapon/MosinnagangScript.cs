using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MosinnagangScript : GunWeapon
{
    protected override void Update()
    {
        if (player.PV.IsMine)
        {
            if (player.isControll && !player.isCC && !player.isDead)
            {
                Zoom();
                Shot();
                Reload();
            }
        }
    }

    protected override void Zoom()
    {
        if (Input.GetMouseButtonDown(1) && isUseZoom)
        {
            if (player.Gun.gameObject.activeSelf)
            {
                player.isZoom = !player.isZoom;

                if (player.isZoom)
                {
                    zoomSpeed = player.moveSpeed * zoomDownPercent;
                    player.addtionSpeed -= zoomSpeed;
                    isChargeMode = true;
                }
                else
                {
                    player.addtionSpeed += zoomSpeed;
                    if (isChargeMode)
                    {
                        isChargeMode = false;
                        chargeScript.ChargingStop();
                    }
                     
                }
            }
        }

        if (player.isZoom)
        {
            CMManager.Instance().virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(CMManager.Instance().virtualCamera.m_Lens.OrthographicSize, maxRange, LerpSpeed * Time.deltaTime);
        }
        else
        {
            CMManager.Instance().virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(CMManager.Instance().virtualCamera.m_Lens.OrthographicSize, minRange, LerpSpeed * Time.deltaTime);
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

                            GameObject go = ObjectPoolingManager.Instance.GetBulletQueue();
                            go.GetComponent<PhotonView>().RPC("BulletAttribute", RpcTarget.All, Knockback, kbTime, KbResistacne, KbPower, Slow, STime, SSpeed, CC, ccTime,penetrate);
                            go.GetComponent<PhotonView>().RPC("DDRPC", RpcTarget.AllViaServer, FirePos.position, player.dir, damage + (player.level == 1 ? 0 : (player.level - 1) * LvDamagePercent) + player.addtionDamage, addtionDamage, player.DSkinIndex, player.myTeam, critical);

                            PV.RPC("GetGESound", RpcTarget.AllViaServer);
                            PV.RPC("GetQSound", RpcTarget.AllViaServer, "ShotPart", 0, bulletSoundVol, FirePos.position, false);
                            AfterShoot();
                            Invoke("AmmoOut", 1.0f);
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
                        curAmmo -= 5; // rate +addtionRate  3 - 2.8 = 0.2  5초딜 최대한 0에 수렴할수록 공속 느려짐
                        float critical = (player.pCriScript != null) ? player.pCriScript.Critical() : 1f;
                        float damage = player.isZoom ? bulletZoomDamage : bulletDamage;
                        float totalAddDamage = (damage * chargeScript.DamageUPRate) * (chargeScript.curChargingGauge / chargeScript.maxChargingGauge);
                        addtionDamage += totalAddDamage;
                        GameObject go = ObjectPoolingManager.Instance.GetBulletQueue();
                        go.GetComponent<PhotonView>().RPC("BulletAttribute", RpcTarget.All, Knockback, kbTime, KbResistacne, KbPower, Slow, STime, SSpeed, CC, ccTime, penetrate);
                        go.GetComponent<PhotonView>().RPC("DDRPC", RpcTarget.AllViaServer, FirePos.position, player.dir, damage + (player.level == 1 ? 0 : (player.level - 1) * LvDamagePercent) + player.addtionDamage, addtionDamage, player.DSkinIndex, player.myTeam, critical);
                        addtionDamage -= totalAddDamage;

                        PV.RPC("GetGESound", RpcTarget.AllViaServer);
                        PV.RPC("GetQSound", RpcTarget.AllViaServer, "ShotPart", 0, bulletSoundVol, FirePos.position, false);
                        AfterShoot();
                        Invoke("AmmoOut", 1.0f);
                    }
                }
            }          
        }       
    }

    void AmmoOut()
    {
        PV.RPC("GetQSound", RpcTarget.All, "AmmoOutPart", 0, AmmoOutSoundVol, transform.position, true);
    }
}
