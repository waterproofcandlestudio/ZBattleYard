using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

/*
=============
PlayerStance -- Stand, Crouch & Prone player Stances
=============
*/
public enum PlayerStance
{
    stand,
    crouch,
    prone
}
/*
=============
CharacterStance -- Player Stance Settings

These are the Stand, Crouch & Prone settings when
enum "PlayerStance" is changed
=============
*/
[Serializable]
public class CharacterStance
{
    public float cameraHeight;
    public CapsuleCollider stanceCollider;
}
public class StanceController : MonoBehaviour
{
    PlayerInput playerInput;
    CharacterController characterController;
    MovementController movementController;
    CameraController cameraController;

    [Header("Stance")]
    public PlayerStance playerStance;
    [SerializeField] float playerStanceSmoothing;
    [SerializeField] CharacterStance playerStandStance;
    [SerializeField] CharacterStance playerCrouchStance;
    [SerializeField] CharacterStance playerProneStance;
    float stanceCheckErrorMargin = 0.05f; // Collision detection crouching

    public Transform feetTransform; // Collision detection crouching

    Vector3 stanceCapsuleCenterVelocity;
    float stanceCapsuleHeightVelocity;

    [Header("Settings")]
    [SerializeField] LayerMask playerMask;    // Collision detection crouching


    void Awake() => GetReference();
    void Start() => InitVariables();
    void Update() => UpdateStance();


    public void CheckJumpException()
    {
        if (playerStance == PlayerStance.crouch)    // If Stance == Crouch ==> [Stand Up]
        {
            if (StanceCheck(playerStandStance.stanceCollider.height))   // Chech if head collides on top with something to not be able to stand up
                return; 
            playerStance = PlayerStance.stand;
        }
    }
    public void CheckSprintNStance()   // Sprinting && Stances exceptions
    {
        if (movementController.inputWASD.y <= 0.1f /*playerSettings.stamina <= 0*/) /// Sprint  [0.2f]
            MovementController.isSprinting = false;

        if ((playerStance == PlayerStance.crouch) && StanceCheck(playerStandStance.stanceCollider.height))  /// Check if there's an object above player so he can change "Stance" when starts sprinting
        {
            MovementController.isSprinting = false;
            return;
        }
        if ((playerStance == PlayerStance.prone) && StanceCheck(playerStandStance.stanceCollider.height))
        {
            MovementController.isSprinting = false;
            return;
        }
        if ((playerStance == PlayerStance.prone) && StanceCheck(playerStandStance.stanceCollider.height))
        {
            MovementController.isSprinting = false;
            return;
        }
        
        if (MovementController.isSprinting)     /// Stance when sprinting
        {
            if (playerStance == PlayerStance.crouch || playerStance == PlayerStance.prone) // Stand up ==> if player sprints && is (crouching || proning)
            {
                playerStance = PlayerStance.stand;
            }
            if (playerInput.OnFoot.Crouch.triggered) // If player crouches while sprinting ==> stops sprinting 
            {
                MovementController.isSprinting = false;
                playerStance = PlayerStance.crouch;
            }
            if (playerInput.OnFoot.Prone.triggered) // ""    ""    prones   ""      ""     ==> stops sprinting 
            {
                MovementController.isSprinting = false;
                playerStance = PlayerStance.prone;
            }
        }
    }

