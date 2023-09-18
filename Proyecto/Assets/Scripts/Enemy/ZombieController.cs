using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class ZombieController : MonoBehaviour
{
    NavMeshAgent agent = null;
    Animator anim = null;
    ZombieStats stats = null;
    Transform target;
    ZombieSoundManager zombieSoundManager = null;

    [SerializeField] float rotationSmooth = 4;
    //[SerializeField] float stoppingDistance = 3;
    [SerializeField] float baseSpeed;
    [SerializeField] float speedMultiplierPerRound = 1f;
    [SerializeField] float maxSpeed = 5f;
    [SerializeField] float speedVisualizer;

    bool canAttack;
    float timeBetweenAttacks = 1;
    float attackTimer;

    //[SerializeField] float minSpeed;
    //[SerializeField] float maxSpeed;

    void Awake() => GetReferences();
    void Start() => Initialize();
    void Update()
    {
        CanAttackTimer();
        MoveToTarget();
    }

    #region - Logic -

    //void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag == "ZSpawnWall")
    //    {
    //        //Physics.IgnoreCollision(transform , collision);
    //        //Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), collision.collider);
    //    }
    //}

    void CanAttackTimer()
    {
        if (attackTimer >= timeBetweenAttacks)
        {
            canAttack = true;
        }

        else
        {
            canAttack = false;
            attackTimer += Time.deltaTime;
        }
    }
    void MoveToTarget()
    {
        agent.SetDestination(target.position);
        anim.SetFloat("Speed", 1f, 0.3f, Time.deltaTime);
        RotateToTarget();

        float distanceToTarget = Vector3.Distance(target.position, transform.position);
        if(distanceToTarget <= agent.stoppingDistance + 0.2f) // Stop zombie when near player ==> Attack
        {
            anim.SetFloat("Speed", 0f);

            if (canAttack)
            {
                CharacterStats targetStats = target.GetComponent<CharacterStats>();
                AttackTarget(targetStats);
            }
        }
    }

    void RotateToTarget() // Hacer q coja todos los ejes menos Y
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0; // Enemy doesn't rotate on y axis looking to player!
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSmooth);
    }

    void AttackTarget(CharacterStats statsToDamage)
    {
        anim.SetTrigger("attack");
        stats.DealDamage(statsToDamage);
        attackTimer = 0;
        zombieSoundManager.PlayAttackEffect();
    }

    #endregion

    void GetReferences()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        stats = GetComponent<ZombieStats>();
        zombieSoundManager = GetComponent<ZombieSoundManager>();
    }
    void Initialize()
    {
        target = PlayerDiana.instance;

        float entitySpeed = Mathf.RoundToInt(baseSpeed * (EnemySpawner.currentWave + 1) * speedMultiplierPerRound);
        if (entitySpeed > maxSpeed)
            agent.speed = maxSpeed;
        else 
            agent.speed = entitySpeed;

        speedVisualizer = agent.speed;
        attackTimer = 0;
    }
}
