using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class EquipmentManager : MonoBehaviour
{
    PlayerInput playerInput;
    WeaponShooting shooting;
    Inventory inventory;
    PlayerHUD hud;

    [HideInInspector] public float inputScroll;
    float switchWaitTime = 0.1f;
    int previousSelectedWeapon;

    public int currentlyEquippedWeapon = 2;
    public GameObject currentWeaponObject = null;
    public Transform currentWeaponMuzzleFlash = null;

    public Transform WeaponHolderR = null;
    Animator anim;
    public Animator currentWeaponAnim;

    public Weapon defaultWeapon = null;


    void Awake()
    {
        GetReferences();
    }

    void Start()
    {
        InitVariables();
    }

    void Update()
    {
        UpdateMouseWheelLogic();
    }

    void UpdateMouseWheelLogic()
    {
        if (inputScroll > 0f) // forward
            MouseScrollUp();

        else if (inputScroll < 0f) // backwards
            MouseScrollDown();
    }
    public IEnumerator SwapWeapon(int weaponType) // Swapping Logic!
    {
        //shooting.lastShotTime = 0;

        if (shooting.canReload)
        {
            yield return new WaitForSeconds(switchWaitTime);

            UnequipWeapon();
            EquipWeapon(inventory.GetItem(currentlyEquippedWeapon));
        }

        //if (weaponType == 0)
        //{
        //    //anim.SetInteger("weaponType", weaponType);
        //    //anim.SetTrigger("weaponSwitchTrigger");

        //    yield return new WaitForSeconds(switchWaitTime);

        //    currentlyEquippedWeapon = 0;
        //    UnequipWeapon();
        //    EquipWeapon(inventory.GetItem(currentlyEquippedWeapon));
        //}
        //if (weaponType == 1)
        //{
        //    //anim.SetInteger("weaponType", weaponType);
        //    //anim.SetTrigger("weaponSwitchTrigger");

        //    yield return new WaitForSeconds(switchWaitTime);

        //    currentlyEquippedWeapon = 1;
        //    UnequipWeapon();
        //    EquipWeapon(inventory.GetItem(currentlyEquippedWeapon));
        //}
        //if (weaponType == 2)
        //{
        //    //anim.SetInteger("weaponType", weaponType);
        //    //anim.SetTrigger("weaponSwitchTrigger");

        //    yield return new WaitForSeconds(switchWaitTime);

        //    currentlyEquippedWeapon = 2;
        //    UnequipWeapon();
        //    EquipWeapon(inventory.GetItem(currentlyEquippedWeapon));
        //}
    }

    /// Mouse ScrollWheel
    void MouseScrollUp()
    {
        previousSelectedWeapon = currentlyEquippedWeapon;

        if (currentlyEquippedWeapon <= 0)
            currentlyEquippedWeapon = inventory.weapons.Length - 1;

        else
            currentlyEquippedWeapon--;

        if (previousSelectedWeapon != currentlyEquippedWeapon && inventory.weapons[currentlyEquippedWeapon] != null)
            StartCoroutine(SwapWeapon(currentlyEquippedWeapon));

        else
            currentlyEquippedWeapon = previousSelectedWeapon;
    }
    void MouseScrollDown()
    {
        previousSelectedWeapon = currentlyEquippedWeapon;

        if (currentlyEquippedWeapon >= inventory.weapons.Length - 1) // If we are in the weapon inventory limit, go back to the start
            currentlyEquippedWeapon = 0;

        else
            currentlyEquippedWeapon++;

        if (previousSelectedWeapon != currentlyEquippedWeapon && inventory.weapons[currentlyEquippedWeapon] != null)
            StartCoroutine(SwapWeapon(currentlyEquippedWeapon));
        else
            currentlyEquippedWeapon = previousSelectedWeapon;
    }

    /// Gamepad
    void WeaponSwitchGamePad() // Same code as "MouseScrollUp()"!!
    {
        previousSelectedWeapon = currentlyEquippedWeapon;

        if (currentlyEquippedWeapon >= inventory.weapons.Length - 1)
            currentlyEquippedWeapon = 0;

        else
            currentlyEquippedWeapon++;

        if (previousSelectedWeapon != currentlyEquippedWeapon && inventory.weapons[currentlyEquippedWeapon] != null) 
            StartCoroutine(SwapWeapon(currentlyEquippedWeapon));
    }

    public void EquipWeapon(Weapon weapon)
    {
        //currentlyEquippedWeapon = (int)weapon.weaponStyle;

        //anim.SetInteger("weaponType", (int)weapon.weaponType); // Animation Events
        //currentWeaponAnim = currentWeaponObject.GetComponent<Animator>(); // Animation Events
        currentWeaponObject = Instantiate(weapon.prefab, WeaponHolderR); // Removed in tut
        hud.UpdateNewWeaponUI(weapon.name ,weapon.icon, shooting.ReturnCurrentAmmoStats(), shooting.ReturnCurrentAmmoStorageStats());

        currentWeaponMuzzleFlash = currentWeaponObject.transform.GetChild(0); // Get MuzzleFlash "transform.position" from the equiped weapon!!!
    }

    public int GetCurrentlyEquippedWeapon()
    {
        return currentlyEquippedWeapon;
    }
    public void ChangeCurrentlyEquippedWeapon(int i)
    {
        currentlyEquippedWeapon = i;
    }

    void WeaponSwitchNumb(int i)
    {
        if (currentlyEquippedWeapon != i && inventory.weapons[i] != null) // Primary Weapon
        {
            if (shooting.canReload)
            {
                UnequipWeapon();
                EquipWeapon(inventory.GetItem(i));
            }
        }
    }

    public void UnequipWeapon()
    {
        //anim.SetTrigger("unequipWeapon");
        Destroy(currentWeaponObject); // Removed in tut
    }

    void GetReferences()
    {
        anim = GetComponentInChildren<Animator>();
        inventory = GetComponent<Inventory>();
        hud = GetComponent<PlayerHUD>();
        shooting = GetComponent<WeaponShooting>();
    }
    void InitVariables()
    {
        playerInput = new PlayerInput(); // Initialize Player input to use new input system
        playerInput.Enable();

        playerInput.Weapon.SwitchWeapon.performed += e => inputScroll = e.ReadValue<float>(); // Mouse Scroll Switching
        playerInput.Weapon.SwitchWeapon1.performed += e => WeaponSwitchNumb(0);     // KeyDown 1
        playerInput.Weapon.SwitchWeapon2.performed += e => WeaponSwitchNumb(1);     // KeyDown 2
        playerInput.Weapon.SwitchWeapon3.performed += e => WeaponSwitchNumb(2);     // KeyDown 3
        playerInput.WeaponGamePad.SwitchWeapon.performed += e => WeaponSwitchGamePad();     // GamePad Switching 
    }
    void OnDisable()
    {
        playerInput.Disable();
    }
}
