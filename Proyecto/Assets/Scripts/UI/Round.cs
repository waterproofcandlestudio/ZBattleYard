using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Round : MonoBehaviour
{
    //public int round = 1;

    //float countdown = 0f;

    ///// Canvas
    //[Header("Round")]
    //[SerializeField] Text roundText;
    //[Header("CountDown")]
    //[SerializeField] CanvasGroup countDownCanvas;
    //[SerializeField] Text countdownTimerText;

    //void Start()
    //{
    //    InitVariables();
    //}

    //#region - Update -

    //void Update()
    //{
    //    UpdateGameLogic();
    //}

    //#endregion

    //#region - Game Logic -

    //void UpdateGameLogic()
    //{
    //    countdownText.text = Mathf.RoundToInt(countdown).ToString();

    //    if (zombiesSpawnedInRound < zombiesInRound && countdown <= 0)
    //    {
    //        if (zombieSpawnTimer > zombieSpawnWait)
    //        {
    //            SpawnZombies();
    //            zombieSpawnTimer = 0;
    //        }
    //        else
    //        {
    //            zombieSpawnTimer += Time.deltaTime;
    //        }
    //    }
    //    else if (zombiesLeftInRound == 0)
    //    {
    //        StartNextRound();
    //    }


    //    // Show countdown in the end of round!
    //    if (countdown > 0)
    //    {
    //        countdown -= Time.deltaTime;

    //        countDownCanvas.alpha = 1; // Show Canvas
    //    }
    //    if (countdown <= 0)
    //    {
    //        countdown = 0;

    //        countDownCanvas.alpha = 0; // Hide Canvas
    //    }
    //}

    //void StartNextRound()
    //{
    //    zombiesInRound = zombiesLeftInRound = round * 10;
    //    zombiesSpawnedInRound = 0;
    //    countdown = 15;
    //    round++;
    //    roundText.text = round.ToString();
    //}

    //#endregion

    //void InitVariables()
    //{
    //    round = 1;
    //}
}
