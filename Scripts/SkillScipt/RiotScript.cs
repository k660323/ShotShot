using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiotScript : SkillScript, IPunObservable
{
    float totalDamage;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(weaponObject.activeSelf);
        }
        else
        {
            weaponObject.SetActive((bool)stream.ReceiveNext());
        }
    }

    public override void Init(bool isSetCool = true)
    {
        if (!player.isDead && !player.isCC)
            player.isControll = true;

        if (isActive)
        {
            isActive = false;
            skillActive.SetActive(false);
            weaponObject.SetActive(false);
            if (isSetCool)
                SetCoolTime();
        }

        if (skill != null)
        {
            StopCoroutine(skill);
            skill = null;
        }
    }

    protected override void Update()
    {
        if (player.PV.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (!isActive && !player.isSkillActive && !player.Gun.isReload && player.isControll && !player.isCC && !player.isResistance && !player.isDead)
                {
                    if (player.level >= skillUsedLevel)
                    {
                        if (curCool == maxCool)
                        {
                            RiotMode();
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

    void RiotMode()
    {
        player.isControll = false;
        player.AN.SetBool("isWalk", false);
        totalDamage = skillDamage + ((player.level <= skillUsedLevel) ? 0 : (player.level - skillUsedLevel) * addtionRate);
        weaponObject.GetComponent<PhotonView>().RPC("Init", RpcTarget.AllViaServer, totalDamage, player.myTeam, player.DSkinIndex);
        isActive = true;
        skillActive.SetActive(true);

        PV.RPC("GetQSound", RpcTarget.All,0,15f,transform.position);
        if (skill == null)
            skill = Riot();
        StartCoroutine(skill);
    }

    IEnumerator Riot()
    {
        for (int i = 0; i < 20; i++)
        {
            if(player.isDead || player.isCC)
            {
                break;
            }
            weaponObject.SetActive(true);
            player.PV.RPC("FlipX", RpcTarget.AllViaServer, player.SR.flipX ? 0f : 1f);

            player.WeaponPos.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
            PV.RPC("GetGESound", RpcTarget.All);

            yield return new WaitForSeconds(0.1f);
            weaponObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
        }
        Init();
    }

    [PunRPC]
    protected void GetGESound()
    {
        GameObject s_Object = ObjectPoolingManager.Instance.GetGunEffectQueue();
        s_Object.transform.parent = player.WeaponPos.transform;
        s_Object.transform.rotation = player.WeaponPos.transform.rotation;
        s_Object.transform.position = player.Gun.FirePos.position;
        s_Object.SetActive(true);
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
