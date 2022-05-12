using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordListItem : MonoBehaviour
{
    [SerializeField] private Text textBack = null;
    [SerializeField] private Text wordText = null;
    [SerializeField] private Image background = null;
    [SerializeField] private CanvasGroup _wordCVG = null;

    private string word = null;

    public string Word { get => word; set => word = value; }

    public void Setup(string word)
    {
        Word = word;
        textBack.text = word;
        wordText.text = word;
        background.gameObject.SetActive(false);
    }

    public void SetWordFound(Color color)
    {
        // Debug.Log("Word" + Word);
        background.gameObject.SetActive(true);
        background.color = color;

        textBack.color = Color.grey;
        _wordCVG.alpha = 0.2f;
    }
    public void SetAlpha(bool isActive)
    {
        // Debug.Log("SetAlpha: " + isActive);
        _wordCVG.alpha = isActive ? 1 : 0;
        _wordCVG.interactable = isActive;
        _wordCVG.blocksRaycasts = isActive;
    }
    public void SetParent(RectTransform parent)
    {
        transform.SetParent(parent);
    }

}
