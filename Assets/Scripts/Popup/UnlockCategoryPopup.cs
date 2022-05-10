using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UnlockCategoryPopup : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Text categoryNameText = null;
    [SerializeField] private Image categoryIconImage = null;
    [SerializeField] private GameObject coinsUnlockContainer = null;
    [SerializeField] private GameObject keysUnlockContainer = null;
    [SerializeField] private Text unlockCoinAmountText = null;
    [SerializeField] private Text unlockKeyAmountText = null;


    private CategoryInfo category = null;

    public void OnShowing(CategoryInfo categoryInfo)
    {
        this.category = categoryInfo;
        categoryNameText.text = categoryInfo.displayName;
        categoryIconImage.sprite = categoryInfo.icon;

        coinsUnlockContainer.SetActive(categoryInfo.lockType == CategoryInfo.LockType.Coins);
        keysUnlockContainer.SetActive(categoryInfo.lockType == CategoryInfo.LockType.Keys);

        unlockCoinAmountText.text = categoryInfo.unlockAmount.ToString();
        unlockKeyAmountText.text = categoryInfo.unlockAmount.ToString();
    }
    public void OnClickUnlockCategoryButton()
    {
        GameManager.Instance.UnlockCategory(this.category);
    }

    public void CloseUnlockCategoryButton()
    {
        PopupContainer.Instance.ClosePopup();
        // Debug.Log("You have clicked the button!");
    }

}
