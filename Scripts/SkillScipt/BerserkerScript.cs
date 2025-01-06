using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkerScript : SkillScript
{
    public bool[] isActives = new bool[4]; // 70% 50% 30% 10%
    public override void Init(bool isSetCool = true)
    {
        for (int i = 0; i < isActives.Length; i++)
            isActives[i] = false;
        addtionRate = 0f;
    }

    private void Start()
    {
        if(player.level >= skillUsedLevel)
        {
            Berserker();
        }
    }

    public void Berserker()
    {
        if (player.level >= skillUsedLevel)
        {
            if (0.5f < player.curHp / player.maxHp && player.curHp / player.maxHp <= 0.7f && !isActives[0])
            {
                isActives[0] = true;
                player.addtionDamage -= addtionRate;
                addtionRate = (player.Gun.bulletDamage * 0.1f);
                player.addtionDamage += addtionRate;
                if (!player.isStatusHit)
                    player.SR.color = new Color(1f, 200f / 255f, 200f / 255f);
            }
            else if (0.3f < player.curHp / player.maxHp && player.curHp / player.maxHp <= 0.5f && !isActives[1])
            {
                for (int i = 0; i < isActives.Length - 2; i++)
                    isActives[i] = true;
                player.addtionDamage -= addtionRate;
                addtionRate = (player.Gun.bulletDamage * 0.2f);
                player.addtionDamage += addtionRate;
                if (!player.isStatusHit)
                    player.SR.color = new Color(1f, 150f / 255f, 150f / 255f);
            }
            else if (0.1f < player.curHp / player.maxHp && player.curHp / player.maxHp <= 0.3f && !isActives[2])
            {
                for (int i = 0; i < isActives.Length - 1; i++)
                    isActives[i] = true;
                player.addtionDamage -= addtionRate;
                addtionRate = (player.Gun.bulletDamage * 0.4f);
                player.addtionDamage += addtionRate;
                if (!player.isStatusHit)
                    player.SR.color = new Color(1f, 100f / 255f, 100f / 255f);
            }
            else if (0f < player.curHp / player.maxHp && player.curHp / player.maxHp <= 0.1f && !isActives[3])
            {
                for (int i = 0; i < isActives.Length; i++)
                    isActives[i] = true;
                player.addtionDamage -= addtionRate;
                addtionRate = (player.Gun.bulletDamage * 0.6f);
                player.addtionDamage += addtionRate;
                if (!player.isStatusHit)
                    player.SR.color = new Color(1f, 50f / 255f, 50f / 255f);
            }
        }
    }

    public void BerserkerHeal()
    {
        if (player.level >= skillUsedLevel)
        {
            if (0.7f < player.curHp / player.maxHp && isActives[0])
            {
                isActives[0] = false;
                player.addtionDamage -= addtionRate; // 0.1 [0]
                addtionRate = 0f;
                if (!player.isStatusHit)
                    player.SR.color = new Color(1f, 1f, 1f);
            }
            else if (0.5f < player.curHp / player.maxHp && player.curHp / player.maxHp <= 0.7f && isActives[1])
            {
                for (int i = 1; i < isActives.Length; i++)
                    isActives[i] = false;
                player.addtionDamage -= addtionRate; // 0.3 [1]
                addtionRate = (player.Gun.bulletDamage * 0.1f); // 0.1 [0]
                player.addtionDamage += addtionRate;
                if (!player.isStatusHit)
                    player.SR.color = new Color(1f, 200f / 255f, 200f / 255f);
            }
            else if (0.3f < player.curHp / player.maxHp && player.curHp / player.maxHp <= 0.5f && isActives[2])
            {
                for (int i = 2; i < isActives.Length; i++)
                    isActives[i] = false;
                player.addtionDamage -= addtionRate; // 0.5 [2]
                addtionRate = (player.Gun.bulletDamage * 0.3f); // 0.3 [1]
                player.addtionDamage += addtionRate;
                if (!player.isStatusHit)
                    player.SR.color = new Color(1f, 150f / 255f, 150f / 255f);
            }
            else if (0.1f < player.curHp / player.maxHp && player.curHp / player.maxHp <= 0.3f && isActives[3])
            {
                isActives[3] = false;
                player.addtionDamage -= addtionRate; // 0.7 [3]
                addtionRate = (player.Gun.bulletDamage * 0.5f); // 0.5 [2]
                player.addtionDamage += addtionRate;
                if (!player.isStatusHit)
                    player.SR.color = new Color(1f, 100f / 255f, 100f / 255f);
            }
        }
    }
}
