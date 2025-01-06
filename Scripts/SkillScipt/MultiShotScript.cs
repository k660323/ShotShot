using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiShotScript : SkillScript
{
    public float defaultAngle; 
    float startAngle;
    public bool penetrate;
    protected override void Update()
    {
        if (player.PV.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.F)) //¸ÖÆ¼¼¦
            {
                if (!player.isSkillActive && !player.Gun.isReload && player.isControll && !player.isCC && !player.isDead)
                {
                    if (player.level >= skillUsedLevel)
                    {
                        startAngle = defaultAngle;
                        if (curCool == maxCool)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                Quaternion q = Quaternion.AngleAxis(startAngle, Vector3.forward);
                                Vector2 v2Dir = (q * player.dir).normalized;
                                float critical = (player.pCriScript != null) ? player.pCriScript.Critical() : 1f;
                                float totalDamage = skillDamage + ((player.level <= skillUsedLevel) ? 0 : (player.level - skillUsedLevel) * addtionRate);
                                GameObject go = ObjectPoolingManager.Instance.GetBulletQueue();
                                go.GetComponent<PhotonView>().RPC("BulletAttribute", RpcTarget.All, Knockback, kbTime, KbResistacne, KbPower, Slow, STime, SSpeed, CC, ccTime, penetrate);
                                go.GetComponent<PhotonView>().RPC("DDRPC", RpcTarget.AllViaServer, player.Gun.FirePos.position, v2Dir, totalDamage + player.addtionDamage, 0.0f, player.DSkinIndex, player.myTeam, critical);
                                PV.RPC("GetQSound", RpcTarget.AllViaServer, Random.Range(0, audioClips.Length), bulletSoundVol, transform.position, false);
                                startAngle -= 30f;
                            }
                          
                            PV.RPC("GetQSound", RpcTarget.AllViaServer, 0, bulletSoundVol, player.Gun.FirePos.position, false);
                            SetCoolTime();
                        }
                    }
                }
            }

            CoolTime();
        }
    }
}
