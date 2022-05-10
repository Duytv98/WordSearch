using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListLevelTest : MonoBehaviour
{
    [SerializeField] private Text levelText = null;
    [SerializeField] private Image categoryIcon = null;
    [SerializeField] private Image completedIcon = null;
    [SerializeField] private Image lockedIcon = null;
    [SerializeField] private Image playIcon = null;
    [SerializeField] private Image backgroundImage = null;
    [SerializeField] private Image ShadowImage = null;
    [SerializeField] private Image ShadowImage1 = null;

    private TextAsset levelData;
    public System.Action<int> OnLevelItemSelected { get; set; }

    private int levelIndex;
    private bool isEvent = true;
    public void Setup(CategoryInfo activeCategory, int level)
    {
        this.levelData = activeCategory.levelFiles[level];
        this.levelIndex = level;

        levelText.text = "LEVEL " + (this.levelIndex + 1).ToString();
        categoryIcon.sprite = activeCategory.icon;
        // backgroundImage.color = activeCategory.categoryColor;
        var color = activeCategory.categoryColor;
        // Debug.Log(color);
        ShadowImage.color = color;
        ShadowImage1.color =  new Color(color.r,color.g,color.b,0.25f);

        HideAllIcons();
        // Debug.Log(GameManager.Instance.IsLevelLocked(activeCategory, levelIndex));
        if (GameManager.Instance.IsLevelCompleted(activeCategory, levelIndex))
        {
            SetCompleted();
        }
        else if (GameManager.Instance.IsLevelLocked(activeCategory, levelIndex))
        {
            isEvent = false;
            SetLocked();
        }
        else
        {
            SetPlayable();
        }
    }
    private void SetCompleted()
    {
        completedIcon.enabled = true;
    }

    private void SetLocked()
    {
        lockedIcon.enabled = true;
    }

    private void SetPlayable()
    {
        playIcon.enabled = true;
    }

    private void HideAllIcons()
    {
        completedIcon.enabled = false;
        lockedIcon.enabled = false;
        playIcon.enabled = false;
    }
    public void OnClicked()
    {
        // Debug.Log(this.levelIndex);
        if (OnLevelItemSelected != null)
        {
            OnLevelItemSelected(levelIndex);
        }
    }
}
