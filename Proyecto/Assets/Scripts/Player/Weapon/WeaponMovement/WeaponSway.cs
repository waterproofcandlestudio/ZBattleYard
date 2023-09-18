using UnityEngine;

/// <QuickNote>
///  
/// Use "Lerp()"  for position control 
/// Use "Slerp()" for rotation control (Works better for rotation!)
/// 
/// </QuickNote>

public class WeaponSway : MonoBehaviour 
{
    #region - Variables -

    PlayerInput playerInput;
    PlayerStats stats;
    MovementController movementController;

    [Header("Extra Activation Options     「✔」 ")]
    [SerializeField] bool isMine;
    [Space]

    [SerializeField] bool enableMousePosition = true;
    [SerializeField] bool enableMouseSway = true;
    [SerializeField] bool enableMouseTilt = true;
    [SerializeField] bool enableAutomaticBreath = true;
    [SerializeField] bool enableWASDRotation = true;
    [SerializeField] bool enableWASDPosition = true;
    [SerializeField] bool enableWASDWalk = true;
    [SerializeField] bool enableWeaponKnockback = true;

    public static bool activateWeaponKnockback = false;

    [Space]

    [Header("Mouse Position  ~~(__^·>  ⟟  ")]
    [SerializeField] float mouseIntensityPosition = 0.03f; // 0.015f
    [SerializeField] float mouseMaxIntensityPosition = 0.05f; // 0.03f
    [SerializeField] float mouseIntensitySmoothPosition = 4;
    [SerializeField] bool invertMousePositionX = true;
    [SerializeField] bool invertMousePositionY = true;

    [Header("Mouse Sway  ~~(__^·>  ")]
    [SerializeField] float mouseIntensitySway = 0.4f;
    [SerializeField] float mouseSmoothSway = 4;
    //[SerializeField] private float multiplier = 1;
    //[SerializeField] private float maxSway = 0;
    [SerializeField] bool invertMouseSwayX = false;
    [SerializeField] bool invertMouseSwayY = false;

    [Header("Mouse Tilt  ~~(__^·>  ")]
    [SerializeField] float mouseIntensityTilt = 4;
    [SerializeField] float mouseMaxIntensityTilt = 5;
    [SerializeField] float mouseSmoothIntensityTilt = 4;
    //[SerializeField] private float smoothRotation = 12
    [Space]
    // [X == False ==> Heavy gun]  [Y == False ==> Medium gun]  X or Y must be false to work properly
    [SerializeField] bool rotationX = false;
    [SerializeField] bool rotationY = true;
    [SerializeField] bool rotationZ = true;
    [Space]
    [SerializeField] bool invertMouseTiltX = false;
    [SerializeField] bool invertMouseTiltY = false;

    [Space]

    [Header("Automatic Breathe ༄")]
    [SerializeField] Transform weaponParent;
    [SerializeField] float breathForce = 1;          // 2
    //[SerializeField] float breathForceRotating = 2;  // 10
    //[SerializeField] float breathForceSprinting = 2; // 6
    Vector3 weaponParentOrigin;
    Vector3 targetWeaponBobPosition;

    [Space]

    [Header("WASD Rotation (:̲̅:̲̅:̲̅[̲̅  ]̲̅:̲̅:̲̅:̲̅) ꩜ 🌀 ")]
    [SerializeField] float WASDIntensityRotationX = 10;
    [SerializeField] float WASDIntensityRotationY = 3;
    [SerializeField] float WASDSmoothingRotation = 20;
    [SerializeField] bool invertWASDRotationX = true;
    [SerializeField] bool invertWASDRotationY = false;
    Vector3 newWeaponRotation;
    Vector3 newWeaponMovementRotation;
    Vector3 newWeaponMovementRotationVelocity;
    Vector3 targetWeaponMovementRotation;
    Vector3 targetWeaponMovementRotationVelocity;

    [Space]

