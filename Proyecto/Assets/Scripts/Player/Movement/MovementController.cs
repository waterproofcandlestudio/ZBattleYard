using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

// Controls Player movement functionality mainly

/*      NOTES [Fluid movement Methods()]
   
MoveTowards     ==>     Doesn't change velocity at any moment

SmoothDamp      ==>     Starts && Ends Slow, rises velocity in the middle (Good 4 walking, running & Jumping natural movement effects)

Lerp            ==>     Starts fast at beggining n slows when reaches target
                            Works 4 linear calculations: Returns straight lines

Slerp           ==>     Starts fast at beggining n slows when reaches target
                            Works 4 curves: Returns curves based on the center u declare

*/

public class MovementController : MonoBehaviour
{
    AudioSource audioSource;
    PlayerInput playerInput;
    CharacterController characterController;
    PlayerStats stats;
    PerkManager perkManager;
    StanceController stanceController;

    [Header("Audio clips")]
    [SerializeField] AudioClip startRunning;
    [SerializeField] AudioClip jump_AudioClip;

    [Header("Ground Movement")]
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    float moveSpeed;
    Vector3 moveDirection = Vector3.zero;
    public static bool isSprinting;

    [Header("Air Movement")]
    //[SerializeField] float fallingSmoothing;
    [SerializeField] float jumpForce;
    [SerializeField] float gravity;
    [SerializeField] float groundDistance;
    [SerializeField] LayerMask groundMask;
    [SerializeField] bool isCharacterGrounded = false;
    Vector3 velocity = Vector3.zero;

    [HideInInspector] public Vector2 inputWASD;



    [Header("Movement Settings")]
    [SerializeField] bool sprintingHold = false;
    //[SerializeField] float movementSmoothing = 15;
    [Header("Movement - Running")]
    [SerializeField] float runningForwardSpeed = 8;
    [Header("Movement - Walking")]
    [SerializeField] float walkingForwardSpeed = 5;
    [SerializeField] float walkingBackwardSpeed = 3;
    [Header("Speed Effectors")]
    [SerializeField] float speedEffector = 1;
    [SerializeField] float crouchSpeedEffector = 0.6f;
    [SerializeField] float proneSpeedEffector = 0.2f;
    [SerializeField] float fallingSpeedEffector = 0.8f;


    void Awake() => GetReferences();
    void Start() => Initialize();
    void Update()
    {
        // Air movement
        HandleIsGrounded();
        HandleGravity();

        if (stats.IsDead())
            return;
        // Ground movement
        HandleRunning();
        HandleMovement();
    }


    void HandleMovement()
    {
        moveDirection = new Vector3(inputWASD.x, 0, inputWASD.y);
        moveDirection = moveDirection.normalized;
        //newMovementSpeed = Vector3.SmoothDamp(newMovementSpeed, target, ref newMovementSpeedVelocity, Time.deltaTime * (isGrounded ? playerSettings.movementSmoothing : fallingSmoothing)); // Smooth movement
        moveDirection = transform.TransformDirection(moveDirection);
        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
    }
    void HandleRunning()
    {
        stanceController.CheckSprintNStance(); // Sprinting && Stances exceptions

        if (isSprinting)    //moveSpeed = runSpeed;
        {
            if (inputWASD.y > 0 || inputWASD.x != 0) moveSpeed = runningForwardSpeed;  // Forward
            if (inputWASD.y < 0) isSprinting = false; // Backwards
        }
        if (!isSprinting)   //moveSpeed = walkSpeed;
        {
            if (inputWASD.y > 0 || inputWASD.x != 0) moveSpeed = walkingForwardSpeed;  // Forward
            if (inputWASD.y < 0) moveSpeed = walkingBackwardSpeed; // Backwards      
        }

        if (PerkManager.speedMasterActive)  // Speed Master (Stamina) perk active
            moveSpeed *= perkManager.GetSpeedMultiplier();

        // Speed modifiers depending on stances, etc
        if (!isCharacterGrounded) speedEffector = fallingSpeedEffector;
        else if (stanceController.playerStance == PlayerStance.crouch) speedEffector = crouchSpeedEffector;
        else if (stanceController.playerStance == PlayerStance.prone) speedEffector = proneSpeedEffector;
        else speedEffector = 1;
        moveSpeed *= speedEffector;
    }
    void ToggleSprint()
    {
        if (isCharacterGrounded)
        {
            if (!isSprinting)
                audioSource.PlayOneShot(startRunning);
            stats.CheckStamina();
            isSprinting = !isSprinting;
        }
    }
    void StopSprint()
    {
        if (sprintingHold)
            isSprinting = false;
    }

    void HandleIsGrounded() => isCharacterGrounded = Physics.CheckSphere(stanceController.feetTransform.position, groundDistance, groundMask);
    void HandleGravity()
    {
        if (isCharacterGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
    void JumpAction()
    {
        if (isCharacterGrounded)
        {
            velocity.y += Mathf.Sqrt(jumpForce * -2f * gravity);
            audioSource.PlayOneShot(jump_AudioClip);
        }
    }

    public bool IsSprinting() => isSprinting;
    public bool IsGrounded() => isCharacterGrounded;

    void GetReferences()
    {
        characterController = GetComponent<CharacterController>();
        stats = GetComponent<PlayerStats>();
        perkManager = GetComponent<PerkManager>();
        stanceController = GetComponent<StanceController>();
        audioSource = GetComponent<AudioSource>();
    }
    void Initialize()
    {
        playerInput = new PlayerInput(); // Initialize Player input to use new input system
        if (!playerInput.asset.enabled) playerInput.Enable(); // Enable input system!

        playerInput.OnFoot.Jump.performed += e => JumpAction();   // Jump
        playerInput.OnFoot.Sprint.performed += e => ToggleSprint(); // Sprint
        playerInput.OnFoot.SprintReleased.performed += e => StopSprint(); // Stop Sprint
        playerInput.OnFoot.Movement.performed += e => inputWASD = e.ReadValue<Vector2>(); // Movement

        moveSpeed = walkSpeed;
    }
    void OnDestroy()
    {
        playerInput.Disable(); // Enable input system!
    }
    void OnDisable()
    {
        playerInput.Disable(); // Enable input system!
    }
}
