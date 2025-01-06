using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImprovedstaminaScript : SkillScript
{
    public float hpAmount;

    private void Start()
    {
        Improvedstamina();
    }

    public void Improvedstamina()
    {
        if (!isActive)
        {
            if (player.level >= skillUsedLevel)
            {
                isActive = true;
                player.maxHpProperti += hpAmount;
                player.curHpProperti = player.maxHp;
            }
        }   
    }
}
