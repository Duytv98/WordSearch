using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using DG.Tweening;

public class CharacterGridItem : MonoBehaviour
{
    [SerializeField] private Font font = null;
    [SerializeField] private int fontSize = 150;
    public Text characterText;
    private Text cloneText;
    public int Row { get; set; }
    public int Col { get; set; }
    public bool IsHighlighted { get; set; }

    private Color highlightColor = Color.black;

    private Image highlight;
    public Image Highlight { get => highlight; set => highlight = value; }
    public Color HighlightColor { get => highlightColor; set => highlightColor = value; }

    public void Setup(char text, Color color, Vector3 scale, Vector2 scaledLetterOffsetInCell)
    {
        characterText.text = text.ToString();
        characterText.color = color;
        characterText.transform.localScale = scale;
        (transform as RectTransform).anchoredPosition = scaledLetterOffsetInCell;
    }
    public string Log()
    {
        return string.Format("characterText: {0}, row: {1}, col: {2}, anchoredPosition: {3}", characterText.text, Row, Col, (transform as RectTransform).anchoredPosition);
    }
    public Vector3 GetPosition(Camera cam)
    {
        return cam.WorldToViewportPoint(transform.position);
    }
    public void Clone(Transform parent)
    {
        if (cloneText != null)
        {
            return;
        }
        GameObject newText = new GameObject("TextClone");
        newText.transform.SetParent(parent);
        newText.transform.localScale = characterText.transform.localScale;
        newText.transform.localPosition = this.transform.localPosition;

        ContentSizeFitter contentSizeFitter = newText.AddComponent<ContentSizeFitter>();
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        cloneText = newText.AddComponent<Text>();
        cloneText.text = characterText.text;

        cloneText.font = font;
        cloneText.fontSize = fontSize;
        cloneText.color = Color.black;

        newText.SetActive(false);
    }
    public void FlyWord()
    {
        cloneText.gameObject.SetActive(true);
        cloneText.transform.DOMove(Vector3.right * 3, 2f);
        characterText.color =Color.grey;
    }
}
