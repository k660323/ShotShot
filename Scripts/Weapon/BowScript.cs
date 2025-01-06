using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BowScript : GunWeapon
{
    Animator anim;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    protected override void Update()
    {
        if (player.PV.IsMine)
        {
            if (player.isControll && !player.isCC && !player.isDead)
            {
                Shot();
            }
            else
            {
                anim.SetBool("isPull", false);
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
            if (isFireReady && !player.isSkillReady && !player.isSkillActive)
            {
                if (Input.GetMouseButtonDown(0) && curAmmo > 0) // 차징
                {
                    anim.SetBool("isPull", true);
                    chargeScript.ChargingStart();
                }
                else if (Input.GetMouseButton(0) && curAmmo == 0)
                {
                    anim.SetBool("isPull", false);
                    reloadCoroutine = Reloading();
                    StartCoroutine(reloadCoroutine);
                }

                if (Input.GetMouseButtonUp(0) && chargeScript.isActive) // 샷
                {
                    anim.SetBool("isPull", false);
                    chargeScript.ChargingStop();
                    curCool = 1f / (atkRate + addtionATKRate); // rate +addtionRate 높게 나오면 공속up 정수 // rate + addtionRate 낮게 나오면 공속down 

                    float chargePercent = (chargeScript.curChargingGauge / chargeScript.maxChargingGauge);
                    float critical = (player.pCriScript != null) ? player.pCriScript.Critical() : 1f; 
                    float damage = (player.isZoom ? bulletZoomDamage : bulletDamage);
                    float totalAddDamage = (damage * chargeScript.DamageUPRate) * chargePercent; // rate + addtionRate  3 - 2.8 = 0.2  5초딜 최대한 0에 수렴할수록 공속 느려짐
                    
                    addtionDamage += totalAddDamage;
                    GameObject go = ObjectPoolingManager.Instance.GetBulletQueue();
                    go.GetComponent<PhotonView>().RPC("BulletAttribute", RpcTarget.All, Knockback, kbTime, KbResistacne, KbPower, Slow, STime, SSpeed, CC, ccTime, penetrate);
                    go.GetComponent<PhotonView>().RPC("DDRPC", RpcTarget.All, FirePos.position, player.dir, damage + (player.level == 1 ? 0 : (player.level - 1) * LvDamagePercent) + player.addtionDamage, addtionDamage, player.DSkinIndex, player.myTeam, chargePercent, critical);
                    addtionDamage -= totalAddDamage;

                    PV.RPC("GetGESound", RpcTarget.AllBufferedViaServer);
                    PV.RPC("GetQSound", RpcTarget.AllBufferedViaServer, "ShotPart", 0, bulletSoundVol, FirePos.position, false);
                    AfterShoot();
                }
            }
        }
    }
}