    [Header("WASD Position (:̲̅:̲̅:̲̅[̲̅  ]̲̅:̲̅:̲̅:̲̅)  ⟟  ")]
    [SerializeField] float WASDIntensityPosX = 0.05f;
    [SerializeField] float WASDIntensityPosZ = 0.1f;
    [SerializeField] float WASDMaxIntensityPosX = 0.1f;
    [SerializeField] float WASDMaxIntensityPosZ = 0.2f;
    [SerializeField] float WASDSmoothIntensityPos = 3;
    [SerializeField] bool invertWASDPositionX = false;
    [SerializeField] bool invertWASDPositionZ = true;

    [Header("WASD Walking Movement (:̲̅:̲̅:̲̅[̲̅  ]̲̅:̲̅:̲̅:̲̅) 𓂽 ")]
    [SerializeField, Range(0, 10000f)] float HBStabilizationAmplitudeX = 0.15f;
    [SerializeField, Range(0, 30)] float HBStabilizationFrequencyX = 10.0f;
    [SerializeField, Range(0, 10000f)] float HBStabilizationAmplitudeY = 0.15f;
    [SerializeField, Range(0, 30)] float HBStabilizationFrequencyY = 10.0f;
    float HBStabilizedSpeed = 0;

    [SerializeField, Range(1, 2)] float sprintingFreqScale = 1.5f;
    float actualFreqX = 0;
    float actualFreqY = 0;

    float toggleSpeed = 3.0f;

    [Header("Shooting Knockback    ▄︻═デ┻┳── - - - - -")]



    [Header("Position")]

    [Header("Z [Front/Back] ")]
    [SerializeField] float knockbackIntensityZ = 2f;
    [SerializeField] float maxKnockbackZ = 5f;
    [SerializeField] float knockbackSmoothIntensityZ = 5f;
    [Header("Y [Up/Down] ")]
    [SerializeField] float knockbackIntensityY = 0.3f;
    [SerializeField] float maxKnockbackY = 0.5f;
    //[SerializeField] float knockbackSmoothIntensityY = 2.5f;

    [Header("Rotation")]

    [Header("X [Up/Down] ")]
    //[SerializeField] float knockbackRotationIntensityX = 5f;
    //[SerializeField] float maxKnockbackRotationX = 5f;
    //[SerializeField] float knockbackRotationSmoothIntensityX = 2.5f;


    Vector3 startPos;


    ////////////////////////////// New Procedural Recoil //////////////////////////////
    Vector3 currentRotationRecoil, targetRotationRecoil, targetPositionRecoil, currentPositionRecoil, initialGunPositionRecoil;

    [Header("Procedura Recoil")]
    [SerializeField] float recoilX;
    [SerializeField] float recoilY;
    [SerializeField] float recoilZ;

    [SerializeField] float kickBackZ;

    public float snappiness, returnAmount;
    //////////////////////////////                       //////////////////////////////


    // Internal privates
    Vector3 initialPosition;
    Quaternion originRotation;
    float idleCounter;

    /// Inputs
    Vector2 input;      // Total mouse input calculator
    Vector2 mouseInput; // Mouse movement calculator
    Vector2 WASDInput; // WASD Input
    Vector2 padInput;   // Gamepad mouse movement calculator

    #endregion

    #region - Awake | Start | Update | FixedUpdate | LateUpdate -

    void Awake() => GetReferences();
    void Start() => InitVariables();

    void Update()
    {
        if (!isMine) return; // If it's activated the movement physics work, if it's not, it doesn't (Basically made to save resouces with npcs that also have guns...)

        if (isMine && !stats.IsDead())
        {            
            InputCalculations(); /// Get mouse input!!!     

            /// Update Weapon Sway && Movement methods!
            if (enableMouseTilt)      UpdateMouseTilt();
            if (enableMouseSway)      UpdateMouseSway();
            if (enableWASDRotation)   UpdateWASDtRotation();
        }
    }
    void FixedUpdate()
    {
        if (!isMine) return;

        if (isMine && !stats.IsDead())
        {
            if (enableAutomaticBreath && (WASDInput.x == 0 && WASDInput.y == 0)) UpdateAutomaticBreath();
        }
    }
    void LateUpdate()
    {
        if (!isMine) return;

        if (isMine && !stats.IsDead())
        {
            if (enableMousePosition)  UpdateMousePosition();
            if (enableWASDPosition)   UpdateWASDPosition();
            if (enableWASDWalk)       UpdatePlayerWalk();
            if (activateWeaponKnockback) ShootingKnockback(); // Weapon
            //if (activateWeaponKnockback)
            //{
            //    Recoil();
            //    Back(); // Weapon
            //}
        }
    }

