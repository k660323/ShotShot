using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GraphicManager : MonoBehaviour
{
    FullScreenMode screenMode;

    List<Resolution> resolutions = new List<Resolution>();
    public Dropdown resoultionDropDown;
    int resoultionNum;
    int curResoultionNum;

    public Toggle fullScreenBtn;

    public Dropdown graphicDropDown;
    int graphicNum;

    public Toggle VsyncBtn;
    public int vsyncNum;

    public Slider maxFreamSliders;
    int curFreamNum;
    public Text curFrameText;

    private void Awake()
    {
        var objs = FindObjectsOfType<GraphicManager>();
        if (objs.Length != 1)
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
     

        for (int i=0; i<Screen.resolutions.Length;i++)
        {
            if(Mathf.Round(((float)Screen.resolutions[i].width / Screen.resolutions[i].height)*100) >= 16)
            {
                    resolutions.Add(Screen.resolutions[i]);     
            }
        }

        //resolutions.AddRange(Screen.resolutions); // 뭉텅이로 넣는다
        resoultionDropDown.options.Clear();

        int optionNum = 0;
        foreach (Resolution item in resolutions)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            
            option.text = item.width + "x" + item.height + " " + item.refreshRate + "hz";
            resoultionDropDown.options.Add(option);
            if (item.width == Screen.width && item.height == Screen.height)
            {
                resoultionDropDown.value = optionNum;
                curResoultionNum = optionNum;
            }
            optionNum++;
        }
        resoultionDropDown.RefreshShownValue();
        
        // 전체 화면
        fullScreenBtn.isOn = Screen.fullScreenMode.Equals(FullScreenMode.Windowed) ? true : false;

        // 그래픽 품질
        graphicDropDown.value = QualitySettings.GetQualityLevel();

        // 수직 동기화
        VsyncBtn.isOn = QualitySettings.vSyncCount == 1 ? true : false;
        // 프레임
        Application.targetFrameRate = 60;
        //프레임 슬라이더
        maxFreamSliders.value = Application.targetFrameRate;
        //현제 프레임
        curFrameText.text = Application.targetFrameRate.ToString();
    }

    public void OnEnable()
    {
        // 해상도
        resoultionDropDown.value = curResoultionNum;
        // 전체 화면
        fullScreenBtn.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow) ? true : false;
        // 그래픽 품질
        graphicDropDown.value = QualitySettings.GetQualityLevel();

        // 수직 동기화
        VsyncBtn.isOn = QualitySettings.vSyncCount == 1 ? true : false;

        //프레임 슬라이더
        maxFreamSliders.value = Application.targetFrameRate;
        //현제 프레임
        curFrameText.text = Application.targetFrameRate.ToString();
    }

    #region 해상도/그래픽/프레임 설정
    // 해상도 설정
    public void ResoltionOptionChange(int x)
    {
        resoultionNum = x;
    }

    // 전체 화면
    public void FullScreenBtn(bool isFull)
    {
        screenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    // 그래픽 설정
    public void GraphicOptionChange(int value)
    {
        graphicNum = value;
    }

    // 수직 동기화
    public void vSyncBtn(bool isSync)
    {
        vsyncNum = isSync ? 1 : 0;
    }

    // 프레임
    public void Fream(float value)
    {
        curFrameText.text = ((int)value).ToString();
        curFreamNum = (int)value;
    }

    // 적용
    public void OkBtnClick()
    {
        // 해상도 // 전체화면
        Screen.SetResolution(resolutions[resoultionNum].width, resolutions[resoultionNum].height, screenMode);
        curResoultionNum = resoultionNum;
        // 그래픽 설정
        QualitySettings.SetQualityLevel(graphicNum);
        // 수직 동기화
        QualitySettings.vSyncCount = vsyncNum;
        // 프레임
        Application.targetFrameRate = curFreamNum;
    }

    #endregion
}
