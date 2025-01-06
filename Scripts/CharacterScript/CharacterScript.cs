using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using ExitGames.Client.Photon;
using System.Threading;
using UnityEngine.EventSystems;
using DG.Tweening;

public abstract class CharacterScript : MonoBehaviourPunCallbacks, IPunObservable
{
    public short myTeam;

    [HideInInspector]
    public int DSkinIndex;
    [HideInInspector]
    public PhotonView PV;
    [HideInInspector]
    public AudioListener audioListener;

    private static CharacterScript instance;

    public static CharacterScript Instance
    {
        get
        {
            return instance;
        }
    }

    protected GraphicRaycaster c_GR;
    protected PointerEventData c_Ped;
    [HideInInspector]
    public GameObject moveObject;
    [HideInInspector]
    public Transform SpawnDS;
    [HideInInspector]
    public Text NickNameText;
    [HideInInspector]
    public Image HpBar;
    [HideInInspector]
    public Image GaugeBar;

    // 기본 무기
    [HideInInspector]
    public GameObject WeaponPos;
    [HideInInspector]
    public GunWeapon Gun;

    [HideInInspector]
    public SpriteRenderer IconImage;

    [HideInInspector]
    public GameObject healParticle;

    [HideInInspector]
    public SpriteRenderer SR;
    [HideInInspector]
    public Rigidbody2D _RB;
    [HideInInspector]
    public Animator AN;

    public float h;
    public float v;
    protected Vector3 destPos;
    protected Quaternion destRot;

    protected Vector2 mousePos;
    protected Vector2 weaponPos;
    public Vector2 dir;
    protected float angle;
    protected Hashtable playerCP;

    public bool isStatusHit;

    public float curHp;
    public float curHpProperti
    {
        get
        {
            return curHp;
        }
        set
        {
            if (pCScript)
            {
                if (curHp > value)
                {
                    pCScript.Cancel();
                }
            }
            if (pBScript)
            {
                if (curHp > value)
                {
                    curHp = value;
                    pBScript.Berserker();
                }
                else if (curHp < value)
                {
                    curHp = value;
                    pBScript.BerserkerHeal();
                }
            }
            else
            {
                curHp = value;
            }
        }
    }

    public float maxHp;
    public float maxHpProperti
    {
        get
        {
            return maxHp;
        }
        set
        {
            if (pBScript)
            {
                if (maxHp > value)
                {
                    maxHp = value;
                    pBScript.BerserkerHeal();
                }
                else if (maxHp < value)
                {
                    maxHp = value;
                    pBScript.Berserker();
                }
            }
            else
            {
                maxHp = value;
            }
        }
    }
    public float maxHpUpRate;
    public bool isDead;

    public float curEXP;
    public float maxEXP;
    public float maxExpUpRate;

    public float XP;
    public float xpUpRate;

    public int level;
    public int maxLevel;

    public int kill;

    public float moveSpeed;
    public float addtionSpeed;
    public bool isControll;

    public float addtionDamage;

    public bool isZoom;

    protected List<RaycastResult> results;
    protected bool isUI;

    public int myNum;

    protected volatile int _locked = 0;

    // 스킬 패시브
    public BerserkerScript pBScript;
    public ImprovedstaminaScript pIScript;
    public ExpendGun exGScript;
    public QuickMovementScript pQScript;
    public CloakingScript pCScript;
    public AgilityScript pAScript;
    public CriticalScript pCriScript;

    public bool isCC;
    public bool isPhysics;
    public bool isResistance;

    public bool isDeadLock;
    public volatile int playerDeadLock = 0;
    public volatile int CCLock = 0;
    public volatile int PhysicsLock = 0;
    public volatile int SlowLock = 0;
    // 기본 사운드
    [SerializeField]
    protected AudioClip[] LevelUPClips;
    [SerializeField]
    protected AudioClip[] HitClips;
    [SerializeField]
    protected AudioClip[] DieClips;
    [SerializeField]
    protected AudioClip[] MoveClips;

    public int MoveSoundIndex = 0;

    [SerializeField]
    SkillScript[] skillScripts;

