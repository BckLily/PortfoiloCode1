using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 커서 상태를 설정하기 위해서 만든 클래스
/// </summary>
internal class CursorState
{
    /// <summary>
    /// 커서를 고정시키는 함수
    /// </summary>
    /// <param name="_state">true는 고정, false는 고정 해제</param>
    public static void CursorLockedSetting(bool _state)
    {    
        if (_state)
        {
            Cursor.lockState = UnityEngine.CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = UnityEngine.CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}


public class PlayerAction : MonoBehaviour
{
    private Transform playerTr;
    private PlayerCtrl playerCtrl;

    ///<summary>
    /// CameraRaycast Script
    ///</summary>
    private CameraRaycast cameraRaycast;

    #region 플레이어의 현재 동작 관련 변수
    public bool isBuild;
    public bool isBuy;
    public bool isHeal;
    public bool isRepair;

    private float healingPoint;
    public float incHealingPoint;
    public float incHealingPoint_Perk;
    /// <summary>
    /// 회복 아이템 사용시 회복되는 현재 수치
    /// </summary>
    public float currHealingPoint { get { return healingPoint * (1 + ((incHealingPoint + incHealingPoint_Perk) * 0.01f)); } }

    private float healingSpeed;
    public float incHealingSpeed;
    public float incHealingSpeed_Perk;
    /// <summary>
    /// 회복 아이템 사용 속도의 현재 수치
    /// </summary>
    public float currHealingSpeed
    {
        get
        {
            float _value = healingSpeed * (1 - ((incHealingSpeed + incHealingSpeed_Perk) * 0.01f));
            return ((_value >= 0.05f) ? _value : 0.05f);
        }
    }

    public float dontUseHealingItem_Percent = 0f;

    private float buildSpeed;
    public float incBuildSpeed;
    public float incBuildSpeed_Perk = 0f;
    /// <summary>
    /// 방어 물자 건설 시 걸리는 시간
    /// </summary>
    public float currBuildSpeed
    {
        get
        {
            float _value = buildSpeed * (1 - ((incBuildSpeed + incBuildSpeed_Perk) * 0.01f));

            return ((_value >= 2.5f) ? _value : 2.5f);
        }
    }

    private float repairSpeed;
    public float incRepairSpeed;
    /// <summary>
    /// 현재 방어물자 수리 속도
    /// </summary>
    public float currRepariSpeed
    {
        get
        {
            float _value = repairSpeed * (1 - (incRepairSpeed * 0.01f));
            return ((_value >= 0.5f) ? _value : 0.5f);
        }
    }

    public float incBuildMaxHealthPoint = 0f;
    public bool buildingAutoRepair = false;
    public bool selfHealing = false;
    #endregion


    private Animator playerAnim;

    private float searchTime = 0f;
    private float searchDelay = 0.05f;
    private GameObject target = null;
    string targetTag = null;

    [Header("Target Information")]
    /// <summary>
    /// Player가 보고 있는 타겟의 정보를 표시해줄 Text
    /// </summary>
    public Text targetInfoText;
    /// <summary>
    /// Player가 보고 있는 타겟의 정보를 표시할 Panel
    /// </summary>
    public GameObject targetInfoPanel;

    [Space(5, order = 0)] 
    [Header("Aim Point", order = 1)]
    public GameObject crosshair;
    public GameObject gaugeRing;

    #region LayerMask
    // 타겟으로 사용할 Layer
    LayerMask allLayerMask;

    #endregion

    void Start()
    {
        playerTr = transform.parent.GetComponent<Transform>();
        playerCtrl = playerTr.GetComponent<PlayerCtrl>();
        cameraRaycast = GetComponent<CameraRaycast>();

        isBuild = false;
        isBuy = false;
        isHeal = false;

        healingPoint = 50f;
        incHealingPoint = 0f;
        healingSpeed = 5f;
        incHealingSpeed = 0f;
        buildSpeed = 7.5f;
        incBuildSpeed = 0f;
        repairSpeed = 5f;
        incRepairSpeed = 0f;

        CursorState.CursorLockedSetting(true);

        LayerMask blueprint = LayerMask.NameToLayer("BLUEPRINT");
        LayerMask enemyLayer = LayerMask.NameToLayer("ENEMY");
        LayerMask defensiveGoodsLayer = LayerMask.NameToLayer("DEFENSIVEGOODS");
        LayerMask storeLayer = LayerMask.NameToLayer("STORE");
        LayerMask playerLayer = LayerMask.NameToLayer("PLAYER");
        LayerMask bunkerDoorLayer = LayerMask.NameToLayer("BUNKERDOOR");
        LayerMask wallLayer = LayerMask.NameToLayer("WALL");

        allLayerMask = (1 << blueprint) | (1 << enemyLayer) | (1 << defensiveGoodsLayer) | (1 << storeLayer) | (1 << playerLayer) | (1 << bunkerDoorLayer) | (1 << wallLayer);
    }

    void Update()
    {
        if (playerCtrl.isUIOpen == false)
        {
            CheckLooking();
            Action();
        }
        else
        {
            if (targetInfoPanel.activeSelf == true)
            {
                targetInfoPanel.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 플레이어가 보고 있는 위치게 어떤 물건이 있는지 확인하는 함수
    /// </summary>
    private void CheckLooking()
    {
        searchTime += Time.deltaTime;
        if (searchTime >= searchDelay)
        {
            searchTime -= searchDelay;

            try
            {
                target = (GameObject)cameraRaycast.GetRaycastTarget(6f, allLayerMask).transform.gameObject;
                targetTag = target.tag;
            }
            catch (NullReferenceException e)
            {
                targetTag = null;
                targetInfoPanel.SetActive(false);
                return;
            }
        }
    }

    /// <summary>
    /// target의 Tag에 따라 어떤 동작을 할지 결정하는 함수
    /// </summary>
    private void Action()
    {
        if (target == null)
        {
            GaugeClear();
            return;
        }

        if (selfHealing == false && targetTag == "BLUEPRINT")
        {
            if (targetInfoPanel.activeSelf == false) { targetInfoPanel.SetActive(true); }

            bool canBuild = !target.GetComponent<Blueprint>().isBuild;

            if (playerCtrl.haveDefStruct == false)
            {
                targetInfoText.text = TargetInfoTextSetting("아이템이 필요합니다.");
            }
            else
            {
                if (target.GetComponent<Blueprint>()._uid != playerCtrl._haveItemUID)
                {
                    targetInfoText.text = TargetInfoTextSetting("아이템이 필요합니다.");
                }
                else
                {
                    targetInfoText.text = (TargetInfoTextSetting(canBuild ? "건설 가능" : "건설 완료"));

                    if (canBuild)
                    {
                        if (Input.GetKey(KeyCode.E))
                        {
                            isBuild = true;
                            if (FillGauge(currBuildSpeed))
                            {
                                playerCtrl.haveDefStruct = false;
                                playerCtrl.ItemSetting("0");

                                playerCtrl.ActionTextSetting("건설 완료");

                                Blueprint _bluePrint = target.GetComponent<Blueprint>();

                                _bluePrint.BuildingBuild();

                                if (buildingAutoRepair == true)
                                {
                                    _bluePrint.StartAutoRepair();
                                }
                            }
                        }
                        else if (Input.GetKeyUp(KeyCode.E))
                        {
                            isBuild = false;
                            GaugeClear();
                        }
                    }
                    else
                    {
                        GaugeClear();
                        if (targetInfoPanel.activeSelf == true)
                        {
                            targetInfoPanel.SetActive(false);
                        }
                    }
                }
            }
        }
        else if (selfHealing == false && (targetTag == "FENCE" || targetTag == "BARBEDWIRE"))
        {
            if (targetInfoPanel.activeSelf == false) { targetInfoPanel.SetActive(true); }

            DefensiveStructure defSturct = target.GetComponent<DefensiveStructure>();

            if (defSturct.currHP < defSturct.startHp)
            {
                targetInfoText.text = TargetInfoTextSetting("수리 필요");
                if (Input.GetKey(KeyCode.E))
                {
                    isRepair = true;
                    if (FillGauge(currRepariSpeed))
                    {
                        playerCtrl.ActionTextSetting("수리 완료");
                        defSturct.Repair();
                    }
                }
                else if (Input.GetKeyUp(KeyCode.E))
                {
                    isRepair = false;
                    GaugeClear();
                }
            }
            else
            {
                GaugeClear();
                if (targetInfoPanel.activeSelf == true)
                {
                    targetInfoPanel.SetActive(false);
                }
            }
        }
        else if (selfHealing == false && (targetTag == "STORE" && Vector3.Distance(this.transform.position, target.transform.position) <= 5f))
        {
            if (targetInfoPanel.activeSelf == false) { targetInfoPanel.SetActive(true); }
            targetInfoText.text = TargetInfoTextSetting("상점");
            if (Input.GetKeyDown(KeyCode.E))
            {
                target.GetComponent<Store>().OpenStore(playerTr);
            }
        }
        else if (selfHealing == false && targetTag == "PLAYER")
        {
            PlayerCtrl _targetPlayer = target.GetComponent<PlayerCtrl>();
            if (_targetPlayer.currHP < _targetPlayer.maxHp)
            {
                if (targetInfoPanel.activeSelf == false) { targetInfoPanel.SetActive(true); }
                if (Input.GetKey(KeyCode.E))
                {
                    isHeal = true;
                    if (FillGauge(healingSpeed))
                    {
                        playerCtrl.ActionTextSetting("회복 완료");
                        float _rand = UnityEngine.Random.Range(0f, 100f);

                        if (dontUseHealingItem_Percent < _rand)
                        {
                            playerCtrl.haveMedikit = false;
                            playerCtrl.ItemSetting("0");
                        }

                        _targetPlayer.Healing(currHealingPoint);
                    }
                }
                else if (Input.GetKeyUp(KeyCode.E))
                {
                    isHeal = false;
                    GaugeClear();
                }
            }
        }
        else
        {
            if (playerCtrl.haveMedikit && playerCtrl.currHP < playerCtrl.maxHp)
            {
                if (targetInfoPanel.activeSelf == false) { targetInfoPanel.SetActive(true); }
                targetInfoText.text = TargetInfoTextSetting("회복 가능");
                if (Input.GetKey(KeyCode.E))
                {
                    selfHealing = true;
                    if (FillGauge(healingSpeed))
                    {
                        playerCtrl.ActionTextSetting("회복 완료");

                        float _rand = UnityEngine.Random.Range(0f, 100f);
                        if (dontUseHealingItem_Percent < _rand)
                        {
                            playerCtrl.haveMedikit = false;
                            playerCtrl.ItemSetting("0");
                        }
                        playerCtrl.Healing(currHealingPoint);
                    }
                }
                else if (Input.GetKeyUp(KeyCode.E))
                {
                    selfHealing = false;
                    GaugeClear();
                }
            }
            else
            {
                GaugeClear();
                if (targetInfoPanel.activeSelf == true)
                {
                    targetInfoPanel.SetActive(false);
                }
            }
        }
    }

    #region Target Information UI
    /// <summary>
    /// Target의 Information Text를 Setting할 때 사용하는 함수
    /// </summary>
    /// <param name="_text">Change this text to bold</param>
    /// <returns>필요한 부분을 Bold체로 변경한 값을 반환</returns>
    private string TargetInfoTextSetting(string _text)
    {
        string _string = string.Format($"<b>{_text}</b>");
        return _string;
    }
    #endregion

    #region Gauge UI Control Function
    /// <summary>
    /// 특정 동작시 게이지를 채우기 위해서 동작하는 함수
    /// </summary>
    /// <param name="_chargingTime">게이지를 채우는데 걸리는 시간</param>
    /// <returns>게이지가 100%가 되면 true 아닐경우 false</returns>
    private bool FillGauge(float _chargingTime)
    {
        if (crosshair.activeSelf == true) { crosshair.SetActive(false); }
        if (gaugeRing.activeSelf == false) { gaugeRing.SetActive(true); }
        Image ringImgae = gaugeRing.GetComponent<Image>();
        ringImgae.fillAmount += (1 / _chargingTime) * Time.deltaTime;

        if (ringImgae.fillAmount >= 1f) { return true; }

        return false;
    }

    /// <summary>
    /// FillGauge를 실행하던 중 취소되거나 완료 시 실행되는 함수.
    /// </summary>
    private void GaugeClear()
    {
        if (gaugeRing.activeSelf == true)
        {
            isHeal = false;
            isRepair = false;
            isBuild = false;
            selfHealing = false;

            gaugeRing.GetComponent<Image>().fillAmount = 0f;
            gaugeRing.SetActive(false);
        }
        if (crosshair.activeSelf == false) { crosshair.SetActive(true); }
    }
    #endregion

}
