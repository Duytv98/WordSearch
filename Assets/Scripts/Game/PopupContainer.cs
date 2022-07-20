using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class PopupContainer : MonoBehaviour
{
    public static PopupContainer Instance;
    [Space]
    [SerializeField] private Leaderboard leaderboard = null;
    [SerializeField] private SettingsPopup settingsPopup = null;
    [SerializeField] private SelectCategoryPopup selectCategoryPopup = null;
    [SerializeField] private LevelCompletePopup levelCompletePopup = null;
    [SerializeField] private DailyGift dailyGift = null;
    // [SerializeField] private TestDailyGift dailyGift = null;
    [SerializeField] private ChooseHighlighLetterPopup chooseHighlighLetterPopup = null;

    [Space]
    [SerializeField] private UnlockCategoryPopup unlockCategoryPopup = null;
    [SerializeField] private NotEnoughCoinsPopup notEnoughCoinsPopup = null;
    [SerializeField] private NotEnoughKeysPopup notEnoughKeysPopup = null;
    [SerializeField] private StorePopup storePopup = null;

    [Space]
    [SerializeField] private Image background = null;
    [SerializeField] private Image background1 = null;

    private float animDuration = 0.35f;

    private bool isShow = false;


    private List<string> backStack;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start()
    {
        backStack = new List<string>();
    }

    public void ShowLevelCompletePopup(int coinsAwarded, int keysAwarded)
    {
        Show("LevelCompletePopup", 184f);
        levelCompletePopup.OnShowing(coinsAwarded, keysAwarded);
    }
    public void ShowHighlighLetterPopup(bool isBooterUse, List<char> listLetterExist)
    {
        Show("ChooseHighlighLetterPopup");
        chooseHighlighLetterPopup.OnShowing(isBooterUse, listLetterExist);
    }
    public void ShowSettingsPopup()
    {
        Show("SettingsPopup");
        settingsPopup.OnShowing();
    }
    public void ShowUnlockCategoryPopup(CategoryInfo categoryInfo)
    {
        Show("UnlockCategoryPopup");
        unlockCategoryPopup.OnShowing(categoryInfo);
    }
    public void ShowNotEnoughCoinsPopup()
    {
        Show("NotEnoughCoinsPopup");
    }
    public void ShowNotEnoughKeysPopup()
    {
        Debug.Log("ShowNotEnoughKeysPopup");
        Show("NotEnoughKeysPopup");
        ClosePopup("UnlockCategoryPopup", false);
    }
    public void ShowStorePopup()
    {
        Show("StorePopup");
        ClosePopup("NotEnoughCoinsPopup", false);
    }
    public void ShowLeaderboard()
    {
        Show("Leaderboard", 220f);
        leaderboard.OnShowing();
    }
    public void ShowSelectCategoryPopup()
    {
        Show("SelectCategoryPopup");
        selectCategoryPopup.OnShowing();
    }
    public void ShowDailyGift()
    {
        Show("dailyGift");
        dailyGift.OnShowing();
    }
    private void Show(string keyName, float y = 0)
    {
        if (backStack.Count == 0)
        {
            FadeInPanelBG(background, keyName);
        }
        else if (backStack.Count == 1)
        {
            if (isShow) return;
            isShow = true;
            FadeInPanelBG(background1, keyName);
        }
        AddBackStack(keyName);

        Debug.Log(keyName);
        GameObject popup = GetPopup(keyName);
        popup.SetActive(true);
        (popup.transform as RectTransform).DOAnchorPosY(y, animDuration)
        .SetEase(Ease.OutBack);
    }
    public void CloseCurrentPopup()
    {
        AudioManager.Instance.Play_Click_Button_Sound();
        ClosePopup(backStack[backStack.Count - 1]);
    }
    public void ClosePopup(string keyName, bool isClose = true)
    {
        if (backStack.Count == 1)
        {
            FadeOutPanelBG(background);
        }
        else if (backStack.Count == 2)
        {
            if (isClose)
            {
                isShow = false;
                FadeOutPanelBG(background1);
            }
        }
        RemoveBackStack(keyName);
        var activeEvent = background.GetComponent<Button>();
        activeEvent.interactable = false;

        GameObject popup = GetPopup(keyName);
        popup.transform.DOLocalMoveY(1920, animDuration * 0.5f)
                       .SetEase(Ease.OutSine)
                       .OnComplete(() =>
                       {
                           popup.transform.localPosition = new Vector3(0, -2880f, 0);
                           popup.SetActive(false);
                           activeEvent.interactable = true;
                           if (keyName.Equals("Leaderboard")) leaderboard.Close();
                       });
    }
    public void FadeInPanelBG(Image panelBG, string keyName)
    {
        Color col = panelBG.color;
        col.a = 0;
        panelBG.color = col;
        panelBG.DOFade(0.85f, animDuration)
        .OnComplete(() =>
        {
            if (!keyName.Equals("LevelCompletePopup")) panelBG.raycastTarget = true;
        });
    }
    public void FadeOutPanelBG(Image panelBG)
    {
        Color col = panelBG.color;
        col.a = 0.85f;
        panelBG.raycastTarget = false;
        panelBG.DOFade(0f, animDuration * 0.5f)
        .SetDelay(animDuration * 0.5f);
    }


    public GameObject GetPopup(string key)
    {
        Debug.Log("GetPopup: " + key);
        switch (key)
        {
            case "LevelCompletePopup":
                return levelCompletePopup.gameObject;
            case "ChooseHighlighLetterPopup":
                return chooseHighlighLetterPopup.gameObject;
            case "SettingsPopup":
                return settingsPopup.gameObject;
            case "UnlockCategoryPopup":
                return unlockCategoryPopup.gameObject;
            case "NotEnoughCoinsPopup":
                return notEnoughCoinsPopup.gameObject;
            case "NotEnoughKeysPopup":
                return notEnoughKeysPopup.gameObject;
            case "StorePopup":
                return storePopup.gameObject;
            case "Leaderboard":
                return leaderboard.gameObject;
            case "SelectCategoryPopup":
                return selectCategoryPopup.gameObject;
            case "dailyGift":
                return dailyGift.gameObject;
            default:
                return null;
        }
    }


    private void AddBackStack(string keyName)
    {
        if (backStack.Count == 0) backStack.Add(keyName);
        if (keyName != backStack[backStack.Count - 1])
        {
            backStack.Add(keyName);
        }
    }
    private void RemoveBackStack(string keyName)
    {
        if (!backStack.Contains(keyName)) return;
        backStack.RemoveAt(backStack.LastIndexOf(keyName));
    }
}
