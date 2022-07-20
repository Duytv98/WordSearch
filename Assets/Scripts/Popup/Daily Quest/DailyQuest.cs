using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;

public class DailyQuest : MonoBehaviour, IEnhancedScrollerDelegate
{

    [SerializeField] private DataToday dataToday = null;
    private List<Quest> _data;
    [SerializeField] private EnhancedScroller scroller;
    [SerializeField] private EnhancedScrollerCellView cellViewPrefab;

    [SerializeField] private Image mask;
    private int maximum = 5;
    public void OnShowing()
    {
        _data = dataToday.GetListQuestToday();
        scroller.Delegate = this;
        UpdateCureentFill();
    }
    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return _data.Count;
    }
    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 133f;
    }
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        ItemQuest cellView = scroller.GetCellView(cellViewPrefab) as ItemQuest;
        cellView.name = "Quest " + dataIndex.ToString();
        cellView.SetData(_data[dataIndex]);

        cellView.buttonclicked = CollectedGift;
        return cellView;
    }

    public void CollectedGift(int idQuest)
    {
        Debug.Log("click nhe");
    }


    public void UpdateCureentFill()
    {
        int current = 0;
        foreach (var quest in _data)
        {
            if (quest.isCompleted) current++;
        }
        this.mask.fillAmount = GetCureentFill(current);
    }

    private float GetCureentFill(float current)
    {
        return current / maximum;
    }
}
