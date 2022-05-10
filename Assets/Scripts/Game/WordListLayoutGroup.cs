using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class WordListLayoutGroup : LayoutGroup
{
    #region Insepector Variables

    public int spacing;
    public int rows;

    #endregion

    #region Unity Methods

    // Vào 1 // Luồng 1
    public override void CalculateLayoutInputHorizontal()
    {
        // Debug.Log("CalculateLayoutInputHorizontal");
        CalculateLayoutInputForAxis(0);
    }
    // Vào 1 // Luồng 2
    public override void SetLayoutHorizontal()
    {
        // Debug.Log("SetLayoutHorizontal");
        SetLayoutAlongAxis(0);
    }

    // Vào 1 // Luồng 3
    public override void CalculateLayoutInputVertical()
    {
        // Debug.Log("CalculateLayoutInputVertical");
        CalculateLayoutInputForAxis(1);
    }
    // Vào 1 // Luồng 4
    public override void SetLayoutVertical()
    {
        // Debug.Log("SetLayoutVertical");
        SetLayoutAlongAxis(1);
    }

    #endregion

    #region Private Methods

    //Vào 2 // Luồng 1
    //Vào 2 // Luồng 3
    private void CalculateLayoutInputForAxis(int axis)
    {
        // Debug.Log("CalculateLayoutInputForAxis");
        float preferredSize = 0;

        if (axis == 0)
        {
            float columnPreferredSize = GetRowPreferredWidth();
            float tempPreferredSize = 0;

            for (int i = 0; i < transform.childCount; i++)
            {
                RectTransform child = transform.GetChild(i) as RectTransform;

                tempPreferredSize += LayoutUtility.GetPreferredSize(child, 0) + spacing;

                if (tempPreferredSize >= columnPreferredSize)
                {
                    preferredSize = Mathf.Max(preferredSize, tempPreferredSize);
                    tempPreferredSize = 0;
                }
            }
        }
        else if (transform.childCount > 0)
        {
            preferredSize = LayoutUtility.GetPreferredSize(transform.GetChild(0) as RectTransform, 1) * rows + spacing * (rows - 1);
        }

        preferredSize += (axis == 0 ? m_Padding.horizontal : m_Padding.vertical);

        SetLayoutInputForAxis(preferredSize, preferredSize, preferredSize, axis);
    }

    // Vào 2 // Luồng 2
    // Vào 2 // Luồng 4
    private void SetLayoutAlongAxis(int axis)
    {
        // Debug.Log("SetLayoutAlongAxis");
        float preferredWidth = GetRowPreferredWidth();
        float xStartOffset = GetStartOffset(0, 0);
        float yStartOffset = GetStartOffset(1, 0);
        Vector2 position = new Vector2(xStartOffset, yStartOffset);

        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform child = transform.GetChild(i) as RectTransform;

            float childPreferredWidth = LayoutUtility.GetPreferredSize(child, 0);

            SetChildAlongAxis(child, axis, position[axis], LayoutUtility.GetPreferredSize(child, axis));

            position.x += childPreferredWidth + spacing;

            if (position.x - xStartOffset >= preferredWidth)
            {
                position.x = xStartOffset;
                position.y += LayoutUtility.GetPreferredSize(child, 1) + spacing;
            }
        }
    }
    // Vào 3 // Luồng 1
    // Vào 3 // Luồng 2
    // Vào 3 // Luồng 3
    // Vào 3 // Luồng 4
    private float GetRowPreferredWidth()
    {
        // Debug.Log("GetRowPreferredWidth");
        float totalSize = 0;

        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform child = transform.GetChild(i) as RectTransform;

            if (i != 0)
            {
                totalSize += spacing;
            }

            totalSize += LayoutUtility.GetPreferredSize(child, 0);
        }

        return totalSize / (float)rows;
    }

    #endregion
}

