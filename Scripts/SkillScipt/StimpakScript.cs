using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StimpakScript : SkillScript
{
    float moveDRate;
    float atkDRate;
    float reloadDRate;

    float TMPSpeed;
    float TMPReloadTime;
    float TMPATKRate;

    void Start()
    {
        moveDRate = 0.5f;
        atkDRate = 0.5f;
        reloadDRate = 0.5f;

        addtionRate = 0.05f;
    }

    public override void Init(bool isSetCool = true)
    {
        if (skill != null)
        {
            StopCoroutine(skill);
            skill = null;
        }
        if(isActive)
        {
            isActive = false;
            skillActive.SetActive(false);
            player.addtionSpeed -= TMPSpeed;// 50% ����
            player.Gun.addtionATKRate -= TMPATKRate; //50% ����
            player.Gun.addtionReloadTime += TMPReloadTime; // 50% ����
            if (isSetCool)
                SetCoolTime();
        }
    }

    protected override void Update()
    {
        if (player.PV.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (!isActive && !player.isSkillReady && !player.isSkillActive && !player.Gun.isReload && player.isControll && !player.isCC && !player.isDead)
                {
                    if (player.level >= skillUsedLevel)
                    {
                        if (curCool == maxCool)
                        {
                            PV.RPC("GetQSound", RpcTarget.AllViaServer, Random.Range(0, 2), 15f, transform.position, true);
                            if (skill == null)
                                skill = Stimpak();
                            StartCoroutine(skill);
                        }
                    }
                }
            }
            CoolTime();
        }

    }
    public IEnumerator Stimpak()
    {
        isActive = true;
        skillActive.SetActive(true);
        // �⺻ rate���� �������� ��� �϶� // addtion�� �Ͻ����� ���
        float levelPlus = (player.level <= skillUsedLevel ? 0 : (player.level - skillUsedLevel) * addtionRate);
        TMPSpeed = player.moveSpeed * moveDRate + levelPlus; 
        TMPATKRate = player.Gun.atkRate * atkDRate; 
        TMPReloadTime = player.Gun.reloadTime * reloadDRate + levelPlus; 

        player.addtionSpeed += TMPSpeed;// 50% ����
        player.Gun.addtionATKRate += TMPATKRate; // 50% ����
        player.Gun.addtionReloadTime -= TMPReloadTime; // 50% ����
        yield return new WaitForSeconds(10.0f);
        skill = null;
        Init();
    }
}
