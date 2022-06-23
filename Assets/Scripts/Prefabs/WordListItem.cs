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
    [SerializeField] private CanvasGroup _wordBGCVG = null;

    [SerializeField] private Color colorTextMatched;

    
    [SerializeField] private Color[] colorsBG = null;
    [SerializeField] private Color[] colorsWord = null;

    private string word = null;
    public string Word { get => word; set => word = value; }


    public void Setup(string word)
    {
        Word = word;
        textBack.text = word;
        wordText.text = word;
        background.gameObject.SetActive(false);
    }

    public void SetWordFound()
    {
        background.gameObject.SetActive(false);

        textBack.color = colorTextMatched;
        _wordBGCVG.alpha = 0.5f;
    }
    public void SetRecommendWord(int indexColor)
    {
        background.gameObject.SetActive(true);
        background.color = colorsBG[indexColor];
        

        textBack.color = colorsWord[indexColor];
        wordText.color = colorsWord[indexColor];
    }
    public void SetAlpha(bool isActive)
    {
        _wordBGCVG.alpha = isActive ? 1 : 0;
        _wordBGCVG.interactable = isActive;
        _wordBGCVG.blocksRaycasts = isActive;
    }
    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
    }
    public float GetWidthSize()
    {
        RectTransform _wordItemRecT = textBack.GetComponent<RectTransform>();
        return _wordItemRecT.sizeDelta.x;
    }

}
