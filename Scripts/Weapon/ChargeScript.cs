using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeScript : MonoBehaviour
{
    Image GaugeBar;
    CharacterScript player;
    public bool isActive;

    public float cSpeed;

    public float curChargingGauge;
    public float maxChargingGauge;

    float TotalDecMoveSpeed;
    public float decreaseMoveRate;
    public float DamageUPRate = 1;
    public void Init()
    {
        if (isActive)
        {
            GaugeBar.transform.parent.gameObject.SetActive(false);
            isActive = false;
            player.addtionSpeed += TotalDecMoveSpeed;
        }
    }

    public void Start()
    {
        player = GetComponentInParent<CharacterScript>();
        GaugeBar = player.GaugeBar;

        GaugeBar.transform.parent.gameObject.SetActive(false);
    }

    public void ChargingStart()
    {
        if (isActive)
            ChargingStop();

        curChargingGauge = 0f;
        GaugeBar.transform.parent.gameObject.SetActive(true);
        isActive = true;
        TotalDecMoveSpeed = player.moveSpeed * decreaseMoveRate;
        player.addtionSpeed -= TotalDecMoveSpeed;
    }

    public void ChargingStop()
    {
        if (!isActive)
            return;

        isActive = false;
        GaugeBar.transform.parent.gameObject.SetActive(false);
        player.addtionSpeed += TotalDecMoveSpeed;
    }

    protected void Update()
    {
        if (isActive)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ChargingStop();
                return;
            }

            if (curChargingGauge < maxChargingGauge)
                curChargingGauge += cSpeed * Time.deltaTime;
            else if (curChargingGauge > maxChargingGauge)
                curChargingGauge = maxChargingGauge;

            GaugeBar.fillAmount = curChargingGauge / maxChargingGauge;
        }
    }
}
