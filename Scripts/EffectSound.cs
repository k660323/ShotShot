using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSound : MonoBehaviour
{   
    AudioSource audioSource;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if (audioSource.clip != null)
            StartCoroutine(PlayClip());
    }

    IEnumerator PlayClip()
    {
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        transform.parent = null;
        audioSource.clip = null;
        ObjectPoolingManager.Instance.InsertSQueue(gameObject);
    }
}
