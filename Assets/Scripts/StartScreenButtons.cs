﻿using Facebook.Unity;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartScreenButtons : MonoBehaviour
{
    
    [SerializeField]
    GameObject logoutButton; // reference to the logout button
    [SerializeField]
    GameObject statsPanel; // reference to the stats panel
    [SerializeField]
    GameObject PopupGUI; // helper gui

    [SerializeField]
    GameObject questionAddPanel;

    private const string addQuestionURL = "https://opentdb.com/trivia_add_question.php"; // the url if the user wants to add a question to the db



    public void Play()
    {
        // needs internet access 
        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            string popup = "No internet connection. Enable your internet and try again!";
            StartCoroutine(DisplayPopup(popup));
            return;
        }  
        GameManager.Instance.ToCategorySelect();
    }
       
   // Logins with facebook 
    public void FBLogin()
    {      
        GameManager.Instance.GetComponent<FacebookManager>().Login(logoutButton);     
        if (statsPanel.activeSelf)
        {
            ToggleStats();
        }       
    }

    public void FBLogout()
    {
        if(GameManager.Instance.GetComponent<FacebookManager>().Logout())
        {
            logoutButton.SetActive(false);
        }

        StartCoroutine(WaitForLogout());
       
        string popup = "Successfully logged out of Facebook!";
        StartCoroutine(DisplayPopup(popup));
    }

    public void ToggleStats()
    {
        statsPanel.SetActive(!statsPanel.activeSelf);
    }

    public void ToggleQuestionAddPanel()
    {
        questionAddPanel.SetActive(!questionAddPanel.activeSelf);
    }

    public IEnumerator DisplayPopup(string popuptext)
    {
        PopupGUI.GetComponentInChildren<Text>().text = popuptext;
        PopupGUI.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        PopupGUI.SetActive(false);
    }

    IEnumerator WaitForLogout()
    {
        while(FB.IsLoggedIn)
        {
            yield return null;
        }

    }
}