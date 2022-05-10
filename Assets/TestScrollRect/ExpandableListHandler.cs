using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ExpandableListHandler<T>
{
    #region Classes

    private class Animation
    {
        private RectTransform target;
        private int index;
        private float timer;
        private float from;
        private float to;
    }

    #endregion

    #region Inspector Variables

    #endregion

    #region Member Variables

    private List<T> dataObjects;
    private ExpandableListItem<T> listItemPrefab;
    private RectTransform listContainer;
    private ScrollRect listScrollRect;
    private float expandAnimDuration;

    private ObjectPool listItemPool;
    private List<RectTransform> listItemPlaceholders;
    private int topItemIndex;
    private int bottomItemIndex;
    private int expandedItemIndex;
    private float expandedHeight;

    #endregion

    #region Properties

    public System.Action<ExpandableListItem<T>> OnItemCreated { get; set; }
    public bool IsExpandingOrCollapsing { get; private set; }

    private Vector2 ListItemSize { get { return listItemPrefab.RectT.sizeDelta; } }

    #endregion

    #region Constructor

    public ExpandableListHandler(List<T> dataObjects, ExpandableListItem<T> listItemPrefab, RectTransform listContainer, ScrollRect listScrollRect, float expandAnimDuration)
    {
        this.dataObjects = dataObjects;
        // Debug.Log("Defaut ListItemSize: " + ListItemSize);
        this.listItemPrefab = listItemPrefab;
        // Debug.Log("Defaut ListItemSize: " + ListItemSize);
        this.listContainer = listContainer;
        this.listScrollRect = listScrollRect;
        this.expandAnimDuration = expandAnimDuration;
        //Tạo Pool Container lưu trữ các category chưa sử dụng đến 
        listItemPool = new ObjectPool(listItemPrefab.gameObject, 0, ObjectPool.CreatePoolContainer(listContainer));
        listItemPlaceholders = new List<RectTransform>();
    }

    #endregion

    #region Public Methods

    public void Setup()
    {
        //gọi hàm OnListScrolled khi vị trí các con của listScrollRect
        listScrollRect.onValueChanged.AddListener(OnListScrolled);
        //Tạo ra list_item trống để chứa các category // Được add vào list listItemPlaceholders
        CreateListItemPlaceholders();

        LayoutRebuilder.ForceRebuildLayoutImmediate(listContainer);

        Reset();
    }

    public void Reset()
    {
        listContainer.anchoredPosition = Vector2.zero;

        for (int i = 0; i < listItemPlaceholders.Count; i++)
        {
            RectTransform placeholder = listItemPlaceholders[i];
            RemoveListItem(placeholder);
            placeholder.sizeDelta = ListItemSize;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(listContainer);

        expandedItemIndex = -1;

        UpdateList(true);
    }

    public void ExpandListItem(int index, float extraHeight)
    {
        // Debug.Log(" ExpandListItem: ");
        // Debug.Log(" mở rộng list level: " + expandedItemIndex);

        if (IsExpandingOrCollapsing)
        {
            return;
        }

        if (expandedItemIndex != -1)
        {
            CollapseListItem(expandedItemIndex, true);
        }

        IsExpandingOrCollapsing = true;

        RectTransform placeholder = listItemPlaceholders[index];
        // Debug.Log(ListItemSize.y);
        float toHeight = ListItemSize.y + 120 + extraHeight;

        ScrollToMiddle(placeholder, toHeight, index);

        expandedItemIndex = index;
        expandedHeight = toHeight;

        UIAnimation animation = UIAnimation.Height(placeholder, expandedHeight, expandAnimDuration);

        listScrollRect.velocity = Vector2.zero;
        listScrollRect.enabled = false;

        animation.OnAnimationFinished += (GameObject obj) =>
        {
            listScrollRect.enabled = true;
            IsExpandingOrCollapsing = false;
        };

        PlayAnimation(animation);

        CoroutineStarter.Start(UpdateListWhileAnimating());

        SetExpanded(placeholder, true);
    }

    public void CollapseListItem(int index, bool isExpandingNewItem = false)
    {
        Debug.Log(" Thu gọn list level: ");
        if (IsExpandingOrCollapsing || expandedItemIndex != index)
        {
            return;
        }
        // Debug.Log("CollapseListItem: ");
        IsExpandingOrCollapsing = true;

        RectTransform placeholder = listItemPlaceholders[index];

        UIAnimation animation = UIAnimation.Height(placeholder, ListItemSize.y, expandAnimDuration);

        animation.OnAnimationFinished += (GameObject obj) =>
        {
            if (placeholder.childCount == 1)
            {
                placeholder.GetChild(0).GetComponent<ExpandableListItem<T>>().Collapsed();
            }
        };

        if (!isExpandingNewItem)
        {
            // If we are not expanding a new list item then scroll this list item to the middle of the screen
            ScrollToMiddle(placeholder, ListItemSize.y, index);
            expandedItemIndex = -1;

            listScrollRect.velocity = Vector2.zero;
            listScrollRect.enabled = false;

            animation.OnAnimationFinished += (GameObject obj) =>
            {
                listScrollRect.enabled = true;
                IsExpandingOrCollapsing = false;
            };

            CoroutineStarter.Start(UpdateListWhileAnimating());
        }

        PlayAnimation(animation);

        SetExpanded(placeholder, false);
    }

    public void Refresh()
    {
        for (int i = topItemIndex; i <= bottomItemIndex; i++)
        {
            RectTransform placeholder = listItemPlaceholders[i];

            if (placeholder.childCount == 1)
            {
                ExpandableListItem<T> listItem = placeholder.GetChild(0).GetComponent<ExpandableListItem<T>>();

                listItem.IsExpanded = i == expandedItemIndex;
                listItem.Setup(dataObjects[i], i == expandedItemIndex);
            }
        }
    }

    public void ScrollToMiddle(int index)
    {
        Debug.Log("ScrollToMiddle: ");
        ScrollToMiddle(listItemPlaceholders[index], ListItemSize.y, index, false);
        UpdateList();
        ExpandableListItem<T> listItem = listItemPlaceholders[index].GetChild(0).GetComponent<ExpandableListItem<T>>();
        listItem.ItemClicked();
    }

    #endregion

    #region Private Methods

    private void OnListScrolled(Vector2 pos)
    {
        // Debug.Log("OnListScrolled: ");
        UpdateList();
    }

    private void CreateListItemPlaceholders()
    {
        for (int i = 0; i < dataObjects.Count; i++)
        {
            GameObject placeholder = new GameObject("list_item");
            RectTransform placholderRectT = placeholder.AddComponent<RectTransform>();

            placholderRectT.SetParent(listContainer, false);
            // Debug.Log("ListItemSize: " + ListItemSize);

            placholderRectT.sizeDelta = ListItemSize;

            listItemPlaceholders.Add(placholderRectT);
        }
    }

    private void UpdateList(bool reset = false)
    {
        // Debug.Log("reset: " + reset);
        if (reset)
        {
            topItemIndex = 0;
            //Tìm ra các Placeholders rỗng hiển thị trên màn hình và thêm category vào từng item và đổ data vào từng item category
            //Hàm trả vê index Placeholders cuối cúng hiển thị
            bottomItemIndex = FillList(topItemIndex, 1);
        }
        else
        {
            RecycleList();

            topItemIndex = FillList(topItemIndex, -1);
            bottomItemIndex = FillList(bottomItemIndex, 1);
        }
    }

    private int FillList(int startIndex, int indexInc)
    {
        // Debug.Log("startIndex: " + startIndex + "   indexInc: " + indexInc + "   listItemPlaceholders.Count: " + listItemPlaceholders.Count);
        int lastVisibleIndex = startIndex;

        for (int i = startIndex; i >= 0 && i < listItemPlaceholders.Count; i += indexInc)
        {
            RectTransform placeholder = listItemPlaceholders[i];
            //Kiểm tra nếu gặp item nào k nằm trên màn hình sẽ thoát khỏi vòng for
            if (!IsVisible(i, placeholder))
            {
                break;
            }

            lastVisibleIndex = i;

            if (placeholder.childCount == 0)
            {
                //add list item vào từng Placeholders
                //Set up, truyền data vào từng item category
                AddListItem(i, placeholder, indexInc == -1);
            }
        }

        return lastVisibleIndex;
    }

    private void RecycleList()
    {
        // Debug.Log("topItemIndex: " + topItemIndex + "   bottomItemIndex: " + bottomItemIndex);


        //kiểm tra nếu phần tử trên cùng vị trí topItemIndex nằm ngoài màn hình tiến hành xóa trả nó về pool_container và tăng topItemIndex lên 1
        for (int i = topItemIndex; i <= bottomItemIndex; i++)
        {
            RectTransform placeholder = listItemPlaceholders[i];

            if (IsVisible(i, placeholder))
            {
                break;
            }
            else if (placeholder.childCount == 1)
            {

                RemoveListItem(placeholder);

                topItemIndex++;
            }
        }
        //Tương tự kiểm tra đối với việc kéo xuống sẽ giảm bottomItemIndex đi 1 nếu item đó bị kéo xuống quá khỏi tầm nhìn
        for (int i = bottomItemIndex; i >= topItemIndex; i--)
        {
            RectTransform placeholder = listItemPlaceholders[i];

            if (IsVisible(i, placeholder))
            {
                break;
            }
            else if (placeholder.childCount == 1)
            {
                RemoveListItem(placeholder);

                bottomItemIndex--;
            }
        }

        // Check if top index is now greater than bottom index, if so then all elements were recycled so we need to find the new top
        //Kiểm tra xem chỉ số trên cùng có lớn hơn chỉ số dưới cùng hay không, nếu vậy thì tất cả các phần tử đã được tái chế, vì vậy chúng ta cần tìm chỉ số trên cùng mới
        if (topItemIndex > bottomItemIndex)
        {
            int targetIndex = (topItemIndex < listItemPlaceholders.Count) ? topItemIndex : bottomItemIndex;
            RectTransform targetPlaceholder = listItemPlaceholders[targetIndex];
            float viewportTop = listContainer.anchoredPosition.y;

            if (-targetPlaceholder.anchoredPosition.y < viewportTop)
            {
                for (int i = targetIndex; i < listItemPlaceholders.Count; i++)
                {
                    if (IsVisible(i, listItemPlaceholders[i]))
                    {
                        topItemIndex = i;
                        bottomItemIndex = i;
                        break;
                    }
                }
            }
            else
            {
                for (int i = targetIndex; i >= 0; i--)
                {
                    if (IsVisible(i, listItemPlaceholders[i]))
                    {
                        topItemIndex = i;
                        bottomItemIndex = i;
                        break;
                    }
                }
            }
        }
    }

    private bool IsVisible(int index, RectTransform placeholder)
    {
        RectTransform viewport = listScrollRect.viewport as RectTransform;

        float placeholderTop = -placeholder.anchoredPosition.y - placeholder.rect.height / 2f;
        float placeholderbottom = -placeholder.anchoredPosition.y + placeholder.rect.height / 2f;

        float viewportTop = listContainer.anchoredPosition.y;
        float viewportbottom = listContainer.anchoredPosition.y + viewport.rect.height;

        return placeholderTop < viewportbottom && placeholderbottom > viewportTop;
    }


    //Set up, truyền data vào từng item category
    private void AddListItem(int index, RectTransform placeholder, bool addingOnTop)
    {
        bool itemInstantiated;

        ExpandableListItem<T> listItem = listItemPool.GetObject<ExpandableListItem<T>>(placeholder, out itemInstantiated);

        SetPlaceholderSize(index, placeholder, addingOnTop);

        listItem.RectT.anchorMin = Vector2.zero;
        listItem.RectT.anchorMax = Vector2.one;
        listItem.RectT.offsetMin = Vector2.zero;
        listItem.RectT.offsetMax = Vector2.zero;

        listItem.Index = index;
        listItem.ExpandableListHandler = this;

        if (itemInstantiated)
        {
            if (OnItemCreated != null)
            {
                OnItemCreated(listItem);
            }

            listItem.Initialize(dataObjects[index]);
        }

        listItem.IsExpanded = (index == expandedItemIndex);
        listItem.Setup(dataObjects[index], index == expandedItemIndex);
    }

    private void SetPlaceholderSize(int index, RectTransform placeholder, bool addingOnTop)
    {
        float expectedHeight = (index == expandedItemIndex) ? expandedHeight : ListItemSize.y;

        if (addingOnTop)
        {
            float offset = expectedHeight - placeholder.rect.height;

            listContainer.anchoredPosition = new Vector2(listContainer.anchoredPosition.x, listContainer.anchoredPosition.y + offset);
        }

        placeholder.sizeDelta = new Vector2(placeholder.sizeDelta.x, expectedHeight);

        LayoutRebuilder.MarkLayoutForRebuild(listContainer);
    }

    private void RemoveListItem(Transform placeholder)
    {
        if (placeholder.childCount == 1)
        {
            ExpandableListItem<T> listItem = placeholder.GetChild(0).GetComponent<ExpandableListItem<T>>();

            // Trả lại đối tượng về khu vự lưu trữ pool
            ObjectPool.ReturnObjectToPool(listItem.gameObject);

            // Thông báo rằng nó đã bị xóa khỏi danh sách
            listItem.Removed();
        }
    }

    /// <summary>
    /// Scrolls the given expanding placeholder to teh middle of the viewport
    /// </summary>
    private void ScrollToMiddle(RectTransform placeholder, float height, int index, bool animate = true)
    {
        float viewportMiddle = listContainer.anchoredPosition.y + listScrollRect.viewport.rect.height / 2f;
        float placeholderMiddle = -(placeholder.anchoredPosition.y + placeholder.rect.height / 2f) + height / 2f;

        if (expandedItemIndex != -1 && index > expandedItemIndex)
        {
            placeholderMiddle -= expandedHeight - ListItemSize.y;
        }

        float moveAmt = placeholderMiddle - viewportMiddle;

        // Make sure the list items top is not move passed to viewports top
        float viewportTop = listContainer.anchoredPosition.y;
        float placeholderTopAfterMove = placeholderMiddle - height / 2f - moveAmt;

        if (placeholderTopAfterMove < viewportTop)
        {
            moveAmt -= viewportTop - placeholderTopAfterMove;
        }

        // Make sure the move amount doesn't move the containers top/bottom edge past the viewport bounds
        if (moveAmt > 0)
        {
            float listHeight = (expandedItemIndex == -1) ? listContainer.rect.height + height - ListItemSize.y : listContainer.rect.height;
            float viewportBottom = listHeight - listContainer.anchoredPosition.y - listScrollRect.viewport.rect.height;

            if (moveAmt > viewportBottom)
            {
                moveAmt = viewportBottom;
            }
        }
        else if (moveAmt < 0 && Mathf.Abs(moveAmt) > listContainer.anchoredPosition.y)
        {
            moveAmt = -listContainer.anchoredPosition.y;
        }

        float toPos = listContainer.anchoredPosition.y + moveAmt;

        if (animate)
        {
            PlayAnimation(UIAnimation.PositionY(listContainer, toPos, expandAnimDuration));
        }
        else
        {
            listContainer.anchoredPosition = new Vector2(listContainer.anchoredPosition.x, toPos);
        }
    }

    private void PlayAnimation(UIAnimation anim)
    {
        anim.style = UIAnimation.Style.EaseOut;
        anim.startOnFirstFrame = true;
        anim.Play();
    }

    private IEnumerator UpdateListWhileAnimating()
    {
        // Debug.Log("UpdateListWhileAnimating: ");
        while (IsExpandingOrCollapsing)
        {
            LayoutRebuilder.MarkLayoutForRebuild(listContainer);

            yield return new WaitForEndOfFrame();

            UpdateList();
        }
    }

    private void SetExpanded(RectTransform placeholder, bool isExpanded)
    {
        if (placeholder.childCount == 1)
        {
            placeholder.GetChild(0).GetComponent<ExpandableListItem<T>>().IsExpanded = isExpanded;
        }
    }

    #endregion
}

