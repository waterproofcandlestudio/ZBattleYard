using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    [SerializeField] protected int health;
    [SerializeField] protected int maxHealth;
    [SerializeField] protected float stamina;
    [SerializeField] protected float maxStamina;
    [SerializeField] protected bool isDead;
    protected bool initHealthRegenValue;


    void Start() => InitVariables();


    #region - Logic -

    public virtual void CheckHealth()
    {
        if (health <= 0)
        {
            health = 0;
            Die();
        }
        if (health >= maxHealth)
        {
            health = maxHealth;
        }
    }
    public virtual void Die()
    {
        EnemySpawner.enemyDied_RoundCheck = true;
        isDead = true;
    }
    public bool IsDead() => isDead;

    public void SetHealthTo(int healthToSetTo)
    {
        health = healthToSetTo;
        CheckHealth();
    }
    public virtual void TakeDamage(int damage)
    {
        int healthAfterDamage = health - damage;
        SetHealthTo(healthAfterDamage);
        if (transform.tag == "Player")  
            PlayerStats.healthResetCooldown = true;
        initHealthRegenValue = true;
    }
    public void InstantKill() => Die();
    public void Heal(int heal)
    {
        health += heal * (int)Time.deltaTime;
        SetHealthTo(health);
    }

    public int GetMaxHealth() => maxHealth;

    /// Stamina
    public virtual void CheckStamina()
    {
        if (stamina <= 0)
        {
            stamina = 0;
            return;
        }
        if (stamina >= maxStamina)
        {
            stamina = maxStamina;
        }
    }
    public virtual void UseStamina(float amount) => stamina -= amount * Time.deltaTime;

    public virtual void InitVariables()
    {
        ResetStats();
        isDead = false;
    }

    protected void ResetStats()
    {
        SetHealthTo(maxHealth);        
        stamina = maxStamina;
    }

    #endregion
}
