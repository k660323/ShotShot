using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DumplingScript : SkillScript
{
    protected override void Update()
    {
        if (player.PV.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Space)) //´ýºí¸µ
            {
                if (player.h != 0 || player.v != 0 && player.isControll && !player.isCC && !player.isResistance && !player.isDead)
                {
                    if (player.level >= skillUsedLevel)
                    {
                        if (curCool == maxCool)
                        {
                            PV.RPC("GetQSound", RpcTarget.AllViaServer, 0, 7f, transform.position, false);
                            StartCoroutine(ActiveDump(player.h, player.v));
                        }
                    }

                }
            }

            CoolTime();
        }
        
    }

    public IEnumerator ActiveDump(float h, float v)
    {
        player.Gun.ZoomCancle();

        player.isPhysics = true;
        isActive = true;
        SetCoolTime();
        player._RB.AddForce(new Vector2(h, v).normalized * 12f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.25f);
        if (player.PhysicsLock == 0)
            player.isPhysics = false;
        isActive = false;
    }
}
