using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class LevelCompletePopup : MonoBehaviour
{
    [Space]

    // [SerializeField] private GameObject playAgainButton = null;
    [SerializeField] private GameObject nextLevelButton = null;
    [SerializeField] private GameObject backToCategoriesButton = null;
    [SerializeField] private GameObject backToDailyPuzzle = null;

    [Space]

    [SerializeField] private GameObject rewardsContainer = null;
    [SerializeField] private GameObject coinRewardContainer = null;
    [SerializeField] private GameObject keyRewardContainer = null;
    [SerializeField] private Text coinRewardAmountText = null;
    [SerializeField] private Text keyRewardAmountText = null;

    private bool playmode = false;
    public void OnShowing(int coinsAwarded, int keysAwarded)
    {
        Debug.Log("OnShowing");
        bool awardCoins = coinsAwarded > 0;
        bool awardKeys = keysAwarded > 0;

        coinRewardContainer.SetActive(awardCoins);

        playmode = GameManager.Instance.ActiveGameMode == GameManager.GameMode.Casual ? false : true;

        bool allLevelsCompletedCasual = ScreenManager.Instance.CheckAllLevelCompleteCasual();
        // playAgainButton.SetActive(!playmode);
        keyRewardContainer.SetActive(playmode && awardKeys);
        bool allLevelsCompleted = GameManager.Instance.AllLevelsComplete(GameManager.Instance.ActiveCategoryInfo);

        Debug.Log("playmode: " + playmode);
        Debug.Log("allLevelsCompletedCasual: " + allLevelsCompletedCasual);
        if (playmode) nextLevelButton.SetActive(!allLevelsCompleted);
        else nextLevelButton.SetActive(!allLevelsCompletedCasual);

        backToCategoriesButton.SetActive(playmode);
        backToDailyPuzzle.SetActive(!playmode);



        coinRewardAmountText.text = "x " + coinsAwarded;
        keyRewardAmountText.text = "x " + keysAwarded;
    }
    public void CloseLevelCompletePopup()
    {
        PopupContainer.Instance.ClosePopup("LevelCompletePopup");
    }
    public void OnClickNextLevel()
    {
        if (playmode) GameManager.Instance.StartLevel(GameManager.Instance.ActiveCategoryInfo, GameManager.Instance.ActiveLevelIndex + 1);
        else ScreenManager.Instance.NextLevelCasual();
        // CloseLevelCompletePopup();
        PopupContainer.Instance.CloseCurrentPopup();

    }
    public void OnClickPlayAgainButton()
    {
        // GameManager.Instance.StartCasual(GameManager.Instance.ActiveCategoryInfo, GameManager.Instance.ActiveDifficultyIndex);
        // CloseLevelCompletePopup();
        PopupContainer.Instance.CloseCurrentPopup();

    }
    public void OnClickBackToCategoriesButton()
    {
        ScreenManager.Instance.BackToHome();
        PopupContainer.Instance.CloseCurrentPopup();

    }
}
