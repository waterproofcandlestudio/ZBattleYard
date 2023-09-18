using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GeneralOptionsMenuManager : MonoBehaviour
{
    [SerializeField] GameObject graphicsCanvas = null;
    [SerializeField] GameObject controlsCanvas = null;
    [SerializeField] GameObject audioCanvas = null;


    public void SetActiveGraphicOptions(bool state)    // Playing game
    {
        graphicsCanvas.SetActive(true);
        controlsCanvas.SetActive(false);
        audioCanvas.SetActive(false);
    }
    public void SetActiveControlOptions(bool state)    // Playing game
    {
        graphicsCanvas.SetActive(false);
        controlsCanvas.SetActive(true);
        audioCanvas.SetActive(false);
    }
    public void SetActiveAudioOptions(bool state)    // Playing game
    {
        graphicsCanvas.SetActive(false);
        controlsCanvas.SetActive(false);
        audioCanvas.SetActive(true);
    }

    public void MainMenu() => SceneManager.LoadScene(0);
    public void Quit() => Application.Quit();

    void Start()
    {
        SetActiveGraphicOptions(true);
    }
}
