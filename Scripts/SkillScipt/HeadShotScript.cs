using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadShotScript : SkillScript
{
    public bool penetrate;

    protected override void Update()
    {
        if (player.PV.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (!player.isSkillActive && !player.Gun.isReload && player.isControll && !player.isCC && !player.isDead)
                {
                    if (player.level >= skillUsedLevel)
                    {
                        if (curCool == maxCool)
                        {
                            float critical = (player.pCriScript != null) ? player.pCriScript.Critical() : 1f;
                            float totalDamage = skillDamage + ((player.level <= skillUsedLevel) ? 0 : (player.level - skillUsedLevel) * addtionRate);
                            GameObject go = ObjectPoolingManager.Instance.GetBulletQueue();
                            go.GetComponent<PhotonView>().RPC("BulletAttribute", RpcTarget.AllViaServer, Knockback, kbTime, KbResistacne, KbPower, Slow, STime, SSpeed, CC, ccTime, penetrate);
                            go.GetComponent<PhotonView>().RPC("DDRPC", RpcTarget.AllViaServer, player.Gun.FirePos.position, player.dir, totalDamage, player.Gun.addtionDamage, player.DSkinIndex, player.myTeam, critical);
                            PV.RPC("GetGESound", RpcTarget.AllViaServer);
                            PV.RPC("GetQSound", RpcTarget.AllViaServer, 0, bulletSoundVol, player.Gun.FirePos.position, false);
                            SetCoolTime();
                        }
                    }             
                }
            }          

            CoolTime();
        }
    }

    [PunRPC]
    protected void GetGESound()
    {
        GameObject s_Object = ObjectPoolingManager.Instance.GetGunEffectQueue();
        s_Object.transform.parent = player.WeaponPos.transform;
        s_Object.transform.rotation = player.WeaponPos.transform.rotation;
        s_Object.transform.position = player.Gun.FirePos.position;
        s_Object.SetActive(true);
    }
}
