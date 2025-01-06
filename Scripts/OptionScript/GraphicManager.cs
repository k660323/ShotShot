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
        // �ػ� �ʱ�ȭ �� ����

        // 60hz�� �־���
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

        //resolutions.AddRange(Screen.resolutions); // �����̷� �ִ´�
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
        
        // ��ü ȭ��
        fullScreenBtn.isOn = Screen.fullScreenMode.Equals(FullScreenMode.Windowed) ? true : false;

        // �׷��� ǰ��
        graphicDropDown.value = QualitySettings.GetQualityLevel();

        // ���� ����ȭ
        VsyncBtn.isOn = QualitySettings.vSyncCount == 1 ? true : false;
        // ������
        Application.targetFrameRate = 60;
        //������ �����̴�
        maxFreamSliders.value = Application.targetFrameRate;
        //���� ������
        curFrameText.text = Application.targetFrameRate.ToString();
    }

    public void OnEnable()
    {
        // �ػ�
        resoultionDropDown.value = curResoultionNum;
        // ��ü ȭ��
        fullScreenBtn.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow) ? true : false;
        // �׷��� ǰ��
        graphicDropDown.value = QualitySettings.GetQualityLevel();

        // ���� ����ȭ
        VsyncBtn.isOn = QualitySettings.vSyncCount == 1 ? true : false;

        //������ �����̴�
        maxFreamSliders.value = Application.targetFrameRate;
        //���� ������
        curFrameText.text = Application.targetFrameRate.ToString();
    }

    #region �ػ�/�׷���/������ ����
    // �ػ� ����
    public void ResoltionOptionChange(int x)
    {
        resoultionNum = x;
    }

    // ��ü ȭ��
    public void FullScreenBtn(bool isFull)
    {
        screenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    // �׷��� ����
    public void GraphicOptionChange(int value)
    {
        graphicNum = value;
    }

    // ���� ����ȭ
    public void vSyncBtn(bool isSync)
    {
        vsyncNum = isSync ? 1 : 0;
    }

    // ������
    public void Fream(float value)
    {
        curFrameText.text = ((int)value).ToString();
        curFreamNum = (int)value;
    }

    // ����
    public void OkBtnClick()
    {
        // �ػ� // ��üȭ��
        Screen.SetResolution(resolutions[resoultionNum].width, resolutions[resoultionNum].height, screenMode);
        curResoultionNum = resoultionNum;
        // �׷��� ����
        QualitySettings.SetQualityLevel(graphicNum);
        // ���� ����ȭ
        QualitySettings.vSyncCount = vsyncNum;
        // ������
        Application.targetFrameRate = curFreamNum;
    }

    #endregion
}
