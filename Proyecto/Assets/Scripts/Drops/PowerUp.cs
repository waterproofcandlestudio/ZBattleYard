using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Weapon", menuName = "Drops/PowerUp")]
public class PowerUp : ScriptableObject
{
    public GameObject prefab;
    public PowerType powerType;
    public int duration;
    public Sprite icon;
    public AudioClip pickupSound;
}
public enum PowerType 
{ 
    instaKill, 
    maxAmmo, 
    slowTime, 
    doublePoints, 
    nuke, 
    magicBoxDiscount,
    randomPerk
}
