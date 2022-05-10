using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PopupContainer : SingletonComponent<PopupContainer>
{
    [SerializeField] private LevelCompletePopup levelCompletePopup = null;
    [SerializeField] private CategorySelectedPopup categorySelectedPopup = null;
    [SerializeField] private ChooseHighlighLetterPopup chooseHighlighLetterPopup = null;
    [SerializeField] private SettingsPopup settingsPopup = null;
    [SerializeField] private UnlockCategoryPopup unlockCategoryPopup = null;
    [SerializeField] private NotEnoughCoinsPopup notEnoughCoinsPopup = null;
    [SerializeField] private GameObject notEnoughKeysPopup = null;
    [SerializeField] private RewardAdGranted rewardAdGranted = null;
    [SerializeField] private StorePopup storePopup = null;
    [SerializeField] private GameObject background = null;
    [SerializeField] private GameObject backgroundFade = null;
    // Start is called before the first frame update
    private Vector3 scaleChange = new Vector3(-0.5f, -0.5f, -0.5f);

    public void ShowLevelCompletePopup(int coinsAwarded, int keysAwarded)
    {
        Show(levelCompletePopup.gameObject);
        background.SetActive(false);
        backgroundFade.SetActive(true);

        levelCompletePopup.OnShowing(coinsAwarded, keysAwarded);
    }
    public void ShowCategorySelectedPopup(CategoryInfo categoryInfo)
    {
        Show(categorySelectedPopup.gameObject);
        categorySelectedPopup.OnShowing(categoryInfo);
    }
    public void ShowHighlighLetterPopup()
    {
        Show(chooseHighlighLetterPopup.gameObject);
        chooseHighlighLetterPopup.OnShowing();
    }
    public void ShowSettingsPopup()
    {
        Show(settingsPopup.gameObject);
    }
    public void ShowUnlockCategoryPopup(CategoryInfo categoryInfo)
    {
        Show(unlockCategoryPopup.gameObject);
        unlockCategoryPopup.OnShowing(categoryInfo);
    }
    public void ShowNotEnoughCoinsPopup()
    {
        Show(notEnoughCoinsPopup.gameObject);
    }
    public void ShowNotEnoughKeysPopup()
    {
        ClosePopup();
        Show(notEnoughKeysPopup);
    }
    public void ShowRewardAdGranted()
    {
    }
    public void ShowStorePopup()
    {
    }
    private void Show(GameObject gameObject)
    {
        background.SetActive(true);
        gameObject.transform.localScale += scaleChange;
        gameObject.SetActive(true);
        gameObject.transform.DOScale(Vector3.one, 0.5f)
        .SetEase(Ease.OutBack);
    }
    public void ClosePopup()
    {
        background.SetActive(false);
        backgroundFade.SetActive(false);
        levelCompletePopup.gameObject.SetActive(false);
        categorySelectedPopup.gameObject.SetActive(false);
        chooseHighlighLetterPopup.gameObject.SetActive(false);
        settingsPopup.gameObject.SetActive(false);
        unlockCategoryPopup.gameObject.SetActive(false);
        notEnoughCoinsPopup.gameObject.SetActive(false);
        notEnoughKeysPopup.SetActive(false);
        rewardAdGranted.gameObject.SetActive(false);
        storePopup.gameObject.SetActive(false);
    }


}
