using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    PlayerInput playerInput;
    PlayerStats stats;

    public Transform cameraHolder;

    [Header("View Settings")]
    [SerializeField] float viewClampYMin = -80;
    [SerializeField] float viewClampYMax = 80;

    public float aimingSensitivityEffector;

    public bool viewXInverted;
    public bool viewYInverted;

    [Header("Stance Settings")]
    public float cameraHeight;
    public float cameraHeightVelocity;

    float xRot;

    Vector3 moveDirection;
    public static bool cameraLocked;

    void Awake() => GetReferences();
    void Start() => InitializeVariables();
    void Update()
    {
        if (cameraLocked)
            return;
        CalculateView();
    }

    /// <summary>
    ///     Calculates view depending on used device
    /// </summary>
    void CalculateView()
    {
        CalculateViewMouse(playerInput.OnFoot.Look.ReadValue<Vector2>(), 0.1f); // Mouse
        CalculateViewMouse(playerInput.OnFootGamePad.Look.ReadValue<Vector2>(), 1); // Gamepad

        //if (Mouse.current.IsActuated())  // If u are using Keyboard, show interact key binded by keyboard control
        //{
        //    CalculateViewMouse(playerInput.OnFoot.Look.ReadValue<Vector2>(), 0.1f); // Mouse
        //    return;
        //}
        //if (Gamepad.current.IsActuated())   // If u are using Gamepad, show interact key binded by Gamepad control
        //{
        //    CalculateViewMouse(playerInput.OnFootGamePad.Look.ReadValue<Vector2>(), 10); // Gamepad
        //}
    }

    /// <summary>
    /// 
    ///     CalculateViewMouse && CalculateViewGamePad -- Mouse && GamePad view camera
    ///     
    ///     Called by Update.
    ///     
    ///     Returns player view camera rotation.
    ///     It works with 2 types of movement:
    ///     Rotate the entire player on Y axis to look left or right.
    ///     Rotate the camera on X axis to look up and down.
    ///     
    ///     It also considers if player is aiming or not to reduce speed!
    ///     
    ///     Compatible with gamepad via different methods and sensivities
    ///     (gamePad is the same sensivity X2)
    /// 
    /// </summary>
    /// <param name="input"></param>
    void CalculateViewMouse(Vector2 input, float sensivityMultiplier)
    {
        float mouseX = input.x * PlayerPrefs.GetFloat("sensivity") * sensivityMultiplier;
        float mouseY = input.y * PlayerPrefs.GetFloat("sensivity") * sensivityMultiplier;

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, viewClampYMin, viewClampYMax);

        if (!stats.IsDead()) // || UIManager.isPaused
        {
            cameraHolder.localRotation = Quaternion.Euler(new Vector3(xRot, 0, 0));
            transform.Rotate(new Vector3(0, mouseX, 0));
        }
        else if (Cursor.lockState == CursorLockMode.Locked)
            UnlockCursor();
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void GetReferences()
    {
        stats = GetComponent<PlayerStats>();
    }
    void InitializeVariables()
    {
        playerInput = new PlayerInput(); // Initialize Player input to use new input system
        if (!playerInput.asset.enabled) playerInput.Enable(); // Enable input system!

        cameraLocked = false;
        LockCursor();
    }

    void OnDisable()
    {
        playerInput.Disable();
    }
}
