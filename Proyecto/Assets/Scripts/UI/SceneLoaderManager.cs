using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderManager : MonoBehaviour
{
    //[SerializeField] Object[] gameLevels;
    [SerializeField] Object trainStation;
    [SerializeField] Object mainMenu;
    [SerializeField] Object loadingScene;

    //public void LoadLevel_Scene(int index)
    //{
    //    SceneManager.LoadScene(gameLevels[index].ToString());
    //}
    public void LoadTrainStation_Scene() => SceneManager.LoadScene(trainStation.ToString());
    public void MainMenu_Scene() => SceneManager.LoadScene(mainMenu.ToString());
    public void RestartScene_Scene() => SceneManager.LoadScene(SceneManager.GetActiveScene().ToString());
    public void QuitGame() => Application.Quit();
}
