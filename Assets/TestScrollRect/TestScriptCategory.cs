using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestScriptCategory : ExpandableListItem<CategoryInfo>
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
    [SerializeField] private GameObject purdahEasy = null;
    [SerializeField] private GameObject purdahMedium = null;
    [SerializeField] private GameObject purdahHard = null;


    private int levelOfDifficult = 0;









    [SerializeField] private RectTransform content = null;
    [SerializeField] private RectTransform levelListContent = null;


    private CategoryInfo category;
    private ObjectPool levelListItemPool;

    private List<GameObject> activeLevelListItems;
    // Start is called before the first frame update



    public override void Initialize(CategoryInfo categoryInfo)
    {

        activeLevelListItems = new List<GameObject>();
    }

    public override void Setup(CategoryInfo dataObject, bool isExpanded)
    {
        category = dataObject;
        textName.text = category.displayName.ToUpper();

        textName.text = category.displayName;
        iconImage.sprite = category.icon;
        backgroundImage.color = category.categoryColor;
        SetProgress(category);
        SetLocked(category);

        ShowPurdah(this.levelOfDifficult);
        if (isExpanded)
        {
            SetupLevelListItems();
        }
        else
        {
            //OnItemClicked();
        }
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

    public override void Collapsed()
    {
        ReturnLevelListItemsToPool();
    }

    public override void Removed()
    {
        ReturnLevelListItemsToPool();
    }

    public void SetLevelListItemPool(ObjectPool levelListItemPool)
    {
        this.levelListItemPool = levelListItemPool;
    }

    public override void ItemClicked()
    {
        // OnItemClicked();
    }

    public void OnItemClicked()
    {
        // Debug.Log("click: " + category.displayName);
        // Don't call Expand or collapse while the handler is expanding an item, the handler will just ignore those calls
        // Debug.Log("IsExpanded: " + IsExpanded);
        if (ExpandableListHandler.IsExpandingOrCollapsing)
        {
            return;
        }
        // Debug.Log("IsExpanded: " + IsExpanded);
        if (IsExpanded)
        {
            // If the item is expanded then collapse it
            Collapse();
        }
        else
        {

            // Debug.Log(GameManager.Instance.IsCategoryLocked(category));
            if (GameManager.Instance.IsCategoryLocked(category))
            {
                PopupContainer.Instance.ShowUnlockCategoryPopup(this.category);
            }
            else
            {
                // Set the level list items
                // Đặt các mục danh sách cấp độ
                SetupLevelListItems();

                // Force the level list container to resize
                // Buộc vùng chứa danh sách mức thay đổi kích thước
                LayoutRebuilder.ForceRebuildLayoutImmediate(levelListContent);

                // Expand this pack item so all the levels appear
                // Mở rộng mục gói này để tất cả các cấp xuất hiện
                content.sizeDelta = new Vector2(content.sizeDelta.x, levelListContent.rect.height);
                Expand(levelListContent.rect.height);
            }
        }
    }
    private void SetupLevelListItems()
    {
        ReturnLevelListItemsToPool();
        for (int i = levelOfDifficult * 10; i < 10 + levelOfDifficult * 10; i++)
        {
            // TextAsset levelData = category.levelFiles[i];
            ListLevelTest levelListItem = levelListItemPool.GetObject<ListLevelTest>(levelListContent);

            levelListItem.Setup(category, i);

            levelListItem.OnLevelItemSelected = OnLevelItemSelected;

            activeLevelListItems.Add(levelListItem.gameObject);
        }
    }

    private void ReturnLevelListItemsToPool()
    {
        for (int i = 0; i < activeLevelListItems.Count; i++)
        {
            ObjectPool.ReturnObjectToPool(activeLevelListItems[i]);
        }

        activeLevelListItems.Clear();
    }

    private void OnLevelItemSelected(int levelIndex)
    {
        GameManager.Instance.ActiveCategoryInfo = category;
        GameManager.Instance.StartLevel(category, levelIndex);
    }



    public void SetLevelOfDifficult(int difficult)
    {
        this.levelOfDifficult = difficult;
        ShowPurdah(this.levelOfDifficult);
        Refresh();
    }

    private void ShowPurdah(int difficult)
    {
        purdahEasy.SetActive(difficult != 0);
        purdahMedium.SetActive(difficult != 1);
        purdahHard.SetActive(difficult != 2);
    }

}




