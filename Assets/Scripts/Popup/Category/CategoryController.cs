using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EnhancedUI.EnhancedScroller;
public class CategoryController : MonoBehaviour, IEnhancedScrollerDelegate
{
    private List<CategoryInfo> _data;
    private int _selectedIndex = 0;
    [SerializeField] private EnhancedScroller scroller;
    [SerializeField] private EnhancedScrollerCellView cellViewPrefab;
    private bool isJump = false;

    public void Initialize(List<CategoryInfo> categoryInfos)
    {
        _data = categoryInfos;
        _data[_selectedIndex].Selected = true;
        CategoryScreen.Instance.ShowLevel(_selectedIndex, false);
        scroller.Delegate = this;
        scroller.ReloadData();
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        // in this example, we just pass the number of our data elements
        return _data.Count;
    }


    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 129f;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        ItemCategory cellView = scroller.GetCellView(cellViewPrefab) as ItemCategory;
        cellView.name = _data[dataIndex].saveId.ToString();

        cellView.selected = CellViewSelected;
        cellView.SetData(dataIndex, _data[dataIndex]);

        return cellView;
    }

    private void CellViewSelected(EnhancedScrollerCellView cellView)
    {
        if (cellView == null) return;
        else
        {
            _selectedIndex = (cellView as ItemCategory).DataIndex;
            for (var i = 0; i < _data.Count; i++)
            {
                _data[i].Selected = i == _selectedIndex;
            }
            var itemCategory = cellView as ItemCategory;
            if (itemCategory.isCategoryLocked) PopupContainer.Instance.ShowUnlockCategoryPopup(_data[_selectedIndex]);

            ShowInfoCategory(itemCategory, itemCategory.isCategoryLocked);

            scroller.RefreshActiveCellViews();
            scroller.JumpToDataIndex(_selectedIndex, 0.5f, 0.5f, true, EnhancedScroller.TweenType.easeOutSine, 0.2f, null, EnhancedScroller.LoopJumpDirectionEnum.Closest);
        }
    }
    private void ShowInfoCategory(ItemCategory itemCategory, bool isActive)
    {
        CategoryScreen.Instance.ShowLevel(_selectedIndex, isActive);
    }

    public void OnMoveRight()
    {
        if (isJump) return;
        isJump = true;
        var endIndexView = scroller.EndCellViewIndex;
        var maxIndexData = _data.Count - 1;
        var jumpDataIndex = (endIndexView + 5) > maxIndexData ? maxIndexData : (endIndexView + 5);
        scroller.JumpToDataIndex(jumpDataIndex, 1f, 0f, true, EnhancedScroller.TweenType.easeOutSine, 0.2f, MoveComplete, EnhancedScroller.LoopJumpDirectionEnum.Closest);

    }

    public void OnMoveLeft()
    {
        if (isJump) return;
        isJump = true;
        var startIndexView = scroller.StartCellViewIndex;
        var maxIndexData = _data.Count - 1;
        var jumpDataIndex = (startIndexView - 5) < 0 ? 0 : (startIndexView - 5);

        scroller.JumpToDataIndex(jumpDataIndex, 0f, 0f, true, EnhancedScroller.TweenType.easeOutSine, 0.2f, MoveComplete, EnhancedScroller.LoopJumpDirectionEnum.Closest);

    }
    private void MoveComplete()
    {
        isJump = false;
    }
    public void Refresh()
    {
        scroller.ReloadData();
    }
}
