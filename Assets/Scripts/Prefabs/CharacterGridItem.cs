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
    [SerializeField] private Image Bg = null;
    [SerializeField] private Image imgCharacter;

    private char text;
    private Text cloneText;
    public int Row { get; set; }
    public int Col { get; set; }
    public bool IsHighlighted { get; set; }
    private bool isChoose = false;
    private bool isActive = true;
    public bool IsActive { get => isActive; set => isActive = value; }
    public bool IsChoose { get => isChoose; set => isChoose = value; }


    [SerializeField] private Color defaultColor;
    [SerializeField] private Sprite defaultSprite;


    private Color color;
    private Sprite sprite;

    public void SetChoose(Color color, Sprite sprite)
    {
        IsChoose = true;
        this.color = color;
        this.sprite = sprite;
        SetColor(color, sprite);
    }

    public void Setup(char text, Vector3 scale, Vector2 scaledLetterOffsetInCell, int row, int col, Sprite sprite)
    {
        var maxSize = GameManager.Instance.ActiveBoard.cols;
        this.text = text;
        (transform as RectTransform).anchoredPosition = scaledLetterOffsetInCell;
        Row = row;
        Col = col;
        imgCharacter.sprite = sprite;
        imgCharacter.transform.localScale = scale;
        imgCharacter.SetNativeSize();
        if (maxSize > 10)
        {
            var rt = (RectTransform)imgCharacter.transform;
            rt.anchoredPosition = Vector2.zero;
        }
        else if (maxSize > 7)
        {
            var rt = (RectTransform)imgCharacter.transform;
            rt.anchoredPosition = new Vector2(0f, 3f);
        }

    }
    public string Log()
    {
        return string.Format("characterText: {0}, row: {1}, col: {2}, anchoredPosition: {3}", text, Row, Col, (transform as RectTransform).anchoredPosition);
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
        // newText.transform.localScale = characterText.transform.localScale;
        newText.transform.localPosition = this.transform.localPosition;

        ContentSizeFitter contentSizeFitter = newText.AddComponent<ContentSizeFitter>();
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        cloneText = newText.AddComponent<Text>();
        // cloneText.text = characterText.text;

        cloneText.font = font;
        cloneText.fontSize = fontSize;
        cloneText.color = Color.black;

        newText.SetActive(false);
    }
    public void FlyWord()
    {
        cloneText.gameObject.SetActive(true);
        cloneText.transform.DOMove(Vector3.right * 3, 2f)
        .OnComplete(() => Destroy(cloneText.gameObject));
        SetWordUnuseds();
    }
    public void SetWordUnuseds()
    {
        // characterText.color = Color.grey;
        IsActive = false;
    }
    public void SetColor(Color color, Sprite sprite)
    {
        // characterText.color = color;
        imgCharacter.color = color;
        Bg.sprite = sprite;
    }
    public void UnDoColor()
    {
        if (IsChoose)
        {
            // characterText.color = color;
            imgCharacter.color = color;
            Bg.sprite = sprite;
        }
        else
        {
            // characterText.color = defaultColor;
            imgCharacter.color = defaultColor;
            Bg.sprite = defaultSprite;
        }
    }
}
