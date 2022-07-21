using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnhancedUI.EnhancedScroller;

public class DailyQuest : MonoBehaviour, IEnhancedScrollerDelegate
{

    [SerializeField] private DataToday dataToday = null;
    private List<Quest> _data;
    [SerializeField] private EnhancedScroller scroller;
    [SerializeField] private EnhancedScrollerCellView cellViewPrefab;

    [SerializeField] private Image mask;
    [SerializeField] private TextMeshProUGUI txtProgress;

    [SerializeField] private Sprite btnPlay;
    [SerializeField] private Sprite btnCollect;
    [SerializeField] private Sprite btnCollected;

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
        var quest = _data[dataIndex];
        var status = GetButtonStatus(quest);
        cellView.SetData(dataIndex, _data[dataIndex], GetButtonSprite(status), status);

        cellView.buttonclicked = CollectedGift;
        return cellView;
    }

    public void CollectedGift(int idQuest)
    {
        var quest = _data[idQuest];
        Debug.Log(idQuest);
        Debug.Log(quest.name);
        var status = GetButtonStatus(quest);
        Debug.Log(status);
        if (status.Equals("Collect"))
        {
            if (quest.giftType == Quest.GiftType.Coins) DataController.Instance.SetCoins(quest.amountGift);
            else DataController.Instance.SetKeys(quest.amountGift);
            quest.isCollect = true;
            dataToday.UpdateListQuestToday();
            scroller.ReloadData();
        }
        else if (status.Equals("play"))
        {
            PopupContainer.Instance.CloseCurrentPopup();
        }

        Debug.Log("click nhe");
    }


    public void UpdateCureentFill()
    {
        int current = 0;
        foreach (var quest in _data)
        {
            if (quest.isCompleted) current++;
        }
        txtProgress.text = string.Format("{0}/{1}", current, maximum);
        this.mask.fillAmount = GetCureentFill(current);
    }

    private float GetCureentFill(float current)
    {
        return current / maximum;
    }
    private string GetButtonStatus(Quest quest)
    {
        if (!quest.isCompleted && !quest.isCollect)
        {
            return "play";
        }
        else if (quest.isCompleted && !quest.isCollect)
        {
            return "Collect";
        }
        else if (!quest.isCompleted && quest.isCollect)
        {
            return null;
        }
        else return "Collected";
    }
    private Sprite GetButtonSprite(string status)
    {
        switch (status)
        {
            case "play": return btnPlay;
            case "Collect": return btnCollect;
            case "Collected": return btnCollected;
            default: return null;
        }
    }
}
