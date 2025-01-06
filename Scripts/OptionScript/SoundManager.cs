using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider[] sliders;
    private void Awake()
    {
        var objs = FindObjectsOfType<SoundManager>();
        if (objs.Length != 1)
        {
            Destroy(gameObject);
            return;
        }
        InitUI();
    }

    void InitUI()
    {      
        // 사운드 슬라이드 초기화 및 세팅
        float value;
        mixer.GetFloat("MasterSound", out value);
        sliders[0].value = Mathf.Pow(10, value / 20f);
        mixer.GetFloat("BGSound", out value);
        sliders[1].value = Mathf.Pow(10, value / 20f);
        mixer.GetFloat("SFXSound", out value);
        sliders[2].value = Mathf.Pow(10, value / 20f);
        mixer.GetFloat("UISound", out value);
        sliders[3].value = Mathf.Pow(10, value / 20f);
    }

    // 각 슬라이더에서 호출 해야함 
    #region 오디오
    public void MasterSoundVolume(float val)
    {
        if (val == 0)
        {
            mixer.SetFloat("MasterSound", -80f);
        }
        else
        {
            mixer.SetFloat("MasterSound", Mathf.Log10(val) * 20f);
        }
    }

    public void BGSoundVolume(float val)
    {
        if (val == 0)
        {
            mixer.SetFloat("BGSound", -80f);
        }
        else
        {
            mixer.SetFloat("BGSound", Mathf.Log10(val) * 20f);
        }
    }

    public void SFXSoundVolume(float val)
    {
        if (val == 0)
        {
            mixer.SetFloat("SFXSound", -80f);
        }
        else
        {
            mixer.SetFloat("SFXSound", Mathf.Log10(val) * 20f);
        }
    }

    public void UISoundVolume(float val)
    {
        if (val == 0)
        {
            mixer.SetFloat("UISound", -80f);
        }
        else
        {
            mixer.SetFloat("UISound", Mathf.Log10(val) * 20f);
        }
    }

    #endregion

}
