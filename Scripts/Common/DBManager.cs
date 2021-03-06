using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerClass;
using SimpleJSON;
using System;
using UnityEngine.UI;
using System.IO;
using LitJson;

using UnityEngine.Video;


public class DBManager : MonoBehaviour
{
    private static DBManager instance = null;
    public static DBManager Instance { get { return instance; } private set { instance = value; } }


    [Header("PHP URL String")]
    private string allClassUrl;
    private string allWeaponUrl;
    private string allSkillUrl;
    private string allPlayerSkillUrl;
    private string allDefensiveStructureUrl;
    private string allItemUrl;
    private string allMonsterUrl;
    private string allMonsterSkillUrl;
    private string allStageSpawnUrl;

    [Header("File Path")]
    private string streamingAssetPath;
    private string jsonPath;
    private string classPath;
    private string weaponPath;
    private string skillPath;
    private string playerSkillPath;
    private string defensiveStructurePath;
    private string itemPath;
    private string monsterPath;
    private string monsterSkillPath;
    private string stageSpawnPath;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        allClassUrl = "127.0.0.1/Unity/AllClass.php";
        allWeaponUrl = "127.0.0.1/Unity/AllWeapon.php";
        allSkillUrl = "127.0.0.1/Unity/AllSkill.php";
        allPlayerSkillUrl = "127.0.0.1/Unity/AllPlayerSkill.php";
        allDefensiveStructureUrl = "127.0.0.1/Unity/AllDefensiveStructure.php";
        allItemUrl = "127.0.0.1/Unity/AllItem.php";
        allMonsterUrl = "127.0.0.1/Unity/AllMonster.php";
        allMonsterSkillUrl = "127.0.0.1/Unity/AllMonsterSkill.php";
        allStageSpawnUrl = "127.0.0.1/Unity/AllStageSpawn.php";

        streamingAssetPath = Application.streamingAssetsPath + "/";
        jsonPath = "JSON/";
        classPath = "Class/";
        weaponPath = "Weapon/";
        skillPath = "Skill/";
        playerSkillPath = "PlayerSkill/";
        defensiveStructurePath = "DefensiveStructure/";
        itemPath = "Item/";
        monsterPath = "Monster/";
        monsterSkillPath = "MonsterSkill/";
        stageSpawnPath = "StageSpawn/";