    /// <summary>
    ///     Calculates actual Stance properties and applies them (considering Camera && Colliders)
    /// </summary>
    void UpdateStance()
    {
        CharacterStance currentStance = playerStandStance;
        if (playerStance == PlayerStance.crouch)
            currentStance = playerCrouchStance;
        else if (playerStance == PlayerStance.prone)
            currentStance = playerProneStance;

        /// Changes
        cameraController.cameraHeight = Mathf.SmoothDamp(cameraController.cameraHolder.localPosition.y, currentStance.cameraHeight, ref cameraController.cameraHeightVelocity, Time.fixedDeltaTime * playerStanceSmoothing); // Cam
        cameraController.cameraHolder.localPosition = new Vector3(cameraController.cameraHolder.localPosition.x, cameraController.cameraHeight, cameraController.cameraHolder.localPosition.z); // Position

        characterController.height = Mathf.SmoothDamp(characterController.height, currentStance.stanceCollider.height, ref stanceCapsuleHeightVelocity, Time.fixedDeltaTime * playerStanceSmoothing); // Collider Heigh 
        characterController.center = Vector3.SmoothDamp(characterController.center, currentStance.stanceCollider.center, ref stanceCapsuleCenterVelocity, Time.fixedDeltaTime * playerStanceSmoothing); // Collider Center
    }

    /*
    =============
    Crouch -- What happens when player crouches

    Called by Awake.

    Calculates when player presses crouch button what will happen depending on the 
    PlayerStance and colliders on top of the player. 
    If there's an object up of the player while crouching, he won't be able to Stand up!
    =============
    */
    void Crouch()
    {
        if (playerStance == PlayerStance.crouch)     // Crouch ==> Stand
        {
            if (StanceCheck(playerStandStance.stanceCollider.height))    // Chech if head collides on top with something to not be able to stand up
                return;

            playerStance = PlayerStance.stand;
            return;
        }
        if (playerStance == PlayerStance.prone)    // Prone ==> Crouch
        {
            if (StanceCheck(playerCrouchStance.stanceCollider.height))  // Chech if head collides on top with something to not be able to stand up
                return;

            playerStance = PlayerStance.crouch;
            return;
        }

        if (StanceCheck(playerStandStance.stanceCollider.height))   // [Again...] Chech if head collides on top with something to not be able to stand up
            return;

        playerStance = PlayerStance.crouch;
    }
    /*
    =============
    Prone -- What happens when player Prones

    Called by Awake.

    [Same as Crouch but with Prone]
    Calculates when player presses Prone button what will happen depending on the 
    PlayerStance and colliders on top of the player. 
    If there's an object up of the player while pronning, he won't be able to Stand up!
    =============
    */
    void Prone()
    {
        if (playerStance == PlayerStance.prone)     // Prone ==> Stand
        {
            if (StanceCheck(playerCrouchStance.stanceCollider.height))  // Chech if head collides on top with something to not be able to stand up
                return;

            if (StanceCheck(playerStandStance.stanceCollider.height))   // Chech if head collides on top with something to not be able to stand up
                return;


            playerStance = PlayerStance.stand;
            return;
        }
        if (StanceCheck(playerCrouchStance.stanceCollider.height))      // [Again...] Chech if head collides on top with something to not be able to stand up
            return;

        playerStance = PlayerStance.prone;
    }

    /*
    =============
    StanceCheck -- What happens when player Prones

    Called by Crouch & Prone.

    Checks if player can change from stance depending on the collisions over him if 
    there's something on top of him.
    =============
    */
    bool StanceCheck(float stanceCheckHeight)
    {
        var start = new Vector3(feetTransform.position.x, feetTransform.position.y + characterController.radius + stanceCheckErrorMargin, feetTransform.position.z);
        var end = new Vector3(feetTransform.position.x, feetTransform.position.y - characterController.radius - stanceCheckErrorMargin + stanceCheckHeight, feetTransform.position.z);

        return Physics.CheckCapsule(start, end, characterController.radius, playerMask);
    }

    void GetReference()
    {
        movementController = GetComponent<MovementController>();
        characterController = GetComponent<CharacterController>();
        cameraController = GetComponent<CameraController>();
    }

    void InitVariables()
    {
        playerInput = new PlayerInput(); // Initialize Player input to use new input system
        playerInput.Enable(); // Enable input system!

        playerInput.OnFoot.Crouch.performed += e => Crouch();   // Crouch
        playerInput.OnFoot.Prone.performed += e => Prone(); // Prone
    }

    void OnDisable()
    {
        playerInput.Disable();
    }
}
