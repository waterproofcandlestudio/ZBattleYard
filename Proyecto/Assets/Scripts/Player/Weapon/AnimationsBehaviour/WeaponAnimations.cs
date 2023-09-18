using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimations : MonoBehaviour
{
    MovementController movementController;
    [HideInInspector] public Animator weaponAnimator;

    bool isInitialised;
    bool isGroundedTrigger;
    float weaponAnimationSpeed;

    void Awake() => GetReferences();
    void OnEnable() => weaponAnimator.SetBool("Reloading", false);

    void Update()
    {
        if (!isInitialised)     return;
        SetWeaponAnimations();
    }

    void SetWeaponAnimations()
    {
        if (movementController.IsGrounded() && !isGroundedTrigger)
        {
            weaponAnimator.SetTrigger("Land");
            isGroundedTrigger = true;
        }
        else if (!movementController.IsGrounded() && isGroundedTrigger) 
            isGroundedTrigger = false;

        weaponAnimator.SetFloat("weaponAnimationSpeed", weaponAnimationSpeed);
    }

    void GetReferences()
    {
        movementController = GetComponentInParent<MovementController>();
        weaponAnimator = GetComponentInChildren<Animator>();
    }
}
