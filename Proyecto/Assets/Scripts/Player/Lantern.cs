using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lantern : MonoBehaviour
{
    PlayerInput playerInput;

    [Header("Lantern")]
    bool lanternOn = false;

    void Awake() => Initialize();

    /*
    =============
    ToggleLantern -- Lantern

    Called by Awake.

    Activates &  desactivates lantern.
    =============
    */
    void ToggleLantern()
    {
        if (lanternOn == false)
        {
            lanternOn = true;
            gameObject.SetActive(true);
        }
        else
        {
            lanternOn = false;
            gameObject.SetActive(false);
        }
    }

    void Initialize()
    {
        playerInput = new PlayerInput(); // Initialize Player input to use new input system
        playerInput.Enable(); // Enable input system!
        playerInput.OnFoot.Lantern.performed += e => ToggleLantern();     // Lantern

        gameObject.SetActive(false); // Lantern is active when I start game so I can use the script, but after initializing I desactivate Lantern to start game with it off but being able to use now the script!
    }
    void OnDisable()
    {
        playerInput.Disable();
    }
}
