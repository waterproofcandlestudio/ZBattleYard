using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceChange : MonoBehaviour
{
    //public static DeviceChange instance;

    //void Awake() => instance = this;

    UnityEngine.InputSystem.PlayerInput _controls;
    public static ControlDeviceType currentControlDevice;
    public enum ControlDeviceType
    {
        KeyboardAndMouse,
        Gamepad,
    }
    void Start()
    {
        _controls = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        _controls.onControlsChanged += OnControlsChanged;
    }
    private void OnControlsChanged(UnityEngine.InputSystem.PlayerInput obj)
    {
        if (obj.currentControlScheme == "Gamepad")
        {
            if (currentControlDevice != ControlDeviceType.Gamepad)
            {
                currentControlDevice = ControlDeviceType.Gamepad;
                // Send Event
                // EventHandler.ExecuteEvent("DeviceChanged", currentControlDevice);
            }
        }
        else
        {
            if (currentControlDevice != ControlDeviceType.KeyboardAndMouse)
            {
                currentControlDevice = ControlDeviceType.KeyboardAndMouse;
                // Send Event
                // EventHandler.ExecuteEvent("DeviceChanged", currentControlDevice);
            }
        }
    }
}
