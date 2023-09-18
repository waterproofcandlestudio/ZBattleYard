using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    WeaponShooting shooting;
    [SerializeField] PowerUpUIManager powerUpUIManager;
    PowerUp powerUpMan;

    /// PowerUps
    [SerializeField] public static bool instaKillActive;
    [SerializeField] public static bool doublePointsActive;
    [SerializeField] public static bool magicBoxSaleActive;
    [SerializeField] public static bool slowTimeActive;

    void Awake()
    {
        GetReferences();
    }
    void Start()
    {
        InitVariables();
    }

    void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "PowerUp")     // If player collides/interacts with a "PowerUp", activate it's workage
        {
            GameObject powerUpGameObject = other.gameObject;
            powerUpMan = powerUpGameObject.GetComponent<PowerUpLogic>().powerUp;

            AudioSource.PlayClipAtPoint(powerUpMan.pickupSound, gameObject.transform.position);

            ActivatePowerUpGameChanger();

            Destroy(powerUpGameObject);
            return;
        }
        else
            return;
    }

    #region - Logic -

    void ActivatePowerUpGameChanger()
    {
        PowerType type = powerUpMan.powerType;
        int duration = powerUpMan.duration;

        if (type == PowerType.instaKill)
        {
            ActivateInstaKill(duration);
            powerUpUIManager.AddPowerUpImageFeedback(powerUpMan);   // UI Feedback
        }

        if (type == PowerType.maxAmmo)
        {
            ActivateMaxAmmo();
        }

        if (type == PowerType.doublePoints)
        {
            ActivateDoublePoints(duration);
            powerUpUIManager.AddPowerUpImageFeedback(powerUpMan);
        }

        if (type == PowerType.nuke)
        {
            ActivateNuke();
        }

        if (type == PowerType.magicBoxDiscount)
        {
            ActivateMagicBoxSale(duration);
            powerUpUIManager.AddPowerUpImageFeedback(powerUpMan);
        }

        if (type == PowerType.slowTime)
        {
            ActivateSlowTime(duration);
            powerUpUIManager.AddPowerUpImageFeedback(powerUpMan);
        }

        if (type == PowerType.randomPerk)
        {
            ActivateRandomPerk();
        }

        else
            return;
    }


    #region - PowerUp Activation Methods -

    //void ActivateInstaKill(int duration)
    //{
    //    instaKillActive = true;
    //    //yield return new WaitForSeconds(duration);

    //    for (float timer = duration; timer >= 0; timer -= Time.deltaTime)
    //    {
    //        if (instaKillActive)
    //        {
    //            //win();
    //            instaKillActive = false;
    //        }
    //    }

    //    instaKillActive = false;
    //}
    void ActivateInstaKill(int duration)     // Every shot kills instantly every enemy (for a period of time)
    {
        instaKillActive = true;
    }
    void ActivateMaxAmmo()   // Restores ammo for every gun of the player
    {
        shooting.RefillAllAmmo();
    }
    void ActivateDoublePoints(int duration)  // Player wins double of standard points on every action (for a period of time)
    {
        doublePointsActive = true;
    }
    //IEnumerator ActivateDoublePoints(int duration)  // Player wins double of standard points on every action (for a period of time)
    //{
    //    doublePointsActive = true;
    //    yield return new WaitForSeconds(duration);
    //    doublePointsActive = false;
    //}
    void ActivateNuke()     // Wipes/Kills every zombie spawned in map instantly
    {
        PlayerHUD.instance.UpdateScoreNukeAmount();

        List<CharacterStats> enemyList = GetComponentInChildren<EnemySpawner>().enemyList;

        foreach (ZombieStats enemy in enemyList)
        {
            if(enemy != null)
            {
                if (!enemy.IsDead())
                {
                    enemy.NukeDeath();
                    StatsManager.kills += 1;
                }
            }
        }
    }
    void ActivateMagicBoxSale(int duration)  // Magic box is on sale, purchases avaliable for only 10$ (for a period of time)
    {
        magicBoxSaleActive = true;
    }
    void ActivateSlowTime(int duration)  // Enemies move slow (for a period of time)
    {
        slowTimeActive = true;
    }
    //IEnumerator ActivateMagicBoxSale(int duration)  // Magic box is on sale, purchases avaliable for only 10$ (for a period of time)
    //{
    //    magicBoxSaleActive = true;
    //    yield return new WaitForSeconds(duration);
    //    magicBoxSaleActive = false;
    //}
    //IEnumerator ActivateSlowTime(int duration)  // Enemies move slow (for a period of time)
    //{
    //    slowTimeActive = true;
    //    yield return new WaitForSeconds(duration);
    //    slowTimeActive = false;
    //}
    void ActivateRandomPerk()    // Gives player a random perk of the ones avaliable in the map
    {
        
    }

    #endregion

    #endregion

    void GetReferences()
    {
        shooting = GetComponent<WeaponShooting>();
    }

    void InitVariables()
    {
        instaKillActive     = false;
        doublePointsActive  = false;
        magicBoxSaleActive  = false;
        slowTimeActive      = false;
    }
}
