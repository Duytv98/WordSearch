using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInGameContainer : MonoBehaviour
{


    // [SerializeField] private WordListContainer wordListContainer = null;
    [SerializeField] private Button btnClearWords = null;
    [SerializeField] private Button btnSuggestManyWords = null;
    [SerializeField] private Button btnRecommendWord = null;


    public void SetInteractableButtonClearWords(bool status)
    {
        btnClearWords.interactable = status;
    }
    public void SetInteractableButtonSuggestManyWords(bool status)
    {
        btnSuggestManyWords.interactable = status;
    }
    public void SetInteractableButtonRecommendWord(bool status)
    {
        btnRecommendWord.interactable = status;
    }

}
