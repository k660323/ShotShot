using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    private static OptionManager instance;

    public static OptionManager Instance
    {
        get
        {
            if(instance == null)
            {
                var obj = FindObjectOfType<OptionManager>();
                if(obj != null)
                {
                    instance = obj;
                }
                else
                {
                    var newObj = Instantiate(Resources.Load<GameObject>("OptionGroup"), GameObject.Find("Canvas").transform.position, Quaternion.identity,GameObject.Find("Canvas").transform).GetComponent<OptionManager>();
                    instance = newObj;
                }
            }
            return instance;
        }
    }

    FullScreenMode screenMode;

    List<Resolution> resolutions = new List<Resolution>();
    public Dropdown resoultionDropDown;
    int resoultionNum;

    public Toggle fullScreenBtn;

    public AudioMixer mixer;
    public Slider[] sliders;
    private void Awake()
    {
        var objs = FindObjectsOfType<OptionManager>();
        if(objs.Length != 1)
        {
            Destroy(gameObject);
            return;
        }
        InitUI();
    }

    void InitUI()
    {
        // 해상도 초기화 및 세팅

        // 60hz만 넣어줌
        //for(int i = 0; i < Screen.resolutions.Length; i++)
        //{
        //    if (Screen.resolutions[i].refreshRate == 60)
        //        resolutions.Add(Screen.resolutions[i]);
        //}

        resolutions.AddRange(Screen.resolutions); // 뭉텅이로 넣는다
        resoultionDropDown.options.Clear();

        int optionNum = 0;
        foreach(Resolution item in resolutions)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
           
            option.text = item.width + "x" + item.height + " " + item.refreshRate + "hz";
            resoultionDropDown.options.Add(option);
           if(item.width == Screen.width && item.height == Screen.height)
                resoultionDropDown.value = optionNum;
            optionNum++;
        }
        resoultionDropDown.RefreshShownValue();
        fullScreenBtn.isOn = Screen.fullScreenMode.Equals(FullScreenMode.Windowed) ? true : false;

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

    #region 화면
    public void DropBoxOptionChange(int x)
    {
        resoultionNum = x;
    }

    public void FullScreenBtn(bool isFull)
    {
        screenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    public void OkBtnClick()
    {
        Screen.SetResolution(resolutions[resoultionNum].width, resolutions[resoultionNum].height, screenMode);
    }
    #endregion

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
