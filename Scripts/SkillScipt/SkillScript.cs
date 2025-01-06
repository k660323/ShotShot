using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class SkillScript : MonoBehaviourPunCallbacks
{
    protected IEnumerator skill;

    [SerializeField]
    protected AudioClip[] audioClips;
    [SerializeField]
    protected float bulletSoundVol;

    public string abilityName;
    public string abilityText;

    protected PhotonView PV;

    public Sprite abilityImage;
    public enum skillType
    {
        패시브,
        액티브,
    }
    public SkillScript[] swapSkillUse;
    public GameObject PreviousObject;


    public GameObject weaponObject;
    public CharacterScript player;

    public skillType Type;
    public int skillUsedLevel;

    public Image coolTime;
    public Text coolText;
    public GameObject skillActive;
    public string shortcutkeys;

    public bool isActive;

    public bool coolStart;
    public float maxCool;
    public float curCool;

    public bool Knockback;
    public float kbTime;
    public bool KbResistacne;
    public float KbPower;

    public bool Slow;
    public float STime;
    public float SSpeed;

    public bool CC;
    public float ccTime;

    public float skillDamage;
    public float addtionRate;
    protected void Awake()
    {
        PV = GetComponent<PhotonView>();
        player = GetComponentInParent<CharacterScript>();
        curCool = maxCool;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (PV.IsMine)
            CoolTime();
    }

    public virtual void Init(bool isSetCool = true) { }

    public void SetCoolTime()
    {
        curCool = maxCool;
        coolTime.fillAmount = 1f;
        coolStart = true;
        coolText.text = ((short)curCool).ToString();
    }

    protected void CoolTime()
    {
        if (coolStart)
        {
            if (curCool > 0)
            {
                curCool -= Time.deltaTime;
                coolTime.fillAmount = (curCool / maxCool);
                coolText.text = ((short)curCool).ToString();
            }
            else
            {
                coolStart = false;
                curCool = maxCool;
                coolText.text = "";
            }
        }
    }

    [PunRPC]
    protected void GetQSound(int index, float maxRange, Vector3 startPos, bool isFollow)
    {
        GameObject s_Object = ObjectPoolingManager.Instance.GetSQueue();
        s_Object.GetComponent<AudioSource>().clip = audioClips[index];
        s_Object.GetComponent<AudioSource>().maxDistance = maxRange;
        if (isFollow)
            s_Object.transform.parent = transform;
        s_Object.transform.position = startPos;
        s_Object.SetActive(true);
    }
}
