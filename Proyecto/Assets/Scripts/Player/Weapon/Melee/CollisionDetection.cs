using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    [SerializeField] WeaponDecals weaponDecals;
    PerkManager perkManager;

    [SerializeField] int damage = 10;
    [SerializeField] float impactForce = 30f;
    [SerializeField] float batRadius = 1f;


    void Awake()    => GetReferences(); 
    void OnTriggerEnter(Collider hit)
    {
        Collider[] collidersToMove = Physics.OverlapSphere(transform.position, batRadius); // Get all the nearby objects
        foreach (Collider nearbyObject in collidersToMove)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>(); // Search for rigidbodies 
            if (rb != null) // If object has a rigidbody
                rb.AddExplosionForce(impactForce, transform.position, batRadius); // Add a force to the object to pull it away from the grenade
        }            
        Damage(hit);
        Decals(hit); // Particles through colliders
    }

    void Damage(Collider hit)
    {
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("EnemyBody") || hit.transform.gameObject.layer == LayerMask.NameToLayer("EnemyHead"))
        {
            CharacterStats enemyStats = hit.transform.GetComponentInParent<CharacterStats>();
            if (enemyStats.IsDead())
                return;

            if (PowerUpManager.instaKillActive) enemyStats.InstantKill();
            else    // Normal damage
            {
                if (PerkManager.strikeActive)   enemyStats.TakeDamage(damage * perkManager.GetGoldenStrikeMultiplier());
                else                            enemyStats.TakeDamage(damage);
            }
            PlayerHUD.instance.UpdateScoreHitAmount();
        }
    }

    void Decals(Collider hit)
    {
        //if (hit.transform.tag == "Default")
        //{
        //    GameObject impactGO = Instantiate(weaponDecals.impactUntagged, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation);
        //    Destroy(impactGO, 1f);
        //}
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("EnemyBody"))
        {
            //GameObject impactGO = Instantiate(weaponDecals.impactEnemyFrontSplash, new Vector3(gameObject.transform.GetComponent<Collider>().ClosestPoint(hit.transform.position))); // front blood
            GameObject impactGO = Instantiate(weaponDecals.impactEnemyFrontSplash, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation); // front blood
            GameObject impactGO2 = Instantiate(weaponDecals.impactEnemyBackSplash, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation); // Back blood
            Destroy(impactGO, 1f);
            Destroy(impactGO2, 2f);
        }
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("EnemyHead"))
        {
            GameObject impactGO = Instantiate(weaponDecals.impactEnemyFrontSplash, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation); // front blood
            GameObject impactGO2 = Instantiate(weaponDecals.impactEnemyBackSplash, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation); // Back blood
            Destroy(impactGO, 1f);
            Destroy(impactGO2, 2f);
        }
        //if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Wood"))
        //{
        //    GameObject impactGO = Instantiate(weaponDecals.impactWood, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation);
        //    Destroy(impactGO, 1f);
        //}
        //if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Sand"))
        //{
        //    GameObject impactGO = Instantiate(weaponDecals.impactSand, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation);
        //    Destroy(impactGO, 1f);
        //}
        //if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Metal"))
        //{
        //    GameObject impactGO = Instantiate(weaponDecals.impactMetal, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation);
        //    Destroy(impactGO, 1f);
        //}
        //if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Dirt"))
        //{
        //    GameObject impactGO = Instantiate(weaponDecals.impactDirt, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation);
        //    Destroy(impactGO, 1f);
        //}
        //if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Concrete"))
        //{
        //    GameObject impactGO = Instantiate(weaponDecals.impactConcrete, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation);
        //    Destroy(impactGO, 1f);
        //}
        //// Extras...
        //if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Glass"))
        //{
        //    GameObject impactGO = Instantiate(weaponDecals.impactGlass, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation);
        //    Destroy(impactGO, 1f);
        //}
        //if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Plaster"))
        //{
        //    GameObject impactGO = Instantiate(weaponDecals.impactPlaster, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation);
        //    Destroy(impactGO, 1f);
        //}
        //if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Plastic"))
        //{
        //    GameObject impactGO = Instantiate(weaponDecals.impactPlastic, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation);
        //    Destroy(impactGO, 1f);
        //}
        //if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Paper"))
        //{
        //    GameObject impactGO = Instantiate(weaponDecals.impactPaper, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation);
        //    Destroy(impactGO, 1f);
        //}
        //if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Water"))
        //{
        //    GameObject impactGO = Instantiate(weaponDecals.impactWater, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation);
        //    Destroy(impactGO, 1f);
        //}
        //if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Mud"))
        //{
        //    GameObject impactGO = Instantiate(weaponDecals.impactMud, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation);
        //    Destroy(impactGO, 1f);
        //}
        //if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Stone"))
        //{
        //    GameObject impactGO = Instantiate(weaponDecals.impactStone, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation);
        //    Destroy(impactGO, 1f);
        //}
        //if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Brick"))
        //{
        //    GameObject impactGO = Instantiate(weaponDecals.impactBrick, new Vector3(hit.transform.position.x, transform.position.y, transform.position.z), hit.transform.rotation);
        //    Destroy(impactGO, 1f);
        //}
    }

    void GetReferences()
    {
        //weaponDecals = GameObject.FindGameObjectWithTag("WeaponController").GetComponent<WeaponDecals>();
        perkManager = GetComponentInParent<PerkManager>();
    }
}

