using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgilityScript : SkillScript
{
    public float speedAmount;

    public void Start()
    {
        if (PV.IsMine)
            Agility();
    }

    public void Agility()
    {
        if (!isActive)
        {
            if (player.level >= skillUsedLevel)
            {
                isActive = true;
                player.moveSpeed += speedAmount;
            }
        }
    }
}
