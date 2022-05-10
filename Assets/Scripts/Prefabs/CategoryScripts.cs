using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PolyAndCode.UI;

public class CategoryScripts : MonoBehaviour, ICell
{
    [SerializeField] private Text textName = null;

    [SerializeField] private Image iconImage = null;
    [SerializeField] private Image backgroundImage = null;
    [SerializeField] private ProgressBar levelProgressBar = null;
    [SerializeField] private Text levelProgressText = null;

    [Space]

    [SerializeField] private GameObject progressBarContainer = null;
    [SerializeField] private GameObject lockedContainer = null;
    [SerializeField] private GameObject coinsUnlockContainer = null;
    [SerializeField] private GameObject keysUnlockContainer = null;
    // [SerializeField] private GameObject iapUnlockContainer = null;

    [Space]

    [SerializeField] private Text coinsUnlockAmountText = null;
    [SerializeField] private Text keysUnlockAmountText = null;
    // [SerializeField] private Text iapUnlockPriceText = null;
    private CategoryInfo category = null;
    // Start is called before the first frame update

    public void Setup(CategoryInfo categoryInfo, int index)
    {
        this.category = categoryInfo;
        textName.text = category.displayName;
        iconImage.sprite = category.icon;
        backgroundImage.color = category.categoryColor;
        SetProgress(category);
        SetLocked(category);
    }
    void SetProgress(CategoryInfo category)
    {
        // Debug.Log("   ================     ");
        // Debug.Log("Name category: " + category.displayName);
        // Debug.Log("Check key: " + GameManager.Instance.LastCompletedLevels.ContainsKey(category.saveId));
        // if(GameManager.Instance.LastCompletedLevels.ContainsKey(category.saveId))  Debug.Log("level active: " + GameManager.Instance.LastCompletedLevels[category.saveId]);
        // Debug.Log(" ");
        int totalLevels = category.levelFiles.Count;
        int numLevelsCompleted = GameManager.Instance.LastCompletedLevels.ContainsKey(category.saveId) ? GameManager.Instance.LastCompletedLevels[category.saveId] + 1 : 0;
        levelProgressBar.SetProgress((float)numLevelsCompleted / (float)totalLevels);
        levelProgressText.text = string.Format("{0} / {1}", numLevelsCompleted, totalLevels);
    }
    void SetLocked(CategoryInfo category)
    {
        bool isCategoryLocked = GameManager.Instance.IsCategoryLocked(category);
        // Debug.Log(category.displayName + " lock " + isCategoryLocked);
        // if (category.displayName == "CAREERS") Debug.Log(category.displayName + " lock " + isCategoryLocked);

        progressBarContainer.SetActive(!isCategoryLocked);
        lockedContainer.SetActive(isCategoryLocked);

        coinsUnlockContainer.SetActive(isCategoryLocked && category.lockType == CategoryInfo.LockType.Coins);
        keysUnlockContainer.SetActive(isCategoryLocked && category.lockType == CategoryInfo.LockType.Keys);
        switch (category.lockType)
        {
            case CategoryInfo.LockType.Coins:
                coinsUnlockAmountText.text = "x " + category.unlockAmount;
                break;
            case CategoryInfo.LockType.Keys:
                keysUnlockAmountText.text = "x " + category.unlockAmount;
                break;
                // case CategoryInfo.LockType.IAP:
                //     // SetIAPPrice(category.iapProductId);
                //     break;
        }
    }

    public void Onclick()
    {
        switch (category.lockType)
        {
            case CategoryInfo.LockType.None:
                GameManager.Instance.ActiveCategoryInfo = this.category;
                PopupContainer.Instance.ShowCategorySelectedPopup(this.category);
                // Debug.Log(JsonUtility.ToJson(this.category));
                break;
            case CategoryInfo.LockType.Coins:
            case CategoryInfo.LockType.Keys:
                PopupContainer.Instance.ShowUnlockCategoryPopup(this.category);
                break;
        }
    }
}
