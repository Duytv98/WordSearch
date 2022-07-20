using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SelectCategoryPopup : MonoBehaviour
{

    public static SelectCategoryPopup Instance;
    [SerializeField] CategoryController categoryController;
    [SerializeField] LevelController levelController;
    [SerializeField] DataController dataController;
    [SerializeField] TextMeshProUGUI txtHeader;

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
    public void OnShowing()
    {
        categoryController.Initialize(dataController.CategoryInfos);
    }


    public void ShowLevel(int selectedIndex, bool isActive)
    {
        indexCategory = selectedIndex;
        categoryInfo = dataController.CategoryInfos[indexCategory];
        txtHeader.text = categoryInfo.displayName;
        levelController.Initialize(categoryInfo.levelFiles.Count, categoryInfo.saveId, isActive);
    }
    public void SelectedLevel(int indexLevel)
    {
        this.indexLevel = indexLevel;
        DataController.Instance.ActiveCategoryInfo = categoryInfo;
        GameManager.Instance.StartLevel(categoryInfo, this.indexLevel);
        Close();
    }
    public void RefreshCategoryScroller()
    {
        levelController.Active();
        categoryController.Refresh();
    }
    private void Close()
    {
        PopupContainer.Instance.CloseCurrentPopup();
    }
}
