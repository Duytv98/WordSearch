using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using EnhancedUI.EnhancedScroller;

public delegate void SelectedDelegate(EnhancedScrollerCellView cellView);
public class ItemCategory : EnhancedScrollerCellView
{

    private CategoryInfo _data;
    public int DataIndex { get; private set; }

    public SelectedDelegate selected;

    [SerializeField] private Sprite bgSelected;
    [SerializeField] private Sprite bgUnselected;
    [SerializeField] private Sprite bgNotSelect;
    [SerializeField] private GameObject space;
    [SerializeField] private Image bg;
    [SerializeField] private Image icon;

    public bool isCategoryLocked;
    void OnDestroy()
    {
        if (_data != null)
        {
            _data.selectedChanged -= SelectedChanged;
        }
    }
    public void SetData(int dataIndex, CategoryInfo data)
    {
        // set the underlying data source. This is used when
        // we need to refresh the cell view
        _data = data;
        DataIndex = dataIndex;
        icon.sprite = _data.icon;
        isCategoryLocked = DataController.Instance.IsCategoryLocked(_data);
        icon.SetNativeSize();

        // update the UI text with the cell data
        // someTextText.text = _data.someText;

        // call the refresh method which just sets the selection highlight

        _data.selectedChanged -= SelectedChanged;
        _data.selectedChanged += SelectedChanged;
        RefreshCellView();
    }

    /// <summary>
    /// Called when the selected cell index is changed in the controller
    /// </summary>
    public override void RefreshCellView()
    {
        SelectedChanged(_data.Selected);
    }
    private void SelectedChanged(bool selected)
    {

        if (selected)
        {
            bg.sprite = isCategoryLocked ? bgNotSelect : bgSelected;
            icon.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            (icon.transform as RectTransform).anchoredPosition = new Vector2(0, -74.6f);
        }
        else
        {
            bg.sprite = isCategoryLocked ? bgNotSelect : bgUnselected;
            icon.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            (icon.transform as RectTransform).anchoredPosition = new Vector2(0, -57f);

            // backgroundImage.color = _data.isSelected ? selectedColor : unselectedColor;
        }
        space.SetActive(selected);
    }
    public void OnSelected()
    {
        if (selected != null) selected(this);
    }
}