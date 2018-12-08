﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    private const string catUrl = "https://opentdb.com/api_category.php";

    private const string defaultGetQuestUrl = "https://opentdb.com/api.php?amount=10";
    private const string getSessionTokenUrl = "https://opentdb.com/api_token.php?command=request";
    private string SessionToken;

    private const int pooledCategoryButtons = 24;

    public Dictionary<string, int> AllCategoriesDictionary;
    

    [SerializeField]
    GameObject categoryButtonPrefab;
    public List<Question> questionList;


    RequestData requestData;
     
    #region Singleton
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    // Use this for initialization
    private void Start()
    {
        // Object Pool the category buttons
        ObjectPooler.PreLoadInstances(categoryButtonPrefab, pooledCategoryButtons, gameObject.transform);

        AllCategoriesDictionary = new Dictionary<string, int>();
        questionList = new List<Question>();
        StartCoroutine(GetSessionToken());
        StartCoroutine(GetCategories());
    }

    IEnumerator GetSessionToken()
    {
        using (WWW www = new WWW(getSessionTokenUrl))
        {
            yield return www;
            string retrievedData = www.text;
            Token generatedToken = JsonUtility.FromJson<Token>(retrievedData);
            SessionToken = generatedToken.token;
            //Debug.Log("Session token is: " + SessionToken);
        }
               
    }

    // Async Task used to get all the categories of the game
    IEnumerator GetCategories()
    {

        TriviaCategories catData;

        using (WWW www = new WWW(catUrl))
        {
            yield return www;
            string text = www.text;
            

            catData = JsonUtility.FromJson<TriviaCategories>(text);
            
            //SetupCategoryData();
        }

        foreach(Category category in catData.trivia_categories)
        {

            string[] info = category.name.Split(':');
            string name;
            if (info.Length > 1)
            {
                name = info[1].Substring(1);
            }
            else
            {
                name = category.name;
            }
            
            AllCategoriesDictionary.Add(name, category.id);
        }

        SceneManager.LoadScene("MainMenu");
    }
  
    // Async Task that takes a url from the trivia api and Retrieves Quesitons
    // Storing them into a RequestData object.
    IEnumerator GetQuestions(string[] questUrl)
    {
        foreach (string URL in questUrl)
        {
            using (WWW www = new WWW(URL))
            {
                yield return www;
                string text = www.text;
                
                requestData = JsonUtility.FromJson<RequestData>(text);
            }

            foreach (Question question in requestData.results)
            {
                questionList.Add(question);
            }
        }

        //RandomizeQuestionList();
        SceneManager.LoadScene("MainGame");
    }

    public void StartGame(List<int> selectedCategories, Difficulty difficulty, GameObject[] categories)
    {
        // Pool the category buttons
        foreach (GameObject category in categories)
        {
            ObjectPooler.StoreInstance(category, this.transform);
        }
        // Get questions for each category and store them in the question list
        string[] requestURLS = GenerateUrlArray(selectedCategories, difficulty);
        
        StartCoroutine(GetQuestions(requestURLS));
       
    }

    /*
    private void RandomizeQuestionList()
    {
        System.Random rng = new System.Random();
        int n = questionList.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Question value = questionList[k];
            questionList[k] = questionList[n];
            questionList[n] = value;
        }
        
        foreach (Question question in questionList)
        {
            Debug.Log(question.ToString());
        }
    }
    */

    private string[] GenerateUrlArray(List<int> selectedIds, Difficulty difficulty)
    {
        string[] requestURLS = new string[selectedIds.Count];
        StringBuilder requestURL = new StringBuilder();
        for (int i =0; i <selectedIds.Count; ++i)
        {
            requestURL.Clear();
            // append category and token
            requestURL.Append(defaultGetQuestUrl).
                Append("&token=").Append(SessionToken).Append("&category=");
           

            requestURL.Append(selectedIds[i]);
            requestURL.Append("&difficulty=").Append(difficulty.ToString());
            requestURLS[i] = requestURL.ToString();
        }
        
        return requestURLS;
    }
    
}
