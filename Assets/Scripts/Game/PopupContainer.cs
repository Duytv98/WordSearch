using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PopupContainer : SingletonComponent<PopupContainer>
{
    [SerializeField] private LevelCompletePopup levelCompletePopup = null;
    [SerializeField] private CategorySelectedPopup categorySelectedPopup = null;
    [SerializeField] private ChooseHighlighLetterPopup chooseHighlighLetterPopup = null;
    [SerializeField] private SettingsPopup settingsPopup = null;
    [SerializeField] private UnlockCategoryPopup unlockCategoryPopup = null;
    [SerializeField] private NotEnoughCoinsPopup notEnoughCoinsPopup = null;
    [SerializeField] private NotEnoughKeysPopup notEnoughKeysPopup = null;
    [SerializeField] private RewardAdGranted rewardAdGranted = null;
    [SerializeField] private StorePopup storePopup = null;
    [SerializeField] private GameObject background = null;
    [SerializeField] private GameObject backgroundFade = null;

    private string popupActive = null;
    // Start is called before the first frame update
    // private Vector3 scaleChange = new Vector3(-0.5f, -0.5f, -0.5f);

    public void ShowLevelCompletePopup(int coinsAwarded, int keysAwarded)
    {
        Show("LevelCompletePopup");
        background.SetActive(false);
        backgroundFade.SetActive(true);

        levelCompletePopup.OnShowing(coinsAwarded, keysAwarded);
    }
    public void ShowCategorySelectedPopup(CategoryInfo categoryInfo)
    {
        Show("CategorySelectedPopup");
        categorySelectedPopup.OnShowing(categoryInfo);
    }
    public void ShowHighlighLetterPopup()
    {
        Show("ChooseHighlighLetterPopup");
        chooseHighlighLetterPopup.OnShowing();
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
        Show("NotEnoughKeysPopup");
    }
    public void ShowRewardAdGranted()
    {
    }
    public void ShowStorePopup()
    {
    }
    private void Show(string keyName)
    {
        popupActive = keyName;
        background.SetActive(true);
        GameObject popup = GetPopup(keyName);
        popup.SetActive(true);
        popup.transform.DOMoveY(0, 0.5f)
        .SetEase(Ease.OutBack);
    }
    public void CloseCurrentPopup()
    {
        Debug.Log("close");
        ClosePopup(popupActive);
    }
    public void ClosePopup(string keyName, bool isActiveBackground = false)
    {

        Debug.Log("close 1111111111111");

        var activeEvent = background.GetComponent<Button>();
        activeEvent.interactable = false;
        CanvasGroup popupCV = GetPopupCV(keyName);
        popupCV.interactable = false;
        GameObject popup = GetPopup(keyName);
        popup.transform.DOLocalMoveY(-2880f, 0.5f)
                       .SetEase(Ease.InBack)
                       .OnComplete(() =>
                       {
                           background.SetActive(isActiveBackground);
                           backgroundFade.SetActive(false);
                           popup.SetActive(false);
                           popupCV.interactable = true;
                           activeEvent.interactable = true;
                       });
    }

    public void SettingsPopupShowButton(bool isLogIn)
    {
        settingsPopup.SetButton(isLogIn);
    }
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
            default:
                return null;
        }
    }


}
