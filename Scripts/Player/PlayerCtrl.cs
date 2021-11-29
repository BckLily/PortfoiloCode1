using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using InterfaceSet;
using System;
using UnityEngine.UI;



public class PlayerCtrl : LivingEntity
{
    #region 플레이어 UI 관련 변수
    public Image playerHpImage;
    public Text playerHpText;
    private PlayerUI playerUI;

    [SerializeField]
    private Text playerLevelText;

    public bool isUIOpen;
    public bool doAction
    {
        get { return playerAction.isBuild || playerAction.isRepair || playerAction.isHeal || playerAction.selfHealing; }
    }
    #endregion

    #region 플레이어 Status 관련 변수
    /// <summary>
    /// 플레이어 최대 체력
    /// </summary>
    public float maxHp { get { return startHp + addHP; } }
    /// <summary>
    /// 플레이어 추가 체력
    /// </summary>
    public float addHP { get; private set; }
    /// <summary>
    /// 플레이어 추가 방어력
    /// </summary>
    public float addArmour { get; private set; }

    internal float addAttack = 0f;
    private float attackPerk_Percent = 0f;
    private float addAttack_Perk = 0f;
    /// <summary>
    /// 현재 플레이어 추가 공격력
    /// </summary>
    public float currAddAttack
    {
        get
        {
            float _value = addAttack;

            if (playerSkillManager.perk2_Level >= 1)
            {
                int percent = UnityEngine.Random.Range(0, 100);
                if (!(percent >= (100 - attackPerk_Percent)))
                {
                    _value += addAttack_Perk;
                }
            }

            return _value;
        }
    }

    public Image bloodScreen;

    private int playerMaxLevel = 18;
    private int statusMaxLevel = 5;
    private int abilityMaxLevel = 3;
    private int perkMaxLevel = 1;

    public int level = 1;
    public int skillPoint = 0;
    public int _point;

    internal float targetExp = 50f;
    internal float _playerExp = 0f;

    #endregion

    #region 플레이어 이동 관련 변수
    public CharacterController controller;

    private float useSpeed;
    private float changedSpeed;
    private float walkSpeed;
    private float runSpeed;
    private float crouchSpeed;
    private float gravity;

    private float motionChangeSpeed;
    #endregion

    [Space(5)]
    [Header("Player Camera", order = 1)]
    #region 플레이어 카메라 관련 변수
    public Transform playerCameraTr;
    private PlayerAction playerAction;
    private Vector3 cameraPosition;

    private Rigidbody myRb;

    /// <summary>
    /// 플레이어의 상체 회전 속도
    /// </summary>
    private float upperBodyRotation;
    /// <summary>
    /// 상체 회전의 한계 값.
    /// </summary>
    private float upperBodyRotationLimit;
    /// <summary>
    /// 화면(카메라) 회전 속도
    /// </summary>
    /// <param name=""></param>
    [SerializeField]
    [Range(1.5f, 8f)]
    private float lookSensitivity;

    #endregion

    #region 플레이어 행동 bool 변수
    private bool isMove;
    private bool isCrouch;

    #endregion

    #region 플레이어 관련 변수
    private Transform tr;
    public string playerName { get; set; }
    public PlayerClass.ePlayerClass playerClass { get; set; }
    public Animator playerAnim;

    #endregion

    #region 플레이어 직업 관련 변수
    Dictionary<string, string> classDict = null;

    #endregion

    #region 플레이어 무기 관련 변수
    public WeaponManager weaponManager = null;
    public float incCarryBullet;

    private float incAttackSpeed;
    private float incAttackSpeed_Perk;
    public float currIncAttackSpeed
    {
        get
        {
            float _value = (incAttackSpeed + incAttackSpeed_Perk);
            return _value;
        }
    }

    #endregion

    #region 플레이어 스킬 관련 변수
    private PlayerSkillManager playerSkillManager;

    public List<string> _select_SkillList = null;

    #endregion

    #region 아이템 관련 변수
    public string _haveItemUID = null;
    public bool isHaveItem
    {
        get
        {
            return haveMedikit || haveDefStruct;
        }
    }
    public bool haveMedikit = false;
    public bool haveDefStruct = false;

    #endregion

    public GameObject crosshairPanel;

    private void Awake()
    {
        playerUI = GetComponent<PlayerUI>();
        playerAction = playerCameraTr.GetComponent<PlayerAction>();
        tr = this.GetComponent<Transform>();

        myRb = this.controller.GetComponent<Rigidbody>();

        playerSkillManager = GetComponent<PlayerSkillManager>();
    }

