using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerkManager : MonoBehaviour
{
    WeaponShooting shooting;
    [SerializeField] PerkUIManager perkUIManager;
    PlayerPickup playerPickup;
    Inventory inventory;
    StaminaProgressBar staminaProgressBar;
    PlayerStats stats;
    CollisionDetection collisionDetection;  // Melee gun collision detection with objects & enemies

    [SerializeField] int perkInventoryLength = 10;
    [SerializeField] public Perk[] perkList;

    [Header("Power Ups")]
    [SerializeField] public static bool speedMasterActive;
    [SerializeField] public static bool holsterMasterActive;
    [SerializeField] public static bool armoredFortActive;
    [SerializeField] public static bool bulletLegionActive;
    [SerializeField] public static bool flyingHandActive;

    [SerializeField] public static bool headBlowerActive;
    [SerializeField] public static bool wildcardActive;
    [SerializeField] public static bool devilPactActive;
    [SerializeField] public static bool strikeActive;

    [Space]
    [Header("Perk Multipliers")]
    [Range(0.5f, 1.0f)][SerializeField] float shotLegionMultiplier = 0.8f;
    [Range(0.2f, 1.0f)][SerializeField] float reloadMultiplier = 0.5f;
    public bool activateSpeedMaster = false;
    [Range(1.1f, 2.0f)][SerializeField] float staminaMultiplier = 1.5f;
    [Range(1.01f, 2.0f)][SerializeField] float speedMultiplier = 1.1f;
    [Range(1, 6)][SerializeField] int strikeMultiplier = 2;
    public bool activateArmoredFort = false;
    [Range(2, 3)][SerializeField] int healthMultiplier = 2;
    [Range(1, 6)][SerializeField] int headBlowerMultiplier = 2;

    void Awake()    => GetReferences();
    void Start()    => InitVariables();

    #region - Logic -

    public void ActivatePowerUpGameChanger(Perk perk)
    {
        PerkType type = perk.perkType;
        perkList[CheckFirstEmptyArraySpace()] = perk;   // Insert perk in player's perks array list
        perkUIManager.AddPowerUpImageFeedback(perk);   // UI Feedback

        if (type == PerkType.speedMaster)
            ActivateSpeedMaster();
        if (type == PerkType.holsterMaster)
            ActivateHolsterMaster();
        if (type == PerkType.armoredFort)
            ActivateArmoredFort();
        if (type == PerkType.bulletLegion)
            ActivateBulletLegion();
        if (type == PerkType.flyingHand)
            ActivateFlyingHand();
        if (type == PerkType.headBlower)
            ActivateHeadBlower();
        if (type == PerkType.wildcard)
            ActivateWildcard();
        if (type == PerkType.devilPact)
            ActivateDevilPact();
        if (type == PerkType.goldenStrike)
            ActivateGoldenStrike();
    }

    int CheckFirstEmptyArraySpace()
    {
        for (int i = 0; i < perkList.Length; i++)
            if (perkList[i] == null)
                return i;
        return 0;
    }


    #region - PowerUp Activation Methods -

    void ActivateSpeedMaster()
    {
        speedMasterActive = true;
        activateSpeedMaster = true;
    }
    void ActivateHolsterMaster() 
    {
        holsterMasterActive = true;
        inventory.ResizeInventorySpace(3);
    }
    void ActivateArmoredFort() 
    {
        armoredFortActive = true;
        activateArmoredFort = true;
    }
    void ActivateBulletLegion() 
    {
        bulletLegionActive = true;
    }
    void ActivateFlyingHand()  
    {
        flyingHandActive = true;
    }
    void ActivateHeadBlower()   
    {
        headBlowerActive = true;
    }
    void ActivateWildcard()  
    {
        wildcardActive = true;
    }
    void ActivateDevilPact()   
    {
        devilPactActive = true;
    }
    void ActivateGoldenStrike()  
    {
        strikeActive = true;
    }


    #endregion

    void ResetPerks()
    {
        speedMasterActive = false;
        holsterMasterActive = false;
        armoredFortActive = false;
        bulletLegionActive = false;
        flyingHandActive = false;
        headBlowerActive = false;
        wildcardActive = false;
        devilPactActive = false;
        strikeActive = false;
    }

    public float GetShotLegionMultiplier()  =>  shotLegionMultiplier;
    public float GetReloadMultiplier()      =>  reloadMultiplier;
    public float GetStaminaMultiplier()     =>  staminaMultiplier;
    public float GetSpeedMultiplier()       =>  speedMultiplier;
    public int GetGoldenStrikeMultiplier()  =>  strikeMultiplier;
    public int GetHealthMultiplier()        =>  healthMultiplier;
    public int GetHeadBlowerMultiplier()    =>  headBlowerMultiplier;

    #endregion

    #region - Reference & Init -

    void GetReferences()
    {
        shooting = GetComponent<WeaponShooting>();
        playerPickup = GetComponent<PlayerPickup>();
        inventory = GetComponent<Inventory>();
        staminaProgressBar = GetComponentInChildren<StaminaProgressBar>();
        stats = GetComponent<PlayerStats>();
        collisionDetection = GetComponentInChildren<CollisionDetection>();
    }

    void InitVariables()
    {
        ResetPerks();
        Array.Resize(ref perkList, perkInventoryLength);   // Resize perk list
    }

    #endregion
}
