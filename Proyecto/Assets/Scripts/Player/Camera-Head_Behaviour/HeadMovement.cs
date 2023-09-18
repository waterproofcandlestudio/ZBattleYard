using UnityEngine;

public class HeadMovement : MonoBehaviour
{
    PlayerInput playerInput;
    MovementController movementController;

    [Header("Activation                               「✔」 ")]
    [SerializeField] bool enableWASDRotation = true;      // Uncompatible w "StabilizedHeadBob"
    [SerializeField] bool enableWASDPosition = true;
    [SerializeField] bool enableMousePosition = false;     // Not working as properly as expected
    [SerializeField] bool enableHeadBob = true;
    [SerializeField] bool enableHeadBob2 = false; // Uncompatible w "WASDRotation"
    [SerializeField] bool enableFOV = true;
    [SerializeField] bool enableShake = true;

    [Header("WASD Rotation")]
    [SerializeField] Transform headRotationTransform;
    [SerializeField] float WASDIntensityRotationX = 1;
    [SerializeField] float WASDIntensityRotationY = 1;
    [SerializeField] float WASDSmoothingRotation = 0.2f;
    [SerializeField] bool invertWASDRotationX = true;
    [SerializeField] bool invertWASDRotationY = false;
    Vector3 newCamRotation;
    Vector3 newCamMovementRotation;
    Vector3 newCamMovementRotationVelocity;
    Vector3 targetCamRotation;
    Vector3 targetCamMovementRotationVelocity;

    [Header("WASD Position")]
    [SerializeField] float WASDIntensityPosX = 0.05f;
    [SerializeField] float WASDIntensityPosZ = 0.05f;
    [SerializeField] float WASDMaxIntensityPosX = 0.1f;
    [SerializeField] float WASDMaxIntensityPosZ = 0.1f;
    [SerializeField] float WASDSmoothIntensityPos = 3f;
    [SerializeField] bool invertWASDPositionX = false;
    [SerializeField] bool invertWASDPositionZ = true;

    [Header("Mouse Position")]
    [SerializeField] float mouseIntensityPosition = 0.03f; // 0.015f
    [SerializeField] float mouseMaxIntensityPosition = 0.05f; // 0.03f
    [SerializeField] float mouseIntensitySmoothPosition = 4f;
    [SerializeField] bool invertMousePositionX = true;
    [SerializeField] bool invertMousePositionY = true;

    [Header("Headbob 1")]
    [SerializeField] Transform cameraHolder;
    [HideInInspector] float currentBobFrequency;
    [SerializeField] float walkingBobFrequency = 5f;
    [SerializeField] float runningBobFrequency = 7.5f;
    [SerializeField] float bobHorizontalAmplitude = 0.025f;
    [SerializeField] float bobVerticalAmplitude = 0.025f;
    [Range(0, 1)][SerializeField] float headBobSmoothing = 0.1f;
    [SerializeField] bool focusOnTarget = true;
    public static bool isWalkingBobbing; // State
    float walkingTime;
    Vector3 targetCameraPosition;
    new Transform camera;

    [Header("FOV")]
    [SerializeField] Camera camFOV;
    [SerializeField] float FovIncreaseWalk = 5;
    [SerializeField] float FovIncreaseRun = 10;
    [SerializeField] float smoothInterpolation = 2f;

    [Header("HeadBob 2")]
    [SerializeField, Range(0, 0.1f)] float HBStabilizationAmplitude = 0.0025f;
    [SerializeField, Range(0, 30)] float HBStabilizationFrequency = 10.0f;
    float HBStabilizedSpeed = 0;
    float toggleSpeed = 3.0f;
    Vector3 startPos;

    [Header("Shake")]
    [SerializeField] ShakeTransformEventData dataRotationRunning;
    ShakeTransform shakeHolderTransform;

    // Internal privates
    Vector3 initialPosition;
    Quaternion originRotation;
    /// Inputs
    Vector2 input;      // Total mouse input calculator
    Vector2 mouseInput; // Mouse movement calculator
    Vector2 padInput;   // Gamepad mouse movement calculator
    Vector2 WASDInput; // WASD Input

    void Awake() => GetReferences();
    void Start() => InitVariables();
    void Update() => InputCalculations();
    void LateUpdate()
    {
        /// WASD
        if (enableWASDRotation) UpdateWASDtRotation();
        if (enableWASDPosition) UpdateWASDPosition(); 

        /// Mouse
        if (enableMousePosition) UpdateMousePosition();

        /// Extra
        if (enableHeadBob) UpdateBobbing();
        if (enableHeadBob2) UpdateStabilizedBobbing();

        if (enableFOV) DynamicFOV();

        if (enableShake) MovementShake();
    }