    void Start()
    {
        StartCoroutine(CoPlayerClassSetting());

        controller.enabled = true;

        startHp = 100f;
        addHP = 0f;
        currHP = maxHp;
        addArmour = 0f;
        addAttack = 0f;
        addAttack_Perk = 0f;

        walkSpeed = 6f;
        runSpeed = walkSpeed * 1.5f;
        crouchSpeed = walkSpeed * 0.35f;
        changedSpeed = walkSpeed;
        useSpeed = changedSpeed;
        gravity = -0.098f;

        motionChangeSpeed = 4f;

        upperBodyRotation = 0f;
        upperBodyRotationLimit = 35f;

        isMove = false;
        isCrouch = false;

        cameraPosition = playerCameraTr.localPosition;

        incAttackSpeed = 0f;
        incAttackSpeed_Perk = 0f;
        incCarryBullet = 0f;

        HPGaugeChange();

        _select_SkillList = new List<string>();

        _point = 400;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        currHP = maxHp;

        if (PlayerPrefs.HasKey("LookSensitivity"))
        {
            Debug.Log("Load Sensitivity");
            lookSensitivity = PlayerPrefs.GetFloat("LookSensitivity");
        }
        else
        {
            lookSensitivity = 4.5f;
        }
    }


    #region 플레이어 직업을 세팅하고 직업 관련 데이터를 가져오는 함수 관련
    /// <summary>
    /// 플레이어 직업 관련된 값을 Dictionary 변수에 저장할 때까지 시도하는 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator CoPlayerClassSetting()
    {
        yield return new WaitForSeconds(0.05f);
        while (classDict == null)
        {
            classDict = DBManager.Instance.GetClassInfo(playerClass);
            yield return null;
        }

        PlayerWeaponChange(classDict["WeaponUID"]);
    }

    #endregion

    #region Player의 무기를 변경하는 함수 관련
    public bool PlayerWeaponChange(string _weaponUID)
    {
        string _rarity = _weaponUID.Substring(5, 4);
        if (_rarity == "0001" && GameManager.instance.perk0_Active == false)
        {
            ActionTextSetting("1단계 특전을 활성화해야 합니다.");
            return false;
        }
        else if (_rarity == "0002" && GameManager.instance.perk1_Active == false)
        {
            ActionTextSetting("2단계 특전을 활성화해야 합니다.");
            return false;
        }

        ActionTextSetting("무기 변경 중");
        weaponManager.WeaponChange(_weaponUID);
        return true;
    }

    #endregion

    void Update()
    {
        if (Input.GetKey(KeyCode.RightBracket))
        {
            lookSensitivity += 0.05f;
            lookSensitivity = Mathf.Clamp(lookSensitivity, 0.5f, 8f);
        }
        else if (Input.GetKey(KeyCode.LeftBracket))
        {
            lookSensitivity -= 0.05f;
            lookSensitivity = Mathf.Clamp(lookSensitivity, 0.5f, 8f);
        }
        else if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Debug.Log("Save Sensitivity");
            PlayerPrefs.SetFloat("LookSensitivity", lookSensitivity);
            PlayerPrefs.Save();
        }

        if (isUIOpen == false)
            PlayerMove();

        if (Input.GetKeyDown(KeyCode.Escape) && transform.Find("StoreCanvas") == null)
        {
            MenuOpen();
        }

        if (isUIOpen && crosshairPanel.activeSelf)
            crosshairPanel.SetActive(false);
        else if (!isUIOpen && !crosshairPanel.activeSelf)
            crosshairPanel.SetActive(true);

        #region Editor Test Code
#if UNITY_EDITOR
        // 무기 변경 테스트 코드
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            playerClass = PlayerClass.ePlayerClass.Soldier;
            classDict = null;
            StartCoroutine(CoPlayerClassSetting());
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            playerClass = PlayerClass.ePlayerClass.Medic;
            classDict = null;
            StartCoroutine(CoPlayerClassSetting());
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            playerClass = PlayerClass.ePlayerClass.Engineer;
            classDict = null;
            StartCoroutine(CoPlayerClassSetting());
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PlayerWeaponChange(weaponManager.currGun.weaponDict["WeaponUID"]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SpwanManager.Instance.EnemySpawnFunc();
        }
        else if (Input.GetKeyDown(KeyCode.Backslash))
        {
            _point = 10000;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            _playerExp += 90;
            CheckLevelUp();
        }

#endif
        #endregion

