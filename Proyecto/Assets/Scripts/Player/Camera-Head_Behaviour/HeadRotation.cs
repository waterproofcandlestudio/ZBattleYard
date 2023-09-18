using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class HeadRotation : MonoBehaviour
{
    MovementController movementController;

    [SerializeField] bool enableWASDRotation = true;      // Uncompatible w "StabilizedHeadBob"

    [Header("WASD Rotation")]
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

    Vector2 WASDInput; // WASD Input

    Quaternion originRotation;

    void Awake() => GetReferences();
    void Start() => InitVariables();

    void Update() => InputCalculations();

    void LateUpdate()
    {
        /// WASD
        if (enableWASDRotation) UpdateWASDtRotation();
    }

    void InputCalculations()
    {
        WASDInput = movementController.inputWASD;
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
        //transform.localRotation = Quaternion.Euler(newCamRotation + newCamMovementRotation);
        this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation, Quaternion.Euler(newCamRotation + newCamMovementRotation) * originRotation, Time.smoothDeltaTime * WASDSmoothingRotation);
    }

    void GetReferences()
    {
        movementController = GetComponentInParent<MovementController>();
    }
    void InitVariables()
    {
        originRotation = transform.localRotation; // Get the rotation of the object before the game starts to make it appear rotated
    }
}
