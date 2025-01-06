using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniffScript : CharacterScript
{
    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnPhotonSerializeView(stream, info);
        if (stream.IsWriting)
        {
            stream.SendNext(WeaponPos.transform.GetChild(0).gameObject.activeSelf);
            stream.SendNext(WeaponPos.transform.GetChild(1).gameObject.activeSelf);
            stream.SendNext(WeaponPos.transform.GetChild(2).gameObject.activeSelf);
            stream.SendNext(WeaponPos.transform.GetChild(2).GetChild(0).gameObject.activeSelf);
        }
        else
        {
            WeaponPos.transform.GetChild(0).gameObject.SetActive((bool)stream.ReceiveNext());
            WeaponPos.transform.GetChild(1).gameObject.SetActive((bool)stream.ReceiveNext());
            WeaponPos.transform.GetChild(2).gameObject.SetActive((bool)stream.ReceiveNext());
            WeaponPos.transform.GetChild(2).GetChild(0).gameObject.SetActive((bool)stream.ReceiveNext());
        }
    }

    protected override void Start()
    {
        DSkinIndex = 3;

        curHpProperti = 90f;
        maxHpProperti = 90f;
        maxHpUpRate = 0.065f;
        isDead = false;

        curEXP = 0;
        maxEXP = 50;
        maxExpUpRate = 0.1f;

        XP = 50;
        xpUpRate = 0.05f;

        maxLevel = 20;
        level = 1;

        kill = 0;

        moveSpeed = 4.8f;
        addtionSpeed = 0f;
        isControll = true;

        addtionDamage = 0f;

        if (PV.IsMine)
            StartCoroutine(smallUP());
    }   
}
