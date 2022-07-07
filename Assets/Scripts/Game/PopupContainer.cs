using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class PopupContainer : MonoBehaviour
{
    public static PopupContainer Instance;
    [SerializeField] private LevelCompletePopup levelCompletePopup = null;
    [SerializeField] private CategorySelectedPopup categorySelectedPopup = null;
    [SerializeField] private ChooseHighlighLetterPopup chooseHighlighLetterPopup = null;
    [SerializeField] private SettingsPopup settingsPopup = null;
    [SerializeField] private UnlockCategoryPopup unlockCategoryPopup = null;
    [SerializeField] private NotEnoughCoinsPopup notEnoughCoinsPopup = null;
    [SerializeField] private NotEnoughKeysPopup notEnoughKeysPopup = null;
    [SerializeField] private RewardAdGranted rewardAdGranted = null;
    [SerializeField] private StorePopup storePopup = null;

    [SerializeField] private TestDailyGift dailyGift = null;
    [SerializeField] private Gift giftPopup = null;
    [SerializeField] private GiftsFast giftsFastPopup = null;
    [SerializeField] private Image background = null;
    [SerializeField] private Image backgroundFade = null;

    private float animDuration = 0.35f;
    private string popupActive = null;
    // Start is called before the first frame update
    // private Vector3 scaleChange = new Vector3(-0.5f, -0.5f, -0.5f);
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    public void ShowLevelCompletePopup(int coinsAwarded, int keysAwarded)
    {
        Show("LevelCompletePopup");
        Color colorBg = background.color;
        colorBg.a = 0;
        background.color = colorBg;
        Color colorBgF = background.color;
        colorBgF.a = 0;
        background.color = colorBgF;

        levelCompletePopup.OnShowing(coinsAwarded, keysAwarded);
    }
    public void ShowCategorySelectedPopup(CategoryInfo categoryInfo)
    {
        Show("CategorySelectedPopup");
        categorySelectedPopup.OnShowing(categoryInfo);
    }
    public void ShowHighlighLetterPopup(bool isBooterUse)
    {
        Show("ChooseHighlighLetterPopup");
        chooseHighlighLetterPopup.OnShowing(isBooterUse);
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
        ClosePopup("UnlockCategoryPopup", true);
        Show("NotEnoughKeysPopup", false);
    }
    public void ShowRewardAdGranted()
    {
    }
    public void ShowStorePopup()
    {
    }
    public void ShowGiftPopup(string headerText, string messageText)
    {
        Debug.Log("ShowGiftPopup");
        Show("GiftPopup");
        giftPopup.OnShowing(headerText, messageText);
    }
    public void ShowDailyGift()
    {
        Debug.Log("ShowDailyGift");
        Show("dailyGift");
        dailyGift.OnShowing();
    }
    public void ShowGiftsFastPopup(Tuple<string, Booter> tuple)
    {
        Debug.Log("ShowGiftsFastPopup");
        Show("GiftsFastPopup");
        giftsFastPopup.OnShowing(tuple);
    }
    private void Show(string keyName, bool active = true)
    {
        popupActive = keyName;
        if (keyName == "LevelCompletePopup") FadeInPanelBG(backgroundFade, 0.4f);
        else if (active) FadeInPanelBG(background);
        GameObject popup = GetPopup(keyName);
        popup.SetActive(true);
        popup.transform.DOMoveY(0, animDuration)
        .SetEase(Ease.OutBack);
    }
    public void CloseCurrentPopup()
    {
        Debug.Log("close");
        Debug.Log("popupActive: " + popupActive);
        AudioManager.Instance.Play_Click_Button_Sound();
        ClosePopup(popupActive);
    }
    public void ClosePopup(string keyName, bool isActiveBackground = false)
    {

        var activeEvent = background.GetComponent<Button>();
        activeEvent.interactable = false;
        // CanvasGroup popupCV = GetPopupCV(keyName);
        // popupCV.interactable = false;
        GameObject popup = GetPopup(keyName);
        popup.transform.DOLocalMoveY(1920, animDuration*0.5f)
                       .SetEase(Ease.OutSine)
                       .OnComplete(() =>
                       {
                           if (!isActiveBackground)
                           {
                               if (keyName == "LevelCompletePopup")
                               {
                                   FadeOutPanelBG(backgroundFade, 0.4f);
                               }
                               else
                               {
                                   FadeOutPanelBG(background);
                               }
                           }
                           popup.transform.localPosition = new Vector3(0, -2880f, 0);
                           popup.SetActive(false);
                        //    popupCV.interactable = true;
                           activeEvent.interactable = true;
                       });
    }
    public void FadeInPanelBG(Image panelPopupImg, float a = 0.92f)
    {
        Color col = panelPopupImg.color;
        col.a = 0;
        panelPopupImg.color = col;

        panelPopupImg.DOFade(a, animDuration).OnComplete(() =>
        {
            panelPopupImg.raycastTarget = true;
        });
    }
    public void FadeOutPanelBG(Image panelPopupImg, float a = 0.92f)
    {
        Color col = panelPopupImg.color;
        col.a = a;
        panelPopupImg.color = col;
        panelPopupImg.raycastTarget = false;
        panelPopupImg.DOFade(0f, animDuration*0.5f);

    }

    // public void SettingsPopupShowButton(bool isLogIn)
    // {
    //     settingsPopup.SetButton(isLogIn);
    // }
    public GameObject GetPopup(string key)
    {
        switch (key)
        {
            case "LevelCompletePopup":
                return levelCompletePopup.gameObject;
            case "CategorySelectedPopup":
                return categorySelectedPopup.gameObject;
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
            case "RewardAdGranted":
                return rewardAdGranted.gameObject;
            case "StorePopup":
                return storePopup.gameObject;
            case "GiftPopup":
                return giftPopup.gameObject;
            case "dailyGift":
                return dailyGift.gameObject;
            case "GiftsFastPopup":
                return giftsFastPopup.gameObject;
            default:
                return null;
        }
    }
    public CanvasGroup GetPopupCV(string key)
    {
        switch (key)
        {
            case "LevelCompletePopup":
                return levelCompletePopup.GetComponent<CanvasGroup>();
            case "CategorySelectedPopup":
                return categorySelectedPopup.GetComponent<CanvasGroup>();
            case "ChooseHighlighLetterPopup":
                return chooseHighlighLetterPopup.GetComponent<CanvasGroup>();
            case "SettingsPopup":
                return settingsPopup.GetComponent<CanvasGroup>();
            case "UnlockCategoryPopup":
                return unlockCategoryPopup.GetComponent<CanvasGroup>();
            case "NotEnoughCoinsPopup":
                return notEnoughCoinsPopup.GetComponent<CanvasGroup>();
            case "NotEnoughKeysPopup":
                return notEnoughKeysPopup.GetComponent<CanvasGroup>();
            case "RewardAdGranted":
                return rewardAdGranted.GetComponent<CanvasGroup>();
            case "StorePopup":
                return storePopup.GetComponent<CanvasGroup>();
            case "GiftPopup":
                return giftPopup.GetComponent<CanvasGroup>();
            case "GiftsFastPopup":
                return giftsFastPopup.GetComponent<CanvasGroup>();
            default:
                return null;
        }
    }


}
