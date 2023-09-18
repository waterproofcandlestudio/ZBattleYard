using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] PowerUpManager powerUpManager;
    [SerializeField] StatsManager statsManager;

    [SerializeField] bool giveMaxCash;
    [SerializeField] bool giveInitialCash = true;

    [SerializeField] int initialCash = 500;
    public int scoreAmountNukePowerUp = 500;
    public int scoreAmountOnHit = 10;
    public int scoreAmountOnHeadHit = 20;
    public int scoreAmountOnKill = 100;
    public int currentScore;

    [SerializeField] TextMeshProUGUI scoreText;

    [SerializeField] AudioClip buyClip;

    void Awake() => GetReferences();
    void Start()
    {
        InitVariables();

        // Extra money added at the beggining
        if (giveMaxCash)        currentScore += 100000;
        if (giveInitialCash)    currentScore += initialCash;
        //

        UpdateScore();  // Show in in-game canvas
    }

    public void UpdateScore() => scoreText.text = currentScore.ToString();

    public void AddKillToScore()
    {
        AddScore(scoreAmountOnKill);
        StatsManager.kills += 1;
        statsManager.UpdateTabbingStats();
    }
    public void AddNukeToScore()    => AddScore(scoreAmountNukePowerUp);
    public void AddHitToScore()     => AddScore(scoreAmountOnHit);
    public void AddHeadHitToScore()
    {
        AddScore(scoreAmountOnHeadHit);
        StatsManager.headshots++;
    }

    public void AddScore(int pointsToAdd)
    {
        if (PowerUpManager.doublePointsActive)
        {
            pointsToAdd *= 2;
            currentScore += pointsToAdd;
            StatsManager.allGamePoints += pointsToAdd;
        }
        else
        {
            currentScore += pointsToAdd;
            StatsManager.allGamePoints += pointsToAdd;
        }

        UpdateScore();
    }

    public void SubstractScore(int amount)
    {
        currentScore -= amount;
        SFXManager.PlaySound_AudioMixer(buyClip, Camera.main.transform.position);
        UpdateScore();
    }

    void GetReferences()
    {

    }
    void InitVariables() => currentScore = 0;
}
