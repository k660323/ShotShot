using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterInfo : MonoBehaviour
{
    public enum skillType
    {
        �нú�,
        ��ų,
    }
   

    public int diffcult;
    public Sprite weaponImage;
    public Sprite charactorImage;
    public string name;
    public string comment;

    public Sprite[] abilityImage;
    public string[] abilityName;
    public string[] abilityText;
}