    public SkillScript this[int index]
    {
        get
        {
            return skillScripts[index];
        }
        set
        {
            skillScripts[index] = value;
        }
    }
    [HideInInspector]
    public GameObject skillGroup;


    public SkillScript curSkill;
    public bool isSkillReady;
    public bool isSkillActive;

    IEnumerator CCCoroutine;
    IEnumerator PhysicsCoroutine;

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(moveSpeed);
            stream.SendNext(addtionSpeed);
            stream.SendNext((Vector2)transform.position);
            stream.SendNext(WeaponPos.transform.rotation);
            stream.SendNext(curHp);
            stream.SendNext(maxHp);
            stream.SendNext(HpBar.fillAmount);
            stream.SendNext(_RB.velocity);
            stream.SendNext(gameObject.tag);
            stream.SendNext((short)gameObject.layer);
            stream.SendNext((short)moveObject.layer);
            stream.SendNext(XP);
            stream.SendNext(healParticle.activeSelf);
            
            stream.SendNext(SR.color);
            stream.SendNext(HpBar.color);

            stream.SendNext(myTeam);
        }
        else
        {
            moveSpeed = (float)stream.ReceiveNext();
            addtionSpeed = (float)stream.ReceiveNext();
            destPos = (Vector2)stream.ReceiveNext();
            destRot = (Quaternion)stream.ReceiveNext();
            curHp = (float)stream.ReceiveNext();
            maxHp = (float)stream.ReceiveNext();
            HpBar.fillAmount = (float)stream.ReceiveNext();
            _RB.velocity = (Vector2)stream.ReceiveNext();
            gameObject.tag = (string)stream.ReceiveNext();
            gameObject.layer = (short)stream.ReceiveNext();
            moveObject.layer = (short)stream.ReceiveNext();
            XP = (float)stream.ReceiveNext();
            healParticle.SetActive((bool)stream.ReceiveNext());

            SR.color = (Color)stream.ReceiveNext();
            HpBar.color = (Color)stream.ReceiveNext();


            SetTeamNickNameColor((short)stream.ReceiveNext());
        }
    }

    protected void SetTeamNickNameColor(short teamIndex)
    {
        if (myTeam == teamIndex)
            return;

        myTeam = teamIndex;
        if (teamIndex == 0)
            return;

        NickNameText.color = CharacterScript.instance.myTeam == teamIndex ? Color.yellow : Color.red;
        IconImage.color = CharacterScript.instance.myTeam == teamIndex ? Color.yellow : Color.red;
    }
    // Start is called before the first frame update
    protected void Awake()
    {
        PV = GetComponent<PhotonView>();
        IconImage = transform.Find("Icon").GetComponent<SpriteRenderer>();
        if (PV.IsMine)
        {
            instance = this;
            CMManager.Instance().virtualCamera.Follow = transform;
            CMManager.Instance().virtualCamera.LookAt = transform;
            CMManager.Instance().cinemachineConfiner.m_BoundingShape2D = GameManager.MapInfo.go.GetComponent<PolygonCollider2D>();
            audioListener = GetComponent<AudioListener>();
            audioListener.enabled = true;
            c_GR = GameObject.Find("Canvas").GetComponent<GraphicRaycaster>();
            c_Ped = new PointerEventData(null);
            myTeam =  SessionInfo.Instance.myTeam;
            IconImage.color = new Color(1f, 1f, 1f, 1f);
        }
        moveObject = transform.Find("MoveCollider").gameObject;
        Transform playerCanvas = transform.Find("PlayerCanvas");
        SpawnDS = playerCanvas.Find("SpawnDS");
        NickNameText = playerCanvas.Find("NickNameText").GetComponent<Text>();
        HpBar = playerCanvas.Find("HpImage").GetComponent<Image>();
        GaugeBar = playerCanvas.Find("GaugeBarBg").Find("GaugeBar").GetComponent<Image>();
        WeaponPos = transform.Find("WeaponPos").gameObject;
        Gun = WeaponPos.transform.GetComponentInChildren<GunWeapon>();    
        healParticle =transform.Find("Heal Particle").gameObject;
        
        SR = GetComponent<SpriteRenderer>();
        _RB = GetComponent<Rigidbody2D>();
        AN = GetComponent<Animator>();

        skillGroup = transform.Find("SkillGroup").gameObject;

        NickNameText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        NickNameText.color = PV.IsMine ? Color.green : Color.red;

        playerCP = PhotonNetwork.LocalPlayer.CustomProperties;


        PassiveCheck();
    }

    public void PassiveCheck()
    {
        for (int i = 0; i < skillGroup.transform.childCount; i++)
        {
            if (skillGroup.transform.GetChild(i).GetComponent<SkillScript>().Type == SkillScript.skillType.패시브)
            {
                if (pBScript == null)
                {
                    pBScript = skillGroup.transform.GetChild(i).GetComponent<BerserkerScript>();
                    if (pBScript != null)
                        continue;
                }
                    
                if (pIScript == null)
                {
                    pIScript = skillGroup.transform.GetChild(i).GetComponent<ImprovedstaminaScript>();
                    if (pIScript != null)
                        continue;
                }

                if (exGScript == null)
                {
                    exGScript = skillGroup.transform.GetChild(i).GetComponent<ExpendGun>();
                    if (exGScript != null)
                        continue;
                }

                if (pQScript == null)
                {
                    pQScript = skillGroup.transform.GetChild(i).GetComponent<QuickMovementScript>();
                    if (pQScript != null)
                        continue;
                }

                if (pCScript == null)
                {
                    pCScript = skillGroup.transform.GetChild(i).GetComponent<CloakingScript>();
                    if (pCScript != null)
                        continue;
                }

                if (pAScript == null)
                {
                    pAScript = skillGroup.transform.GetChild(i).GetComponent<AgilityScript>();
                    if (pAScript != null)
                        continue;
                }
                
                if(pCriScript == null)
                {
                    pCriScript = skillGroup.transform.GetChild(i).GetComponent<CriticalScript>();
                    if (pCriScript != null)
                        continue;
                }
            }
        }
    }

    protected abstract void Start(); // 기본 상태 초기화

    // Update is called once per frame
    protected void Update()
    {
        if (PV.IsMine)
        {
            Aim();
        }
        else
        {
            float distance = (destPos - transform.position).sqrMagnitude;

            if (distance < 0.005f || distance > 100f)
            {
                transform.position = destPos;
            }
            else
            {
                transform.position = Vector2.Lerp(transform.position, destPos, 30f * Time.deltaTime);
            }


            WeaponPos.transform.rotation = Quaternion.Lerp(WeaponPos.transform.rotation, destRot, 20f * Time.deltaTime);
        }
    }

    protected void LateUpdate()
    {
        if (PV.IsMine)
            HpBar.fillAmount = curHp / maxHp;
    }

    protected void FixedUpdate()
    {
        Move();
        PhysicsCheck();
    }

    protected void Move()
    {
        if (PV.IsMine)
        {
            if (!isDead && isControll && !isCC)
            {
                if (isPhysics && !isResistance) // 물리효과 받을때 저항이 불가능
                    return;

                h = Input.GetAxisRaw("Horizontal");
                v = Input.GetAxisRaw("Vertical");

                if (h != 0)
                    PV.RPC("FlipX", RpcTarget.AllViaServer, h);
                AN.SetBool("isWalk", Mathf.Abs(h) + Mathf.Abs(v) != 0 ? true : false);

                if(moveSpeed + addtionSpeed > 0.0f)
                    _RB.position += new Vector2(h, v).normalized * (moveSpeed + addtionSpeed) * Time.deltaTime;
            }
            else if (isDead || isControll || isCC || isResistance)
            {
                h = 0f;
                v = 0f;
                AN.SetBool("isWalk", false);
            }
        }
    }

    protected void PhysicsCheck()
    {
        if (!isPhysics) // 물리 초기화 여부
            _RB.velocity = Vector2.zero;
    }

    #region 상태이상 중첩x 취소x 1번째 방법
    public void PhysicsState(float duration, bool isResistance)
    {
        if (Interlocked.CompareExchange(ref PhysicsLock, 1, 0) == 0)
        {
            StartCoroutine(Physics(duration, isResistance));
        }
    }

    public void CCState(float duration)
    {
        if (Interlocked.CompareExchange(ref CCLock, 1, 0) == 0)
        {
            StartCoroutine(CC(duration));
        }
    }

    public void SlowState(float duration,float slowSpeed)
    {
        if (Interlocked.CompareExchange(ref SlowLock, 1, 0) == 0)
        {
            StartCoroutine(Slow(duration, slowSpeed));
        }
    }

    IEnumerator Physics(float duration, bool isResistance)
    {
        this.isResistance = isResistance;
        isPhysics = true;
        yield return new WaitForSeconds(duration);
        this.isResistance = false;
        isPhysics = false;
        PhysicsLock = 0;
    }

    IEnumerator CC(float duration)
    {
        isCC = true;
        yield return new WaitForSeconds(duration);
        isCC = false;
        CCLock = 0;
    }

    IEnumerator Slow(float duration,float slowSpeed)
    {
        addtionSpeed -= slowSpeed;
        yield return new WaitForSeconds(duration);
        addtionSpeed += slowSpeed;
        SlowLock = 0;
    }
    #endregion

    #region 상태이상 취소 다시 2번째 방법  중첩x 취소후 실행

    public void PhysicsState2(float duration, bool isResistance)
    {
        if (PhysicsLock == 0)
        {
            if (PhysicsCoroutine != null)
                StopCoroutine(PhysicsCoroutine);
            PhysicsCoroutine = Physics2(duration, isResistance);
            StartCoroutine(PhysicsCoroutine);
        }
    }

    public void CCSate2(float duration)
    {
        if (CCLock == 0)
        {
            if (CCCoroutine != null)
                StopCoroutine(CCCoroutine);
            CCCoroutine = CC2(duration);
            StartCoroutine(CCCoroutine);
        }
    }

    IEnumerator Physics2(float duration, bool isResistance)
    {
        this.isResistance = isResistance;
        isPhysics = true;
        yield return new WaitForSeconds(duration);
        this.isResistance = false;
        isPhysics = false;
    }

    IEnumerator CC2(float duration)
    {
        isCC = true;
        yield return new WaitForSeconds(duration);
        isCC = false;
    }

    #endregion

    [PunRPC]
    protected void FlipX(float h)
    {
        SR.flipX = h > 0 ? true : false;
    }

    protected void UIClickCheck()
    {
        c_Ped.position = Input.mousePosition;
        results = new List<RaycastResult>();
        c_GR.Raycast(c_Ped, results);

        if (results.Count <= 0)
        {
            isUI = false;
            return;
        }

        if (results[0].gameObject.CompareTag("ExceptUI"))
            isUI = false;
        else
            isUI = true;
    }

    protected void Aim()
    {
        if (!isDead && isControll && !isCC && !isResistance)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            weaponPos = WeaponPos.transform.position;
            dir = (mousePos - weaponPos).normalized;
            angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            WeaponPos.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            WeaponPos.transform.Rotate(SR.flipX ? 0 : 180, 0, 0);
        }
    }

    public virtual void Hit(Player owner,short _teamNum, float damage,int DSkinIndex = 0, string type ="normal")
    {
        if (isDead)
            return;

        PV.RPC("ShowDamageSkin", RpcTarget.All,(int)damage, DSkinIndex);

        if (type == "normal")
            CMManager.Instance().CameraShake(1f, 1f, 0.5f);
        else if(type == "Boom")
            CMManager.Instance().CameraShake(1.5f, 2f, 1.5f);
        else if(type == "LandMine")
            CMManager.Instance().CameraShake(5f, 5f, 3f);
        else if(type == "SpiderMine")
            CMManager.Instance().CameraShake(1f, 1f, 1f);
        else if(type == "Sniping")
            CMManager.Instance().CameraShake(1f, 1f, 1f);

        if (curHp > 0)
        {
            curHpProperti -= damage;
            PV.RPC("GetQSound", RpcTarget.AllViaServer, "HitPart", 0, 10f, transform.position, false);
        }

        if (curHp <= 0)
        {
            isDead = true;

            if (owner.ActorNumber != PV.OwnerActorNr || (SessionInfo.Instance.isTeamMode && myTeam != _teamNum))
                GameManager.Instance.GetComponent<PhotonView>().RPC("Kill", owner, XP);  //RPC("Kill", RpcTarget.OthersBuffered, actorNum, XP);
            PV.RPC("GetQSound", RpcTarget.AllViaServer, "DiePart", Random.Range(0, DieClips.Length), 10f, transform.position, false);
            gameObject.layer = 9;
            moveObject.layer = 9;

            curHpProperti = 0f;
            addtionDamage = 0;
            AN.SetBool("isAlive", !isDead);
            PV.RPC("TriggerAnim", RpcTarget.AllViaServer, "isDie");
            GameManager.Instance.spawnManager.RespawnPanel(GameManager.Instance.spawnManager.myNum);

            // 스킬 초기화
            for (int i = 0; i < skillGroup.transform.childCount; i++)
                skillGroup.transform.GetChild(i).GetComponent<SkillScript>().Init();
            // 총 기능 초기화
            Gun.InitAnim();
            Gun.ZoomCancle();
            Gun.chargeScript.Init();
            GameManager.Instance.StartCoroutine(GameManager.Instance.noticeSound(1));
        }
    }

    [PunRPC]
    public void ShowDamageSkin(int damage, int DSkinIndex)
    {
        FloatingTextManager.Instance.CreateFloater(SpawnDS, damage, DSkinIndex);
    }
    // 피깍
    public void SH(int actorNum, short teamNum, string state, float damage, int TickCount, int DSkinIndex)
    {
        StartCoroutine(StatusHit(actorNum,teamNum, state, damage, TickCount, DSkinIndex));
    }

    protected IEnumerator StatusHit(int actorNum,short teamNum, string state, float damage, int TickCount,int DSkinIndex)
    {
        switch (state)
        {
            case "poison":
                isStatusHit = true;
                for (int i = 0; i < TickCount; i++)
                {
                    curHpProperti -= (damage / TickCount);
                    HpBar.color = new Color(113f / 255f, 0f, 1f, HpBar.color.a);
                    SR.color = new Color(113f / 255f, 0f, 1f, SR.color.a);
                    if (curHpProperti > 0)
                    {
                        yield return new WaitForSeconds(1f);
                    }
                    else
                    {
                        Hit(PV.Owner, teamNum, damage, DSkinIndex);
                        break;
                    }
                }
                break;
            default:
                break;
        }
        isStatusHit = false;
        HpBar.color = new Color(1f, 40f / 255f, 40f / 255f, 1f);
        SR.color = new Color(1f, 1f, 1f, HpBar.color.a);
        if (pBScript != null)
            pBScript.Berserker();

    }

    public virtual void killFunction()
    {
        if (pQScript != null)
        {
            pQScript.ResetCoolTime();
        }
    }

    public virtual IEnumerator smoothUP(float XP)
    {
        while (true)
        {
            if (Interlocked.CompareExchange(ref _locked, 1, 0) == 0) // _locked 가 0이면 1로 바꿈
            {
                for (int i = 0; i < 5; i++)
                {
                    curEXP += (int)(XP / 5f);
                    while (true)
                    {
                        yield return new WaitForSeconds(0.1f);
                        if (curEXP >= maxEXP)
                        {
                            if (level != maxLevel)
                                LevelUp();
                            else
                                yield break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                _locked = 0;
                break;
            }
            yield return null;
        }
    }

    protected IEnumerator smallUP()
    {
        int a = 0;
        while (true)
        {
            if (Interlocked.CompareExchange(ref _locked, 1, 0) == 0) // _locked 가 0이면 1로 바꿈
            {
                if (level < maxLevel)
                    curEXP += ++a;
                else
                    yield break;

                if (curEXP >= maxEXP)
                    LevelUp();

                a = 0;
                _locked = 0;
            }
            else if (Interlocked.CompareExchange(ref _locked, 1, 0) == 1)
            {
                a++;
            }
            yield return new WaitForSeconds(1f);
        }
    }

    protected virtual void LevelUp()
    {
        PV.RPC("GetQSound", RpcTarget.AllViaServer, "LevelUPPart", 0, 5f, transform.position, true);
        level++;

        if (pIScript != null)
        {
            pIScript.Improvedstamina();
        }

        if (exGScript != null)
        {
            exGScript.ChargingSetting();
            exGScript.ExpandAmmo();
        }

        if (pQScript != null)
        {
            pQScript.ReloadSpeedUP();
        }

        if (pAScript != null)
        {
            pAScript.Agility();
        }

        if (level != maxLevel)
            curEXP -= maxEXP;
        else
            curEXP = 0;

        maxEXP += (int)(maxEXP * maxExpUpRate);
        XP += (int)(XP * xpUpRate);
        maxHpProperti += (maxHp * maxHpUpRate);
        GameManager.Instance.LevelNotice();
    }

    public void MoveSound()
    {
        PV.RPC("GetQSound", RpcTarget.AllViaServer, "MovePart", MoveSoundIndex, 3f, transform.position, false);
    }

    [PunRPC]
    protected void GetQSound(string clipPart, int index, float maxRange, Vector3 startPos, bool isFollow)
    {
        GameObject s_Object = ObjectPoolingManager.Instance.GetSQueue();
        switch (clipPart)
        {
            case "LevelUPPart":
                s_Object.GetComponent<AudioSource>().clip = LevelUPClips[index];
                break;
            case "HitPart":
                s_Object.GetComponent<AudioSource>().clip = HitClips[index];
                break;
            case "DiePart":
                s_Object.GetComponent<AudioSource>().clip = DieClips[index];
                break;
            case "MovePart":
                s_Object.GetComponent<AudioSource>().clip = MoveClips[index];
                break;
            default:
                s_Object.GetComponent<AudioSource>().clip = null;
                break;
        }
        s_Object.GetComponent<AudioSource>().maxDistance = maxRange;
        if (isFollow)
            s_Object.transform.parent = transform;
        s_Object.transform.position = startPos;
        s_Object.SetActive(true);
    }

    public void Init()
    {
        gameObject.layer = 7;
        moveObject.layer = 7;

        isControll = true;
        isStatusHit = false;
        isDead = false;
        isDeadLock = false;
        isCC = false;
        isPhysics = false;
        isResistance = false;
        isSkillActive = false;
        isSkillReady = false;
        curSkill = null;

        curHpProperti = maxHp;
        Gun.curAmmo = Gun.maxAmmo;
        addtionDamage = 0f;
        addtionSpeed = 0f;

        AN.SetBool("isAlive", true);
        SR.color = new Color(1, 1, 1);

        for (int i = 0; i < WeaponPos.transform.childCount; i++)
        {
            WeaponPos.transform.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < skillScripts.Length; i++)
        {
            skillScripts[i].Init();
        }

        Gun.gameObject.SetActive(true);

        StartCoroutine(Invincibility());
    }

    IEnumerator Invincibility()
    {
        gameObject.tag = "Invincibility";

        // 5초 무적
        SR.color = new Color(SR.color.r, SR.color.g, SR.color.b, 100f / 255f);
        yield return new WaitForSeconds(1f);
        SR.color = new Color(SR.color.r, SR.color.g, SR.color.b, 1f);
        yield return new WaitForSeconds(1f);
        SR.color = new Color(SR.color.r, SR.color.g, SR.color.b, 100f / 255f);
        yield return new WaitForSeconds(1f);
        SR.color = new Color(SR.color.r, SR.color.g, SR.color.b, 1f);
        yield return new WaitForSeconds(1f);
        SR.color = new Color(SR.color.r, SR.color.g, SR.color.b, 100f / 255f);
        yield return new WaitForSeconds(1f);
        SR.color = new Color(SR.color.r, SR.color.g, SR.color.b, 1f);

        gameObject.tag = "Player";
    }

    [PunRPC]
    public void TriggerAnim(string s)
    {
        switch (s)
        {
            case "isReload":
                AN.SetTrigger("isReload");
                break;
            case "isReloadOut":
                AN.SetTrigger("isReloadOut");
                break;
            case "isSitUp":
                AN.SetTrigger("isSitUp");
                break;
            case "isSitDown":
                AN.SetTrigger("isSitDown");
                break;
            case "isDie":
                AN.SetTrigger("isDie");
                break;
            default:
                break;
        }
    }

    [PunRPC]
    public void Teleport(Vector3 telPos)
    {
        transform.position = telPos;
        destPos = telPos;
    }
}
