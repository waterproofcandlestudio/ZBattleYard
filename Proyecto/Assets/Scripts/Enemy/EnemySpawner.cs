using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] SFXManager sfxManager;
    StatsManager statsManager;
    GrenadeLogic grenadeLogic;

    [SerializeField] AudioClip gameStart;
    [SerializeField] AudioClip waveComplete;

    public enum SpawnState { SPAWNING, WAITING, COUNTING };

    [SerializeField] bool startGame = true;
    [Header("Wave Properties")]
    [SerializeField] int maximumWavesNumber = 999;
    [SerializeField] float timeBetweenWaves = 5f;
    float waveCountdown = 0f;
    [SerializeField] Transform[] spawners;
    [SerializeField] List<Transform> activeSpawners;

    [Header("Player rewards")]
    [SerializeField] int newGrenadesAmountPerRound = 2;

    [Header("Round Entites")]
    [SerializeField] int firstRoundZombiesToScale = 6;
    [SerializeField] int maximumRoundZombiesOnMap = 30;
    [SerializeField] int maximumRoundZombies = 150;
    //[SerializeField] Enemy[] enemiyEntitiesOnWaves;
    [SerializeField] GameObject zombie;
    [SerializeField] public List<CharacterStats> enemyList; // Stores enemies in round
    [SerializeField] List<Wave> waves;

    /// Canvas
    [Header("Canvas")]
    [Header("Round")]
    [SerializeField] TextMeshProUGUI waveText;
    [Header("CountDown")]
    [SerializeField] CanvasGroup countDownCanvas;
    [SerializeField] TextMeshProUGUI countdownTimerText;

    SpawnState state = SpawnState.COUNTING;
    public static int currentWave;    // Current wave based in array position
    int roundCount = 0; // Current wave based in HUD round.text

    int playersPlaying = 1;
    public static bool enemyDied_RoundCheck;
    float spawnDelayTimer;


    void Awake() => GetReferences();
    void Start()
    {
        InitializeVariables();
        InitializeRoundsInfo();
        for (int i = 0; i < spawners.Length; i++)
        {
            if (spawners[i] == null)
                startGame = false;
        }
        
        UpdateActiveEnemySpawns();
        //SFXManager.PlaySound(gameStart);
    }

    public void UpdateActiveEnemySpawns()
    {
        if (activeSpawners.Count == 0)
        {
            for (int i = 0; i < spawners.Length; i++)
            {
                if (spawners[i].gameObject.activeSelf)
                    activeSpawners.Add(spawners[i]);
            }
        }
        else
        {
            for (int i = 0; i < spawners.Length; i++)
            {
                if (spawners[i].gameObject.activeSelf && !activeSpawners.Contains(spawners[i]))
                    activeSpawners.Add(spawners[i]);          
            }
        }
    }
    void Update()
    {
        if(startGame)
        {
            ShowNewRoundCanvas();
            if (enemyDied_RoundCheck)
                CheckAndRemoveIfEnemyInListIsDead();

            if (state == SpawnState.WAITING) // Check if all enemies are dead
            {
                if (!EnemiesAreDead())  return;
                else                    CompleteWave();
            }

            if (waveCountdown <= 0)
            {
                SpawnWave(waves[currentWave]);
                spawnDelayTimer -= Time.deltaTime;
            }
            else
            {
                waveCountdown -= Time.deltaTime * 0.1f;
                countdownTimerText.text = Mathf.RoundToInt(waveCountdown).ToString();
            }
        }
    }

    void CheckAndRemoveIfEnemyInListIsDead()
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i] == null)
                enemyList.RemoveAt(i);
        }
        enemyDied_RoundCheck = false;
    }
    void ShowNewRoundCanvas()
    {
        if (waveCountdown > 0)          // Show countdown in the end of round!
        {
            waveCountdown -= Time.deltaTime;
            countDownCanvas.alpha = 1; // Show Canvas
        }
        if (waveCountdown <= 0)
        {
            waveCountdown = 0;
            countDownCanvas.alpha = 0; // Hide Canvas
        }
    }

    void SpawnWave(Wave wave)
    {
        state = SpawnState.SPAWNING;

        if (CanSpawn(waves[currentWave]))   // Spawn enemies
        {
            SpawnZombie(wave.enemy);
            spawnDelayTimer = wave.spawnDelay;
            return;
        }

        if (wave.enemyAmount == 0)
            state = SpawnState.WAITING;
    }
    bool CanSpawn(Wave wave) // SpawnDelay
    {
        if (enemyList.Count < maximumRoundZombiesOnMap && wave.enemyAmount > 0 && spawnDelayTimer <= 0)
            return true;

        return false;
    }
    void SpawnZombie(GameObject enemy)
    {
        waves[currentWave].enemyAmount--;

        int randomInt = UnityEngine.Random.Range(0, spawners.Length);
        //
        while(!spawners[randomInt].gameObject.activeSelf) 
        {
            randomInt = UnityEngine.Random.Range(0, spawners.Length);
        }
        // Modificar
        Transform randomSpawner = spawners[randomInt];

        GameObject newEnemy = Instantiate(enemy, spawners[randomInt].position, spawners[randomInt].rotation);
        CharacterStats newEnemyStats = newEnemy.GetComponent<CharacterStats>();

        enemyList.Add(newEnemyStats);
    }

    bool EnemiesAreDead() // Check if enemies are dead
    {
        int i = 0;

        foreach (CharacterStats enemy in enemyList)
        {
            if (enemy.IsDead()) i++;
            else return false;
        }
        return true;
    }
    void CompleteWave() // Logic when waves are completed
    {
        enemyList.Clear();
        roundCount++;
        waveText.text = (roundCount + 1).ToString();
        state = SpawnState.COUNTING;
        waveCountdown = timeBetweenWaves;

        if (currentWave + 1 > waves.Count - 1)      currentWave = 0;
        else                                        currentWave++;

        statsManager.UpdateTabbingStats();
        grenadeLogic.AddNewRoundGrenades(newGrenadesAmountPerRound);
        sfxManager.PlaySound(waveComplete);
    }

    public int GetRoundNumb_UI()   =>  (roundCount + 1);

    void GetReferences()
    {
        statsManager = GetComponent<StatsManager>();
        grenadeLogic = GetComponentInParent<GrenadeLogic>();
    }
    void InitializeVariables()
    {
        enemyList.Clear();
        waveCountdown = timeBetweenWaves;
        currentWave = 0;
        waveText.text = (roundCount + 1).ToString();
    }
    void InitializeRoundsInfo()
    {
        waves.Clear();
        // First round (Separated because of first round "enemiyAmount")
        waves.Add(new Wave()
        {
            name = "Wave " + 0,
            enemyAmount = Mathf.RoundToInt(firstRoundZombiesToScale * playersPlaying),
            spawnDelay = 1f,
            enemy = zombie
        }
        );
        // Rest of rounds
        for (int i = 1; i < maximumWavesNumber; i++)
        {
            waves.Add(new Wave() 
            { 
                name = "Wave " + (i + 1),
                enemyAmount = EnemyAmountRound(i),
                spawnDelay = 1f,
                enemy = zombie
            }
            );
        }
    }

    int EnemyAmountRound(int roundIndex)
    {
        int enemyAmountRound = Mathf.RoundToInt((firstRoundZombiesToScale * roundIndex * 1.25f) * playersPlaying);

        if (enemyAmountRound < maximumRoundZombies)
            return enemyAmountRound;

        return maximumRoundZombies;
    }
}