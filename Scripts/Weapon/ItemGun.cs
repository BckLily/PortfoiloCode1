using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Asset Menu 생성 / 파일 이름 / 메뉴 이름 / 메뉴에서 위치
[CreateAssetMenu(fileName ="New Gun", menuName ="New Gun/Gun", order =int.MaxValue)]
public class ItemGun : ScriptableObject
{
    public enum GunType
    {
        Rifle= 0, SMG=1, SG =2,
    }
    public enum GunRarity
    {
        Common =0, Rare=1, Epic=2,
    }
    
    public GunType gunType { get; private set; } 
    public GunRarity gunRarity { get; private set; }

    public int reloadBullet { get; private set; } 
    public int maxBullet { get; private set; } 

    public float damage { get; private set; } 
    public float fireDelay { get; private set; }
    public float reloadTime { get; private set; }

    public string gunName { get; private set; } 

}
