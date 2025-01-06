using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealPackSpawnerScript : MonoBehaviour
{
    [SerializeField]
    GameObject healPackObject;

    [SerializeField]
    Image respawnGauge;

    public float reActiveCool;
    public float curCool;

    // Update is called once per frame
    void Update()
    {
        if (healPackObject == null)
            return;

        if (!healPackObject.activeSelf)
        {
            if (reActiveCool > curCool)
            {
                curCool += Time.deltaTime;
                respawnGauge.fillAmount = curCool / reActiveCool;
            }

            if (reActiveCool <= curCool)
            {
                healPackObject.SetActive(true);
                curCool = 0f;
                respawnGauge.fillAmount = curCool / reActiveCool;
            }
        }
    }
}
