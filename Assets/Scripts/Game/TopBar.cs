using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopBar : MonoBehaviour
{
    [SerializeField] private CanvasGroup SettingsButton = null;
    [SerializeField] private CanvasGroup backButton = null;
    [SerializeField] private GameObject mainScreenContainer = null;
    [SerializeField] private Text mainText = null;
    [SerializeField] private Text pickCategoryText = null;
    [SerializeField] private GameObject categoryContainer = null;
    [SerializeField] private Text categoryNameText = null;
    [SerializeField] private Text levelNumberText = null;

    [SerializeField] private Text coinAmountText = null;
    [SerializeField] private Text keyAmountText = null;

    [SerializeField] private GameObject keyContainer = null;
    [SerializeField] private GameObject timeContainer = null;


    // Update is called once per frame
    void Update()
    {
        if (int.Parse(coinAmountText.text) != GameManager.Instance.Coins) coinAmountText.text = GameManager.Instance.Coins.ToString();
        if (int.Parse(keyAmountText.text) != GameManager.Instance.Keys) keyAmountText.text = GameManager.Instance.Keys.ToString();

    }
    public void OnSwitchingScreens(string toScreenId)
    {
        SetActiveBackButton(true);
        keyContainer.SetActive(true);
        timeContainer.SetActive(false);
        switch (toScreenId)
        {
            case "home":
                SetActiveBackButton(false);
                mainScreenContainer.SetActive(true);
                mainText.gameObject.SetActive(true);
                pickCategoryText.gameObject.SetActive(false);
                categoryContainer.SetActive(false);
                break;
            case "levels":
                mainScreenContainer.SetActive(true);
                mainText.gameObject.SetActive(true);
                pickCategoryText.gameObject.SetActive(true);
                categoryContainer.SetActive(false);
                break;
            case "game":
                mainScreenContainer.SetActive(false);
                categoryContainer.SetActive(true);
                categoryNameText.gameObject.SetActive(true);
                categoryNameText.text = GameManager.Instance.ActiveCategoryInfo.displayName;
                levelNumberText.gameObject.SetActive(true);
                levelNumberText.text = "LEVEL " + (GameManager.Instance.ActiveLevelIndex + 1);
                
                keyContainer.SetActive(false);
                timeContainer.SetActive(true);
                break;
            case "dailyGift":
                mainScreenContainer.SetActive(false);
                categoryContainer.SetActive(true);
                categoryNameText.gameObject.SetActive(true);
                categoryNameText.text = "daily Gift";
                levelNumberText.gameObject.SetActive(false);
                break;
            case "dailyPuzzle":
                mainScreenContainer.SetActive(false);
                categoryContainer.SetActive(true);
                categoryNameText.gameObject.SetActive(true);
                categoryNameText.text = "daily Puzzle";
                levelNumberText.gameObject.SetActive(false);
                break;
        }
    }
    public void SetTopBarCasualGame(int level)
    {
        SetActiveBackButton(true);
        mainScreenContainer.SetActive(false);
        categoryContainer.SetActive(true);
        categoryNameText.gameObject.SetActive(true);
        categoryNameText.text = "Daily Puzzle";
        levelNumberText.gameObject.SetActive(true);
        string textLevel;
        switch (level)
        {
            case 0:
                textLevel = "Easy";
                break;
            case 1:
                textLevel = "Medium";
                break;
            case 2:
                textLevel = "Hard";
                break;
            default:
                textLevel = null;
                break;

        }
        levelNumberText.text = textLevel;
    }
    private void SetActiveBackButton(bool isActive)
    {
        backButton.alpha = isActive ? 1f : 0f;
        backButton.interactable = isActive;
        backButton.blocksRaycasts = isActive;
    }
}
