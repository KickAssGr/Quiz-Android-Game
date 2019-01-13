﻿using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class Stats : MonoBehaviour
{

    [SerializeField]
    PlayerStats playerStats;

    [SerializeField]
    Text nameText;

    [SerializeField]
    Text highScoreText;

    [SerializeField]
    Text correctQuestionsText;

    [SerializeField]
    Text highestStreakText;

    [SerializeField]
    Text levelText;

    [SerializeField]
    Text expText;

    [SerializeField]
    Slider expBar;

    DatabaseManager dbManager;


    private void OnEnable()
    {
        dbManager = GameManager.Instance.GetComponent<DatabaseManager>();
        dbManager.ReadDatabase();
        StartCoroutine(UpdateGUI());
    }

    IEnumerator UpdateGUI()
    {
        FacebookManager fbManager = GameManager.Instance.GetComponent<FacebookManager>();
        while (dbManager.readingDB || fbManager.isLogging)
        {
            yield return null;
        }
        nameText.text = playerStats.Name;
        highScoreText.text = playerStats.HighScore.ToString();
        correctQuestionsText.text = playerStats.TotalCorrectQuestionsAnswered.ToString();
        highestStreakText.text = "x" + playerStats.HighestStreak.ToString();
        levelText.text = playerStats.Level.ToString();
        expText.text = playerStats.Experience + "/" + playerStats.ExpToNextLevel;
        expBar.value = (float)playerStats.Experience /(float) playerStats.ExpToNextLevel;

    }
}