    #endregion

    #region - Logic -

    void InputCalculations()
    {
        mouseInput = playerInput.OnFoot.Look.ReadValue<Vector2>();  // Mouse
        padInput = playerInput.OnFootGamePad.Look.ReadValue<Vector2>(); // Gamepad
        input = mouseInput + padInput; // As Keyboard and Gamepad won't be used at the same time by the same player, I just add them together in the same variable!

        WASDInput = movementController.inputWASD;
    }

    #region WASD

    // WASD Input Rotation
    void UpdateWASDtRotation()
    {
        // Sway (Keyboard moving)
        targetWeaponMovementRotation.z = WASDIntensityRotationX * (invertWASDRotationX ? -WASDInput.x : WASDInput.x);
        targetWeaponMovementRotation.x = WASDIntensityRotationY * (invertWASDRotationY ? -WASDInput.y : WASDInput.y);

        targetWeaponMovementRotation = Vector3.SmoothDamp(targetWeaponMovementRotation, Vector3.zero, ref targetWeaponMovementRotationVelocity, Time.fixedDeltaTime * WASDSmoothingRotation);
        newWeaponMovementRotation = Vector3.SmoothDamp(newWeaponMovementRotation, targetWeaponMovementRotation, ref newWeaponMovementRotationVelocity, Time.fixedDeltaTime * WASDSmoothingRotation);

        // Apply the final rotation adding up every rotation calculated in the previous code lines
        //transform.localRotation = Quaternion.Euler(newWeaponRotation + newWeaponMovementRotation);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(newWeaponRotation + newWeaponMovementRotation) * originRotation, Time.fixedDeltaTime * WASDSmoothingRotation);
    }

