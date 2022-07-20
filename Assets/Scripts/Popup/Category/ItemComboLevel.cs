using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;

public class ItemComboLevel : EnhancedScrollerCellView
{
    [SerializeField] private Sprite spPlayed;
    [SerializeField] private Sprite spNotPlay;
    [SerializeField] private ItemLevel[] ListItem;

    public void SetData(int index, int maxCount, string idCategory)
    {
        var startIndex = index * 8;
        var lastCompletedLevels = DataController.Instance.LastCompletedLevels;
        int levelPlay = 0;
        if (lastCompletedLevels.ContainsKey(idCategory)) levelPlay = lastCompletedLevels[idCategory] + 1;


        foreach (var itemLevel in ListItem)
        {
            var isActiveLevel = startIndex <= levelPlay;
            if (startIndex < maxCount) itemLevel.SetUp(startIndex, isActiveLevel ? spPlayed : spNotPlay, isActiveLevel);
            else itemLevel.Disable();
            startIndex++;
        }
    }
}
