using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.SceneManagement;

public class PlayerStats : CharacterStats
{
    GameObject playerCanvas;
    GameObject audioManager;

    UIManager ui;
    PlayerHUD hud;
    StatsManager statsManager;
    PerkManager perkManager;
    MovementController movementController;
    SFXManager sFXManager;

    [Header("Sound Feedback")]
    [SerializeField] AudioLowPassFilter lowPassFilter_GeneralVolume;
    AudioLowPassFilter lowPassFilter_Music;
    float defaultCutoffFrequency_LowPassFilter;
    [SerializeField] AudioSource heartBeat_Sfx;
    float heartBeat_DefaultVolume = 1;

    [SerializeField] AudioSource hurt_AudioSource;
    [SerializeField] AudioClip playerHurt_AudioClip;

    [SerializeField] AudioClip breath_AudioClip;
    [SerializeField] AudioSource staminaBreath_Sfx;
    float staminaBreath_DefaultVolume = 1;

    public static bool healthRegenerated = true;
    public static bool staminaRegenerated = true;

    [Header("Health")]
    [SerializeField] float healthRegenStartTime = 5f;
    float healthCountdown = 0f;
    public static bool healthResetCooldown;
    [SerializeField] int healthRegenPerSecond = 5;
    float decimalHealth;

    [Header("Stamina")]
    [SerializeField] float staminaRegenStartTime = 2f;
    float staminaCountdown = 0f;
    public static bool staminaResetCooldown;
    [SerializeField] float staminaRegenPerSecond = 20f;

    void Awake() => GetReferences();
    void Start() => InitVariables();

    void Update()
    {
        UpdateHealth();
        UpdateStamina();
    }

    #region - Logic -

    #region Health

    void UpdateHealth()
    {
        if (perkManager.activateArmoredFort)    // Armored Fort (Health) Perk
        {
            health *= perkManager.GetHealthMultiplier();
            maxHealth *= perkManager.GetHealthMultiplier();

            perkManager.activateArmoredFort = false;
        }

        if (healthResetCooldown)
        {
            healthCountdown = healthRegenStartTime;
            healthResetCooldown = false;
        }

        if (health < maxHealth)
            healthRegenerated = false;

        if (!healthRegenerated)
            RegenHealth();

        Hurt_AudioEffect();
    }
    void Hurt_AudioEffect()
    {
        // LowPassFilter - General volume
        float healthPercentage = (float)health / (float)maxHealth;
        lowPassFilter_GeneralVolume.cutoffFrequency = defaultCutoffFrequency_LowPassFilter * healthPercentage; 

        // Heart beat - sfx
        float heartBeatVolume = heartBeat_DefaultVolume - (healthPercentage);
        heartBeat_Sfx.volume = heartBeatVolume;
    }
    public override void CheckHealth()  /// Health
    {
        base.CheckHealth();
        hud.UpdateHealth(health, maxHealth);
    }

    public override void Die()
    {
        base.Die();
        statsManager.UpdateEndgameStats();
        ui.SetActiveHud(false);
    }
    void RegenHealth()      // Player health regen
    {
        healthCountdown -= Time.deltaTime;

        if (healthCountdown <= 0)
        {
            if (initHealthRegenValue)
            {
                decimalHealth = health;
                initHealthRegenValue = false;
            }

            decimalHealth += healthRegenPerSecond * Time.deltaTime;
            health = Mathf.RoundToInt(decimalHealth);

            if (health >= maxHealth)
            {
                health = maxHealth;
                staminaRegenerated = true;
            }
            hud.UpdateHealth(health, maxHealth);
        }
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        hurt_AudioSource.PlayOneShot(playerHurt_AudioClip);
    }
    #endregion

    #region Stamina
    void UpdateStamina()
    {
        if (perkManager.activateSpeedMaster)    // Speed Master (stamina) Perk
        {
            stamina *= perkManager.GetStaminaMultiplier();
            maxStamina *= perkManager.GetStaminaMultiplier();

            perkManager.activateSpeedMaster = false;
        }

        if(stamina <= 0)    // Stop sprinting if there's no stamina
            MovementController.isSprinting = false;

        if (movementController.IsSprinting())  
        {
            staminaResetCooldown = true;
            UseStamina(10);
            CheckStamina();
        }
        if (staminaResetCooldown)
        {
            staminaCountdown = staminaRegenStartTime;
            staminaResetCooldown = false;
        }

        if (stamina < maxStamina)
            staminaRegenerated = false;

        if (!staminaRegenerated)
            RegenStamina();

        Stamina_AudioEffect();
    }
    void Stamina_AudioEffect()
    {
        // Stamina breath - sfx
        float staminaPercentage = (float)stamina / (float)maxStamina;
        float staminaBreathVolume = staminaBreath_DefaultVolume - (staminaPercentage);
        staminaBreath_Sfx.volume = staminaBreathVolume;
    }
    public override void CheckStamina()
    {
        /// Stamina
        base.CheckStamina();
        hud.UpdateStamina(stamina, maxStamina);
    }

    void RegenStamina()      // Player stamina regen
    {
        staminaCountdown -= Time.deltaTime;

        if (staminaCountdown <= 0)
        {
            stamina += staminaRegenPerSecond * Time.deltaTime;
            if (stamina >= maxStamina)
            {
                stamina = maxStamina;
                staminaRegenerated = true;
            }
            hud.UpdateStamina(stamina, maxStamina);
        }
    }
    public float GetStamina()   =>  stamina;
    public float GetMaxStamina()  =>  maxStamina;

    #endregion

    #endregion

    void GetReferences()
    {
        hud = GetComponent<PlayerHUD>();
        ui = GetComponent<UIManager>();
        statsManager = GetComponentInChildren<StatsManager>();
        perkManager = GetComponent<PerkManager>();
        movementController = GetComponentInParent<MovementController>();
        sFXManager = GetComponentInChildren<SFXManager>();
    }
    public override void InitVariables()
    {
        defaultCutoffFrequency_LowPassFilter = lowPassFilter_GeneralVolume.cutoffFrequency;
        isDead = false;
        ResetStats();
    }
}
