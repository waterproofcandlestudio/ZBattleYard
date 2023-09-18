using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryBox : MonoBehaviour
{
    Animator controller;
    Animation gunMovement;

    [Header("Booleans")]
    [SerializeField] public static bool openBox = false;
    [SerializeField] public static bool boxIsOpen;
    [SerializeField] public static bool canTakeWeapon;
    [SerializeField] private static bool triggerWeapon;

     [Header("Gun Logic")]
    public GameObject[] guns;
    [SerializeField] private GameObject gunsList;

    //public int[] gunIndex;
    /*[SerializeField] */public int price = 950;
    public int salePrice = 10;
    public AudioClip boxMusic;

    public int selectedWeapon = 0;
    public float timer;
    public int counter, counterCompare;
    public Transform gunPosition;

    [Header("Colliders")]
    [SerializeField] private Collider boxCollider;
    [SerializeField] private Collider allowPickupCollider;

    WeaponDecals weaponDecals;

    void Start()
    {
        controller = GetComponentsInChildren<Animator>()[0];
        gunMovement = GetComponentsInChildren<Animation>()[0];
    }

    void FixedUpdate()
    {
        UpdateMysteryBox();
    }

    void ResetGuns()
    {
        // Insert guns into the array that controls randomness
        int i = 0;
        foreach (Transform child in gunsList.transform)
        {
            guns[i] = child.transform.gameObject;
            i++;
        }
    }

    public void UpdateMysteryBox()
    {
        //if(canTakeWeapon == true)
        //{
        //    allowPickupCollider.enabled = false;
        //}
        //if (canTakeWeapon == false)
        //{
        //    allowPickupCollider.enabled = true;
        //}
        if (openBox == true) // When opening box
        {
            OpenMysteryBox();
            openBox = false;
            canTakeWeapon = false;
            triggerWeapon = true;
            boxIsOpen = true;
        }
        else if (controller.GetCurrentAnimatorStateInfo(0).IsName("OpenMysteryBox") || gunMovement.IsPlaying("GunRiseMysteryBox")) // Box is playing: Timer && Randomizer
        {

            if(boxIsOpen == false) // Reset code
                ResetMysteryBox();

            else // Principal code
            {
                timer += Time.deltaTime;

                if (timer < 4.0f && counter < counterCompare)
                    counter++;

                else if (counter == counterCompare)
                {
                    counter = 0;
                    RandomizeWeapon();
                    counterCompare++;
                }
                else if (triggerWeapon) // Last weapon
                {
                    canTakeWeapon = true;
                    triggerWeapon = false;
                }
                guns[selectedWeapon].transform.position = gunPosition.transform.position;
            }
        }
        else // Box is closed
        {
            counter = 0;
            counterCompare = 0;
            timer = 0f;
            DisableGuns();
            boxIsOpen = false;
        }
    }

    public void OpenMysteryBox()
    {
        OpenLid();
        StartGunMovement();
        GetComponent<AudioSource>().clip = boxMusic;
        GetComponent<AudioSource>().Play();
    }

    void RandomizeWeapon()
    {
        int gunCount = guns.Length; // Numb of guns in the box
        int rand = Random.Range(0, gunCount);

        while (rand == selectedWeapon) /// PROBLEMATIC
            rand = Random.Range(0, gunCount);

        selectedWeapon = rand;

        //for (int i = 0; i < guns.Length; i++) // Extra to disable guns(Just in case, can comment it if i want to)
        //{
        //    if (i != selectedWeapon)
        //    {
        //        guns[i].SetActive(false);
        //    }
        //}

        DisableGuns();

        guns[selectedWeapon].SetActive(true);
        guns[selectedWeapon].transform.position = gunPosition.transform.position;
    }

    void DisableGuns()
    {
        for (int i = 0; i < guns.Length; i++)
            guns[i].SetActive(false);
    }

    void OpenLid() { controller.Play("OpenMysteryBox"); }

    void CloseLid() { controller.Play("CloseMysteryBox"); }

    void StartGunMovement() { gunMovement.Play(); }

    void StopGunMovement() { gunMovement.Stop(); }

    void ResetMysteryBox()
    {
        DisableGuns();
        CloseLid();
        StopGunMovement();
        counter = 0;
        counterCompare = 0;
        timer = 0f;
        openBox = false;
        canTakeWeapon = false;
        triggerWeapon = true;
        boxIsOpen = false;
    }



    //private void OnTriggerStay(Collider collide)
    //{
    //    if (collide.tag == "Player")
    //    {
    //        if (Input.GetKeyDown("f") && GameManager.playerCash >= mysteryBoxPrice)
    //        {
    //            openBox = true;
    //            GameManager.playerCash -= mysteryBoxPrice;
    //        }

    //        if (canTakeWeapon && Input.GetKeyDown("f"))
    //        {
    //            src_weaponController = FindObjectOfType<src_WeaponController>();

    //            //src_weaponController.DeselectWeapon();
    //            //src_weaponController.weaponsInUse[src_weaponController.weaponToSelect] = src_weaponController.weaponsInGame[gunIndex[selectedWeapon]];
    //            /* FOR GRENADES
    //            if (src_weaponController.setElement == 8)
    //            {
    //                //pickupSound pickupGOW1 = hit.transform.GetComponent<Pickup>();
    //                //addStickGrenades(pickupGOW1.amount);
    //            }
    //            */

    //            DisableGuns();
    //            CloseLid();
    //        }
    //    }

    //}
}
