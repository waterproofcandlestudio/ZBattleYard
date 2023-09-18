using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject creditsMenu;
    [SerializeField] GameObject loadingScenePanel;

    void Start()
    {
        ActivateMainMenu(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ActivateMainMenu(bool state)
    {
        mainMenu.SetActive(state);
        optionsMenu.SetActive(!state);
    }
    public void ActivateCreditsMenu(bool state)
    {
        mainMenu.SetActive(state);
        creditsMenu.SetActive(!state);
    }

    public void Play()
    {
        loadingScenePanel.SetActive(true);
        mainMenu.SetActive(false);
        SceneManager.LoadSceneAsync(1);
    }
    public void Quit() => Application.Quit();
}
