using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormArrowScript : SkillScript
{
    float temp;
    public float decreaseMoveRate;
    public bool penetrate;

    public override void Init(bool isSetCool = true)
    {
        if (skill != null)
        {
            StopCoroutine(skill);
            skill = null;
        }
        if (isActive)
        {
            isActive = false;
            player.addtionSpeed += temp;
            skillActive.SetActive(false);
            if (isSetCool)
                SetCoolTime();
        }
    }

    protected override void Update()
    {
        if (player.PV.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Q)) // ÆøÇ³ È­»ì
            {
                if (!player.isSkillActive && !player.Gun.isReload && player.isControll && !player.isCC && !player.isDead)
                {
                    if (player.level >= skillUsedLevel)
                    {
                        if (curCool == maxCool)
                        {
                            Storm();
                            
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

    protected void Storm()
    {
        isActive = true;
        skillActive.SetActive(true);

        if (skill == null)
            skill = StartStormArrow();
        StartCoroutine(skill);
    }

    IEnumerator StartStormArrow()
    {
        temp = player.moveSpeed * decreaseMoveRate;
        player.addtionSpeed -= temp;
        for (int i = 0; i < 30; i++)
        {
            if (player.isDead || player.isCC)
            {
                Init();
                break;
            }

            float critical = (player.pCriScript != null) ? player.pCriScript.Critical() : 1f;
            GameObject go = ObjectPoolingManager.Instance.GetBulletQueue();
            go.GetComponent<PhotonView>().RPC("BulletAttribute", RpcTarget.All, Knockback, kbTime, KbResistacne, KbPower, Slow, STime, SSpeed, CC, ccTime, penetrate);
            go.GetComponent<PhotonView>().RPC("DDRPC", RpcTarget.AllViaServer, player.Gun.FirePos.position, player.dir, skillDamage + (player.level <= skillUsedLevel ? 0 : (player.level - skillUsedLevel) * addtionRate) + player.addtionDamage, 0.0f, player.DSkinIndex, player.myTeam, critical);
            PV.RPC("GetQSound", RpcTarget.AllViaServer, Random.Range(0,audioClips.Length), bulletSoundVol, transform.position, false);
            yield return new WaitForSeconds(0.2f);
           
        }
        skill = null;
        Init();
    }
}
