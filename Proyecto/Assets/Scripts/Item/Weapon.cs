using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="new Weapon", menuName = "Items/Weapon")]
public class Weapon : Item
{
    public GameObject prefab;
    public GameObject muzzleFlashParticles;
    public int damage;
    public int magazineSize;
    public int storedAmmo;
    public float reloadTime;
    public float fireRate;
    public float range;
    public WeaponType weaponType;
    //public WeaponStyle weaponStyle;
    public int weaponPrice;
    public int ammoPrice;
    public AudioClip shotSound;
    public AudioClip reloadSound;
    public AudioClip wepAppearingSound;
    public AudioClip wepSaveSound;
}

public enum WeaponType { Melee, Pistol, AR, Shotgun, Sniper}

public enum WeaponStyle { Primary, Secondary, Tertiary}