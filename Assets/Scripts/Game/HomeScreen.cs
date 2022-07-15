using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
// using PolyAndCode.UI;
public class HomeScreen : MonoBehaviour
{
    [SerializeField] private string id = "main";
    [SerializeField] private GiftsFast giftsFast = null;
    [SerializeField] private DailyGift dailyGift = null;
    private Tuple<string, Booter> infoGift = null;

    private string idCollect = null;

    public void Initialize()
    {
        StartCoroutine(CheckDailyGift());
    }

    private IEnumerator CheckDailyGift()
    {
        var tuple = dailyGift.GetGiftDay();
        yield return new WaitForSeconds(1f);
        if (tuple != null)
        {
            infoGift = tuple;
            PopupContainer.Instance.ShowGiftsFastPopup(infoGift);
        }
        // casualGame.StartCasual(0);
    }
    public void CollectionGift()
    {
        dailyGift.CollectionFastGift(infoGift);
        giftsFast.Close(-1);
    }


    public void OnSelectCategory()
    {

        ScreenManager.Instance.Show("levels");
    }
    public void ShowDailyGift()
    {

        // ScreenManager.Instance.Show("dailyGift");
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