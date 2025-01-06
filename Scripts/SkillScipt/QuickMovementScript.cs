using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickMovementScript : SkillScript
{
    public SkillScript skillScript;

    public float addReloadTimeSpeed;
    public float addReloadMoveSpeed;

    public bool isReloadSUp;
    public bool isResetCTime;
    private void Start()
    {
        ReloadSpeedUP();
    }

    public void ReloadSpeedUP()
    {
        if (isReloadSUp)
        {
            if (player.level >= skillUsedLevel)
            {
                isReloadSUp = false;
                player.Gun.reloadTime -= addReloadTimeSpeed;
                player.Gun.reloadingMoveSpeedRate -= addReloadMoveSpeed;
            }
        }    
    }

    public void ResetCoolTime()
    {
        if (isResetCTime)
        {
            if (player.level >= skillUsedLevel)
            {
                skillScript.coolStart = false;
                skillScript.curCool = skillScript.maxCool;
                skillScript.coolTime.fillAmount = 0f;
                skillScript.coolText.text = "";
                skillScript.isActive = false;
            }
        }      
    }
}
