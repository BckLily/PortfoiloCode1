using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillManager : MonoBehaviour
{
    private PlayerCtrl playerCtrl;

    #region 플레이어 스킬 관련 변수
    public PlayerUI playerUI;

    internal int status0_Level = 0;
    internal int status1_Level = 0;
    internal int status2_Level = 0;
    internal int ability0_Level = 0;
    internal int ability1_Level = 0;
    internal int perk0_Level = 0;
    internal int perk1_Level = 0;
    internal int perk2_Level = 0;

    public bool skillSettingComplete = false;

    #endregion

    void Start()
    {
        playerCtrl = GetComponent<PlayerCtrl>();
        playerUI = GetComponent<PlayerUI>();

    }

    /// <summary>
    /// 설정된 스킬 3개를 가져와서 UID를 통해서 스킬 정보를 가져오고 UI에 표시하는 함수.
    /// 그런데 UID를 가져와도 정보를 다 가져오는게 아니라서 이 함수 내에서 String 값으로 텍스트에 직접 넣어줘야한다.
    /// 일반적인 방식은 아마 가졍는 데이터에 스킬의 설명도 같이 있어서 그 파일을 수정하면 되는 방식일텐데
    /// 가라로 만들다보니 많이 이상해졌다.
    /// </summary>
    /// <returns></returns>
    internal IEnumerator SelectSkillSetting()
    {
        CursorState.CursorLockedSetting(false);

        string imagePath = "Player/Images/Skills/";
        string imageName = "_img";

        playerUI.skillSelectObj.SetActive(true);

        for (int i = 0; i < 3; i++)
        {
            Dictionary<string, string> skillInfo = DBManager.Instance.GetPlayerSkill(playerCtrl._select_SkillList[i]);
            playerUI.skillInfo_ImageList[i].sprite = Resources.Load<Sprite>($"{imagePath}{skillInfo["PlayerSkill_Name"]}{imageName}");

            if (playerCtrl._select_SkillList[i] == "030000000")
            {

                playerUI.skillInfo_TextList[i].text = $"<b><size=50>{"공격력 증가".ToString()}</size></b>\n" +
                    $"공격력이 <color=red>{(FindPlayerSkill.GetPlayerSkill(skillInfo["PlayerSkill_Name"], skillInfo["PlayerSkill_SkillUID"], 1)[0] * float.Parse(skillInfo["PlayerSkill_Coefficient"])).ToString()}</color>" +
                    $" 증가합니다.";
            }
            else if (playerCtrl._select_SkillList[i] == "030000001")
            {
                playerUI.skillInfo_TextList[i].text = $"<b><size=50>{"방어력 증가".ToString()}</size></b>\n" +
                    $"방어력이 <color=red>{(FindPlayerSkill.GetPlayerSkill(skillInfo["PlayerSkill_Name"], skillInfo["PlayerSkill_SkillUID"], 1)[0] * float.Parse(skillInfo["PlayerSkill_Coefficient"])).ToString()}</color>" +
                    $" 증가합니다.";
            }
            else if (playerCtrl._select_SkillList[i] == "030000002")
            {
                playerUI.skillInfo_TextList[i].text = $"<b><size=50>{"최대 체력 증가".ToString()}</size></b>\n" +
                    $"최대 체력이 <color=red>{(FindPlayerSkill.GetPlayerSkill(skillInfo["PlayerSkill_Name"], skillInfo["PlayerSkill_SkillUID"], 1)[0] * float.Parse(skillInfo["PlayerSkill_Coefficient"])).ToString()}</color>" +
                    $" 증가합니다.";
            }
            else if (playerCtrl._select_SkillList[i] == "030010000")
            {
                playerUI.skillInfo_TextList[i].text = $"<b><size=50>{"최대 탄약 증가".ToString()}</size></b>\n" +
                    $"보유 최대 총알이 <color=red>{(FindPlayerSkill.GetPlayerSkill(skillInfo["PlayerSkill_Name"], skillInfo["PlayerSkill_SkillUID"], 1)[0] * float.Parse(skillInfo["PlayerSkill_Coefficient"])).ToString()}</color>" +
                    $"% 증가합니다.";
            }
            else if (playerCtrl._select_SkillList[i] == "030010001")
            {
                playerUI.skillInfo_TextList[i].text = $"<b><size=50>{"공격 속도 증가".ToString()}</size></b>\n" +
                    $"공격 속도가 <color=red>{(FindPlayerSkill.GetPlayerSkill(skillInfo["PlayerSkill_Name"], skillInfo["PlayerSkill_SkillUID"], 1)[0] * float.Parse(skillInfo["PlayerSkill_Coefficient"])).ToString()}</color>" +
                    $"% 증가합니다.";
            }
            else if (playerCtrl._select_SkillList[i] == "030010002")
            {
                playerUI.skillInfo_TextList[i].text = $"<b><size=50>{"회복량 증가".ToString()}</size></b>\n" +
                    $"아이템 회복량이 <color=red>{(FindPlayerSkill.GetPlayerSkill(skillInfo["PlayerSkill_Name"], skillInfo["PlayerSkill_SkillUID"], 1)[0] * float.Parse(skillInfo["PlayerSkill_Coefficient"])).ToString()}</color>" +
                    $" 증가합니다.";
            }
            else if (playerCtrl._select_SkillList[i] == "030010003")
            {
                playerUI.skillInfo_TextList[i].text = $"<b><size=50>{"회복 속도 증가".ToString()}</size></b>\n" +
                    $"아이템 사용 속도가 <color=red>{(FindPlayerSkill.GetPlayerSkill(skillInfo["PlayerSkill_Name"], skillInfo["PlayerSkill_SkillUID"], 1)[0] * float.Parse(skillInfo["PlayerSkill_Coefficient"])).ToString()}</color>" +
                    $"% 증가합니다.";
            }
            else if (playerCtrl._select_SkillList[i] == "030010004")
            {
                playerUI.skillInfo_TextList[i].text = $"<b><size=50>{"건설 속도 증가".ToString()}</size></b>\n" +
                    $"건설 속도가 <color=red>{(FindPlayerSkill.GetPlayerSkill(skillInfo["PlayerSkill_Name"], skillInfo["PlayerSkill_SkillUID"], 1)[0] * float.Parse(skillInfo["PlayerSkill_Coefficient"])).ToString()}</color>" +
                    $"% 증가합니다.";
            }
            else if (playerCtrl._select_SkillList[i] == "030010005")
            {
                playerUI.skillInfo_TextList[i].text = $"<b><size=50>{"수리 속도 증가".ToString()}</size></b>\n" +
                    $"수리 속도가 <color=red>{(FindPlayerSkill.GetPlayerSkill(skillInfo["PlayerSkill_Name"], skillInfo["PlayerSkill_SkillUID"], 1)[0] * float.Parse(skillInfo["PlayerSkill_Coefficient"])).ToString()}</color>" +
                    $"% 증가합니다.";
            }

            yield return null;
        }

    }

}
