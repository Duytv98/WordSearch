using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class CollectGift : MonoBehaviour
{
    [SerializeField] private RectTransform bg = null;
    [SerializeField] private Transform content = null;
    [SerializeField] private Image panelPopupImg = null;
    public void ShowGift()
    {
        gameObject.SetActive(true);
        panelPopupImg.DOFade(0.92f, 0.2f);
        bg.DOLocalRotate(new Vector3(0, 0, -360), 4f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental).SetRelative(true).SetEase(Ease.Linear);
        content.localScale = new Vector3(0.5f, 0.5f, 1);
        content.DOScale(new Vector3(1, 1, 1), 1f).SetEase(Ease.OutBack);
    }
    public void CloseGift()
    {
        Debug.Log("closee========");
        content.DOScale(new Vector3(0.5f, 0.5f, 1),0.7f).SetEase(Ease.OutBack);
        panelPopupImg.DOFade(0f, 0.3f)
        .OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
