using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Guns : Weapon
{
    GameObject fireMuzzle;
    string muzzlePath;

    public Transform firePos;
    public Transform handleTr;
    private Transform playerTr;

    public ItemGun.GunRarity gunRarity { get; private set; }
    public ItemGun.GunType gunType { get; private set; }

    IEnumerator muzzleActive = null;

    protected override void Awake()
    {
        base.Awake();

    }

    private void OnEnable()
    {
        muzzlePath = "FireMuzzle/Prefabs/";
        fireMuzzle = Resources.Load<GameObject>(muzzlePath + "MuzzleFlash");
        playerTr = transform.parent.parent.GetComponent<Transform>();

        fireMuzzle = Instantiate(fireMuzzle, firePos);
        fireMuzzle.SetActive(false);
    }

    private void Start()
    {
        StartCoroutine(CoWeaponTypeSetting());
    }

    private IEnumerator CoWeaponTypeSetting()
    {
        while (weaponDict == null) { yield return null; }
        yield return new WaitForSeconds(1f);
        string __type = UID.Substring(2, 3);

        if (__type == "000")
        {
            gunType = ItemGun.GunType.Rifle;
        }
        else if (__type == "001")
        {
            gunType = ItemGun.GunType.SMG;
        }
        else if (__type == "002")
        {
            gunType = ItemGun.GunType.SG;
        }
    }

    /// <summary>
    /// 총이 발사되면 실행되는 함수
    /// </summary>
    public void BulletFire()
    {
        if (muzzleActive == null)
        {
            muzzleActive = MuzzleActive();
            StartCoroutine(muzzleActive);
        }
    }

    IEnumerator MuzzleActive()
    {
        fireMuzzle.SetActive(true);
        yield return new WaitForSeconds(0.04f);
        fireMuzzle.SetActive(false);
        muzzleActive = null;
    }
}
