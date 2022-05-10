using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordListItem : MonoBehaviour
{
    [SerializeField] private Text wordText = null;
    [SerializeField] private GameObject foundIndicator = null;
    [SerializeField] private CanvasGroup _wordCVG = null;

    private string word = null;

    public string Word { get => word; set => word = value; }

    public void Setup(string word)
    {
        Word = word;
        wordText.text = word;
        foundIndicator.SetActive(false);
    }

    public void SetWordFound()
    {
        // foundIndicator.SetActive(true);
        // wordText.color = Color.grey;
        _wordCVG.alpha = 0.5f;

    }
    public void SetAlpha(bool isActive)
    {
        _wordCVG.alpha = isActive ? 1 : 0;
        _wordCVG.interactable = isActive;
        _wordCVG.blocksRaycasts = isActive;
    }
    public void SetParent(RectTransform parent)
    {
        transform.SetParent(parent);
    }

}
