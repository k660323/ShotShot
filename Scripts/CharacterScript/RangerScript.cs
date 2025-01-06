using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangerScript : CharacterScript
{
    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnPhotonSerializeView(stream, info);
        if (stream.IsWriting)
        {
            stream.SendNext(WeaponPos.transform.GetChild(0).gameObject.activeSelf);
        }
        else
        {
            WeaponPos.transform.GetChild(0).gameObject.SetActive((bool)stream.ReceiveNext());
        }
    }

    protected override void Start()
    {
        DSkinIndex = 4;

        curHpProperti = 75f;
        maxHpProperti = 75f;
        maxHpUpRate = 0.055f;
        isDead = false;

        curEXP = 0;
        maxEXP = 50;
        maxExpUpRate = 0.1f;

        XP = 50;
        xpUpRate = 0.05f;

        maxLevel = 20;
        level = 1;

        kill = 0;

        moveSpeed = 5f;
        addtionSpeed = 0f;
        isControll = true;

        addtionDamage = 0f;

        if (PV.IsMine)
            StartCoroutine(smallUP());
    }
}
