using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfoManager : MonoBehaviour
{
    private static CharacterInfoManager instance;

    public static CharacterInfoManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<CharacterInfoManager>();
                if (obj != null)
                {
                    instance = obj;
                }
                else
                {
                    var newObj = Instantiate(Resources.Load<GameObject>("ManagerObject/CharactorInfoManager")).GetComponent<CharacterInfoManager>();
                    instance = newObj;
                }
            }

            return instance;
        }
    }

    public List<CharacterInfo> _charactorInfoList = new List<CharacterInfo>();
}
