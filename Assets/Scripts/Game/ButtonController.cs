using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{

    [Header("InGame")]
    [SerializeField] private GameObject ButtonContainerInGame = null;
    [SerializeField] private Button btnFindWords = null;
    [SerializeField] private Button btnFindLetters = null;
    [SerializeField] private Button btnSuggestManyWords = null;
    [SerializeField] private Button btnRecommendWord = null;
    [SerializeField] private Button btnClearWords = null;
    [SerializeField] private Button btnRotatingScreen = null;

    [Header("Home")]
    [SerializeField] private GameObject ButtonContainerHome = null;
    [SerializeField] private Button btnProfile = null;
    [SerializeField] private Button btnLeaderboard = null;
    [SerializeField] private Button btnDailyGift = null;
    [SerializeField] private Button btnDailyQuest = null;
    public void OnSwitchingScreens(string toScreenId)
    {
        ButtonContainerInGame.SetActive(false);
        ButtonContainerHome.SetActive(false);
        switch (toScreenId)
        {
            case "home":
                ButtonContainerHome.SetActive(true);
                break;
            case "game":
                ButtonContainerInGame.SetActive(true);
                break;
        }
    }

    public bool isActiveEvent = true;

    public bool IsActiveEvent { get => isActiveEvent; set => isActiveEvent = value; }


    public void SetInteractableButtonClearWords(bool status)
    {
        btnClearWords.interactable = status;
    }
    public void SetInteractableButtonSuggestManyWords(bool status)
    {
        btnSuggestManyWords.interactable = status;
    }
    public void SetInteractableButtonRecommendWord(bool status)
    {
        btnRecommendWord.interactable = status;
    }
    public void Test()
    {
        btnProfile.enabled = true;
    }
}