    #region - Logic -
    void InputCalculations()
    {
        /// New input system way! 
        mouseInput = playerInput.OnFoot.Look.ReadValue<Vector2>();  // Mouse
        padInput = playerInput.OnFootGamePad.Look.ReadValue<Vector2>(); // Gamepad
        input = mouseInput + padInput; // As Keyboard and Gamepad won't be used at the same time by the same player, I just add them together in the same variable!

        WASDInput = movementController.inputWASD;
    }

    /// WASD
    #region WASD

    /// WASD Input Position 
    void UpdateWASDPosition()
    {
        ///// Calculate target Position ==> [To change movement direction change "amount" sign!]
        float moveX = Mathf.Clamp((invertWASDPositionX ? -WASDInput.x : WASDInput.x) * WASDIntensityPosX, -WASDMaxIntensityPosX, WASDMaxIntensityPosX);
        float movez = Mathf.Clamp((invertWASDPositionZ ? -WASDInput.y : WASDInput.y) * WASDIntensityPosZ, -WASDMaxIntensityPosZ, WASDMaxIntensityPosZ);

        Vector3 finalPosition = new Vector3(moveX, 0, movez);

        camera.localPosition = Vector3.Lerp(camera.localPosition, finalPosition + initialPosition, Time.deltaTime * WASDSmoothIntensityPos);
    }
    /// WASD Input Rotation
    void UpdateWASDtRotation()
    {
        // Sway (Keyboard moving)
        targetCamRotation.z = WASDIntensityRotationX * (invertWASDRotationX ? -WASDInput.x : WASDInput.x);
        targetCamRotation.x = WASDIntensityRotationY * (invertWASDRotationY ? -WASDInput.y : WASDInput.y);

        targetCamRotation = Vector3.SmoothDamp(targetCamRotation, Vector3.zero, ref targetCamMovementRotationVelocity, Time.smoothDeltaTime * WASDSmoothingRotation);
        newCamMovementRotation = Vector3.SmoothDamp(newCamMovementRotation, targetCamRotation, ref newCamMovementRotationVelocity, Time.smoothDeltaTime * WASDSmoothingRotation);

        // Apply the final rotation adding up every rotation calculated in the previous code lines
        //camera.localRotation = Quaternion.Euler(newCamRotation + newCamMovementRotation);
        camera.localRotation = Quaternion.Slerp(camera.localRotation, Quaternion.Euler(newCamRotation + newCamMovementRotation) * originRotation, Time.smoothDeltaTime * WASDSmoothingRotation);
    }


    #endregion

    /// MOUSE
    #region Mouse

    /// Move gun position
    void UpdateMousePosition()
    {
        ///// Calculate target Position ==> [To change movement direction change "amount" sign!]
        float moveX = Mathf.Clamp((invertMousePositionX ? -input.x : input.x) * mouseIntensityPosition, -mouseMaxIntensityPosition, mouseMaxIntensityPosition);
        float moveY = Mathf.Clamp((invertMousePositionY ? -input.y : input.y) * mouseIntensityPosition, -mouseMaxIntensityPosition, mouseMaxIntensityPosition);

        Vector3 finalPosition = new Vector3(moveX, moveY, 0);

        camera.localPosition = Vector3.Lerp(camera.localPosition, finalPosition + initialPosition, Time.deltaTime * mouseIntensitySmoothPosition);
    }

    #endregion

    /// EXTRA
    
    #region HeadBob

    void CalculateCurrentHeadBobValue() // Calculates "currentBobFrequency" value to use in "CalculateHeadBobOffset(float t)"
    {
        if (MovementController.isSprinting)
        {
            currentBobFrequency = Mathf.SmoothDamp(currentBobFrequency, runningBobFrequency, ref currentBobFrequency, Time.smoothDeltaTime);
            return;
        }
        if (!MovementController.isSprinting)
        {
            currentBobFrequency = Mathf.SmoothDamp(currentBobFrequency, walkingBobFrequency, ref currentBobFrequency, Time.smoothDeltaTime);
            return;
        }
        else
        {
            currentBobFrequency = Mathf.SmoothDamp(currentBobFrequency, 0, ref currentBobFrequency, Time.smoothDeltaTime);
            return;
        }
    }
    void IsWalkingBobbingVerification() // Verifies if "WASDinput > 0" to start the method or not
    {
        if (WASDInput.y > 0 || WASDInput.y < 0 || WASDInput.x > 0 || WASDInput.x < 0)
        {
            isWalkingBobbing = true;
            return;
        }
        else
        {
            isWalkingBobbing = false;
            return;
        }
    }

