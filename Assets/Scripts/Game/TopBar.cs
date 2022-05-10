using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopBar : MonoBehaviour
{
    [SerializeField] private CanvasGroup SettingsButton = null;
    [SerializeField] private CanvasGroup MenuButton = null;
    [SerializeField] private CanvasGroup backButton = null;
    [SerializeField] private CanvasGroup mainScreenContainer = null;
    [SerializeField] private CanvasGroup categoryContainer = null;
    [SerializeField] private Text categoryNameText = null;
    [SerializeField] private Text levelNumberText = null;
    [SerializeField] private Text coinAmountText = null;
    [SerializeField] private Text keyAmountText = null;
    private void Start()
    {
        backButton.alpha = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        coinAmountText.text = "x " + GameManager.Instance.Coins.ToString();
        keyAmountText.text = "x " + GameManager.Instance.Keys.ToString();
        // levelNumberText.text = "LEVEL " + (GameManager.Instance.ActiveLevelIndex + 1);
    }
    public void OnSwitchingScreens(string toScreenId)
    {
        if (toScreenId == "main")
        {
            mainScreenContainer.alpha = 1f;
            categoryContainer.alpha = 0f;
            levelNumberText.gameObject.SetActive(false);
        }
        else
        {
            mainScreenContainer.alpha = 0f;
            categoryContainer.alpha = 1f;
            if (toScreenId == "levels") levelNumberText.gameObject.SetActive(false);
        }
    }

    public void SetCategoryName(string displayName)
    {
        categoryNameText.text = displayName;
    }
    public void SetTextLevel(int level)
    {
        levelNumberText.gameObject.SetActive(true);
        levelNumberText.text = "LEVEL " + (level + 1);
    }
    public void SetAlphaSettingsButton(bool alpha)
    {
        backButton.alpha = alpha ? 1f : 0f;
    }
    public void SetAlphaMenuButton(bool alpha)
    {
        backButton.alpha = alpha ? 1f : 0f;
    }
    public void SetAlphaBackButton(bool alpha)
    {
        backButton.alpha = alpha ? 1f : 0f;
    }
}
