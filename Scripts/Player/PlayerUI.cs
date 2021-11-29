                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerUI : MonoBehaviour, UnityEngine.EventSystems.IPointerClickHandler
{
    public PlayerCtrl playerCtrl;
    public WeaponManager weaponManager;
    public PlayerAction playerAction;
    public PlayerSkillManager playerSkillManager;

    #region Status UI
    [Header("Player Status UI")]
    public GameObject playerStatusUI;
    private bool statusUIisOpen = false;

    public Text nameText;
    public Text hpText;
    public Text classText;
    public Text levelText;
    public Text armourText;

    public Text weaponText;
    public Text damageText;
    public Text attackSpeedText;

    public Text healingPointText;
    public Text healingSpeedText;
    public Text buildSpeedText;
    public Text repairSpeedText;
    public Text autoRepairText;

    Coroutine coUIUpdate;

    public Text pointText;
    public Image expImage;

    #endregion

    #region Player Skill
    [Header("Player SKill")]
    public GameObject skillPointInfoObj;
    public bool havingSkillPoint_isRunning = false;

    public GameObject skillSelectObj;
    public bool selectObjisOpen = false;

    public Image havingSkillPointImage;

    public List<GameObject> skillInfo_ObjList = new List<GameObject>();

    public List<UnityEngine.UI.Image> skillInfo_ImageList = new List<Image>();
    public List<UnityEngine.UI.Text> skillInfo_TextList = new List<Text>();

    #endregion

    #region Player Item UI
    [Header("Player Item UI")]
    public Image itemPanel;
    public Image itemImg;

    #endregion

    #region Player Action UI
    [Header("Player Action UI")]
    public Image playerActionPanel;
    public Text playerActionText;

    IEnumerator IEnumActionText = null;

    #endregion

    #region Menu UI
    [Header("Menu UI")]
    public Image menuPanel;
    #endregion

    private void Start()
    {
        playerSkillManager = GetComponent<PlayerSkillManager>();

        statusUIisOpen = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            statusUIisOpen = !statusUIisOpen;

            try
            {
                playerStatusUI.SetActive(statusUIisOpen);
            }
            catch (System.Exception e)
            {
#if UNITY_EDITOR
                Debug.LogWarning(e.GetType());
#endif
            }

            if (statusUIisOpen == true)
                coUIUpdate = StartCoroutine(StatusUIActive());
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (playerCtrl.skillPoint >= 1 && skillSelectObj.activeSelf == false)
            {
                CursorState.CursorLockedSetting(false);
                skillSelectObj.SetActive(true);
            }
            else if (skillSelectObj.activeSelf == true && playerCtrl.isUIOpen == false)
            {
                CursorState.CursorLockedSetting(true);
                skillSelectObj.SetActive(false);
            }

            selectObjisOpen = skillSelectObj.activeSelf;
        }
    }

    #region Skill Point UI

    public void HavingSkillPoint(bool _havingSkillPoint)
    {
        havingSkillPointImage.gameObject.SetActive(_havingSkillPoint);
    }

    #endregion

    #region Status UI 
    /// <summary>
    /// 스탯 UI를 활성화하고 갱신하는 코루틴
    /// </summary>
    /// <returns></returns>
    public IEnumerator StatusUIActive()
    {
        while (statusUIisOpen)
        {
            try
            {
                nameText.text = $"{playerCtrl.playerName.ToString()}";
                hpText.text = $"{playerCtrl.currHP.ToString()} / {playerCtrl.maxHp.ToString()}";

                string classToKorean;

                switch (playerCtrl.playerClass)
                {
                    case PlayerClass.ePlayerClass.Soldier:
                        classToKorean = "소총병";
                        break;
                    case PlayerClass.ePlayerClass.Medic:
                        classToKorean = "의무병";
                        break;
                    case PlayerClass.ePlayerClass.Engineer:
                        classToKorean = "공병";
                        break;
                    default:
                        classToKorean = "오류 발생";
                        break;
                }

                classText.text = $"{classToKorean.ToString()}";

                levelText.text = $"{playerCtrl.level.ToString()}";
                armourText.text = $"{playerCtrl.addArmour.ToString()}";
                weaponText.text = $"{weaponManager.weaponNameText.text.ToString()}";
                damageText.text = $"{(weaponManager.currGun.damage + playerCtrl.addAttack).ToString("F2")}";
                attackSpeedText.text = $"{weaponManager.currGun.fireDelay.ToString("F2")}";
                healingPointText.text = $"{playerAction.currHealingPoint.ToString("F2")}";
                healingSpeedText.text = $"{playerAction.currHealingSpeed.ToString("F2")}";
                repairSpeedText.text = $"{playerAction.currRepariSpeed.ToString("F2")}";
                buildSpeedText.text = $"{playerAction.currBuildSpeed.ToString("F2")}";

                string autoRepair = playerAction.buildingAutoRepair ? "보유" : "미보유";

                autoRepairText.text = $"{autoRepair.ToString()}";
                pointText.text = $"{playerCtrl._point.ToString()}";
            }
            catch (System.Exception e)
            {
#if UNITY_EDITOR
                Debug.LogWarning(e.GetType());
#endif
            }
            yield return new WaitForSeconds(0.5f);
        }
        yield break;
    }

    public void ExpUISetting()
    {
        expImage.fillAmount = playerCtrl._playerExp / playerCtrl.targetExp;
    }

    #endregion

    #region Button 기능

    public void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
    {
        bool select = false;

        if (eventData.pointerCurrentRaycast.gameObject == skillInfo_ObjList[0])
        {
            playerCtrl.skillPoint -= 1;
            playerSkillManager.skillSettingComplete = false;
            playerCtrl.SkillLevelUp(playerCtrl._select_SkillList[0]);

            select = true;
        }
        else if (eventData.pointerCurrentRaycast.gameObject == skillInfo_ObjList[1])
        {
            playerCtrl.skillPoint -= 1;
            playerSkillManager.skillSettingComplete = false;
            playerCtrl.SkillLevelUp(playerCtrl._select_SkillList[1]);

            select = true;
        }
        else if (eventData.pointerCurrentRaycast.gameObject == skillInfo_ObjList[2])
        {
            playerCtrl.skillPoint -= 1;
            playerSkillManager.skillSettingComplete = false;
            playerCtrl.SkillLevelUp(playerCtrl._select_SkillList[2]);

            select = true;
        }

        if (select)
        {
            skillSelectObj.SetActive(false);

            if (playerCtrl.skillPoint <= 0)
            {
                if (playerCtrl.isUIOpen == false)
                {
                    CursorState.CursorLockedSetting(true);
                }
            }
        }
    }

    #endregion

    #region Item UI
    public void ItemUISetting(int _lastUID)
    {
        if (playerCtrl.isHaveItem == true)
        {
            itemPanel.gameObject.SetActive(true);

            if (playerCtrl.haveMedikit == true)
            {
                itemImg.sprite = Resources.Load<Sprite>("Store/ItemImage/MedikitImg");
            }
            else if (playerCtrl.haveDefStruct == true)
            {
                if (_lastUID == 0)
                {
                    itemImg.sprite = Resources.Load<Sprite>("Store/DefensiveStructureImage/Fence");
                }
                else if (_lastUID == 1)
                {
                    itemImg.sprite = Resources.Load<Sprite>("Store/DefensiveStructureImage/BarbedWire");
                }
            }
        }
        else
        {
            itemPanel.gameObject.SetActive(false);
        }
    }
    #endregion

    #region Player Action UI

    public void PlayerActionTextSetting(string _text)
    {
        if (IEnumActionText != null)
        {
            StopCoroutine(IEnumActionText);
        }
        IEnumActionText = CoActionTextSetting(_text);
        StartCoroutine(IEnumActionText);
    }

    IEnumerator CoActionTextSetting(string _text)
    {
        if (playerActionPanel.gameObject.activeSelf == false)
            playerActionPanel.gameObject.SetActive(true);
        playerActionText.text = string.Format($"{_text.ToString()}");

        yield return new WaitForSeconds(1f);

        playerActionText.text = string.Format("");

        if (playerActionPanel.gameObject.activeSelf == true)
            playerActionPanel.gameObject.SetActive(false);
        IEnumActionText = null;
    }

    #endregion 

}                                                                                                                                                                                                                                                                                                                                                                             