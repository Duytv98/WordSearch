using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CategoryScreen : MonoBehaviour
{

    public static CategoryScreen Instance;
    [SerializeField] CategoryController categoryController;
    [SerializeField] LevelController levelController;
    [SerializeField] DataController dataController;
    [SerializeField] RectTransform content;
    [SerializeField] RectTransform bg;
    private int indexCategory = 0;
    private CategoryInfo categoryInfo;
    private int indexLevel = 0;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    public void Initialize()
    {
        Show();
        categoryController.Initialize(dataController.CategoryInfos);
    }
    public void ShowLevel(int selectedIndex, bool isActive)
    {
        indexCategory = selectedIndex;
        categoryInfo = dataController.CategoryInfos[indexCategory];
        // txtHeader.text = categoryInfo.displayName;
        levelController.Initialize(categoryInfo.levelFiles.Count, categoryInfo.saveId, isActive);
    }
    public void SelectedLevel(int indexLevel)
    {
        this.indexLevel = indexLevel;
        DataController.Instance.ActiveCategoryInfo = categoryInfo;
        GameManager.Instance.StartLevel(categoryInfo, this.indexLevel);
        // Close();
    }
    public void RefreshCategoryScroller()
    {
        levelController.Active();
        categoryController.Refresh();
    }
    public void Show()
    {
        content.gameObject.SetActive(true);
        bg.gameObject.SetActive(true);
        content.DOAnchorPosY(0, 0.7f);
        // .SetEase(Ease.OutBack);
        bg.DOAnchorPosY(0, 0.7f);
        // .SetEase(Ease.OutBack);
    }
}
