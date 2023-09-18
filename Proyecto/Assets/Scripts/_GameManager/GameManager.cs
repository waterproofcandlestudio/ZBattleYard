using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region - Variables -

    public int round = 1;
    int zombiesInRound = 10;
    public static int zombiesLeftInRound = 10;
    int zombiesSpawnedInRound = 0;

    [SerializeField] bool startGame;
    [SerializeField] bool giveMaxCash;
    [SerializeField] bool giveInitialCash = true;

    [SerializeField] Transform[] ZombieSpawnPoints;
    [SerializeField] GameObject zombieEnemy;

    /// Timers
    float zombieSpawnTimer = 0;
    [SerializeField] float zombieSpawnWait = 1;
    float countdown = 0f;

    /// Canvas
    [Header("Round")]
    [SerializeField] Text roundText;
    [Header("CountDown")]
    [SerializeField] CanvasGroup countDownCanvas;
    [SerializeField] Text countdownText;
    [Header("Score")]
    [SerializeField] Text scoreText;
    [SerializeField] Text cashText;

    public static int playerScore = 0;
    public static int playerCash = 0;

    #endregion

    #region - Update -

    void Update()
    {
        UpdateGameLogic();
    }

    #endregion

    #region - Game Logic -

    void UpdateGameLogic()
    {
        // Points && Cash Counter / Checker
        scoreText.text = playerScore.ToString();
        cashText.text = playerCash.ToString();

        if (giveMaxCash == true)
        {
            giveMaxCash = false;
            AddPoints(100000);
        }
        if (startGame == true)
        {
            if(giveInitialCash)
            {
                giveInitialCash = false;
                AddPoints(500);
            }

            countdownText.text = Mathf.RoundToInt(countdown).ToString();

            if (zombiesSpawnedInRound < zombiesInRound && countdown <= 0)
            {
                if (zombieSpawnTimer > zombieSpawnWait)
                {
                    SpawnZombies();
                    zombieSpawnTimer = 0;
                }
                else
                {
                    zombieSpawnTimer += Time.deltaTime;
                }
            }
            else if (zombiesLeftInRound == 0)
            {
                StartNextRound();
            }


            // Show countdown in the end of round!
            if (countdown > 0)
            {
                countdown -= Time.deltaTime;

                countDownCanvas.alpha = 1; // Show Canvas
            }
            if (countdown <= 0)
            {
                countdown = 0;

                countDownCanvas.alpha = 0; // Hide Canvas
            }
        }
    }

    public static void AddPoints(int pointValue)
    {
        playerScore += pointValue;
        playerCash += pointValue;
    }

    void SpawnZombies()
    {
        Vector3 randomSpawnPoint = ZombieSpawnPoints[Random.Range(0, ZombieSpawnPoints.Length)].position;
        Instantiate(zombieEnemy, randomSpawnPoint, Quaternion.identity);
        zombiesSpawnedInRound++;
    }

    void StartNextRound()
    {
        zombiesInRound = zombiesLeftInRound = round * 10;
        zombiesSpawnedInRound = 0;
        countdown = 15;
        round++;
        roundText.text = round.ToString();
    }

    #endregion

    #region

    public static void CashPerHit()
    {
        playerCash += 10;
    }

    public static void CashPerKill()
    {
        playerCash += 100;
    }

    #endregion
}
