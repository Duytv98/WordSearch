using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
// using PolyAndCode.UI;
public class HomeScreen : MonoBehaviour
{
    [SerializeField] private string id = "main";
    [SerializeField] private Text txtCoins;
    [SerializeField] private Text txtKeys;

    private string idCollect = null;


    public void Initialize()
    {
        Debug.Log(DataController.Instance.Coins);
        txtCoins.text = DataController.Instance.Coins.ToString();
        txtKeys.text = DataController.Instance.Keys.ToString();
    }

    public void ShowDailyGift()
    {
        PopupContainer.Instance.ShowDailyGift();
    }
    public void ShowDailyQuest()
    {

        PopupContainer.Instance.ShowDailyQuest();
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