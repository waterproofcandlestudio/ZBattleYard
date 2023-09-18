using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Perk", menuName = "Perks/Perk")]
public class Perk : ScriptableObject
{
    public new string name;
    public string description;
    public PerkType perkType;
    public int price;
    public Sprite icon;
    public AudioClip pickupSound;
    public AudioClip machineSound;
}
public enum PerkType
{
    speedMaster,
    holsterMaster,
    armoredFort,
    bulletLegion,
    flyingHand,
    headBlower,
    wildcard,
    devilPact,
    goldenStrike
}
