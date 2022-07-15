using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using EnhancedUI.EnhancedScroller;
using EnhancedUI;
using UnityEngine.UI;

public class Leaderboard : MonoBehaviour, IEnhancedScrollerDelegate
{

    [SerializeField] private LeaderboardController leaderboardController = null;
    [SerializeField] private RectTransform placesYou = null;
    [SerializeField] private RectTransform rectPosition = null;
    [SerializeField] private RankForYou rankForYou = null;

    private List<PlayerLB> _data;

    public EnhancedScroller vScroller;
    public EnhancedScrollerCellView vCellViewPrefab;



    public void OnShowing()
    {


        _data = leaderboardController.Data;
        rankForYou.SetData(leaderboardController.IndexPlayer, _data[leaderboardController.IndexPlayer]);


        placesYou.DOAnchorPos(rectPosition.anchoredPosition, 0.5f)
        .SetDelay(0.35f)
        .SetEase(Ease.OutBack);


        // foreach (var item in leaderboardController.Data)
        // {
        //     _data.Add(item);
        // }
        Debug.Log(_data.Count);
        vScroller.Delegate = this;
        vScroller.ReloadData();
        // _data = leaderboardController.Data.ToList();
    }
    public void Close()
    {
        placesYou.anchoredPosition = new Vector3(0, 135f, 0f);
    }
    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return _data.Count;
    }
    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 149;
    }
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        // first get a cell from the scroller. The scroller will recycle if it can.
        // We want a vertical cell prefab for the vertical scroller and a horizontal
        // prefab for the horizontal scroller.
        ItemLeaderBoard cellView = scroller.GetCellView(vCellViewPrefab) as ItemLeaderBoard;

        // set the name of the cell. This just makes it easier to see in our
        // hierarchy what the cell is
        // cellView.name = (scroller == vScroller ? "Vertical" : "Horizontal") + " " + _data[dataIndex].itemName;

        // set the selected callback to the CellViewSelected function of this controller. 
        // this will be fired when the cell's button is clicked
        // cellView.selected = CellViewSelected;

        // set the data for the cell
        cellView.SetData(dataIndex, _data[dataIndex], leaderboardController.IndexPlayer);

        // return the cell view to the scroller
        return cellView;
    }

}
