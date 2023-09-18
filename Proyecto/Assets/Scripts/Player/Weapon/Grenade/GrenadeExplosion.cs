using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class GrenadeExplosion : MonoBehaviour
{
    WeaponDecals weaponDecals;

    [Header("Grenade")]
    public float grenadeDelay = 3f;
    private float grenadeCountdown;
    bool hasExploded = false;
    public GameObject explosionEffect;
    public float explosionRadius = 5f;
    public float explosionForce = 700f;
    public int grenadeCount;
    public int damage = 50;
    [SerializeField] LayerMask shootingAvoidLayers; // Layermask to avoid hitting player when shooting

    [Header("Sound")]
    [SerializeField] AudioClip explosionSound;
    [SerializeField][Range(1, 4)] float volumeMultiplier = 1f;


    void Awake() => Initialize();
    void Update() => CalculateGrenade();

    void CalculateGrenade()
    {
        grenadeCountdown -= Time.deltaTime;

        if (grenadeCountdown <= 0f && !hasExploded) // (!hasExploded) == (hasExploded == false)
        {
            Explode();
            hasExploded = true;
        }
    }
    void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);

        /// For destructible objects
        Collider[] collidersToDestroy = Physics.OverlapSphere(transform.position, explosionRadius); // Get all the nearby objects

        foreach (Collider nearbyObject in collidersToDestroy) // Loop through each of the objects
        {
            Destructible dest = nearbyObject.GetComponent<Destructible>();
            if(dest != null)
            {
                dest.Destroy();
            }
        }
        ///
        Collider[] collidersToMove = Physics.OverlapSphere(transform.position, explosionRadius); // Get all the nearby objects

        foreach(Collider nearbyObject in collidersToMove)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>(); // Search for rigidbodies 
            Damage(nearbyObject); // Apply damage to nearby objects
            if (rb != null) // If object has a rigidbody
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius); // Add a force to the object to pull it away from the grenade
            }
        }
        Destroy(gameObject);
        SFXManager.PlaySound_AudioMixer(explosionSound, gameObject.transform.position, volumeMultiplier);
    }
    void Damage(Collider hit)
    {
        // Damage
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("EnemyBody") || hit.transform.gameObject.layer == LayerMask.NameToLayer("EnemyHead"))
        {
            ZombieStats enemyStats = hit.transform.GetComponentInParent<ZombieStats>();

            if (enemyStats.IsDead())
                return;

            if (PowerUpManager.instaKillActive) enemyStats.InstantKill();   // PowerUp

            enemyStats.TakeDamage(damage);
        }
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerStats stats = hit.transform.GetComponent<PlayerStats>();
            if (stats.IsDead())
                return;

            stats.TakeDamage(damage);
        }
    }

    void Decals(Collider hit)
    {
        if (hit.transform.tag == "Untagged")
        {
            GameObject impactGO = Instantiate(weaponDecals.impactUntagged, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation);
            Destroy(impactGO, 1f);
        }
        if (hit.transform.tag == "Enemy")
        {
            GameObject impactGO = Instantiate(weaponDecals.impactEnemyFrontSplash, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation); // front blood
            GameObject impactGO2 = Instantiate(weaponDecals.impactEnemyBackSplash, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation); // Back blood
            Destroy(impactGO, 1f);
            Destroy(impactGO2, 2f);
        }
        if (hit.transform.tag == "Wood")
        {
            GameObject impactGO = Instantiate(weaponDecals.impactWood, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation);
            Destroy(impactGO, 1f);
        }
        if (hit.transform.tag == "Sand")
        {
            GameObject impactGO = Instantiate(weaponDecals.impactSand, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation);
            Destroy(impactGO, 1f);
        }
        if (hit.transform.tag == "Metal")
        {
            GameObject impactGO = Instantiate(weaponDecals.impactMetal, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation);
            Destroy(impactGO, 1f);
        }
        if (hit.transform.tag == "Dirt")
        {
            GameObject impactGO = Instantiate(weaponDecals.impactDirt, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation);
            Destroy(impactGO, 1f);
        }
        if (hit.transform.tag == "Concrete")
        {
            GameObject impactGO = Instantiate(weaponDecals.impactConcrete, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation);
            Destroy(impactGO, 1f);
        }
        // Extras...
        if (hit.transform.tag == "Glass")
        {
            GameObject impactGO = Instantiate(weaponDecals.impactGlass, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation);
            Destroy(impactGO, 1f);
        }
        if (hit.transform.tag == "Plaster")
        {
            GameObject impactGO = Instantiate(weaponDecals.impactPlaster, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation);
            Destroy(impactGO, 1f);
        }
        if (hit.transform.tag == "Plastic")
        {
            GameObject impactGO = Instantiate(weaponDecals.impactPlastic, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation);
            Destroy(impactGO, 1f);
        }
        if (hit.transform.tag == "Paper")
        {
            GameObject impactGO = Instantiate(weaponDecals.impactPaper, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation);
            Destroy(impactGO, 1f);
        }
        if (hit.transform.tag == "Water")
        {
            GameObject impactGO = Instantiate(weaponDecals.impactWater, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation);
            Destroy(impactGO, 1f);
        }
        if (hit.transform.tag == "Mud")
        {
            GameObject impactGO = Instantiate(weaponDecals.impactMud, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation);
            Destroy(impactGO, 1f);
        }
        if (hit.transform.tag == "Stone")
        {
            GameObject impactGO = Instantiate(weaponDecals.impactStone, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation);
            Destroy(impactGO, 1f);
        }
        if (hit.transform.tag == "Brick")
        {
            GameObject impactGO = Instantiate(weaponDecals.impactBrick, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation);
            Destroy(impactGO, 1f);
        }
    }

    void Initialize()
    {
        grenadeCountdown = grenadeDelay;
        shootingAvoidLayers = ~shootingAvoidLayers; // Layermask (to avoid "Player" layermask)
    }
}
