using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;

public class ZombieStats : CharacterStats
{
    Animator animator;
    ZombieController zombieController;
    NavMeshAgent agent = null;

    [SerializeField] int damage = 40;
    public float attackSpeed = 0.65f;

    [SerializeField] float destroyTimer_AfterDie = 10f;
    [SerializeField] int baseHealth = 100;
    [SerializeField] [Range(0.5f, 2f)] float healthMultiplierPerRound = 1f;

    [Header("Dropeables")]
    [SerializeField] PowerUp[] powerUps;
    [SerializeField] [Range(0.0f, 100.0f)] int dropPowerUpProbability = 2;

    int enemyListId;

    void Awake() => GetReferences();
    void Start() => InitVariables();

    public void DealDamage(CharacterStats statsToDamage) => statsToDamage.TakeDamage(damage);   // Damaging functionality

    public override void Die()
    {
        base.Die();        
        PlayerHUD.instance.UpdateScoreKillAmount();
        DropPowerUp();
        EnemySpawner.enemyDied_RoundCheck = true;
        
        animator.enabled = false;
        zombieController.enabled = false;
        agent.isStopped = true;
        Destroy(gameObject, destroyTimer_AfterDie);
    }
    public void NukeDeath()
    {
        base.Die();
        PlayerHUD.instance.UpdateScoreKillAmount();
        DropPowerUp();
        EnemySpawner.enemyDied_RoundCheck = true;

        animator.enabled = false;
        zombieController.enabled = false;
        agent.isStopped = true;
        Destroy(gameObject, destroyTimer_AfterDie);
    }

    void DropPowerUp()
    {
        int luckyNumber = Random.Range(0, 100);

        if(luckyNumber <= dropPowerUpProbability)
        {
            Vector3 position = transform.position;
            int randomPowerUp = Random.Range(0, powerUps.Length);
            GameObject powerUpObject = Instantiate(powerUps[randomPowerUp].prefab, position + new Vector3(0f, 1f, 0f), Quaternion.identity);
            powerUpObject.SetActive(true);
        }
    }

    void GetReferences()
    {
        animator = GetComponent<Animator>();
        zombieController = GetComponent<ZombieController>();
        agent = GetComponent<NavMeshAgent>();
    }
    public override void InitVariables()
    {
        maxHealth = Mathf.RoundToInt(baseHealth * (EnemySpawner.currentWave + 1) * healthMultiplierPerRound);
        SetHealthTo(maxHealth);
        isDead = false;

        //damage = 10;
        //attackSpeed = 1.5f;
    }
}
