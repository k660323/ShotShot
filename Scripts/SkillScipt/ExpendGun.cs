using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpendGun : SkillScript
{
    public int addMaxAmmo;

    public bool isCharging;
    public bool isExpandAmmo;

    private void Start()
    {
        ExpandAmmo();
        ChargingSetting();
    }

    public void ExpandAmmo()
    {
        if (isExpandAmmo)
        {
            if (player.level >= skillUsedLevel)
            {
                isExpandAmmo = false;
                player.Gun.maxAmmo += addMaxAmmo;
            }
        }
    }

    public void ChargingSetting()
    {
        if (isCharging)
        {
            if (player.level >= skillUsedLevel)
            {
                player.Gun.isChargeMode = true;
            }
        }
    }
}