        StartCoroutine(GetAllClassCo());
        StartCoroutine(GetAllWeaponCo());
        StartCoroutine(GetAllSkillCo());
        StartCoroutine(GetAllDefensiveStructureCo());
        StartCoroutine(GetAllItemCo());
        StartCoroutine(GetAllStageSpawnCo());
        StartCoroutine(GetAllMonsterSkillCo());
        StartCoroutine(GetAllMonsterCo());
        StartCoroutine(GetAllPlayerSkillCo());

    }


    #region ?????? Json??? ????????? ?????? ???????????? ??? ??????
    #endregion
    #region Class
    /// <summary>
    /// ??????????????? ????????? ????????? ??? ????????? ????????? Dictionary ???????????? ??????
    /// </summary>
    public Dictionary<string, string> GetClassInfo(ePlayerClass playerClass)
    {
        string jsonString = null;
        try
        {
            jsonString = File.ReadAllText(streamingAssetPath + jsonPath + classPath + playerClass.ToString() + ".json");
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.Log(e);
#endif
            return null;
        }

        JsonData classData = JsonMapper.ToObject(jsonString);

        Dictionary<string, string> _classDict = new Dictionary<string, string>();

        _classDict.Add("ClassName", classData["ClassName"].ToString());
        _classDict.Add("WeaponUID", classData["WeaponUID"].ToString());
        _classDict.Add("StatusSkill0_UID", classData["StatusSkill0_UID"].ToString());
        _classDict.Add("StatusSkill1_UID", classData["StatusSkill1_UID"].ToString());
        _classDict.Add("StatusSkill2_UID", classData["StatusSkill2_UID"].ToString());
        _classDict.Add("AbilitySkill0_UID", classData["AbilitySkill0_UID"].ToString());
        _classDict.Add("AbilitySkill1_UID", classData["AbilitySkill1_UID"].ToString());
        _classDict.Add("Perk0_UID", classData["Perk0_UID"].ToString());
        _classDict.Add("Perk1_UID", classData["Perk1_UID"].ToString());
        _classDict.Add("Perk2_UID", classData["Perk2_UID"].ToString());

        return _classDict;
    }

    /// <summary>
    /// ?????? ?????? ?????? ???????????? ???????????? ?????????<br/>
    /// ????????? ???????????? ?????? ?????? ?????? ????????? ?????? ?????????.
    /// </summary>
    IEnumerator GetAllClassCo()
    {
        WWWForm form = new WWWForm();

        WWW webRequest = new WWW(allClassUrl, form);

        yield return webRequest;

        try
        {
            if (string.IsNullOrEmpty(webRequest.error))
            {
                GetAllClassJson(webRequest.text);
            }
        }
        catch (Exception e)
        {
#if    UNITY_EDITOR
            Debug.Log(e);
#endif
        }
    }

    /// <summary>
    /// ?????? ?????? ???????????? ????????? JSON?????? ???????????? ??????
    /// </summary>
    /// <param name="_jsonData"></param>
    private void GetAllClassJson(string _jsonData)
    {
        var parseData = JSON.Parse(_jsonData);
        var arrayData = parseData["results"];

        Dictionary<string, string> classDic = new Dictionary<string, string>();

        if (arrayData.Count > 0)
        {
            for (int i = 0; i < arrayData.Count; i++)
            {
                classDic.Add("ClassName", arrayData[i]["ClassName"].Value);
                classDic.Add("WeaponUID", arrayData[i]["WeaponUID"].Value);
                classDic.Add("StatusSkill0_UID", arrayData[i]["StatusSkill0_UID"].Value);
                classDic.Add("StatusSkill1_UID", arrayData[i]["StatusSkill1_UID"].Value);
                classDic.Add("StatusSkill2_UID", arrayData[i]["StatusSkill2_UID"].Value);
                classDic.Add("AbilitySkill0_UID", arrayData[i]["AbilitySkill0_UID"].Value);
                classDic.Add("AbilitySkill1_UID", arrayData[i]["AbilitySkill1_UID"].Value);
                classDic.Add("Perk0_UID", arrayData[i]["Perk0_UID"].Value);
                classDic.Add("Perk1_UID", arrayData[i]["Perk1_UID"].Value);
                classDic.Add("Perk2_UID", arrayData[i]["Perk2_UID"].Value);

                string fileName = arrayData[i]["ClassName"].Value;
                JsonData classJson = JsonMapper.ToJson(classDic);

                File.WriteAllText(streamingAssetPath + jsonPath + classPath + fileName + ".json", classJson.ToString());
                classDic.Clear();
            }
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("?????? ???????????????.");
#endif
        }
    }
    #endregion

    #region Weapon
    /// <summary>
    /// ?????? ?????? ????????? ?????? ???????????? ???????????? ???????????? ??????<br/>
    /// ?????? UID??? ??????????????? ????????????.
    /// </summary>
    /// <param name="_weaponUID"></param>
    /// <returns></returns>
    public Dictionary<string, string> GetWeaponInfo(string _weaponUID)
    {
        string jsonString = File.ReadAllText(streamingAssetPath + jsonPath + weaponPath + _weaponUID.ToString() + ".json");

        JsonData weaponData = JsonMapper.ToObject(jsonString);

        Dictionary<string, string> _weaponDict = new Dictionary<string, string>();

        _weaponDict.Add("Weapon_UID", weaponData["Weapon_UID"].ToString());
        _weaponDict.Add("Weapon_Name", weaponData["Weapon_Name"].ToString());
        _weaponDict.Add("Weapon_Damage", weaponData["Weapon_Damage"].ToString());
        _weaponDict.Add("Weapon_AttackSpeed", weaponData["Weapon_AttackSpeed"].ToString());
        _weaponDict.Add("Weapon_AttackDistance", weaponData["Weapon_AttackDistance"].ToString());
        _weaponDict.Add("Weapon_ReloadBullet", weaponData["Weapon_ReloadBullet"].ToString());
        _weaponDict.Add("Weapon_CarryBullet", weaponData["Weapon_CarryBullet"].ToString());
        _weaponDict.Add("Weapon_ReloadTime", weaponData["Weapon_ReloadTime"].ToString());
        _weaponDict.Add("Weapon_AttackRange", weaponData["Weapon_AttackRange"].ToString());

        return _weaponDict;
    }

    /// <summary>
    /// DB?????? ?????? ?????? ???????????? ?????? ???????????? ?????????<br/>
    /// ?????? ?????? ????????? ??????.
    /// </summary>
    /// <returns></returns>
    IEnumerator GetAllWeaponCo()
    {
        WWWForm form = new WWWForm();

        WWW webRequest = new WWW(allWeaponUrl, form);

        yield return webRequest;

        try
        {
            if (string.IsNullOrEmpty(webRequest.error))
            {
                GetAllWeaponJson(webRequest.text);
            }
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.Log(e);
#endif
        }
    }

    /// <summary>
    /// ?????? ?????? ????????? JSON ????????? ???????????? ??????
    /// </summary>
    /// <param name="_jsonData"></param>
    private void GetAllWeaponJson(string _jsonData)
    {
        var parseData = JSON.Parse(_jsonData);
        var arrayData = parseData["results"];

        Dictionary<string, string> weaponDict = new Dictionary<string, string>();

        if (arrayData.Count > 0)
        {
            for (int i = 0; i < arrayData.Count; i++)
            {
                weaponDict.Add("Weapon_UID", arrayData[i]["Weapon_UID"].Value);
                weaponDict.Add("Weapon_Name", arrayData[i]["Weapon_Name"].Value);
                weaponDict.Add("Weapon_Damage", arrayData[i]["Weapon_Damage"].Value);
                weaponDict.Add("Weapon_AttackSpeed", arrayData[i]["Weapon_AttackSpeed"].Value);
                weaponDict.Add("Weapon_AttackDistance", arrayData[i]["Weapon_AttackDistance"].Value);
                weaponDict.Add("Weapon_ReloadBullet", arrayData[i]["Weapon_ReloadBullet"].Value);
                weaponDict.Add("Weapon_CarryBullet", arrayData[i]["Weapon_CarryBullet"].Value);
                weaponDict.Add("Weapon_ReloadTime", arrayData[i]["Weapon_ReloadTime"].Value);
                weaponDict.Add("Weapon_AttackRange", arrayData[i]["Weapon_AttackRange"].Value);

                string fileName = arrayData[i]["Weapon_UID"].Value;
                JsonData classJson = JsonMapper.ToJson(weaponDict);

                File.WriteAllText(streamingAssetPath + jsonPath + weaponPath + fileName + ".json", classJson.ToString());

                weaponDict.Clear();
            }
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("?????? ???????????? ????????????.");
#endif
        }
    }

    #endregion

    #region Skill
    /// <summary>
    /// ?????? ????????? ????????? ?????? ?????? ??? ?????????
    /// ?????? ????????? ???????????? ?????? ????????? ?????? ????????? ???????????? ??? ??? ?????????
    /// </summary>
    /// <param name="skillUID">?????? UID</param>
    /// <returns>Skill Dict</returns>
    public Dictionary<string, string> GetSkillInfo(string skillUID)
    {
        string jsonString = null;
        try
        {
            jsonString = File.ReadAllText(streamingAssetPath + jsonPath + skillPath + skillUID.ToString() + ".json");
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogWarning(e);

#endif
            return null;
        }

        JsonData skillData = JsonMapper.ToObject(jsonString);

        Dictionary<string, string> _skillDict = new Dictionary<string, string>();

        _skillDict.Add("Skill_UID", skillData["Skill_UID"].ToString());
        _skillDict.Add("Skill_Name", skillData["Skill_Name"].ToString());

        return _skillDict;
    }

    /// <summary>
    /// DB?????? ?????? Skill ????????? ???????????? ?????????
    /// </summary>
    /// <returns></returns>
    IEnumerator GetAllSkillCo()
    {
        WWWForm form = new WWWForm();

        WWW webRequest = new WWW(allSkillUrl, form);

        yield return webRequest;

        try
        {
            if (string.IsNullOrEmpty(webRequest.error))
            {
                GetAllSkillJson(webRequest.text);
            }
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.Log(e);
#endif
        }
    }

    /// <summary>
    /// ?????? ????????? Json????????? ????????? ??????
    /// </summary>
    /// <param name="_jsonData">Json Data</param>
    private void GetAllSkillJson(string _jsonData)
    {
        var parseData = JSON.Parse(_jsonData);
        var arrayData = parseData["results"];

        Dictionary<string, string> skillDict = new Dictionary<string, string>();

        if (arrayData.Count > 0)
        {
            for (int i = 0; i < arrayData.Count; i++)
            {
                skillDict.Add("Skill_UID", arrayData[i]["Skill_UID"].Value);
                skillDict.Add("Skill_Name", arrayData[i]["Skill_Name"].Value);


                string fileName = arrayData[i]["Skill_UID"].Value;
                JsonData classJson = JsonMapper.ToJson(skillDict);

                File.WriteAllText(streamingAssetPath + jsonPath + skillPath + fileName + ".json", classJson.ToString());

                skillDict.Clear();
            }
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("?????? ???????????? ????????????.");
#endif
        }
    }

    #endregion

    #region PlayerSkill

    /// <summary>
    /// ??????????????? ????????? ????????? ??? ????????? ????????? Dictionary ???????????? ??????
    /// </summary>
    public Dictionary<string, string> GetPlayerSkill(string _skillUID)
    {
        string jsonString = null;
        try
        {
            jsonString = File.ReadAllText(streamingAssetPath + jsonPath + playerSkillPath + _skillUID.ToString() + ".json");
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.Log(e);
#endif
            return null;
        }

        JsonData playerSkillData = JsonMapper.ToObject(jsonString);

        Dictionary<string, string> _playerSkillDict = new Dictionary<string, string>();

        _playerSkillDict.Add("PlayerSkill_UID", playerSkillData["PlayerSkill_UID"].ToString());
        _playerSkillDict.Add("PlayerSkill_Name", playerSkillData["PlayerSkill_Name"].ToString());
        _playerSkillDict.Add("PlayerSkill_SkillUID", playerSkillData["PlayerSkill_SkillUID"].ToString());
        _playerSkillDict.Add("PlayerSkill_Coefficient", playerSkillData["PlayerSkill_Coefficient"].ToString());

        return _playerSkillDict;
    }

    /// <summary>
    /// DB?????? Player Skill ????????? ???????????? ?????????
    /// </summary>
    /// <returns></returns>
    IEnumerator GetAllPlayerSkillCo()
    {
        WWWForm form = new WWWForm();

        WWW webRequest = new WWW(allPlayerSkillUrl, form);

        yield return webRequest;

        try
        {
            if (string.IsNullOrEmpty(webRequest.error))
            {
                GetAllPlayerSkillJson(webRequest.text);
            }
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.Log(e);
#endif
        }
    }


    /// <summary>
    /// DB?????? ?????? ?????? ???????????? Json ????????? ???????????? ??????
    /// </summary>
    /// <param name="_jsonData">Player Skill Data??? ????????? json ????????? string text</param>
    private void GetAllPlayerSkillJson(string _jsonData)
    {
        var parseData = JSON.Parse(_jsonData);
        var arrayData = parseData["results"];

        Dictionary<string, string> playerSkillDict = new Dictionary<string, string>();

        if (arrayData.Count > 0)
        {
            for (int i = 0; i < arrayData.Count; i++)
            {
                playerSkillDict.Add("PlayerSkill_UID", arrayData[i]["PlayerSkill_UID"].Value);
                playerSkillDict.Add("PlayerSkill_Name", arrayData[i]["PlayerSkill_Name"].Value);
                playerSkillDict.Add("PlayerSkill_SkillUID", arrayData[i]["PlayerSkill_SkillUID"].Value);
                playerSkillDict.Add("PlayerSkill_Coefficient", arrayData[i]["PlayerSkill_Coefficient"].Value);

                string fileName = arrayData[i]["PlayerSkill_UID"].Value;
                JsonData classJson = JsonMapper.ToJson(playerSkillDict);

                File.WriteAllText(streamingAssetPath + jsonPath + playerSkillPath + fileName + ".json", classJson.ToString());

                playerSkillDict.Clear();
            }
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("???????????? ????????? ????????????.");
#endif
        }
    }

    #endregion

    #region DefensiveStructure
    public Dictionary<string, string> GetDefInfo(string _defUID)
    {
        string jsonString = File.ReadAllText(streamingAssetPath + jsonPath + defensiveStructurePath + _defUID.ToString() + ".json");


        JsonData defData = JsonMapper.ToObject(jsonString);

        Dictionary<string, string> _defDict = new Dictionary<string, string>();

        _defDict.Add("Defensive_UID", defData["Defensive_UID"].ToString());
        _defDict.Add("Defensive_Name", defData["Defensive_Name"].ToString());
        _defDict.Add("Defensive_Hp", defData["Defensive_Hp"].ToString());
        _defDict.Add("Defensive_Damage", defData["Defensive_Damage"].ToString());
        _defDict.Add("Defensive_Passable", defData["Defensive_Passable"].ToString());

        return _defDict;
    }

    /// <summary>
    /// DB?????? ?????? ?????? ?????? ????????? ???????????? ?????????
    /// </summary>
    /// <returns></returns>
    IEnumerator GetAllDefensiveStructureCo()
    {
        WWWForm form = new WWWForm();

        WWW webRequest = new WWW(allDefensiveStructureUrl, form);

        yield return webRequest;

        try
        {
            if (string.IsNullOrEmpty(webRequest.error))
            {
                GetAllDefensiveStructureJson(webRequest.text);
            }
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.Log(e);
#endif
        }
    }

    /// <summary>
    /// ????????? ????????? Json ????????? ???????????? ??????
    /// </summary>
    /// <param name="_jsonData"></param>
    private void GetAllDefensiveStructureJson(string _jsonData)
    {
        var parseData = JSON.Parse(_jsonData);
        var arrayData = parseData["results"];

        Dictionary<string, string> structDict = new Dictionary<string, string>();

        if (arrayData.Count > 0)
        {
            for (int i = 0; i < arrayData.Count; i++)
            {
                structDict.Add("Defensive_UID", arrayData[i]["Defensive_UID"].Value);
                structDict.Add("Defensive_Name", arrayData[i]["Defensive_Name"].Value);
                structDict.Add("Defensive_Hp", arrayData[i]["Defensive_Hp"].Value);
                structDict.Add("Defensive_Damage", arrayData[i]["Defensive_Damage"].Value);
                structDict.Add("Defensive_Passable", arrayData[i]["Defensive_Passable"].Value);

                string fileName = arrayData[i]["Defensive_UID"].Value;
                JsonData classJson = JsonMapper.ToJson(structDict);

                File.WriteAllText(streamingAssetPath + jsonPath + defensiveStructurePath + fileName + ".json", classJson.ToString());

                structDict.Clear();
            }
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("???????????? ????????? ????????????.");
#endif
        }
    }

    #endregion

    #region Item
    /// <summary>
    /// DB?????? ?????? ?????? ?????? ????????? ???????????? ?????????
    /// </summary>
    /// <returns></returns>
    IEnumerator GetAllItemCo()
    {
        WWWForm form = new WWWForm();

        WWW webRequest = new WWW(allItemUrl, form);

        yield return webRequest;

        try
        {
            if (string.IsNullOrEmpty(webRequest.error))
            {
                GetAllItemJson(webRequest.text);
            }
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.Log(e);
#endif
        }
    }

    /// <summary>
    /// ????????? ????????? Json ????????? ???????????? ??????
    /// </summary>
    /// <param name="_jsonData"></param>
    private void GetAllItemJson(string _jsonData)
    {
        var parseData = JSON.Parse(_jsonData);
        var arrayData = parseData["results"];

        Dictionary<string, string> itemDict = new Dictionary<string, string>();

        if (arrayData.Count > 0)
        {
            for (int i = 0; i < arrayData.Count; i++)
            {
                itemDict.Add("Item_UID", arrayData[i]["Item_UID"].Value);
                itemDict.Add("Item_Name", arrayData[i]["Item_Name"].Value);
                itemDict.Add("Item_SkillUID", arrayData[i]["Item_SkillUID"].Value);
                itemDict.Add("Item_Coefficient", arrayData[i]["Item_Coefficient"].Value);

                string fileName = arrayData[i]["Item_UID"].Value;
                JsonData classJson = JsonMapper.ToJson(itemDict);

                File.WriteAllText(streamingAssetPath + jsonPath + itemPath + fileName + ".json", classJson.ToString());

                itemDict.Clear();
            }
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("????????? ????????? ????????????.");
#endif
        }
    }

    #endregion

    #region Monster
    /// <summary>
    /// ??????????????? ????????? ????????? ??? ????????? ????????? Dictionary ???????????? ??????
    /// </summary>
    public Dictionary<string, string> GetMonsterInfo(string _monsterUID)
    {
        string jsonString = null;
        try
        {
            jsonString = File.ReadAllText(streamingAssetPath + jsonPath + monsterPath + _monsterUID.ToString() + ".json");
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogWarning(e);
#endif
            return null;
        }

        JsonData monsterInfoData = JsonMapper.ToObject(jsonString);

        Dictionary<string, string> _monsterInfoDict = new Dictionary<string, string>();

        _monsterInfoDict.Add("Monster_UID", monsterInfoData["Monster_UID"].ToString());
        _monsterInfoDict.Add("Monster_Name", monsterInfoData["Monster_Name"].ToString());
        _monsterInfoDict.Add("Monster_Hp", monsterInfoData["Monster_Hp"].ToString());
        _monsterInfoDict.Add("Monster_Armour", monsterInfoData["Monster_Armour"].ToString());
        _monsterInfoDict.Add("Monster_MoveSpeed", monsterInfoData["Monster_MoveSpeed"].ToString());
        _monsterInfoDict.Add("Monster_Damage", monsterInfoData["Monster_Damage"].ToString());
        _monsterInfoDict.Add("Monster_AttackDistance", monsterInfoData["Monster_AttackDistance"].ToString());
        _monsterInfoDict.Add("Monster_AttackSpeed", monsterInfoData["Monster_AttackSpeed"].ToString());
        _monsterInfoDict.Add("Monster_Exp", monsterInfoData["Monster_Exp"].ToString());
        _monsterInfoDict.Add("Monster_SkillUID", monsterInfoData["Monster_SkillUID"].ToString());

        return _monsterInfoDict;
    }

    /// <summary>
    /// DB?????? ?????? ????????? ????????? ???????????? ?????????
    /// </summary>
    /// <returns></returns>
    IEnumerator GetAllMonsterCo()
    {
        WWWForm form = new WWWForm();

        WWW webRequest = new WWW(allMonsterUrl, form);

        yield return webRequest;

        try
        {
            if (string.IsNullOrEmpty(webRequest.error))
            {
                GetAllMonsterJson(webRequest.text);
            }
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.Log(e);
#endif
        }
    }

    /// <summary>
    /// ????????? ????????? Json ????????? ???????????? ??????
    /// </summary>
    /// <param name="_jsonData"></param>
    private void GetAllMonsterJson(string _jsonData)
    {
        var parseData = JSON.Parse(_jsonData);
        var arrayData = parseData["results"];

        Dictionary<string, string> monsterDict = new Dictionary<string, string>();

        if (arrayData.Count > 0)
        {
            for (int i = 0; i < arrayData.Count; i++)
            {
                monsterDict.Add("Monster_UID", arrayData[i]["Monster_UID"].Value);
                monsterDict.Add("Monster_Name", arrayData[i]["Monster_Name"].Value);
                monsterDict.Add("Monster_Hp", arrayData[i]["Monster_Hp"].Value);
                monsterDict.Add("Monster_Armour", arrayData[i]["Monster_Armour"].Value);
                monsterDict.Add("Monster_MoveSpeed", arrayData[i]["Monster_MoveSpeed"].Value);
                monsterDict.Add("Monster_Damage", arrayData[i]["Monster_Damage"].Value);
                monsterDict.Add("Monster_AttackDistance", arrayData[i]["Monster_AttackDistance"].Value);
                monsterDict.Add("Monster_AttackSpeed", arrayData[i]["Monster_AttackSpeed"].Value);
                monsterDict.Add("Monster_Exp", arrayData[i]["Monster_Exp"].Value);
                monsterDict.Add("Monster_SkillUID", arrayData[i]["Monster_SkillUID"].Value);

                string fileName = arrayData[i]["Monster_UID"].Value;
                JsonData classJson = JsonMapper.ToJson(monsterDict);

                File.WriteAllText(streamingAssetPath + jsonPath + monsterPath + fileName + ".json", classJson.ToString());

                monsterDict.Clear();
            }
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("????????? ????????? ????????????.");
#endif
        }
    }

    #endregion

    #region MonsterSkill
    /// <summary>
    /// DB?????? ?????? ????????? ?????? ????????? ???????????? ?????????
    /// </summary>
    /// <returns></returns>
    IEnumerator GetAllMonsterSkillCo()
    {
        WWWForm form = new WWWForm();

        WWW webRequest = new WWW(allMonsterSkillUrl, form);

        yield return webRequest;

        try
        {
            if (string.IsNullOrEmpty(webRequest.error))
            {
                GetAllMonsterSkillJson(webRequest.text);
            }
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.Log(e);
#endif
        }
    }

    /// <summary>
    /// ????????? ????????? Json ????????? ???????????? ??????
    /// </summary>
    /// <param name="_jsonData"></param>
    private void GetAllMonsterSkillJson(string _jsonData)
    {
        var parseData = JSON.Parse(_jsonData);
        var arrayData = parseData["results"];

        Dictionary<string, string> monsterSkillDict = new Dictionary<string, string>();

        if (arrayData.Count > 0)
        {
            for (int i = 0; i < arrayData.Count; i++)
            {
                monsterSkillDict.Add("MonsterSkill_UID", arrayData[i]["MonsterSkill_UID"].Value);
                monsterSkillDict.Add("MonsterSkill_Name", arrayData[i]["MonsterSkill_Name"].Value);
                monsterSkillDict.Add("MonsterSkill_SkillUID", arrayData[i]["MonsterSkill_SkillUID"].Value);
                monsterSkillDict.Add("MonsterSkill_Coefficient", arrayData[i]["MonsterSkill_Coefficient"].Value);

                string fileName = arrayData[i]["MonsterSkill_UID"].Value;
                JsonData classJson = JsonMapper.ToJson(monsterSkillDict);

                File.WriteAllText(streamingAssetPath + jsonPath + monsterSkillPath + fileName + ".json", classJson.ToString());

                monsterSkillDict.Clear();
            }
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("????????? ?????? ???????????? ????????????.");
#endif
        }
    }


    #endregion

    #region StageSpawn
    /// <summary>
    /// DB?????? ?????? ????????? ?????? ????????? ???????????? ?????????
    /// </summary>
    /// <returns></returns>
    IEnumerator GetAllStageSpawnCo()
    {
        WWWForm form = new WWWForm();

        WWW webRequest = new WWW(allStageSpawnUrl, form);

        yield return webRequest;

        try
        {
            if (string.IsNullOrEmpty(webRequest.error))
            {
                GetAllStageSpawnJson(webRequest.text);
            }
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.Log(e);
#endif
        }
    }

    /// <summary>
    /// ????????? ????????? Json ????????? ???????????? ??????
    /// </summary>
    /// <param name="_jsonData"></param>
    private void GetAllStageSpawnJson(string _jsonData)
    {
        var parseData = JSON.Parse(_jsonData);
        var arrayData = parseData["results"];

        Dictionary<string, string> spawnDict = new Dictionary<string, string>();

        if (arrayData.Count > 0)
        {
            for (int i = 0; i < arrayData.Count; i++)
            {
                spawnDict.Add("Stage_Count", arrayData[i]["Stage_Count"].Value);
                spawnDict.Add("Stage_MaxSpawn", arrayData[i]["Stage_MaxSpawn"].Value);

                string fileName = arrayData[i]["Stage_Count"].Value;
                JsonData classJson = JsonMapper.ToJson(spawnDict);

                File.WriteAllText(streamingAssetPath + jsonPath + stageSpawnPath + fileName + ".json", classJson.ToString());

                spawnDict.Clear();
            }
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("???????????? ????????? ????????????.");
#endif
        }
    }

    #endregion

}
