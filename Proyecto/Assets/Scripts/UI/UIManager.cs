using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    PlayerInput playerInput;
    CameraController cameraController;
    PlayerStats stats = null;
    [SerializeField] MusicManager musicManager;
    [SerializeField] UI_AudioManager ui_AudioManager;

    GameObject playerObject;

    bool isPaused = false;

    [SerializeField] GameObject hudCanvas = null;
    [SerializeField] GameObject pauseCanvas = null;
    [SerializeField] GameObject pauseButtonsCanvas = null;
    [SerializeField] GameObject optionsCanvas = null;
    [SerializeField] GameObject endCanvas = null;
    [SerializeField] GameObject loadingScreenPanel;

    void Awake()    => GetReferences();
    void Start()    => InitializeVariables();
    //void Update()   => Time.timeScale = isPaused ? 0 : 1;


    public void ActivateIngameMenu()
    {
        if (!stats.IsDead())
        {
            if (!isPaused)
            {
                SetActivePause(true);
                Time.timeScale = 0f;
                CameraController.cameraLocked = true;
            }
            else if (isPaused)
            {
                SetActivePause(false);
                Time.timeScale = 1f;
                CameraController.cameraLocked = false;
            }
        }
    }

    public void ActivateGame()
    {
        SetActivePause(false);
        Time.timeScale = 1f;
        CameraController.cameraLocked = false;
    }

    void SmoothlyChangeTime(float initialTime, float finalTime)
    {
        Time.timeScale = Mathf.Lerp(initialTime, finalTime, Time.smoothDeltaTime);
    }

    public void SetActiveHud(bool state)    // Playing game
    {
        hudCanvas.SetActive(state);
        endCanvas.SetActive(!state);
        optionsCanvas.SetActive(!state);

        if (!stats.IsDead())        pauseCanvas.SetActive(!state);
    }

    public void SetActivePause(bool state)  // Pausing ingame
    {
        hudCanvas.SetActive(!state);
        pauseCanvas.SetActive(state);
        pauseButtonsCanvas.SetActive(state);
        optionsCanvas.SetActive(!state);

        if (state)  cameraController.UnlockCursor();
        else        cameraController.LockCursor();

        isPaused = state;
        ui_AudioManager.Play_EnterMenu_Sound();
        musicManager.LowPassFilter_SetActive(state); // Audio music plug effect
    }
    public void SetActiveOptionsMenu(bool state)  // Pausing ingame
    {
        pauseButtonsCanvas.SetActive(false);
        optionsCanvas.SetActive(true);
    }

    public bool GameIsPaused() => isPaused;
    public void Restart()
    {
        pauseCanvas.SetActive(false);
        loadingScreenPanel.SetActive(true);
        playerInput.Disable(); 
        playerObject.SetActive(false);
        SceneManager.LoadSceneAsync(1);
        Time.timeScale = 1f;
        playerObject.SetActive(true);
    }
    public void MainMenu()
    {
        pauseCanvas.SetActive(false);
        loadingScreenPanel.SetActive(true);
        playerInput.Disable(); 
        playerObject.SetActive(false);
        SceneManager.LoadSceneAsync(0);
        Time.timeScale = 1f;
        playerObject.SetActive(true);
    }
    public void Quit() => Application.Quit();

    void GetReferences()
    {
        stats = GetComponent<PlayerStats>();
        cameraController = GetComponent<CameraController>();
    }
    void InitializeVariables()
    {
        playerInput = new PlayerInput(); // Initialize Player input to use new input system
        playerInput.Enable(); // Enable input system!
        playerInput.OnFoot.Menu.performed += e => ActivateIngameMenu();   // Jump

        playerObject = gameObject;
        SetActiveHud(true);
    }
    void OnDisable()
    {
        playerInput.Disable();
    }
}
