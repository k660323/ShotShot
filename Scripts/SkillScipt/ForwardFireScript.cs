using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardFireScript : SkillScript, IPunObservable
{
    float totalDamage;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(weaponObject.activeSelf);
            stream.SendNext(weaponObject.GetComponent<FWHitScript>().isKnockBack);
            stream.SendNext((Vector2)weaponObject.transform.position);
            stream.SendNext(weaponObject.transform.rotation);
        }
        else
        {
            weaponObject.SetActive((bool)stream.ReceiveNext());
            weaponObject.GetComponent<FWHitScript>().isKnockBack = (bool)stream.ReceiveNext();
            weaponObject.transform.position = (Vector2)stream.ReceiveNext();
            weaponObject.transform.rotation = (Quaternion)stream.ReceiveNext();
        }
    }

    public override void Init(bool isSetCool = true)
    {
        if (!PV.IsMine)
            return;

        if (skill != null)
        {
            StopCoroutine(skill);
            skill = null;
        }

        if (!player.isDead && !player.isCC)
            player.isControll = true;

        if (isActive)
        {
            isActive = false;
            skillActive.SetActive(false);
            weaponObject.GetComponent<FWHitScript>().isKnockBack = false;
            weaponObject.SetActive(false);
            if (isSetCool)
                SetCoolTime();
        }
          
    }


    protected override void Update()
    {
        if (player.PV.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!isActive && !player.isSkillActive && !player.Gun.isReload && player.isControll && !player.isCC && !player.isResistance && !player.isDead)
                {
                    if (player.level >= skillUsedLevel)
                    {
                        if (curCool == maxCool)
                        {
                            ForwardFireMode();
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

    void ForwardFireMode()
    {
        player.isControll = false;
        player.AN.SetBool("isWalk", false);
        weaponObject.transform.position = (Vector2)transform.position + (player.dir * 2.5f);
        weaponObject.transform.rotation = player.WeaponPos.transform.rotation;
        totalDamage = skillDamage + ((player.level <= skillUsedLevel) ? 0 : (player.level - skillUsedLevel) * addtionRate);
        weaponObject.GetComponent<PhotonView>().RPC("Init", RpcTarget.AllViaServer, totalDamage, player.myTeam, player.DSkinIndex);
        isActive = true;
        skillActive.SetActive(true);

        if (skill == null)
            skill = FWCoroutine();
        StartCoroutine(skill);
    }

    IEnumerator FWCoroutine()
    {
        for (int i = 0; i < 10; i++)
        {
            if (i == 9)
                weaponObject.GetComponent<FWHitScript>().isKnockBack = true;
            weaponObject.SetActive(true);
            PV.RPC("GetQSound", RpcTarget.All, 0, 10f, player.Gun.FirePos.position);
            yield return new WaitForSeconds(0.1f);
            weaponObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
        }

        skill = null;
        Init();
    }

    [PunRPC]
    protected void GetQSound(int index, float maxRange, Vector3 startPos)
    {
        GameObject s_Object = ObjectPoolingManager.Instance.GetSQueue();
        s_Object.GetComponent<AudioSource>().clip = audioClips[index];
        s_Object.GetComponent<AudioSource>().maxDistance = maxRange;
        s_Object.transform.position = startPos;
        s_Object.SetActive(true);
    }
}