        #region Build Cheat

        if (Input.GetKeyDown(KeyCode.F6))
        {
            GameManager.instance.useCheat = true;
            ActionTextSetting("테스트 모드 활성화");
        }

#if UNITY_STANDALONE_WIN

        if (GameManager.instance.useCheat)
        {
            // 직업 설정 테스트 코드
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                playerClass = PlayerClass.ePlayerClass.Soldier;
                classDict = null;
                StartCoroutine(CoPlayerClassSetting());
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                playerClass = PlayerClass.ePlayerClass.Medic;
                classDict = null;
                StartCoroutine(CoPlayerClassSetting());
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                playerClass = PlayerClass.ePlayerClass.Engineer;
                classDict = null;
                StartCoroutine(CoPlayerClassSetting());
            }
            // 포인트 획득
            else if (Input.GetKeyDown(KeyCode.Backslash))
            {
                _point = 10000;
            }
            // 레벨 증가
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                _playerExp += 90;
                CheckLevelUp();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                SpwanManager.Instance.EnemySpawnFunc();
            }
        }
#endif
        #endregion

    }

    private void LateUpdate()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            PlayerRotation();
            UpperBodyRotation();
        }
    }

    #region 플레이어 스킬 관련 
    /// <summary>
    /// 플레이어가 스킬을 획득시 변경되는 값들을 설정하는 함수
    /// </summary>
    /// <param name="_playerSkillUID">획득한 스킬 UID</param>
    /// <param name="_skillLevel">획득한 스킬의 Level</param>
    void PlayerSkillSetting(string _playerSkillUID, int _skillLevel)
    {
        int firstUID;
        int middleUID;
        int lastUID;

        try
        {
            firstUID = int.Parse(_playerSkillUID.Substring(0, 2));
            if (firstUID != 03) { return; }

            middleUID = int.Parse(_playerSkillUID.Substring(2, 3));
            lastUID = int.Parse(_playerSkillUID.Substring(5, 4));
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e);
            return;
        }

        Dictionary<string, string> __skillInfo = DBManager.Instance.GetPlayerSkill(_playerSkillUID);

        string _name = __skillInfo["PlayerSkill_Name"];
        string _skillUID = __skillInfo["PlayerSkill_SkillUID"];
        float _coefficient = float.Parse(__skillInfo["PlayerSkill_Coefficient"]);

        switch (middleUID)
        {
            case 000:
                switch (lastUID)
                {
                    case 0000:
                        // Increase Weapon Damage
                        if (playerSkillManager.status0_Level > statusMaxLevel) { return; }
                        addAttack = FindPlayerSkill.GetPlayerSkill(_name, _skillUID, _skillLevel)[0] * _coefficient;
                        break;
                    case 0001:
                        // Increase Player Armor
                        if (playerSkillManager.status1_Level > statusMaxLevel) { return; }
                        addArmour = FindPlayerSkill.GetPlayerSkill(_name, _skillUID, _skillLevel)[0] * _coefficient;
                        break;
                    case 0002:
                        // Increase Player Max HP
                        if (playerSkillManager.status2_Level > statusMaxLevel) { return; }
                        addHP = FindPlayerSkill.GetPlayerSkill(_name, _skillUID, _skillLevel)[0] * _coefficient;
                        HPGaugeChange();
                        break;
                    default:
#if UNITY_EDITOR
                        Debug.Log("__Wrong UID Input__");
#endif
                        break;
                }

                break;
            case 001:
                switch (lastUID)
                {
                    case 0000:
                        // Incrase Max Carry Bullet
                        if (playerSkillManager.ability0_Level > abilityMaxLevel) { return; }
                        incCarryBullet = FindPlayerSkill.GetPlayerSkill(_name, _skillUID, _skillLevel)[0] * _coefficient;
                        break;
                    case 0001:
                        // Increase Weapon Attack Speed
                        if (playerSkillManager.ability1_Level > abilityMaxLevel) { return; }
                        incAttackSpeed = FindPlayerSkill.GetPlayerSkill(_name, _skillUID, _skillLevel)[0] * _coefficient;
                        break;
                    case 0002:
                        // Increase Item Healing Point
                        if (playerSkillManager.ability0_Level > abilityMaxLevel) { return; }
                        playerAction.incHealingPoint = FindPlayerSkill.GetPlayerSkill(_name, _skillUID, _skillLevel)[0] * _coefficient;
                        break;
                    case 0003:
                        // Increase Healing Item Use Speed
                        if (playerSkillManager.ability1_Level > abilityMaxLevel) { return; }
                        playerAction.incHealingSpeed = FindPlayerSkill.GetPlayerSkill(_name, _skillUID, _skillLevel)[0] * _coefficient;
                        break;
                    case 0004:
                        // Increase Build Speed
                        if (playerSkillManager.ability0_Level > abilityMaxLevel) { return; }
                        playerAction.incBuildSpeed = FindPlayerSkill.GetPlayerSkill(_name, _skillUID, _skillLevel)[0] * _coefficient;
                        break;
                    case 0005:
                        // Increase Repair Speed
                        if (playerSkillManager.ability1_Level > abilityMaxLevel) { return; }
                        playerAction.incRepairSpeed = FindPlayerSkill.GetPlayerSkill(_name, _skillUID, _skillLevel)[0] * _coefficient;
                        break;
                    default:
#if UNITY_EDITOR
                        Debug.Log("__Wrong UID Input__");
#endif
                        break;
                }

                break;
            case 002:
                switch (lastUID)
                {
                    case 0000:
                        if (playerSkillManager.perk0_Level > perkMaxLevel) { return; }
                        // Increase Attack Speed Perk
                        incAttackSpeed_Perk = FindPlayerSkill.GetPlayerSkill(_name, _skillUID, _skillLevel)[0] * _coefficient;
                        StartCoroutine(weaponManager.WeaponStatusSetting());
                        break;
                    case 0001:
                        // Dont Use Bullet Perk
                        if (playerSkillManager.perk1_Level > perkMaxLevel) { return; }
                        float _value = (FindPlayerSkill.GetPlayerSkill(_name, _skillUID, _skillLevel)[0]);
                        weaponManager.dontUseBulletPercent = _value * _coefficient;
                        weaponManager.dontUseBullet = (_value == 1f * _skillLevel) ? true : false;
                        break;
                    case 0002:
                        // Increase Weapon Damage Perk
                        if (playerSkillManager.perk2_Level > perkMaxLevel) { return; }
                        List<float> _list = FindPlayerSkill.GetPlayerSkill(_name, _skillUID, _skillLevel);
                        attackPerk_Percent = _list[0];
                        addAttack_Perk = _list[1] * _coefficient;
                        break;
                    case 0003:
                        // Increase Healing Point Perk
                        if (playerSkillManager.perk0_Level > perkMaxLevel) { return; }
                        playerAction.incHealingPoint_Perk = FindPlayerSkill.GetPlayerSkill(_name, _skillUID, _skillLevel)[0] * _coefficient;
                        break;
                    case 0004:
                        // Increase Healing Speed Perk 
                        if (playerSkillManager.perk1_Level > perkMaxLevel) { return; }
                        playerAction.incHealingSpeed_Perk = FindPlayerSkill.GetPlayerSkill(_name, _skillUID, _skillLevel)[0] * _coefficient;
                        break;
                    case 0005:
                        // Dont Use Healing Item Perk
                        if (playerSkillManager.perk2_Level > perkMaxLevel) { return; }
                        playerAction.dontUseHealingItem_Percent = FindPlayerSkill.GetPlayerSkill(_name, _skillUID, _skillLevel)[0] * _coefficient;
                        break;
                    case 0006:
                        // Increase Build Speed Perk
                        if (playerSkillManager.perk0_Level > perkMaxLevel) { return; }
                        playerAction.incBuildSpeed_Perk = FindPlayerSkill.GetPlayerSkill(_name, _skillUID, _skillLevel)[0] * _coefficient;
                        break;
                    case 0007:
                        // Increase Building Max Health Point Perk
                        if (playerSkillManager.perk1_Level > perkMaxLevel) { return; }
                        playerAction.incBuildMaxHealthPoint = FindPlayerSkill.GetPlayerSkill(_name, _skillUID, _skillLevel)[0] * _coefficient;
                        break;
                    case 0008:
                        // Building Auto Repair Perk
                        if (playerSkillManager.perk2_Level > perkMaxLevel) { return; }
                        playerAction.buildingAutoRepair = (FindPlayerSkill.GetPlayerSkill(_name, _skillUID, _skillLevel)[0] == 1);
                        break;
                    default:
#if UNITY_EDITOR
                        Debug.Log("__Wrong UID Input__" + _skillUID);
#endif
                        break;
                }

                break;
            default:

                break;
        }

    }

    /// <summary>
    /// 동일한 UID를 가지는 스킬의 Level을 증가시키는 함수
    /// </summary>
    /// <param name="_skillUID">Level을 증가시킬 스킬의 UID</param>
    /// SkillLevelUp() 에서 PlayerSkillSetting()으로 넘어간다.
    internal void SkillLevelUp(string _skillUID)
    {
        if (_skillUID == classDict["StatusSkill0_UID"])
        {
            if (playerSkillManager.status0_Level < statusMaxLevel)
            {
                playerSkillManager.status0_Level++;
                _select_SkillList.Clear();
                PlayerSkillSetting(classDict["StatusSkill0_UID"], playerSkillManager.status0_Level);
            }
        }
        else if (_skillUID == classDict["StatusSkill1_UID"])
        {
            if (playerSkillManager.status1_Level < statusMaxLevel)
            {
                playerSkillManager.status1_Level++;
                _select_SkillList.Clear();
                PlayerSkillSetting(classDict["StatusSkill1_UID"], playerSkillManager.status1_Level);
            }
        }
        else if (_skillUID == classDict["StatusSkill2_UID"])
        {
            if (playerSkillManager.status2_Level < statusMaxLevel)
            {
                playerSkillManager.status2_Level++;
                _select_SkillList.Clear();
                PlayerSkillSetting(classDict["StatusSkill2_UID"], playerSkillManager.status2_Level);
            }
        }
        else if (_skillUID == classDict["AbilitySkill0_UID"])
        {
            if (playerSkillManager.ability0_Level < abilityMaxLevel)
            {
                playerSkillManager.ability0_Level++;
                _select_SkillList.Clear();
                PlayerSkillSetting(classDict["AbilitySkill0_UID"], playerSkillManager.ability0_Level);
            }
        }
        else if (_skillUID == classDict["AbilitySkill1_UID"])
        {
            if (playerSkillManager.ability1_Level < abilityMaxLevel)
            {
                playerSkillManager.ability1_Level++;
                _select_SkillList.Clear();
                PlayerSkillSetting(classDict["AbilitySkill1_UID"], playerSkillManager.ability1_Level);
            }
        }
        else if (_skillUID == classDict["Perk0_UID"])
        {
            if (playerSkillManager.perk0_Level < perkMaxLevel)
            {
                playerSkillManager.perk0_Level++;
                PlayerSkillSetting(classDict["Perk0_UID"], playerSkillManager.perk0_Level);
                ActionTextSetting("특전1이 활성화되었습니다.");
            }
        }
        else if (_skillUID == classDict["Perk1_UID"])
        {
            if (playerSkillManager.perk1_Level < perkMaxLevel)
            {
                playerSkillManager.perk1_Level++;
                PlayerSkillSetting(classDict["Perk1_UID"], playerSkillManager.perk1_Level);
                ActionTextSetting("특전2가 활성화되었습니다.");
            }
        }
        else if (_skillUID == classDict["Perk2_UID"])
        {
            if (playerSkillManager.perk2_Level < perkMaxLevel)
            {
                playerSkillManager.perk2_Level++;
                PlayerSkillSetting(classDict["Perk2_UID"], playerSkillManager.perk2_Level);
                ActionTextSetting("특전3이 활성화되었습니다.");
            }
        }

        if (skillPoint >= 1)
        {
            StartCoroutine(SelectSkill());
        }
        playerUI.HavingSkillPoint(skillPoint >= 1 ? true : false);
    }

    internal IEnumerator SelectSkill()
    {
        if (playerSkillManager.skillSettingComplete == true)
        {
            yield break;
        }

        List<int> _skillList = new List<int>();

        if (playerSkillManager.status0_Level < statusMaxLevel)
        {
            _skillList.Add(0);
        }
        if (playerSkillManager.status1_Level < statusMaxLevel)
        {
            _skillList.Add(1);
        }
        if (playerSkillManager.status2_Level < statusMaxLevel)
        {
            _skillList.Add(2);
        }
        if (playerSkillManager.ability0_Level < abilityMaxLevel)
        {
            _skillList.Add(3);
        }
        if (playerSkillManager.ability1_Level < abilityMaxLevel)
        {
            _skillList.Add(4);
        }

        for (int i = 0; i < 3; i++)
        {
            int _rand = _skillList[UnityEngine.Random.Range(0, _skillList.Count)];
            switch (_rand)
            {
                case 0:
                    _select_SkillList.Add(classDict["StatusSkill0_UID"]);

                    break;
                case 1:
                    _select_SkillList.Add(classDict["StatusSkill1_UID"]);

                    break;
                case 2:
                    _select_SkillList.Add(classDict["StatusSkill2_UID"]);

                    break;
                case 3:
                    _select_SkillList.Add(classDict["AbilitySkill0_UID"]);

                    break;
                case 4:
                    _select_SkillList.Add(classDict["AbilitySkill1_UID"]);

                    break;
                default:
#if UNITY_EDITOR
                    Debug.LogWarning("____Out of Range____");
#endif
                    break;
            }
            yield return null;
        }

        playerSkillManager.skillSettingComplete = true;

        StartCoroutine(playerSkillManager.SelectSkillSetting());
    }
    #endregion

    #region 플레이어의 움직임을 컨트롤 하는 영역

    /// <summary>
    /// 플레이어의 움직임을 조절하는 함수
    /// </summary>
    private void PlayerMove()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.LeftShift))
        {
            changedSpeed = runSpeed;
        }
        else if (isCrouch == false)
        {
            changedSpeed = walkSpeed;
        }

        Vector3 dir = new Vector3(h, 0f, v);
        dir.Normalize();

        playerAnim.SetFloat("Horizontal", dir.x);
        playerAnim.SetFloat("Vertical", dir.z);

        useSpeed = Mathf.Lerp(useSpeed, changedSpeed, Time.deltaTime * motionChangeSpeed);
        if (useSpeed != changedSpeed && Mathf.Abs((useSpeed - changedSpeed) / changedSpeed) <= 0.1f)
        {
            useSpeed = changedSpeed;
        }

        dir = transform.TransformDirection(dir) * useSpeed * Time.deltaTime;

        if (dir.magnitude >= 0.01f) { isMove = true; }
        else { isMove = false; }

        playerAnim.SetBool("IsMove", isMove);
        playerAnim.SetBool("IsCrouch", isCrouch);
        playerAnim.SetFloat("Speed", useSpeed);

        dir.y = gravity;
        controller.Move(dir);
    }

    /// <summary>
    /// Player 회전 함수
    /// </summary>
    private void PlayerRotation()
    {
        float yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 characterRotationY = new Vector3(0f, yRotation, 0f) * lookSensitivity;

        tr.Rotate(characterRotationY, Space.Self);
    }

    /// <summary>
    /// 플레이어 상체 회전(위 아래) 함수
    /// </summary>
    private void UpperBodyRotation()
    {
        float rotation = Input.GetAxisRaw("Mouse Y");
        float bodyRotation = rotation * lookSensitivity / 2.32f;

        upperBodyRotation -= bodyRotation;
        upperBodyRotation = Mathf.Clamp(upperBodyRotation, -upperBodyRotationLimit, upperBodyRotationLimit);
        playerAnim.SetFloat("Looking", -upperBodyRotation);

        playerCameraTr.localRotation = Quaternion.Euler(new Vector3(upperBodyRotation * 0.85f, -12.5f, 0));
    }

    #endregion

    #region 플레이어가 피해를 받을 때 발생하는 함수 영역
    /// <summary>
    /// 피해를 받을 때 호출되는 함수
    /// </summary>
    /// <param name="damage">받는 데미지</param>
    /// <param name="hitPoint">공격 받은 위치</param>
    /// <param name="hitNormal">공격 받은 위치의 노말 벡터</param>
    public override float Damaged(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.Damaged(damage - addArmour, hitPoint, hitNormal);

        StartCoroutine(ShowBloodScreen());
        HPGaugeChange();
        if (currHP <= 0)
        {
            currHP = 0f;
        }
        return 0;
    }

    /// <summary>
    /// 피격시 붉은 테두리가 잠깐 생겼다가 사라진다.
    /// </summary>
    /// <returns></returns>
    IEnumerator ShowBloodScreen()
    {
        bloodScreen.color = new Color(1, 0, 0, UnityEngine.Random.Range(0.25f, 0.4f));
        yield return new WaitForSeconds(0.1f);

        bloodScreen.color = Color.clear;
    }

    /// <summary>
    /// 플레이어의 체력이 변경될 경우 체력 게이지를 변경시켜주는 함수<br/>
    /// 체력 숫자 값도 같이 변경시켜준다.
    /// </summary>
    private void HPGaugeChange()
    {
        playerHpImage.fillAmount = currHP / maxHp;
        playerHpText.text = string.Format($"<b>{currHP}</b>");

    }

    #endregion

    #region 아이템 세팅
    public bool ItemSetting(string _itemUid)
    {
        int firstUID = 0;
        int middleUID = 0;
        int lastUID = 0;
        try
        {
            firstUID = int.Parse(_itemUid.Substring(0, 2));
            middleUID = int.Parse(_itemUid.Substring(2, 3));
            lastUID = int.Parse(_itemUid.Substring(5, 4));
        }
        catch (System.Exception e)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"____ Input item uid is Wrong {_itemUid} ____");
