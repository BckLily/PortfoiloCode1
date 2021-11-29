using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Weapon : MonoBehaviour
{
    protected string UID = null; 
    protected string weaponName = null; 

    public int currBullet = 0; 
    public int reloadBullet = 0; 
    public int carryBullet = 0; 
    public int maxCarryBullet = 0;

    public float damage = 0f; 
    public float reloadTime = 0f; 
    public float attackDistance = 0f; 
    protected float attackRange = 0f; 

    public float fireDelay = 0f; 
    public float fireTime = 0f;

    // Weapon_UID / Weapon_Name / Weapon_Damage / Weapon_AttackSpeed / WeaponAttackDistance / Weapon_ReloadBullet
    // Weapon_CarryBullet / Weapon_ReloadTime / Weapon_AttackRange
    public Dictionary<string, string> weaponDict = null;

    protected virtual void Awake()
    {
        StartCoroutine(WeaponSetting());
    }

    /// <summary>
    /// 초기 무기 상태 값을 설정하는 코루틴<br/>
    /// </summary>
    /// <returns></returns>
    IEnumerator WeaponSetting()
    {
        while (weaponDict == null) { yield return null; }

        UID = weaponDict["Weapon_UID"];
        name = weaponDict["Weapon_Name"];

        reloadBullet = int.Parse(weaponDict["Weapon_ReloadBullet"]);
        maxCarryBullet = int.Parse(weaponDict["Weapon_CarryBullet"]);
        currBullet = reloadBullet;
        carryBullet = maxCarryBullet - currBullet;

        damage = float.Parse(weaponDict["Weapon_Damage"]); 
        reloadTime = float.Parse(weaponDict["Weapon_ReloadTime"]);
        attackDistance = float.Parse(weaponDict["Weapon_AttackDistance"]);
        attackRange = float.Parse(weaponDict["Weapon_AttackRange"]);

        fireDelay = 60 / float.Parse(weaponDict["Weapon_AttackSpeed"]);

        yield return null;
    }

}
