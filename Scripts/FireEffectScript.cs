using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FireEffectScript : MonoBehaviour
{
    Animator AN;

    private void Awake()
    {
        AN = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        StartCoroutine(showEffect());
    }

    IEnumerator showEffect()
    {
        yield return new WaitForSeconds(AN.GetCurrentAnimatorStateInfo(0).length);
        transform.parent = null;
        ObjectPoolingManager.Instance.InsertGunEffectQueue(gameObject);
    }

}
