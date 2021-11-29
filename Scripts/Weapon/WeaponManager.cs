using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class WeaponManager : MonoBehaviour
{
    public PlayerCtrl playerCtrl;
    public Transform playerCameraTr;
    public Transform leftHandTr; 
    private Transform weaponTr; 

    public CameraRaycast cameraRaycast;

    private GameObject currWeaponObj; 
    public Guns currGun; 

    private bool isReload; 

    private Dictionary<string, string> weaponDict = null;
    private string weaponPath;

    public Animator anim;

    bool isFire = false; 
        
    public float dontUseBulletPercent = 0f;
    public bool dontUseBullet = false;

    [Space(5)]
    [Header("Weapon Info")]
    [Space(2)]
    public Text weaponBulletText; 
    public Text weaponNameText; 

    #region LayerMask
    LayerMask alllTargetLayerMask;

    #endregion

    private void Awake()
    {
        weaponPath = "Weapons/";
    }

    void Start()
    {
        weaponTr = GetComponent<Transform>(); 

        isReload = false; 

        LayerMask enemyLayer = LayerMask.NameToLayer("ENEMY");
        LayerMask wallLayer = LayerMask.NameToLayer("WALL");
        alllTargetLayerMask = ((1 << enemyLayer) | (1 << wallLayer));

    }

    void Update()
    {
        TryReload(); 
        TryFire(); 

        try
        {
            weaponTr.LookAt(leftHandTr);
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.Log(e);
#endif
        }
    }

    #region 무기 변경
    /// <summary>
    /// 무기 변경에 사용하는 함수
    /// </summary>
    /// <param name="UIDCode">무기 UID</param>
    public void WeaponChange(string UIDCode)
    {
        Dictionary<string, string> _weaponDict = null;
        while (_weaponDict == null)
        {
            _weaponDict = DBManager.Instance.GetWeaponInfo(UIDCode);
        }
        weaponDict = _weaponDict;

        if (currWeaponObj != null)
        {
            Destroy(currWeaponObj.gameObject);
        }

        currWeaponObj = (GameObject)Instantiate(Resources.Load(weaponPath + weaponDict["Weapon_Name"]), this.transform);
        currGun = currWeaponObj.GetComponent<Guns>();
        currWeaponObj.transform.rotation = weaponTr.rotation;
        currWeaponObj.transform.Translate(-currGun.handleTr.localPosition);

        currGun.weaponDict = this.weaponDict;

        StartCoroutine(WeaponStatusSetting());
        WeaponNameChange();
        WeaponBulletChange();
    }

    public IEnumerator WeaponStatusSetting()
    {
        yield return new WaitForSeconds(0.15f);
        currGun.maxCarryBullet = Mathf.RoundToInt(float.Parse(weaponDict["Weapon_CarryBullet"]) * (1 + (playerCtrl.incCarryBullet * 0.01f)));
        currGun.carryBullet = currGun.maxCarryBullet - currGun.reloadBullet;
        currGun.fireDelay = ((60 / float.Parse(weaponDict["Weapon_AttackSpeed"])) * (1 - (playerCtrl.currIncAttackSpeed * 0.01f)));
    }

    #endregion

    #region 재장전
    /// <summary>
    /// 재장전 시도 함수
    /// </summary>
    private void TryReload()
    {
        if (isReload == true)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }
    }

    IEnumerator Reload()
    {
        if (currGun.currBullet < currGun.reloadBullet)
        {
            if (currGun.carryBullet > 0)
            {
                playerCtrl.ActionTextSetting("재장전 중");
                isFire = false;
                anim.SetBool("IsFire", isFire);
                isReload = true;
                yield return new WaitForSeconds(currGun.reloadTime);

                int _reloadBullet = (currGun.reloadBullet - currGun.currBullet);
                if (currGun.carryBullet >= _reloadBullet)
                {
                    currGun.carryBullet -= _reloadBullet;
                    currGun.currBullet += _reloadBullet; ;
                }
                else
                {
                    currGun.currBullet += currGun.carryBullet;
                    currGun.carryBullet = 0;
                }
            }
        }

        isReload = false;
    }

    #endregion

    #region 발사
    /// <summary>
    /// 발사를 시도하는 함수
    /// </summary>
    private void TryFire()
    {
        WeaponBulletChange();

        if (playerCtrl.isUIOpen == true || playerCtrl.doAction == true)
        {
            return;
        }

        if (isReload == false)
        {
            try
            {
                currGun.fireTime += Time.deltaTime;
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                Debug.LogWarning(e);
#endif
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (CheckCanFire())
                {
                    currGun.fireTime = 0f;
                }
            }
            else if (Input.GetMouseButton(0))
            {
                if (CheckCanFire())
                {
                    currGun.fireTime = 0f;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isFire = false;
                anim.SetBool("IsFire", isFire);
            }
        }
    }

    /// <summary>
    /// 발사를 시도했을 때 발사할 수 있는 상황인지 판단하는 함수
    /// </summary>
    /// <returns>true: 가능 false: 불가능</returns>
    private bool CheckCanFire()
    {
        bool canFire = false;

        if (currGun.currBullet > 0)
        {
            if (currGun.fireDelay <= currGun.fireTime)
            {
                if (Cursor.lockState == CursorLockMode.None && CheckRaycastUI() == true)
                {
                    return false;
                }

                isFire = true; 
                anim.SetBool("IsFire", isFire);
                canFire = true;
                int percent = 0;
                if (dontUseBullet)
                {
                    percent = UnityEngine.Random.Range(0, 100);
                }
                if (!(percent >= (100 - dontUseBulletPercent)))
                {
                    currGun.currBullet -= 1;
                }
                else
                {
#if UNITY_EDITOR
                    Debug.Log("___Bullet Dont Use!!____");
#endif
                }

                currGun.BulletFire();

                CheckFireRaycast();
            }
            else
            {
                isFire = false;
                anim.SetBool("IsFire", isFire);
            }
        }
        else
        {
            isFire = false; 
            anim.SetBool("IsFire", isFire);
            isReload = true;
            StartCoroutine(Reload());
        }

        return canFire;
    }


    private void CheckFireRaycast()
    {
        List<RaycastHit> hitTargets = cameraRaycast.GetWeaponRaycastTarget(currGun.attackDistance, alllTargetLayerMask, currGun.gunType);
        GameObject target;

        foreach (RaycastHit hitTarget in hitTargets)
        {
            try
            {
                target = hitTarget.transform.gameObject;

            }
            catch (System.Exception e)
            {
#if UNITY_EDITOR
                Debug.LogWarning("____ Target is Null: " + e);
#endif
                return;
            }

            if (target.CompareTag("ENEMY"))
            {
                playerCtrl._playerExp += target.GetComponent<LivingEntity>().Damaged(currGun.damage + playerCtrl.currAddAttack, hitTarget.point, hitTarget.normal);
                playerCtrl.CheckLevelUp();

                BloodEffectCtrl _effect = PlayerEffectCtrl.GetBloodEffect();
                _effect.transform.position = hitTarget.point;
                _effect.transform.rotation = Quaternion.LookRotation(hitTarget.normal);
            }
            else if (target.CompareTag("WALL"))
            {
                SparkEffectCtrl _effect = PlayerEffectCtrl.GetSparkEffect();
                _effect.gameObject.transform.position = hitTarget.point;
                _effect.gameObject.transform.rotation = Quaternion.LookRotation(hitTarget.normal);
            }
        }

        return;
    }

    /// <summary>
    /// Ray의 Target이 UI인지 확인하는 함수
    /// </summary>
    /// <returns></returns>
    private bool CheckRaycastUI()
    {
        bool lookUI = false;

        RaycastHit hit;

        Vector3 mousePos = Input.mousePosition;
        Camera camera = Camera.main;
        mousePos.z = camera.farClipPlane;
        Vector3 dir = camera.ScreenToWorldPoint(mousePos);
        if (Physics.Raycast(transform.position, dir, out hit, (1 << LayerMask.NameToLayer("UI"))))
        {
            lookUI = true;

        }

        return lookUI;
    }

    #endregion

    #region UI Setting

    /// <summary>
    /// 무기의 이름을 설정하여 UI에 표시하는 함수
    /// </summary>
    private void WeaponNameChange()
    {
        weaponNameText.text = weaponDict["Weapon_Name"];
    }

    /// <summary>
    /// 무기의 총알 수를 UI에 표시하는 함수
    /// </summary>
    private void WeaponBulletChange()
    {
        try
        {
            weaponBulletText.text = string.Format($"<b>{currGun.currBullet}</b> / <b>{currGun.carryBullet}</b>");
        }
        catch (System.Exception e)
        {
#if UNITY_EDITOR
            Debug.LogWarning("____ Weapon Change Exception: " + e + " ____");
#endif
        }
    }

    #endregion
}
