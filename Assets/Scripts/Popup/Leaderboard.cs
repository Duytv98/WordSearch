using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Leaderboard : MonoBehaviour
{

    [SerializeField] private RectTransform placesYou = null;
    [SerializeField] private RectTransform rectPosition = null;




    public void OnShowing()
    {
        placesYou.DOAnchorPos(rectPosition.anchoredPosition, 0.5f)
        .SetDelay(0.35f)
        .SetEase(Ease.OutBack);
    }
    public void Close()
    {
        placesYou.anchoredPosition = new Vector3(0, 135f, 0f);
    }






    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
