using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
// using PolyAndCode.UI;
public class HomeScreen : MonoBehaviour
{
    [SerializeField] private string id = "main";

    private string idCollect = null;




    public void OnSelectCategory()
    {

        ScreenManager.Instance.Show("levels");
    }
    public void ShowDailyGift()
    {
        PopupContainer.Instance.ShowDailyGift();
    }
    public void ShowDailyPuzzle()
    {

        ScreenManager.Instance.Show("dailyPuzzle");
    }
    public void OnPlayNextLevelRandomCategory()
    {
        List<int> listIndexCategorys = new List<int>();
        List<CategoryInfo> categoryInfos = DataController.Instance.CategoryInfos;
        for (int i = 0; i < categoryInfos.Count; i++)
        {
            bool isCategoryLocked = DataController.Instance.IsCategoryLocked(categoryInfos[i]);

            if (!isCategoryLocked) listIndexCategorys.Add(i);
        }
        int indexCategory = listIndexCategorys[UnityEngine.Random.Range(0, listIndexCategorys.Count)];

        DataController.Instance.ActiveCategoryInfo = categoryInfos[indexCategory];

        Debug.Log(categoryInfos.Count);
        GameManager.Instance.StartNextLevel(categoryInfos[indexCategory]);
    }
}