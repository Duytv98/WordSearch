using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SimpleJSON;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Components")]
    [SerializeField] private WordListContainer wordListContainer = null;
    [SerializeField] private GameObject loadingIndicator = null;
    [SerializeField] private GameScreen gameScreen = null;
    [SerializeField] private DataController dataController = null;



    [Header("Debug / Testing")]
    [SerializeField] private bool awardKeyEveryLevel = false;
    [SerializeField] private bool awardCoinsEveryLevel = false;

    private bool isLogIn = false;
    private bool isCompleted;
    public bool IsCompleted { get => isCompleted; set => isCompleted = value; }
    private bool isMusic = true;
    private bool isSound = true;
    public bool IsLogIn { get => isLogIn; set => isLogIn = value; }
    public bool IsMusic { get => isMusic; set => isMusic = value; }
    public bool IsSound { get => isSound; set => isSound = value; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        Application.targetFrameRate = 60;
    }

    public void GetDataBackground()
    {
        IsLogIn = SaveableManager.Instance.IsLogIn();
        IsMusic = SaveableManager.Instance.IsMusic();
        IsSound = SaveableManager.Instance.IsSound();


        if (IsMusic) AudioManager.Instance.PlayMusic();
    }

    // Start Progress
    public void StartLevel(CategoryInfo categoryInfo, int levelIndex)
    {
        dataController.ActiveCategoryInfo = categoryInfo;
        dataController.ActiveLevelIndex = levelIndex;

        ScreenManager.Instance.Show("game");
        gameScreen.Play();
    }
    public void StartNextLevel(CategoryInfo categoryInfo)
    {
        var nextLevelIndex = dataController.GetIndexLevelNext(categoryInfo);
        StartLevel(categoryInfo, nextLevelIndex);
    }



    public bool AllLevelsComplete(CategoryInfo categoryInfo)
    {
        return dataController.LastCompletedLevels.ContainsKey(categoryInfo.saveId) && dataController.LastCompletedLevels[categoryInfo.saveId] >= categoryInfo.levelFiles.Count - 1;
    }
    //trả về true nếu level completed
    public bool IsLevelCompleted(CategoryInfo categoryInfo, int levelIndex)
    {
        return dataController.LastCompletedLevels.ContainsKey(categoryInfo.saveId) && levelIndex <= dataController.LastCompletedLevels[categoryInfo.saveId];
    }
    // trả về true nếu level đang bị khóa
    public bool IsLevelLocked(CategoryInfo categoryInfo, int levelIndex)
    {
        return levelIndex > 0 && (!dataController.LastCompletedLevels.ContainsKey(categoryInfo.saveId) || levelIndex > dataController.LastCompletedLevels[categoryInfo.saveId] + 1);
    }
    public int IsLevelPlayable(CategoryInfo categoryInfo)
    {
        if (!dataController.LastCompletedLevels.ContainsKey(categoryInfo.saveId)) return 0;
        else return dataController.LastCompletedLevels[categoryInfo.saveId];
    }
    // Sử lý list category





    public bool UnlockCategory(CategoryInfo categoryInfo)
    {
        switch (categoryInfo.lockType)
        {
            case CategoryInfo.LockType.Coins:
                if (dataController.Coins < categoryInfo.unlockAmount) { }
                else { }

                break;
            case CategoryInfo.LockType.Keys:
                if (dataController.Keys < categoryInfo.unlockAmount)
                {
                    PopupContainer.Instance.ShowNotEnoughKeysPopup();
                }
                else
                {
                    dataController.SetUnlockedCategories(categoryInfo.saveId);
                    dataController.SetKeys(-categoryInfo.unlockAmount);
                    DataToday.Instance.UpdateAmountCategoryNew(1);
                    CategoryScreen.Instance.RefreshCategoryScroller();
                    // ScreenManager.Instance.RefreshLevelScreen();
                    PopupContainer.Instance.ClosePopup("UnlockCategoryPopup");
                    return true;
                }

                break;
        }
        return false;

    }


    public void AddWordDeleted(string word)
    {
        gameScreen.ActiveBoard.listWordDeleted.Add(word);
    }
}
