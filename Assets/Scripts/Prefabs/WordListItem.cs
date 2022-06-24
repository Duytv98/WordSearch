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

    private string word = null;
    public string Word { get => word; set => word = value; }


    public void Setup(string word)
    {
        Word = word;
        textBack.text = word;
        wordText.text = word;
        background.gameObject.SetActive(false);
        if (Responsive.Instance.IsSmallScreen) SetFontSizeSmallScreen();
    }
    private void SetFontSizeSmallScreen()
    {
        textBack.fontSize = 45;
        wordText.fontSize = 45;
        var rtWord = wordText.GetComponent<RectTransform>();
        rtWord.anchoredPosition = new Vector3(0, 4, 0);

        RectTransform rectTransform = background.GetComponent<RectTransform>();
        SetLeft(rectTransform, -9f);
        SetRight(rectTransform, -9f);
        SetTop(rectTransform, 8.5f);
        SetBottom(rectTransform, 0.5f);

        background.pixelsPerUnitMultiplier = 1;
    }
    public void SetLeft(RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public void SetRight(RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    public void SetTop(RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }

    public void SetBottom(RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
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
        background.color = GameDefine.COLOR_BG[indexColor];


        textBack.color = GameDefine.COLOR_TEXT_HIGHLIGHT[indexColor];
        wordText.color = GameDefine.COLOR_TEXT_HIGHLIGHT[indexColor];
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
