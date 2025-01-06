using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialArrowScript : SkillScript
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

        if (!player.isDead && !player.isCC)
            player.isControll = true;

        if (isActive)
        {
            isActive = false;
            skillActive.SetActive(false);
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
                if (!isActive && !player.isSkillActive && !player.Gun.isReload && player.isControll && !player.isCC && !player.isResistance && !player.isDead)
                {
                    if (player.level >= skillUsedLevel)
                    {
                        if (curCool == maxCool)
                        {
                            // PV.RPC("GetQSound", RpcTarget.AllBufferedViaServer, 0, 7f, transform.position, true);
                            ShootSky();
                        }
                    }
                }             
            }
            CoolTime();
        }
    }

    void ShootSky()
    {
        player.isControll = false;
        player.AN.SetBool("isWalk", false);

        isActive = true;
        skillActive.SetActive(true);

        player.WeaponPos.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 180));
        if (skill == null)
            skill = CreateSkyArrow();
        StartCoroutine(skill);
    }

    IEnumerator CreateSkyArrow()
    {
        // 발사 효과
        for (int i = 0; i < 10; i++)
        {
            player.Gun.GetComponent<PhotonView>().RPC("GetGESound", RpcTarget.AllBufferedViaServer);
            PV.RPC("GetQSound", RpcTarget.AllViaServer, Random.Range(0, audioClips.Length), bulletSoundVol, transform.position, false);
            yield return new WaitForSeconds(0.1f);
        }
        Vector3 CreatePos = player.transform.position + (player.SR.flipX ? Vector3.right * 5 : Vector3.left * 5);
        skill = null;
        Init();
        yield return new WaitForSeconds(1.0f);
        // 피격 발판 생성 및 화살 생성
        GameObject go = PhotonNetwork.Instantiate("Weapon/ArrowZone", CreatePos, Quaternion.identity);
        go.GetComponent<PhotonView>().RPC("InitDamage",RpcTarget.AllViaServer, skillDamage + (player.level - skillUsedLevel) * addtionRate);
    }
}