#endif
        }

        switch (firstUID)
        {
            case 6:
                switch (middleUID)
                {
                    case 0:
                        switch (lastUID)
                        {
                            case 0:
                                _haveItemUID = _itemUid;
                                haveMedikit = true;
                                playerUI.ItemUISetting(lastUID);

                                break;
                            case 1:
                                weaponManager.currGun.carryBullet += weaponManager.currGun.maxCarryBullet;

                                break;
                            case 2:
                                GameManager.instance.bunkerDoor.GetComponent<BunkerDoor>().Repair();

                                break;
                            default:

                                break;
                        }
                        break;
                    case 1:
                        switch (lastUID)
                        {
                            case 0:
                                if (playerSkillManager.perk0_Level >= 1)
                                    return false;
                                GameManager.instance.perk0_Active = true;
                                SkillLevelUp(classDict["Perk0_UID"]);

                                break;
                            case 1:
                                if (playerSkillManager.perk1_Level >= 1)
                                    return false;
                                GameManager.instance.perk1_Active = true;
                                SkillLevelUp(classDict["Perk1_UID"]);

                                break;
                            case 2:
                                if (playerSkillManager.perk2_Level >= 1)
                                    return false;
                                GameManager.instance.perk2_Active = true;
                                SkillLevelUp(classDict["Perk2_UID"]);

                                break;
                            default:
                                Debug.LogWarning("____ Wrong UID input: " + _itemUid);

                                break;
                        }

                        break;
                    default:

                        break;
                }

                break;
            case 7:
                switch (middleUID)
                {
                    case 0:
                        _haveItemUID = _itemUid;
                        haveDefStruct = true;

                        Debug.Log("____ Last UID: " + lastUID + " ____");

                        playerUI.ItemUISetting(lastUID);

                        break;
                    default:

                        break;

                }

                break;
            default:
                playerUI.ItemUISetting(0);

                break;
        }

        return true;
    }

    #endregion

    /// <summary>
    /// 체력 회복할 때 실행하는 함수
    /// </summary>
    /// <param name="_healingPoint">회복할 양</param>
    public void Healing(float _healingPoint)
    {
        currHP += _healingPoint;
        if (currHP >= maxHp)
        {
            currHP = maxHp;
        }

        HPGaugeChange();
    }

    public void ActionTextSetting(string _text)
    {
        playerUI.PlayerActionTextSetting(string.Format($"{_text.ToString()}"));
    }

    public void CheckLevelUp()
    {
        targetExp = (level == 1 ? 50f : 90f);

        if (level < playerMaxLevel && _playerExp >= targetExp)
        {
            _playerExp -= targetExp;
            level += 1;
            skillPoint += 1;

            playerLevelText.text = level.ToString();

            StartCoroutine(SelectSkill());

            playerUI.HavingSkillPoint(skillPoint >= 1 ? true : false);
        }
        else
        {
            return;
        }

        playerUI.ExpUISetting();
        if (_playerExp >= targetExp) { CheckLevelUp(); }
    }

    protected override void Down()
    {
        isUIOpen = true;

        OnDeath();
    }

    public override void OnDeath()
    {
        if (dead == false)
        {
            dead = true;
            base.OnDeath();
            GameManager.instance.GameOver();
        }
    }

    public void MenuOpen()
    {
        playerUI.menuPanel.gameObject.SetActive(!playerUI.menuPanel.gameObject.activeSelf);
        isUIOpen = playerUI.menuPanel.gameObject.activeSelf;
        CursorState.CursorLockedSetting(!isUIOpen);
    }

}
