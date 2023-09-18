using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    EnemySpawner enemySpawner;
    PlayerInput playerInput;
    [SerializeField] Score score;
    PlayerStats stats;
    WeaponShooting shooting;
    Inventory inventory;
    PlayerHUD hud;
    EquipmentManager equipmentManager;
    Camera cam;
    PerkManager perkManager;

    [SerializeField] float interactRange = 3;
    [SerializeField] LayerMask pickupLayer;             // "Pickup" [LayerName to reference in inspector!]
    [SerializeField] LayerMask purchasableWallLayer;            // "WallPurchase"
    [SerializeField] LayerMask mysteryBoxLayer;         // "MysteryBox"
    [SerializeField] LayerMask mysteryBoxWeaponLayer;
    [SerializeField] LayerMask purchasableWeaponLayer;
    [SerializeField] LayerMask perkLayer;

    [Header("Interaction Canvas")]
    [SerializeField] CanvasGroup interactionTextGroup = null;
    [SerializeField] TextMeshProUGUI firstText;
    [SerializeField] TextMeshProUGUI secondText;
    [SerializeField] TextMeshProUGUI thirdText;

    [SerializeField] string pickupString = "Pick";
    [SerializeField] string ammoBuyString = "Ammo";
    [SerializeField] string ammoFullString = "Ammo Full!";
    [SerializeField] string mysteryBoxString = "Mystery Box";
    [SerializeField] string alreadyGotPerkString = "Already got it!";

    void Awake()    => GetReferences();
    void Start()    => Initialize();
    void Update()   => CheckCanvasActivation();

    #region - Logic -


    /// <summary>
    ///     Method just called when player uses binded key to interact with objects
    /// </summary>
    void Interactions()
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f));
        RaycastHit hit;
        // Perks ==> Buy
        if (Physics.Raycast(ray, out hit, interactRange, perkLayer))
        {
            Perk newPerk = hit.transform.GetComponent<PerkMachineLogic>().GetPerkInfo();

            if (!PerkInInventory(hit) && score.currentScore >= newPerk.price)
                BuyPerk(newPerk);
        }
        /// Pickup from floor: Weapon & Consumables
        if (Physics.Raycast(ray, out hit, interactRange, pickupLayer))
        {
            if(!InInventory(hit))
            {
                if (hit.transform.GetComponent<ItemObject>().item as Weapon)    // Weapons
                {
                    Weapon newItem = hit.transform.GetComponent<ItemObject>().item as Weapon; // "as Weapon" ==> I'm casting the component I get as a Weapon like i do with (int), (float)...                    
                    inventory.AddItem(newItem);
                    if (shooting.canReload)
                    {
                        equipmentManager.UnequipWeapon();
                        equipmentManager.EquipWeapon(newItem);
                        hud.UpdateWeaponUI(newItem);
                    }
                }
                else    // Consumables
                {
                    Consumable newItem = hit.transform.GetComponent<ItemObject>().item as Consumable;
                    if (newItem.type == ConsumableType.Medikit)  // Heal
                        stats.Heal(stats.GetMaxHealth());
                    if (newItem.type == ConsumableType.Ammo)    //Ammo
                    {
                        if (inventory.GetItem(0) != null)
                            shooting.InitAmmo(0, inventory.GetItem(0));
                        if (inventory.GetItem(1) != null)
                            shooting.InitAmmo(1, inventory.GetItem(1));
                        if (inventory.GetItem(2) != null)
                            shooting.InitAmmo(2, inventory.GetItem(2));
                    }
                }
                Destroy(hit.transform.gameObject);
            }
            return;
        }

        /// "MysteryBox"
        // Pickup gun from "MysteryBox"
        if (Physics.Raycast(ray, out hit, interactRange, mysteryBoxWeaponLayer))
        {
            if (MysteryBox.canTakeWeapon && !InInventory(hit))
            {
                Weapon newItem = hit.transform.GetComponent<ItemObject>().item as Weapon;
                inventory.AddItem(newItem);
                MysteryBox.boxIsOpen = false;
                equipmentManager.UnequipWeapon();
                equipmentManager.EquipWeapon(newItem);
            }
            return;
        }
        // Open "MysteryBox"
        if (Physics.Raycast(ray, out hit, interactRange, mysteryBoxLayer))
        {
            MysteryBox mysteryBox = hit.transform.GetComponent<MysteryBox>();
            int price = mysteryBox.price;   // Normal price
            if (PowerUpManager.magicBoxSaleActive)         // If the powerUp sale is active it changes!
                price = mysteryBox.salePrice;

            if (score.currentScore >= price && !MysteryBox.boxIsOpen)   // GameManager.playerCash
            {
                MysteryBox.openBox = true;
                score.SubstractScore(price);                
            }
            return;
        }

        /// Purchases
        // Wall/door
        if (Physics.Raycast(ray, out hit, interactRange, purchasableWallLayer))
        {
            int wallPrice = hit.transform.GetComponent<WallBuy>().GetWallPrice();

            if (score.currentScore >= wallPrice)    // GameManager.playerCash
            {
                hit.transform.gameObject.SetActive(false);
                score.SubstractScore(wallPrice);
                enemySpawner.UpdateActiveEnemySpawns();
            }
            return;
        }

        // Weapon from wall
        if (Physics.Raycast(ray, out hit, interactRange, purchasableWeaponLayer))
        {
            Weapon newItem = hit.transform.GetComponent<WeaponBuy>().weapon;
            int weaponPrice = newItem.weaponPrice;
            int ammoPrice = newItem.ammoPrice;
            BuyWeapon(equipmentManager.GetCurrentlyEquippedWeapon(), newItem, weaponPrice, ammoPrice);  // Buy Weapon && Buy Ammo (if has weapon & ammo is not full ==> buy it)
        }
    }

    void BuyWeapon(int index, Weapon newItem, int weaponPrice, int ammoPrice)
    {
        if (inventory.WeaponInInventory(newItem))
        {
            int alreadyWeaponIndex =  inventory.WeaponInInventoryIndex(newItem);
            if (score.currentScore >= ammoPrice) // Buy Ammo
            {
                if((shooting.ReturnCurrentAmmo(inventory.WeaponInInventoryIndex(newItem)) != newItem.magazineSize) || (shooting.ReturnStorageAmmo(inventory.WeaponInInventoryIndex(newItem)) != newItem.storedAmmo))
                {
                    inventory.weapons[alreadyWeaponIndex].storedAmmo = newItem.storedAmmo;
                    shooting.InitAmmo(alreadyWeaponIndex, inventory.GetItem(alreadyWeaponIndex));
                    if (shooting.canReload)
                    {
                        equipmentManager.UnequipWeapon();
                        equipmentManager.EquipWeapon(newItem);
                        hud.UpdateWeaponUI(newItem);
                    }
                    shooting.CheckCanShoot(equipmentManager.currentlyEquippedWeapon);
                    score.SubstractScore(ammoPrice);
                }
            }
        }
        else
        {
            if (score.currentScore >= weaponPrice)  // Buy Weapon
            {
                inventory.AddItem(newItem);
                if (shooting.canReload)
                {
                    equipmentManager.UnequipWeapon();
                    equipmentManager.EquipWeapon(newItem);
                    hud.UpdateWeaponUI(newItem);
                }
                shooting.CheckCanShoot(index);
                score.SubstractScore(weaponPrice);
            }            
        }
    }

    void BuyPerk(Perk newPerk)
    {
        SFXManager.PlaySound_AudioMixer(newPerk.pickupSound, gameObject.transform.position);
        perkManager.ActivatePowerUpGameChanger(newPerk);
        score.SubstractScore(newPerk.price);
    }

    /// <summary>
    ///     Currently updated method to show interaction posibilities canvas only
    /// </summary>
    void CheckCanvasActivation()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Perks ==> Buy
        if (Physics.Raycast(ray, out hit, interactRange, perkLayer))   
        {
            if (PerkInInventory(hit))      // Weapon is in inventory
            {
                interactionTextGroup.alpha = 1; // Show 
                ModifyCanvasText("", alreadyGotPerkString, "");
            }

            else                    // Weapon isn't in inventory, so u can grab it
            {
                Perk perk = hit.transform.GetComponent<PerkMachineLogic>().GetPerkInfo();
                interactionTextGroup.alpha = 1; // Show 
                ModifyCanvasText(perk.name, perk.price.ToString());
            }
            return;
        }

        // Pickup floor weapon
        if (Physics.Raycast(ray, out hit, interactRange, pickupLayer))   
        {
            if (InInventory(hit))      // Weapon is in inventory
                interactionTextGroup.alpha = 0;

            else                    // Weapon isn't in inventory, so u can grab it
            {
                Item item = hit.transform.GetComponent<ItemObject>().item;
                interactionTextGroup.alpha = 1; // Show 
                ModifyCanvasText(item.name, pickupString);
            }
            return;
        }
        // Pickup Mystery box weapon
        if (Physics.Raycast(ray, out hit, interactRange, mysteryBoxWeaponLayer) && MysteryBox.canTakeWeapon) 
        {
            if (InInventory(hit))      // Weapon is in inventory
                interactionTextGroup.alpha = 0;

            else                    // Weapon isn't in inventory, so u can grab it
            {
                Item item = hit.transform.GetComponent<ItemObject>().item;
                interactionTextGroup.alpha = 1; // Show 
                ModifyCanvasText(item.name, pickupString);
            }
            return;
        }

        // Buy
        if (Physics.Raycast(ray, out hit, interactRange, purchasableWeaponLayer) || Physics.Raycast(ray, out hit, interactRange, purchasableWallLayer))
        {
            // Wall
            if (hit.transform.gameObject.GetComponent<WallBuy>())
            {
                WallBuy wall = hit.transform.GetComponent<WallBuy>();
                interactionTextGroup.alpha = 1;
                ModifyCanvasText(wall.GetWallName(), wall.GetWallPrice().ToString());
                return;
            }
            // Weapon
            if (hit.transform.gameObject.GetComponent<WeaponBuy>())
            {
                Weapon weapon = hit.transform.GetComponent<WeaponBuy>().weapon;

                int weaponToCheck = inventory.WeaponInInventoryIndex(weapon);
                int currentAmmo = shooting.ReturnCurrentAmmo(weaponToCheck);
                int storageAmmo = shooting.ReturnStorageAmmo(weaponToCheck);

                // Ammo full of this gun
                if (shooting.WeaponHasMaxAmmo(weaponToCheck, currentAmmo, storageAmmo))
                {
                    interactionTextGroup.alpha = 1;
                    ModifyCanvasText(weapon.name, ammoFullString, "");
                    return;
                }
                // Show ammo price only if already have the weapon
                if (inventory.WeaponInInventory(weapon))  
                {
                    interactionTextGroup.alpha = 1;
                    ModifyCanvasText(ammoBuyString, weapon.ammoPrice.ToString());
                    return;
                }
                else // Show gun price only if doesn't have the weapon
                {
                    interactionTextGroup.alpha = 1;
                    ModifyCanvasText(weapon.name, weapon.weaponPrice.ToString());
                }
                return;
            }

            // Item
            if (hit.transform == GetComponent<ItemBuy>())
            {
                ItemBuy item = hit.transform.GetComponent<ItemBuy>();

                if (InInventory(hit))  // Show ammo price only if already have the weapon
                {
                    interactionTextGroup.alpha = 1;
                    ModifyCanvasText(item.item.name, item.price.ToString());
                }
                else // Show gun price only if doesn't have the weapon
                {
                    interactionTextGroup.alpha = 1;
                    ModifyCanvasText(item.item.name, item.ammoPrice.ToString());
                }
                return;
            }
        }

        // Mystery Box
        if (Physics.Raycast(ray, out hit, interactRange, mysteryBoxLayer) && !MysteryBox.boxIsOpen)
        {
            string secondTxt = "";
            // Prices depending on sale powerUp active or nromal price
            if (PowerUpManager.magicBoxSaleActive)  // Sale
                secondTxt = hit.transform.GetComponent<MysteryBox>().salePrice.ToString();
            else  // Without sale
                secondTxt = hit.transform.GetComponent<MysteryBox>().price.ToString();

            interactionTextGroup.alpha = 1; // Show
            ModifyCanvasText(mysteryBoxString, secondTxt);
        }
        else
        {
            interactionTextGroup.alpha = 0; // Show 
            ActivateInteractionCanvasHelpButton();
        }
    }
    /// <summary>
    ///     Detect if is being used Keyboard or Gamepad and show ingame the interaction binded key help text depending on device being used
    /// </summary>
    void ActivateInteractionCanvasHelpButton() 
    {
        thirdText.text = "(" + playerInput.OnFoot.Interact.GetBindingDisplayString(0) + ")";

        //if (Keyboard.current.IsActuated())  // If u are using Keyboard, show interact key binded by keyboard control
        //{
        //    thirdText.text = "(" + playerInput.OnFoot.Interact.GetBindingDisplayString(0) + ")";        //    thirdText.text = "(" + playerInput.OnFoot.Interact.GetBindingDisplayString(Keyboard.all.Count) + ")";
        //    return;
        //}
        //if (Gamepad.current.IsActuated())   // If u are using Gamepad, show interact key binded by Gamepad control
        //{
        //    thirdText.text = "(" + playerInput.OnFoot.Interact.GetBindingDisplayString(1) + ")";        //    thirdText.text = "(" + playerInput.OnFoot.Interact.GetBindingDisplayString(Gamepad.all.Count) + ")";
        //    return;
        //}
    }

    #endregion

    #region Utilities
    void ModifyCanvasText(string firstTxt, string secondTxt, string thirdTxt)
    {
        firstText.text = firstTxt;
        secondText.text = secondTxt;
        thirdText.text = thirdTxt;
    }
    void ModifyCanvasText(string firstTxt, string secondTxt)
    {
        firstText.text = firstTxt;
        secondText.text = secondTxt;
        ActivateInteractionCanvasHelpButton();
    }

    public bool PerkInInventory(RaycastHit hit)    // Checks if weapon hitted by raycast is in my inventory
    {
        for (int i = 0; i <= (perkManager.perkList.Length - 1); i++)
            if (perkManager.perkList[i] == hit.transform.GetComponent<PerkMachineLogic>().GetPerkInfo())
                return true;

        return false;
    }
    public bool PerkInInventory(Perk perk)    // Checks if weapon hitted by raycast is in my inventory
    {
        for (int i = 0; i <= (perkManager.perkList.Length - 1); i++)
            if (perkManager.perkList[i] == perk)
                return true;

        return false;
    }

    bool InInventory(RaycastHit hit)    // Checks if weapon hitted by raycast is in my inventory
    {
        for (int i = 0; i <= (inventory.weapons.Length - 1); i++)
            if (inventory.weapons[i] == hit.transform.GetComponent<ItemObject>().item)
                return true;

        return false;
    }
    #endregion

    void GetReferences()
    {
        cam = Camera.main;
        enemySpawner = GetComponentInChildren<EnemySpawner>();
        inventory = GetComponent<Inventory>();
        stats = GetComponent<PlayerStats>();
        shooting = GetComponent<WeaponShooting>();
        equipmentManager = GetComponent<EquipmentManager>();
        hud = GetComponent<PlayerHUD>();
        perkManager = GetComponent<PerkManager>();
        ReferenceLayerMasks();
    }
    void ReferenceLayerMasks()
    {
        pickupLayer = LayerMask.GetMask("Pickup");
        purchasableWallLayer = LayerMask.GetMask("PurchasableWall");
        mysteryBoxLayer = LayerMask.GetMask("MysteryBox");
        mysteryBoxWeaponLayer = LayerMask.GetMask("MysteryBoxWeapon");
        purchasableWeaponLayer = LayerMask.GetMask("PurchasableWeapon");
        perkLayer = LayerMask.GetMask("Perk");
    }
    void Initialize()
    {
        playerInput = new PlayerInput(); // Initialize Player input to use new input system
        playerInput.Enable(); // Enable input system!

        playerInput.OnFoot.Interact.performed += e => Interactions();    // Interact
    }

    void OnDisable()
    {
        playerInput.Disable();
    }
}