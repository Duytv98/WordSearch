using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelPuzzleInDay : MonoBehaviour
{
    [SerializeField] private Image imageLevel = null;
    [SerializeField] private GameObject iconComplete = null;
    [SerializeField] private GameObject iconPlay = null;
    // public void SetUp(bool isComplete, bool isProgress, Color[] colors)
    // {
    //     Button btnLevel = gameObject.GetComponent<Button>();
    //     if (isComplete)
    //     {
    //         iconComplete.SetActive(true);
    //         btnLevel.interactable = false;
    //         imageLevel.color = colors[1];
    //     }
    //     else
    //     {

    //         iconComplete.SetActive(false);
    //         if (isProgress)
    //         {
    //             imageLevel.color = colors[4];
    //         }
    //         else
    //         {

    //             imageLevel.color = colors[0];
    //         }
    //     }
    // }


    public void SetUp(bool isComplete, bool isProgress, Color color)
    {
        Button btnLevel = gameObject.GetComponent<Button>();

        iconPlay.SetActive(false);
        iconComplete.SetActive(false);
        iconComplete.SetActive(isComplete);
        btnLevel.interactable = !isComplete;
        imageLevel.color = color;
        if (!isComplete)
        {
            iconPlay.SetActive(isProgress);
        }
    }
}
