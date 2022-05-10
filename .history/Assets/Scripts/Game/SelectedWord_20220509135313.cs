using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedWord : MonoBehaviour
{
    [SerializeField] private Text selectedWordText = null;
    [SerializeField] private GameObject selectedWordContainer = null;
    [SerializeField] private Image selectedWordBkgImage = null;

    public void SetSelectedWord(string word, Color color)
    {
        selectedWordText.text = word;
        selectedWordContainer.SetActive(true);

        selectedWordBkgImage.color = color;
    }

    public void Clear()
    {
        selectedWordText.text = "";
        selectedWordContainer.SetActive(false);
    }

}