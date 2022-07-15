using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
public class LevelCompletePopup : MonoBehaviour, IEnhancedScrollerDelegate
{
    [Space]
    [SerializeField] private GameObject rewardsContainer = null;
    [SerializeField] private GameObject coinRewardContainer = null;

    [SerializeField] private RectTransform leaderBoard = null;
    [SerializeField] private RectTransform rectPosition = null;


    [Space]

    [SerializeField] private LeaderboardController leaderboardController = null;
    private List<PlayerLB> _data;
    public EnhancedScroller hScroller;
    public EnhancedScrollerCellView cellViewPrefab;


    public EnhancedScroller.TweenType hScrollerTweenType = EnhancedScroller.TweenType.immediate;
    public float hScrollerTweenTime = 0f;

    private bool playmode = false;
    public void OnShowing(int coinsAwarded, int keysAwarded)
    {
        Debug.Log("OnShowing");
        bool awardCoins = coinsAwarded > 0;
        bool awardKeys = keysAwarded > 0;

        coinRewardContainer.SetActive(awardCoins);

        // bool allLevelsCompleted = GameManager.Instance.AllLevelsComplete(DataController.Instance.ActiveCategoryInfo);

        leaderBoard.DOAnchorPos(rectPosition.anchoredPosition, 0.5f)
        .SetDelay(0.35f)
        .SetEase(Ease.OutBack)
        .OnComplete(() =>
        {
            JumpButton_OnClick();
        });

        _data = leaderboardController.Data;

        hScroller.Delegate = this;
        hScroller.ReloadData();
        hScroller.JumpToDataIndex(leaderboardController.Data.Count - 1, 0.5f, 0.5f, true, hScrollerTweenType, 0);

    }


    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        // in this example, we just pass the number of our data elements
        return _data.Count;
    }


    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 97f;
    }
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        // first, we get a cell from the scroller by passing a prefab.
        // if the scroller finds one it can recycle it will do so, otherwise
        // it will create a new cell.
        SmallItemLeaderBoard cellView = scroller.GetCellView(cellViewPrefab) as SmallItemLeaderBoard;

        // set the name of the game object to the cell's data index.
        // this is optional, but it helps up debug the objects in
        // the scene hierarchy.
        cellView.name = "Player " + (dataIndex + 1).ToString();

        // // in this example, we just pass the data to our cell's view which will update its UI

        cellView.SetData(dataIndex, _data[dataIndex], leaderboardController.IndexPlayer);

        // return the cell to the scroller
        return cellView;
    }


    public void JumpButton_OnClick()
    {
        int jumpDataIndex = leaderboardController.IndexPlayer;

        hScroller.JumpToDataIndex(jumpDataIndex, 0.5f, 0.5f, true, hScrollerTweenType, hScrollerTweenTime);

    }














    private void close()
    {
        leaderBoard.anchoredPosition = new Vector3(-5, -245f, 0f);
    }
    public void CloseLevelCompletePopup()
    {
        PopupContainer.Instance.ClosePopup("LevelCompletePopup");
        close();
    }
    public void OnClickNextLevel()
    {
        GameManager.Instance.StartNextLevel(DataController.Instance.ActiveCategoryInfo);
        PopupContainer.Instance.CloseCurrentPopup();
        close();

    }
    public void OnClickBackToCategoriesButton()
    {
        ScreenManager.Instance.BackToHome();
        PopupContainer.Instance.CloseCurrentPopup();

    }
}
