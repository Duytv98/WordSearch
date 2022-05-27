using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// using PolyAndCode.UI;
public class HomeScreen : MonoBehaviour
{
    [SerializeField] private string id = "main";

    public void Initialize()
    {

    }

    public void OnSelectCategory()
    {
        // Debug.Log("OnSelectCategory");
        // ScreenManager.Instance.ShowScreenLevel();
        ScreenManager.Instance.Show("levels");
        AudioManager.Instance.Play_Click_Button_Sound();
    }
    public void OnPlayNextLevelRandomCategory()
    {
        List<int> listIndexCategorys = new List<int>();
        List<CategoryInfo> categoryInfos = GameManager.Instance.CategoryInfos;
        for (int i = 0; i < categoryInfos.Count; i++)
        {
            bool isCategoryLocked = GameManager.Instance.IsCategoryLocked(categoryInfos[i]);

            if (!isCategoryLocked) listIndexCategorys.Add(i);
        }
        int indexCategory = listIndexCategorys[Random.Range(0, listIndexCategorys.Count)];

        GameManager.Instance.ActiveCategoryInfo = categoryInfos[indexCategory];

        GameManager.Instance.StartNextLevel(categoryInfos[indexCategory]);

        AudioManager.Instance.Play_Click_Button_Sound();
    }
}