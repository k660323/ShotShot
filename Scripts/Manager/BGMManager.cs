using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    public AudioClip[] bgList;
    public AudioSource bgSound;

    private static BGMManager instance;

    public static BGMManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<BGMManager>();
                if (obj != null)
                {
                    instance = obj;
                }
                else
                {
                    var newObj = Instantiate(Resources.Load<GameObject>("SoundManager"), Vector3.zero, Quaternion.identity).GetComponent<BGMManager>();
                    instance = newObj;
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        var objs = FindObjectsOfType<BGMManager>();
        if (objs.Length != 1)
        {
            Destroy(gameObject);
            return;
        }
        SceneManager.sceneLoaded += OnSceneLoaded;

        bgSound = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        bgSound.Pause();
        for (int i = 0; i < bgList.Length; i++)
        {
            if(arg0.name == bgList[i].name)
            {
                BgSoundPlay(bgList[i],true);
                break;
            }
        }
    }

    public void BgSoundPlay(AudioClip clip, bool isLoop)
    {
        bgSound.clip = clip;
        bgSound.loop = isLoop;
        bgSound.UnPause();
        bgSound.Play();
    }
}
