using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    public static PlayerHUD instance = null;

    [SerializeField] HealthProgressBar healthProgressBar; /// Health
    [SerializeField] StaminaProgressBar staminaProgressBar; /// Health
    [SerializeField] Score scoreUI;
    [SerializeField] WeaponUI weaponUI; /// Weapon


    [SerializeField] CanvasGroup damagePanelGroup;

    void Awake()
    {
        if (instance == null) instance = this;
    }


    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        healthProgressBar.SetValues(currentHealth, maxHealth);
        float healthAlpha = 1 - ((float)currentHealth / (float)maxHealth);
        damagePanelGroup.alpha = healthAlpha;
    }
    public void UpdateNewWeaponUI(string gunName, Sprite weaponIcon, int currentAmmo, int storedAmmo)   =>  weaponUI.UpdateInfo(gunName , weaponIcon, currentAmmo, storedAmmo);       // Update weapon: Icon, magazineSize, storedAmmo
    public void UpdateWeaponUI(Weapon newWeapon) => weaponUI.UpdateInfo(newWeapon.name ,newWeapon.icon, newWeapon.magazineSize, newWeapon.storedAmmo);      // Update weapon: Icon, magazineSize, storedAmmo
    public void UpdateWeaponAmmoUI(int currentAmmo, int storedAmmo) =>  weaponUI.UpdateAmmoUI(currentAmmo, storedAmmo);    // Update weapon ammo only (For reload and shooting without updating icon all the time!)

    public void UpdateScoreKillAmount() => scoreUI.AddKillToScore();
    public void UpdateScoreNukeAmount() => scoreUI.AddNukeToScore();
    public void UpdateScoreHitAmount()  => scoreUI.AddHitToScore();
    public void UpdateScoreHeadHitAmount() => scoreUI.AddHeadHitToScore();

    public void UpdateStamina(float currentStamina, float maxStamina) => staminaProgressBar.SetValues(currentStamina, maxStamina);
}
