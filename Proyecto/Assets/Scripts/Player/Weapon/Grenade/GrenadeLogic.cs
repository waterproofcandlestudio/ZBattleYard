using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GrenadeLogic : MonoBehaviour
{
    PlayerInput playerInput;

    [Header("Grenade Launch")]
    [SerializeField] Transform grenadeSpawnParent;
    [SerializeField] GameObject grenadePrefab;
    [SerializeField] float throwForce = 20f;
    [SerializeField] float grenadeReloadTimer = 2f;
    [SerializeField] int maxGrenadesInInventory = 4;
    [SerializeField] TextMeshProUGUI grenadesInInventoryText;

    int grenadesInInventory = 2;
    bool isThrowingGrenade = false;

    GameObject grenadeInHand;

    void Start() => Initialize();

    public void AddNewRoundGrenades(int amount)
    {
        if (grenadesInInventory >= maxGrenadesInInventory)
            return;

        else
        {
            while ((grenadesInInventory + amount) > maxGrenadesInInventory)
                amount--;

            int amountToRefill = amount;
            while (amountToRefill > 0)
            {
                grenadesInInventory ++;
                grenadesInInventoryText.text = grenadesInInventory.ToString();
                amountToRefill--;
            }
        }
    }
    public void RemoveGrenades(int amount)
    {
        grenadesInInventory -= amount;
        grenadesInInventoryText.text = grenadesInInventory.ToString();
    }
    public void SpawnGrenade()
    {
        if (!isThrowingGrenade && grenadesInInventory > 0)
        {
            RemoveGrenades(1);
            grenadeInHand = Instantiate(grenadePrefab, grenadeSpawnParent.position, grenadeSpawnParent.rotation); // Create grenade
            grenadeInHand.transform.parent = grenadeSpawnParent.transform;
            StartCoroutine(ReloadGrenade());
        }


        //if (infiniteGrenades)
        //{
        //    GrenadeThrowLogic();
        //    return;
        //}
    }
    void GrenadeThrowLogic()
    {
        if (grenadeInHand == null)
            return;

        Rigidbody rb = grenadeInHand.GetComponent<Rigidbody>(); // Get the rigidbody of the grenade
        rb.isKinematic = false;
        rb.AddForce(grenadeInHand.transform.forward * throwForce, ForceMode.VelocityChange); // Add a force to it's rigidbody forward
        grenadeInHand.transform.parent = null;

        grenadeInHand = null;
    }
    IEnumerator ReloadGrenade() // Coroutine - Grenade Timer
    {
        isThrowingGrenade = true;
        yield return new WaitForSeconds(grenadeReloadTimer - .25f);

        yield return new WaitForSeconds(.25f);
        isThrowingGrenade = false;
    }

    void Initialize()
    {
        playerInput = new PlayerInput(); // Initialize Player input to use new input system
        playerInput.Enable(); // Enable input system!
        playerInput.Weapon.Grenade.started += e => SpawnGrenade(); // Grenade
        playerInput.Weapon.Grenade.canceled += e => GrenadeThrowLogic(); // Grenade

        grenadesInInventoryText.text = grenadesInInventory.ToString();
    }
    void OnDisable()
    {
        playerInput.Disable();
    }
}
