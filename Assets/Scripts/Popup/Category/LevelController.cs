using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using EnhancedUI;
using UnityEngine;
using UnityEngine.UI;

using EnhancedUI.EnhancedScroller;
public class LevelController : MonoBehaviour, IEnhancedScrollerDelegate
{

    private SmallList<int> _data;
    private int maxCount = 0;
    private string idCategory;
    [SerializeField] private EnhancedScroller scroller;
    [SerializeField] private EnhancedScrollerCellView cellViewPrefab;

    [SerializeField] private Image mask;


    public void Initialize(int count, string idCategory, bool isActive)
    {
        maxCount = count;
        this.idCategory = idCategory;
        scroller.Delegate = this;
        LoadData((int)Mathf.Ceil((float)count / 8));
        mask.gameObject.SetActive(isActive);
    }
    private void LoadData(int count)
    {
        _data = new SmallList<int>();
        for (var i = 0; i < count; i++)
            _data.Add(i);
        scroller.ReloadData();
    }
    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return _data.Count;
    }
    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 1087f;
    }
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        ItemComboLevel cellView = scroller.GetCellView(cellViewPrefab) as ItemComboLevel;
        cellView.name = "Cell " + dataIndex.ToString();
        cellView.SetData(_data[dataIndex], maxCount, idCategory);
        return cellView;
    }

    public void Active()
    {
        mask.gameObject.SetActive(false);
    }
}
