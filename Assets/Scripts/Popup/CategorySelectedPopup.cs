using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategorySelectedPopup : MonoBehaviour
{

    [Space]
    [SerializeField] private Text categoryNameText = null;
    [SerializeField] private Image categoryIconImage = null;
    [SerializeField] private CanvasGroup selectModeContainer = null;
    [SerializeField] private CanvasGroup SelectDifficultyContainer = null;



    [Space]

    [SerializeField] private Text casualPlayButtonText = null;
    [SerializeField] private CanvasGroup casualContinueButton = null;
    [SerializeField] private CanvasGroup progressPlayButton = null;
    [SerializeField] private Text progressMessageText = null;


    private CategoryInfo categoryInfo = null;



    public void OnShowing(CategoryInfo categoryInfo)
    {
        this.categoryInfo = categoryInfo;
        categoryNameText.text = categoryInfo.displayName;
        categoryIconImage.sprite = categoryInfo.icon;
        OpenModeContainer();

        bool casualHasProgress = GameManager.Instance.HasSavedCasualBoard(categoryInfo);
        bool allLevelsCompleted = GameManager.Instance.AllLevelsComplete(categoryInfo);

        // Debug.Log("Đang chơi dở Casual: " + casualHasProgress + "        AllLevelsComplete: " + allLevelsCompleted);

        casualPlayButtonText.text = casualHasProgress ? "NEW GAME" : "PLAY";
        casualContinueButton.interactable = casualHasProgress;
        casualContinueButton.alpha = casualHasProgress ? 1f : 0.3f;


        progressPlayButton.interactable = !allLevelsCompleted;
        progressPlayButton.alpha = !allLevelsCompleted ? 1f : 0.3f;
        if (allLevelsCompleted)
        {
            progressMessageText.text = "All levels completed!";
        }
    }


    public void ClosePopupCategorySelected()
    {
        PopupContainer.Instance.ClosePopup();
        OpenModeContainer();
        // Debug.Log("You have clicked the button!");
    }
    void OpenDifficultyContainer()
    {
        selectModeContainer.interactable = false;
        selectModeContainer.blocksRaycasts = false;
        selectModeContainer.alpha = 0f;

        SelectDifficultyContainer.interactable = true;
        SelectDifficultyContainer.blocksRaycasts = true;
        SelectDifficultyContainer.alpha = 1f;
    }
    void OpenModeContainer()
    {
        selectModeContainer.interactable = true;
        selectModeContainer.blocksRaycasts = true;
        selectModeContainer.alpha = 1f;

        SelectDifficultyContainer.interactable = false;
        SelectDifficultyContainer.blocksRaycasts = false;
        SelectDifficultyContainer.alpha = 0f;
    }
    public void PlayWithCategory()
    {
        Debug.Log("PlayWithCategory");
        OpenDifficultyContainer();
    }
    public void ContinueWithCategory()
    {
        Debug.Log("ContinueWithCategory");
        GameManager.Instance.ContinueCasual(this.categoryInfo);
        ClosePopupCategorySelected();
        // OpenDifficultyContainer();
    }
    public void PlayNextLevelProgress()
    {
        // Debug.Log("PlayNextLevelProgress");
        GameManager.Instance.StartNextLevel(this.categoryInfo);
        ClosePopupCategorySelected();
        // CategoryInfo activeCategory = GameManager.Instance.ActiveCategoryInfo;
        // int activeLevel = GameManager.Instance.LastCompletedLevels[activeCategory.saveId];
        // TextAsset levelFile = activeCategory.levelFiles[activeLevel];
    }
    public void ShowProgressLevel()
    {
        ClosePopupCategorySelected();
        OpenModeContainer();
        ScreenManager.Instance.ShowScreenLevel();
    }

    public void OnDifficultySelected(int difficultyIndex)
    {
        // switch (difficultyIndex)
        // {
        //     case 0:
        //         Debug.Log("EasyButton");
        //         break;
        //     case 1:
        //         Debug.Log("MediumButton");
        //         break;
        //     case 2:
        //         Debug.Log("HardButton");
        //         break;
        // }
        GameManager.Instance.StartCasual(categoryInfo, difficultyIndex);
        ClosePopupCategorySelected();
    }
}