    // WASD Input Position 
    void UpdateWASDPosition()
    {
        ///// Calculate target Position ==> [To change movement direction change "amount" sign!]
        float moveX = Mathf.Clamp((invertWASDPositionX ? -WASDInput.x : WASDInput.x) * WASDIntensityPosX, -WASDMaxIntensityPosX, WASDMaxIntensityPosX);
        float movez = Mathf.Clamp((invertWASDPositionZ ? -WASDInput.y : WASDInput.y) * WASDIntensityPosZ, -WASDMaxIntensityPosZ, WASDMaxIntensityPosZ);

        Vector3 finalPosition = new Vector3(moveX, 0, movez);

        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, Time.fixedDeltaTime * WASDSmoothIntensityPos);
    }

    #region Walking Effect

    void UpdatePlayerWalk()
    {
        CheckMotion();
    }

    void CheckMotion()
    {
        if (WASDInput.y > 0 || WASDInput.y < 0 || WASDInput.x > 0 || WASDInput.x < 0) // When walking
            HBStabilizedSpeed = 10;
        if (WASDInput.y == 0 && WASDInput.x == 0)
            HBStabilizedSpeed = 0;

        if (HBStabilizedSpeed < toggleSpeed) 
            return;
        if (!movementController.IsGrounded()) 
            return;

        CheckIfSprinting();
        PlayMotion(FootStepMotion());
    }

    void CheckIfSprinting()
    {
        if (MovementController.isSprinting)
        {
            actualFreqX = HBStabilizationFrequencyX * sprintingFreqScale;
            actualFreqY = HBStabilizationFrequencyY * sprintingFreqScale;
        }
        else
        {
            actualFreqX = HBStabilizationFrequencyX;
            actualFreqY = HBStabilizationFrequencyY;
        }
    }

    Vector3 FootStepMotion() // Camera HeadBob Movement
    {
        Vector3 pos = Vector3.zero;
        pos.x += Mathf.Cos(Time.time * actualFreqY / 2) * HBStabilizationAmplitudeY * 2;
        pos.y += Mathf.Sin(Time.time * actualFreqX) * HBStabilizationAmplitudeX;
        return pos;
    }
    void PlayMotion(Vector3 motion) => transform.localPosition += motion * Time.deltaTime;

    #endregion

    #endregion

    #region Mouse

    /// Move gun position
    void UpdateMousePosition()
    {
        ///// Calculate target Position ==> [To change movement direction change "amount" sign!]
        float moveX = Mathf.Clamp((invertMousePositionX ? -input.x : input.x) * mouseIntensityPosition, -mouseMaxIntensityPosition, mouseMaxIntensityPosition);
        float moveY = Mathf.Clamp((invertMousePositionY ? -input.y : input.y) * mouseIntensityPosition, -mouseMaxIntensityPosition, mouseMaxIntensityPosition);

        Vector3 finalPosition = new Vector3(moveX, moveY, 0);

        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, Time.smoothDeltaTime * mouseIntensitySmoothPosition);
    }

    /// Sway Movement (Rotation)
    void UpdateMouseSway()
    {
        //float mouseX = (invertMouseSwayX ? -input.x : input.x) * mouseIntensitySway;
        //float mouseY = (invertMouseSwayY ? -input.y : input.y) * mouseIntensitySway;

        //Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.up);
        //Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.right);

        //Quaternion targetRotation = rotationX * rotationY;

        //transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, mouseSmoothSway * Time.deltaTime);

        Quaternion rotationX = Quaternion.AngleAxis(mouseIntensitySway * (invertMouseSwayX ? -input.x : input.x), Vector3.up);
        Quaternion rotationY = Quaternion.AngleAxis(-mouseIntensitySway * (invertMouseSwayY ? -input.y : input.y), Vector3.right);
        Quaternion targetRotation = originRotation * rotationX * rotationY;

        ///// Apply rotation towards target 
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * mouseSmoothSway);
    }

    /// Tilt Sway Rotation Movement
    void UpdateMouseTilt()
    {
        ///// Calculate target rotation ==> [To change movement direction change "rotationAmount" sign!]
        float tiltX = Mathf.Clamp((invertMouseTiltX ? -input.x : input.x) * mouseIntensityTilt, -mouseMaxIntensityTilt, mouseMaxIntensityTilt);
        float tiltY = Mathf.Clamp((invertMouseTiltY ? -input.y : input.y) * mouseIntensityTilt, -mouseMaxIntensityTilt, mouseMaxIntensityTilt);

        Quaternion finalRotation = Quaternion.Euler(new Vector3
            (
            // [ -tiltX / -tiltY ] ==> Change rotation direction!
            //      [?] ==> If rotationX == true ==> it uses -tiltX amount inserted in script to rotate
            //                  If not, it uses 0 value ==> It doesn't work!
            rotationX == true   ?   -tiltX : 0f,
            rotationY == true   ?   tiltY : 0f,
            rotationZ == true   ?   tiltY : 0f
            ));

        transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRotation * originRotation, Time.smoothDeltaTime * mouseSmoothIntensityTilt);
    }

    #endregion

    #region Automatic [Breath]

    /// Breath Idle Bobbing
    void UpdateAutomaticBreath()
    {
        BreathCalculator(idleCounter, 0.025f, 0.025f);  // 0.025f, 0.025f
        idleCounter += Time.deltaTime;
        if (idleCounter > 6.3f)
            idleCounter = 0;

        weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponBobPosition, Time.smoothDeltaTime * breathForce);
    }
    void BreathCalculator(float p_z, float p_x_intensity, float p_y_intensity)
    {
        targetWeaponBobPosition = weaponParentOrigin + new Vector3(Mathf.Cos(p_z) * p_x_intensity, Mathf.Sin(p_z * 2) * p_y_intensity, 0);
    }

    #endregion

    #region Shooting Knockback

    Vector3 currentEulerAngles;
    Quaternion currentRotation;

    private void ShootingKnockback()
    {
        if(enableWeaponKnockback)
        {
            /// Position [0, y, z]
            float moveZ = Mathf.Clamp(knockbackIntensityZ, -maxKnockbackZ, maxKnockbackZ); // Z (Back)
            float moveY = Mathf.Clamp(knockbackIntensityY, -maxKnockbackY, maxKnockbackY); // Y (Up/Down)
            Vector3 finalPosition = new Vector3(0, moveY, -moveZ);
            transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition, Time.fixedDeltaTime * knockbackSmoothIntensityZ);

            ///// Rotation [0, y, 0] NOT WORKING!!!!
            //Quaternion rotateY = Quaternion.AngleAxis(knockbackRotationIntensityX, Vector3.up);
            ////float rotateY = Mathf.Clamp(knockbackRotationIntensityY, -maxKnockbackRotationY, maxKnockbackRotationY); // Y (Up/Down)
            //Quaternion targetRotation = originRotation * rotateY;
            ////Quaternion finalRotation = new Quaternion(0, rotateY, 0);
            //transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.smoothDeltaTime * knockbackRotationSmoothIntensityX);




            //targetPositionRecoil -= new Vector3(0, 0, kickBackZ);
            //targetPositionRecoil += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));

            //targetPositionRecoil = Vector3.Lerp(targetPositionRecoil, initialGunPositionRecoil, Time.deltaTime * returnAmount);
            //currentPositionRecoil = Vector3.Lerp(currentPositionRecoil, targetPositionRecoil, Time.fixedDeltaTime * snappiness);
            //transform.localPosition = currentPositionRecoil;



            //finalPosition = Quaternion.Euler()
            //transform.rotation = Quaternion.Slerp(transform.rotation, finalPosition, knockbackSmoothIntensityZ * Time.smoothDeltaTime)



            ////modifying the Vector3, based on input multiplied by speed and time
            //currentEulerAngles += new Vector3(-knockbackRotationIntensityX, 0, 0) * Time.smoothDeltaTime * knockbackRotationSmoothIntensityX;

            ////moving the value of the Vector3 into Quanternion.eulerAngle format
            //currentRotation.eulerAngles = currentEulerAngles;

            ////apply the Quaternion.eulerAngles change to the gameObject
            //transform.localRotation = currentRotation;


            activateWeaponKnockback = false;
        }
    }

    /// Procedural Recoil
    void UpdatePos()
    {
        targetRotationRecoil = Vector3.Lerp(targetRotationRecoil, Vector3.zero, Time.deltaTime * returnAmount);
        currentRotationRecoil = Vector3.Slerp(currentRotationRecoil, targetRotationRecoil, Time.deltaTime * snappiness);
        transform.localRotation = Quaternion.Euler(currentRotationRecoil);
    }
    void Recoil()
    {
        targetPositionRecoil -= new Vector3(0, 0, kickBackZ);
        targetPositionRecoil += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }

    void Back()
    {
        targetPositionRecoil = Vector3.Lerp(targetPositionRecoil, initialGunPositionRecoil, Time.deltaTime * returnAmount);
        currentPositionRecoil = Vector3.Lerp(currentPositionRecoil, targetPositionRecoil, Time.fixedDeltaTime * snappiness);
        transform.localPosition = currentPositionRecoil;
    }

    #endregion

    #endregion

    #region - Reference && Initialize -

    void GetReferences()
    {
        stats = GetComponentInParent<PlayerStats>();
        movementController = GetComponentInParent<MovementController>();
    }

    void InitVariables()
    {
        playerInput = new PlayerInput();
        if (!playerInput.asset.enabled) playerInput.Enable(); // Enable input system!

        initialGunPositionRecoil = transform.localPosition;

        initialPosition = transform.localPosition;
        originRotation = transform.localRotation; // Get the rotation of the object before the game starts to make it appear rotated
        weaponParentOrigin = weaponParent.transform.localPosition;
        newWeaponRotation = transform.localRotation.eulerAngles;
        currentEulerAngles = transform.localRotation.eulerAngles;
    }

    #endregion
}
