
using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class GunWeapon : MonoBehaviourPunCallbacks
{
    [SerializeField]
    AudioClip[] shotClips;
    [SerializeField]
    AudioClip[] reloadClips;
    [SerializeField]
    AudioClip[] ammoOutClips;

    [HideInInspector]
    public Transform FirePos;

    protected PhotonView PV;

    public GameObject bullet;

    public float bulletSoundVol;
    public float AmmoOutSoundVol;

    public int _curAmmo;
    public int curAmmo { 
        get { return _curAmmo; } 
        set 
        {
            _curAmmo = value;
            if (_curAmmo < 0)
                _curAmmo = 0;
        } 
    }

    public int maxAmmo;

    public bool isChargeMode;

    public float atkRate;
    public float curCool;
    public float addtionATKRate;
    public bool isFireReady;

    IEnumerator DecreaseMoveCoroutine;
    public int decreaseTimeRate = 1;
    public float shootAfterMoveDecreaseSpeed;

    public float reloadTime;
    public float addtionReloadTime;
    public float reloadingMoveSpeedRate;
    protected float reloadMoveValue;
    public bool isReload;
    protected IEnumerator reloadCoroutine;

    public float bulletDamage;
    public float bulletZoomDamage;
    public float addtionDamage;
    public float LvDamagePercent;

    public int bulletCount;
    public float startAngle;
    public float defaultAngle;
    public float addAngle;
    protected Quaternion qRoate;
    protected Vector2 v2Dir;

    protected CharacterScript player;

    public Image coolTime;

    public bool isUseZoom;
    public float maxRange;
    public float minRange;
    public float LerpSpeed;

    public float zoomDownPercent;
    protected float zoomSpeed;

    public bool Knockback;
    public float kbTime;
    public bool KbResistacne;
    public float KbPower;

    public bool Slow;
    public float STime;
    public float SSpeed;

    public bool CC;
    public float ccTime;

    public bool penetrate;

    [HideInInspector]
    public ChargeScript chargeScript;

    protected void Awake()
    {
        PV = GetComponent<PhotonView>();
        player = GetComponentInParent<CharacterScript>();
        reloadCoroutine = Reloading();
        chargeScript = GetComponent<ChargeScript>();
        FirePos = transform.Find("FirePos");
    }
    protected abstract void Update();

    public virtual void InitAnim() { }

    protected virtual void Zoom()
    {
        if (Input.GetMouseButtonDown(1) && isUseZoom)
        {
            if (player.Gun.gameObject.activeSelf)
            {
                player.isZoom = !player.isZoom;

                if (player.isZoom)
                {
                    zoomSpeed = player.moveSpeed * zoomDownPercent;
                    player.addtionSpeed -= zoomSpeed;
                }
                else
                {
                    player.addtionSpeed += zoomSpeed;
                    zoomSpeed = 0;
                }
            }
        }

        if (player.isZoom)
        {
            CMManager.Instance().virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(CMManager.Instance().virtualCamera.m_Lens.OrthographicSize, maxRange, LerpSpeed * Time.deltaTime);
        }
        else
        {
            CMManager.Instance().virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(CMManager.Instance().virtualCamera.m_Lens.OrthographicSize, minRange, LerpSpeed * Time.deltaTime);
        }
    }

    public virtual void ZoomCancle()
    {
        if (player.isZoom)
        {
            player.isZoom = false;
            player.addtionSpeed += zoomSpeed;
            zoomSpeed = 0;
            CMManager.Instance().virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(CMManager.Instance().virtualCamera.m_Lens.OrthographicSize, minRange, LerpSpeed * Time.deltaTime);
            if (isChargeMode)
            {
                isChargeMode = false;
                chargeScript.ChargingStop();
            }
        }
    }

    protected void AfterShoot()
    {
        if(DecreaseMoveCoroutine == null)
        {
            DecreaseMoveCoroutine = DecreaseMoveSpeed();
            StartCoroutine(DecreaseMoveCoroutine);
        }
    }

    IEnumerator DecreaseMoveSpeed()
    {
        player.addtionSpeed -= shootAfterMoveDecreaseSpeed;
        yield return new WaitForSeconds((1 / (atkRate + addtionATKRate)) / decreaseTimeRate);
        player.addtionSpeed += shootAfterMoveDecreaseSpeed;
        DecreaseMoveCoroutine = null;
    }

    protected abstract void Shot(); // АјАн

    protected virtual void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!isReload && curAmmo != maxAmmo && !player.isSkillReady && !player.isSkillActive && !player.isCC && !player.isDead)
            {
                reloadCoroutine = Reloading();
                StartCoroutine(reloadCoroutine);
            }
            else if (isReload && curAmmo != maxAmmo && !player.isSkillReady && !player.isSkillActive && !player.isCC && !player.isDead)
            {
                ReloadingCancel();
            }

        }
    }

    protected virtual IEnumerator Reloading()
    {
        ZoomCancle();
        isReload = true;
        reloadMoveValue = player.moveSpeed * reloadingMoveSpeedRate;
        player.addtionSpeed -= reloadMoveValue;
        player.AN.SetFloat("ReloadSpeed", 1 / (reloadTime + addtionReloadTime));
        player.PV.RPC("TriggerAnim", RpcTarget.AllViaServer, "isReload");
        yield return new WaitForSeconds(reloadTime + addtionReloadTime);
        player.PV.RPC("TriggerAnim", RpcTarget.AllViaServer, "isReloadOut");
        isReload = false;
        player.addtionSpeed += reloadMoveValue;
        curAmmo = maxAmmo;
        PV.RPC("GetQSound", RpcTarget.All, "ReloadPart", 0, 8f, transform.position, false);
    }

    protected virtual void ReloadingCancel()
    {
        StopCoroutine(reloadCoroutine);
        if (isReload)
        {
            SwapCool();
            player.PV.RPC("TriggerAnim", RpcTarget.AllViaServer, "isReloadOut");
            player.addtionSpeed += reloadMoveValue;
            isReload = false;
        }
    }

    public void SwapCool()
    {
        if (curCool < (1f / (atkRate + addtionATKRate)) / 5f)
        {
            curCool = 0.5f;
        }
    }

    [PunRPC]
    protected void GetGESound()
    {
        GameObject s_Object = ObjectPoolingManager.Instance.GetGunEffectQueue();
        s_Object.transform.parent = player.WeaponPos.transform;
        s_Object.transform.rotation = player.WeaponPos.transform.rotation;
        s_Object.transform.position = player.Gun.FirePos.position;
        s_Object.SetActive(true);
    }

    [PunRPC]
    protected void GetQSound(string name, int index, float maxRange, Vector3 startPos, bool isFollow)
    {
        GameObject s_Object = ObjectPoolingManager.Instance.GetSQueue();
        switch (name)
        {
            case "ShotPart":
                s_Object.GetComponent<AudioSource>().clip = shotClips[index];
                break;
            case "ReloadPart":
                s_Object.GetComponent<AudioSource>().clip = reloadClips[index];
                break;
            case "AmmoOutPart":
                s_Object.GetComponent<AudioSource>().clip = ammoOutClips[index];
                break;
            default:
                break;
        }

        s_Object.GetComponent<AudioSource>().maxDistance = maxRange;
        if (isFollow)
            s_Object.transform.parent = transform;
        s_Object.transform.position = startPos;
        s_Object.SetActive(true);
    }

   
}
