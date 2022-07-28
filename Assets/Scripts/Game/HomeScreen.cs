using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine;
using Spine.Unity;
// using PolyAndCode.UI;
public class HomeScreen : MonoBehaviour
{
    [SerializeField] private string id = "main";
    [SerializeField] private Text txtCoins;
    [SerializeField] private Text txtKeys;
    [SerializeField] private PopupContainer popupContainer;

    [SerializeField] private SkeletonGraphic animation_Logo;

    private bool isPlayAnimationLogo = true;

    public bool IsPlayAnimationLogo { get => isPlayAnimationLogo; set => isPlayAnimationLogo = value; }

    public void Initialize()
    {
        txtCoins.text = DataController.Instance.Coins.ToString();
        txtKeys.text = DataController.Instance.Keys.ToString();
        animation_Logo.AnimationState.ClearTrack(0);
        animation_Logo.AnimationState.SetAnimation(0, "moving", false);
        animation_Logo.AnimationState.AddAnimation(0, "idle", false, 5f);
        animation_Logo.AnimationState.End += HandleEvent;
    }
    void HandleEvent(TrackEntry trackEntry)
    {
        if (IsPlayAnimationLogo) animation_Logo.AnimationState.AddAnimation(0, "idle", false, 3f);
    }


    public void ShowDailyGift()
    {
        popupContainer.ShowDailyGift();
    }
    public void ShowDailyQuest()
    {

        popupContainer.ShowDailyQuest();
    }
    public void ShowProfile()
    {

        popupContainer.ShowLoginPopup();
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