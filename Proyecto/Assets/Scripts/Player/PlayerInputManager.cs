using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public PlayerInput playerInput;

    void Start()
    {
        playerInput = new PlayerInput(); // Initialize Player input to use new input system

        if (!playerInput.asset.enabled)     playerInput.Enable(); // Enable input system!
    }

    void OnDisable()
    {
        playerInput.Disable();
    }
    void OnDestroy()
    {
        playerInput.Disable();
    }
}
