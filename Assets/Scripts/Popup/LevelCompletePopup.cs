using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class LevelCompletePopup : MonoBehaviour
{
    [Space]

    [SerializeField] private GameObject playAgainButton = null;
    [SerializeField] private GameObject nextLevelButton = null;

    [Space]

    [SerializeField] private GameObject rewardsContainer = null;
    [SerializeField] private GameObject coinRewardContainer = null;
    [SerializeField] private GameObject keyRewardContainer = null;
    [SerializeField] private Text coinRewardAmountText = null;
    [SerializeField] private Text keyRewardAmountText = null;

    private bool playmode = false;
    public void OnShowing(int coinsAwarded, int keysAwarded)
    {
        Debug.Log("coinsAwarded: " + coinsAwarded + "  keysAwarded: " + keysAwarded);
        bool awardCoins = coinsAwarded > 0;
        bool awardKeys = keysAwarded > 0;

        coinRewardContainer.SetActive(awardCoins);

        playmode = GameManager.Instance.ActiveGameMode == GameManager.GameMode.Casual ? false : true;
        // Debug.Log(playmode);
        playAgainButton.SetActive(!playmode);
        keyRewardContainer.SetActive(playmode && awardKeys);
        // keyRewardContainer.SetActive();

        bool casualHasProgress = GameManager.Instance.HasSavedCasualBoard(GameManager.Instance.ActiveCategoryInfo);
        bool allLevelsCompleted = GameManager.Instance.AllLevelsComplete(GameManager.Instance.ActiveCategoryInfo);

        nextLevelButton.SetActive(playmode && !allLevelsCompleted);
        Debug.Log("playmode: " + playmode + "   Đang chơi dở Casual: " + casualHasProgress + "        AllLevelsComplete: " + allLevelsCompleted);


        coinRewardAmountText.text = "x " + coinsAwarded;
        keyRewardAmountText.text = "x " + keysAwarded;
    }
    public void CloseLevelCompletePopup()
    {
        PopupContainer.Instance.ClosePopup();
    }
    public void OnClickNextLevel()
    {
        GameManager.Instance.StartLevel(GameManager.Instance.ActiveCategoryInfo, GameManager.Instance.ActiveLevelIndex + 1);
        CloseLevelCompletePopup();
    }
    public void OnClickPlayAgainButton()
    {
        GameManager.Instance.StartCasual(GameManager.Instance.ActiveCategoryInfo, GameManager.Instance.ActiveDifficultyIndex);
        CloseLevelCompletePopup();
    }
    public void OnClickBackToCategoriesButton()
    {
        ScreenManager.Instance.BackToHome();
        CloseLevelCompletePopup();
    }
}
