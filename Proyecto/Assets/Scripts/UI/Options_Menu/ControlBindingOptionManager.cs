using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ControlBindingOptionManager : MonoBehaviour
{
    PlayerInput playerInput;
    [SerializeField] InputActionReference actionToRemap;
    InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    [SerializeField] Slider sensivitySlider;  // Sensibilidad (solo en el menú, tmb toco esto ingame en el movimiento d la cám ("FPS")

    // Input game control Buttons
    [SerializeField] Button menu_Button;
    [SerializeField] Button moveForward_Button;
    [SerializeField] Button moveLeft_Button;
    [SerializeField] Button moveBack_Button;
    [SerializeField] Button moveRight_Button;
    [SerializeField] Button sprint_Button;
    [SerializeField] Button lantern_Button;
    [SerializeField] Button shoot_Button;
    [SerializeField] Button reload_Button;
    [SerializeField] Button melee_Button;
    [SerializeField] Button grenade_Button;
    [SerializeField] Button interact_Button;
    [SerializeField] Button crouch_Button;
    [SerializeField] Button prone_Button;
    [SerializeField] Button seeStats_Button;
    TextMeshProUGUI sensivitySlider_Button_Text;
    TextMeshProUGUI menu_Button_Text;
    TextMeshProUGUI moveForward_Button_Text;
    TextMeshProUGUI moveLeft_Button_Text;
    TextMeshProUGUI moveBack_Button_Text;
    TextMeshProUGUI moveRight_Button_Text;
    TextMeshProUGUI sprint_Button_Text;
    TextMeshProUGUI lantern_Button_Text;
    TextMeshProUGUI shoot_Button_Text;
    TextMeshProUGUI reload_Button_Text;
    TextMeshProUGUI melee_Button_Text;
    TextMeshProUGUI grenade_Button_Text;
    TextMeshProUGUI interact_Button_Text;
    TextMeshProUGUI crouch_Button_Text;
    TextMeshProUGUI prone_Button_Text;
    TextMeshProUGUI seeStats_Button_Text;

    float sensivity_default = 2;

    void Awake()
    {
        GetReferences();
    }
    void Start()
    {
        InitializeVariables();
        //SetSensivity(PlayerPrefs.GetFloat("sensivity"));
    }

    public void SetSensivity(float value)
    {
        value = sensivitySlider.value;
        PlayerPrefs.SetFloat("sensivity", value);
        sensivitySlider_Button_Text.text = PlayerPrefs.GetFloat("sensivity").ToString("F2");
    }

    public void StartRebinding()
    {
        //actionToRemap = inputActionAsset.FindActionMap("Player").FindAction("Look");
        EventSystem.current.SetSelectedGameObject(null);
        menu_Button_Text.text = "Press Any Button";    //
        actionToRemap.action.Disable();

        rebindingOperation = actionToRemap.action.PerformInteractiveRebinding()

            .OnMatchWaitForAnother(0.1f)

            .OnComplete
            (
            operation =>
            {
                //menu_Button_Text.text = InputControlPath.ToHumanReadableString(actionToRemap.action.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
                //menu_Button_Text.text = InputControlPath.ToHumanReadableString(actionToRemap.action.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
                rebindingOperation.Dispose();
                actionToRemap.action.Enable();
            }
            )
            .Start();

        InitializeBindingButtons_OptionsMenu();
    }

    public void Rebind_Shoot()
    {
        StartRebinding();
        ////playerInput.OnFoot.Menu.performed += e => ActivateIngameMenu();   // Jump
        //playerInput.OnFoot.Menu.PerformInteractiveRebinding();
        //InitializeBindingButtons_OptionsMenu();
    }


    void GetReferences()
    {
        sensivitySlider_Button_Text = sensivitySlider.GetComponentInChildren<TextMeshProUGUI>();
        menu_Button_Text = menu_Button.GetComponentInChildren<TextMeshProUGUI>();
        moveForward_Button_Text = moveForward_Button.GetComponentInChildren<TextMeshProUGUI>();
        moveLeft_Button_Text = moveLeft_Button.GetComponentInChildren<TextMeshProUGUI>();
        moveBack_Button_Text = moveBack_Button.GetComponentInChildren<TextMeshProUGUI>();
        moveRight_Button_Text = moveRight_Button.GetComponentInChildren<TextMeshProUGUI>();
        sprint_Button_Text = sprint_Button.GetComponentInChildren<TextMeshProUGUI>();
        lantern_Button_Text = lantern_Button.GetComponentInChildren<TextMeshProUGUI>();
        shoot_Button_Text = shoot_Button.GetComponentInChildren<TextMeshProUGUI>();
        reload_Button_Text = reload_Button.GetComponentInChildren<TextMeshProUGUI>();
        melee_Button_Text = melee_Button.GetComponentInChildren<TextMeshProUGUI>();
        grenade_Button_Text = grenade_Button.GetComponentInChildren<TextMeshProUGUI>();
        interact_Button_Text = interact_Button.GetComponentInChildren<TextMeshProUGUI>();
        crouch_Button_Text = crouch_Button.GetComponentInChildren<TextMeshProUGUI>();
        prone_Button_Text = prone_Button.GetComponentInChildren<TextMeshProUGUI>();
        seeStats_Button_Text = seeStats_Button.GetComponentInChildren<TextMeshProUGUI>();
    }
    void InitializeVariables()
    {
        playerInput = new PlayerInput(); // Initialize Player input to use new input system
        playerInput.Enable(); // Enable input system!

        InitializeBindingButtons_OptionsMenu();
        LoadControlsSettings();
        InitializeSliderPercentajes_Text();
    }
    void InitializeBindingButtons_OptionsMenu()
    {
        menu_Button_Text.text = playerInput.OnFoot.Menu.GetBindingDisplayString();
        moveForward_Button_Text.text = playerInput.OnFoot.Movement.GetBindingDisplayString();
        moveLeft_Button_Text.text = playerInput.OnFoot.Movement.GetBindingDisplayString();
        moveBack_Button_Text.text = playerInput.OnFoot.Movement.GetBindingDisplayString();
        moveRight_Button_Text.text = playerInput.OnFoot.Movement.GetBindingDisplayString();
        sprint_Button_Text.text = playerInput.OnFoot.Sprint.GetBindingDisplayString();
        lantern_Button_Text.text = playerInput.OnFoot.Lantern.GetBindingDisplayString();
        shoot_Button_Text.text = playerInput.Weapon.Fire1Pressed.GetBindingDisplayString();
        reload_Button_Text.text = playerInput.Weapon.Reload.GetBindingDisplayString();
        melee_Button_Text.text = playerInput.Weapon.Melee.GetBindingDisplayString();
        grenade_Button_Text.text = playerInput.Weapon.Grenade.GetBindingDisplayString();
        interact_Button_Text.text = playerInput.OnFoot.Interact.GetBindingDisplayString();
        crouch_Button_Text.text = playerInput.OnFoot.Crouch.GetBindingDisplayString();
        prone_Button_Text.text = playerInput.OnFoot.Prone.GetBindingDisplayString();
        seeStats_Button_Text.text = playerInput.OnFoot.SeeStats.GetBindingDisplayString();
    }

    void LoadControlsSettings()         // Paso los valores de las opciones al "PlayerPrefs" para q no se pierdan con el cambio de escena
    {
        if (PlayerPrefs.HasKey("sensivity"))
            sensivitySlider.value = PlayerPrefs.GetFloat("sensivity");
        else
        {
            sensivitySlider.value = sensivity_default;
            PlayerPrefs.SetFloat("sensivity", sensivity_default);
        }
    }

    void InitializeSliderPercentajes_Text()
    {
        sensivitySlider_Button_Text.text = PlayerPrefs.GetFloat("sensivity").ToString("F2");
    }

    public void Reset_ControlOptions()
    {
        sensivitySlider.value = sensivity_default;
        PlayerPrefs.SetFloat("sensivity", sensivity_default);
    }
}
