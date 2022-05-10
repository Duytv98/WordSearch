using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
public class SelectedWord : MonoBehaviour
{
    [SerializeField] private Text selectedWordText = null;
    [SerializeField] private GameObject selectedWordContainer = null;
    [SerializeField] private Image selectedWordBkgImage = null;

    [SerializeField] private CanvasGroup canvasGroup;
    private Sequence selectedWordFalse = null;
    private bool activeSequence = false;



    public void SetSelectedWord(string word, Color color)
    {
        if (activeSequence)
        {
            selectedWordFalse.Kill(true);
            activeSequence = false;
        }
        // transform.Rotate(0, 0, 0);
        transform.localScale = Vector3.one;
        selectedWordText.text = word;
        selectedWordContainer.SetActive(true);
        canvasGroup.alpha = 1;

        selectedWordBkgImage.color = color;
    }

    public void Clear(bool chooseRight = false)
    {
        Debug.Log("chooseRight: " + chooseRight);
        if (chooseRight) RightChoice();
        else WrongChoice();
    }

    private void RightChoice()
    {
        selectedWordText.text = "";
        selectedWordContainer.SetActive(false);
        canvasGroup.alpha = 0;
    }
    // private void WrongChoice()
    // {
    //     activeSequence = true;
    //     selectedWordFalse = DOTween.Sequence();
    //     selectedWordFalse.Append(transform.DORotate(new Vector3(0, 0, 10), 0.07f));
    //     selectedWordFalse.Append(transform.DORotate(new Vector3(0, 0, -10), 0.2f).SetLoops(3, LoopType.Yoyo));
    //     selectedWordFalse.Append(transform.DORotate(new Vector3(0, 0, 0), 0.07f));
    //     selectedWordFalse.Insert(0, canvasGroup.DOFade(0, selectedWordFalse.Duration()));
    //     selectedWordFalse.OnComplete(() => activeSequence = false);
    // }
    private void WrongChoice()
    {
        activeSequence = true;
        selectedWordFalse = DOTween.Sequence();
        selectedWordFalse.Append(transform.DOScale(new Vector3(1.1f, 1.1f, 1), 1f));
        selectedWordFalse.Insert(0.3f, canvasGroup.DOFade(0, selectedWordFalse.Duration() - 0.3f));
        selectedWordFalse.OnComplete(() => activeSequence = false);
    }

}