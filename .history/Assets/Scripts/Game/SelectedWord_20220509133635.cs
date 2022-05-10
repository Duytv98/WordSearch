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
        selectedWordText.text = word;
        selectedWordContainer.SetActive(true);
        canvasGroup.alpha = 1;

        selectedWordBkgImage.color = color;
    }

    public void Clear(bool chooseRight = false)
    {
       if(chooseRight) RightChoice();
       else WrongChoice();
    }

    private void RightChoice(){
        
    }
    private void WrongChoice(){
        activeSequence =true;
        selectedWordFalse = DOTween.Sequence();
        selectedWordFalse.Append(transform.DORotate(new Vector3(0, 0, 10), 0.07f));
        selectedWordFalse.Append(transform.DORotate(new Vector3(0, 0, -10), 0.2f).SetLoops(3, LoopType.Yoyo));
        selectedWordFalse.Append(transform.DORotate(new Vector3(0, 0, 0), 0.07f));
        selectedWordFalse.Insert(0, canvasGroup.DOFade(0, selectedWordFalse.Duration()));
        selectedWordFalse.OnComplete(() => activeSequence = false);
    }

}