    /*
    =============
    CalculateHeadBobOffset -- Head Bobbing

    Called by Update.

    Returns player position transformation with vector movement
    =============
    */
    void UpdateBobbing()
    {
        IsWalkingBobbingVerification(); 

        if (!isWalkingBobbing) walkingTime = 0;     // Set time and offset to 0
        else walkingTime += Time.deltaTime;

        CalculateCurrentHeadBobValue();

        targetCameraPosition = cameraHolder.position + CalculateHeadBobOffset(walkingTime); // Calculate the camera's target position
        
        camera.position = Vector3.Lerp(camera.position, targetCameraPosition, Time.smoothDeltaTime * headBobSmoothing);    // Interpolate position
        if ((camera.position - targetCameraPosition).magnitude <= 0.001) camera.position = targetCameraPosition;    // Snap to position if it is close enough

        if (focusOnTarget)
            camera.LookAt(FocusTarget());
    }
    Vector3 CalculateHeadBobOffset(float t)
    {
        float horizontalOffset;
        float verticalOffset;
        Vector3 offset = Vector3.zero;

        if (t > 0)
        {
            // Calculate offsets
            horizontalOffset = Mathf.Sin(t * currentBobFrequency) * bobHorizontalAmplitude;
            verticalOffset = Mathf.Cos(t * currentBobFrequency * 2) * bobVerticalAmplitude;

            offset = cameraHolder.right * horizontalOffset + cameraHolder.up * verticalOffset; // Combine offsets relative to the head's position and calculate the camera's target position
        }

        return offset;
    }

    #endregion

    #region FOV

    void DynamicFOV()
    {
        int currentFov = PlayerPrefs.GetInt("fov");

        if (MovementController.isSprinting) // When sprinting
        {
            camFOV.fieldOfView = Mathf.Lerp(currentFov, currentFov + FovIncreaseRun, smoothInterpolation * Time.deltaTime);
            return;
        }
        if (WASDInput.y > 0 || WASDInput.y < 0 || WASDInput.x > 0 || WASDInput.x < 0) // When walking
        {
            camFOV.fieldOfView = Mathf.Lerp(currentFov, currentFov + FovIncreaseWalk, smoothInterpolation * Time.deltaTime);
            return;
        }
        else // When player is static
            camFOV.fieldOfView = Mathf.Lerp(currentFov, currentFov, smoothInterpolation * Time.deltaTime);
    }

    #endregion

    #region HeadBob 2 - Stabilization

    void UpdateStabilizedBobbing()
    {
        CheckMotion();
        ResetPosition();
        camera.LookAt(FocusTarget());
    }

    void CheckMotion()
    {
        if (WASDInput.y > 0 || WASDInput.y < 0 || WASDInput.x > 0 || WASDInput.x < 0) // When walking
            HBStabilizedSpeed = 10;
        if (WASDInput.y == 0 && WASDInput.x == 0)
            HBStabilizedSpeed = 0;

        if (HBStabilizedSpeed < toggleSpeed)        return;
        if (!movementController.IsGrounded())    return;

        PlayMotion(FootStepMotion());
    }

    void PlayMotion(Vector3 motion) => camera.localPosition += motion;

    Vector3 FootStepMotion() // Camera HeadBob Movement
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * HBStabilizationFrequency) * HBStabilizationAmplitude;
        return pos;
    }

    void ResetPosition()
    {
        if (camera.localPosition == startPos)   return;
        camera.localPosition = Vector3.Lerp(camera.localPosition, startPos, 1 * Time.deltaTime);
    }

    #endregion

    Vector3 FocusTarget()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + cameraHolder.localPosition.y, transform.position.z);
        pos += cameraHolder.forward * 15.0f;
        return pos;
    }

    #region Moving Shake (Walking&running)

    void MovementShake()
    {
        if (MovementController.isSprinting)
            shakeHolderTransform.AddShakeEvent(dataRotationRunning); // Shake camera rotation
    }

    #endregion

    #endregion

    void GetReferences()
    {
        camera = Camera.main.transform;
        shakeHolderTransform = GetComponentInParent<ShakeTransform>();
        movementController = GetComponentInParent<MovementController>();
    }
    void InitVariables()
    {
        playerInput = new PlayerInput();
        playerInput.Enable();

        startPos = camera.localPosition;
        camFOV.fieldOfView = PlayerPrefs.GetInt("fov");

        initialPosition = transform.localPosition;
        originRotation = transform.localRotation; // Get the rotation of the object before the game starts to make it appear rotated
    }
    void OnDisable()
    {
        playerInput.Disable();
    }
}
