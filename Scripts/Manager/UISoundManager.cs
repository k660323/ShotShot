using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoundManager : MonoBehaviour
{
    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void UISound(AudioClip audio)
    {
        audioSource.clip = audio;
        audioSource.Play();
    }
}
