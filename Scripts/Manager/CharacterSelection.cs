using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterSelection : MonoBehaviour
{
    [SerializeField]
    GameObject DifficultyGroup;
    [SerializeField]
    Image weaponImage;
    [SerializeField]
    Image charactorImage;
    [SerializeField]
    Text charactorNameText;
    [SerializeField]
    Text CommentText;

    [SerializeField]
    Image[] abilityImage;
    [SerializeField]
    Text[] abilityName;
    [SerializeField]
    Text[] abilityText;
    
    //public List<CharacterInfo> _charactorInfoList = new List<CharacterInfo>();
    public int index;
    private void Awake()
    {       
        index = 0;
        Init();
    }

    private void Start()
    {
        BGMManager.Instance.BgSoundPlay(BGMManager.Instance.bgList[3], false);
    }

    private void Init()
    {
        for (int i = 0; i < DifficultyGroup.transform.childCount; i++)
        {
            DifficultyGroup.transform.GetChild(i).gameObject.SetActive(false);
        }
        
        for (int i = 0; i < CharacterInfoManager.Instance._charactorInfoList[index].diffcult; i++)
        {
            DifficultyGroup.transform.GetChild(i).gameObject.SetActive(true);
        }

        weaponImage.sprite = CharacterInfoManager.Instance._charactorInfoList[index].weaponImage;
        charactorImage.sprite = CharacterInfoManager.Instance._charactorInfoList[index].charactorImage;
        charactorNameText.text = CharacterInfoManager.Instance._charactorInfoList[index].name;
        CommentText.text = CharacterInfoManager.Instance._charactorInfoList[index].comment;

        for (int i = 0; i < 5; i++)
        {
            abilityImage[i].sprite = CharacterInfoManager.Instance._charactorInfoList[index].abilityImage[i];
            abilityName[i].text = CharacterInfoManager.Instance._charactorInfoList[index].abilityName[i];
            abilityText[i].text = CharacterInfoManager.Instance._charactorInfoList[index].abilityText[i];
        }
    }

    public void LeftIndex()
    {
        if(index != 0)
        {
            index--;
            Init();
        }
    }

    public void RightIndex()
    {
        if(index != CharacterInfoManager.Instance._charactorInfoList.Count-1)
        {
            index++;
            Init();
        }
    }
}
