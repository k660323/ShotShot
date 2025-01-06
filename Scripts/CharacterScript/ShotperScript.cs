using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShotperScript : CharacterScript
{

    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnPhotonSerializeView(stream, info);
        if (stream.IsWriting)
        {
            stream.SendNext(WeaponPos.transform.GetChild(0).gameObject.activeSelf);
            stream.SendNext(WeaponPos.transform.GetChild(1).gameObject.activeSelf);
        }
        else
        {
            WeaponPos.transform.GetChild(0).gameObject.SetActive((bool)stream.ReceiveNext());
            WeaponPos.transform.GetChild(1).gameObject.SetActive((bool)stream.ReceiveNext());
        }
    }

    protected override void Start()
    {
        DSkinIndex = 2;

        curHpProperti = 100f;
        maxHpProperti = 100f;
        maxHpUpRate = 0.04f;
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
