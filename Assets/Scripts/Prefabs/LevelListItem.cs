using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PolyAndCode.UI;
public class LevelListItem : MonoBehaviour, ICell
{
    [SerializeField] private Text levelText = null;
    [SerializeField] private Image categoryIcon = null;
    [SerializeField] private Image completedIcon = null;
    [SerializeField] private Image lockedIcon = null;
    [SerializeField] private Image playIcon = null;

    private TextAsset levelFile = null;

    private int levelIndex;

    private bool isEvent = true;


    public void ConfigureCell(TextAsset levelFile, int cellIndex)
    {
        levelText.text = "LEVEL " + (cellIndex + 1).ToString();
    }

    public void Initialize(TextAsset levelFile, int level)
    {
        this.levelFile = levelFile;

        this.levelIndex = level;

        // this.levelFile = JsonUtility.FromJson<LevelInfo>(levelFile.ToString());

        HideAllIcons();

        levelText.text = "LEVEL " + (this.levelIndex + 1).ToString();
        CategoryInfo activeCategory = GameManager.Instance.ActiveCategoryInfo;
        // int activeLevel = GameManager.Instance.LastCompletedLevels[activeCategory.saveId];

        categoryIcon.sprite = activeCategory.icon;

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
    public void onClickLevel()
    {
        if (isEvent)
        {
            GameManager.Instance.StartLevel(GameManager.Instance.ActiveCategoryInfo, levelIndex);
            // Debug.Log(this.levelFile);
        }
    }
}
