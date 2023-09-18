using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class StatsManager : MonoBehaviour
{
    PlayerInput playerInput;
    EnemySpawner enemySpawner;

    /// Canvas
    [Header("Endgame Stats Texts")]
    [SerializeField] TextMeshProUGUI endRoundsSurvivedText;
    [SerializeField] TextMeshProUGUI endPointsText;
    [SerializeField] TextMeshProUGUI endKillsText;
    [SerializeField] TextMeshProUGUI endHeadshotsText;

    [Header("Tabbing ingame Stats Texts")]
    [SerializeField] CanvasGroup hudCanvasGroup;
    [SerializeField] CanvasGroup tabCanvasGroup;
    [SerializeField] TextMeshProUGUI tabRoundsSurvivedText;
    [SerializeField] TextMeshProUGUI tabPointsText;
    [SerializeField] TextMeshProUGUI tabKillsText;
    [SerializeField] TextMeshProUGUI tabHeadshotsText;

    public static int allGamePoints;
    public static int kills;
    public static int headshots;


    void Awake() => GetReferences();
    void Start() => InitializeVariables();

    public void UpdateEndgameStats()
    {
        endRoundsSurvivedText.text = enemySpawner.GetRoundNumb_UI().ToString();
        endPointsText.text = allGamePoints.ToString();
        endKillsText.text = kills.ToString();
        endHeadshotsText.text = headshots.ToString();
    }

    public void UpdateTabbingStats()
    {
        tabRoundsSurvivedText.text = enemySpawner.GetRoundNumb_UI().ToString();
        tabPointsText.text = allGamePoints.ToString();
        tabKillsText.text = kills.ToString();
        tabHeadshotsText.text = headshots.ToString();
    }

    void ActivateStatsUI()  // See stats ingame when tabbing
    {
        UpdateTabbingStats();
        hudCanvasGroup.alpha = 0;
        tabCanvasGroup.alpha = 1;
    }
    void DesactivateStatsUI()  // See stats ingame when tabbing
    {
        UpdateTabbingStats();
        hudCanvasGroup.alpha = 1;
        tabCanvasGroup.alpha = 0;
    }    

    void GetReferences()
    {
        playerInput = new PlayerInput(); // Initialize Player input to use new input system
        playerInput.Enable(); // Enable input system!
        playerInput.OnFoot.SeeStats.performed += e => ActivateStatsUI();   // Jump
        playerInput.OnFoot.SeeStats.canceled += e => DesactivateStatsUI();   // Jump

        enemySpawner = GetComponent<EnemySpawner>();
    }

    void InitializeVariables()
    {
        allGamePoints = 0;
        kills = 0;
        headshots = 0;
        tabCanvasGroup.alpha = 0;
    }

    void OnDisable()
    {
        playerInput.Disable();
    }
}
