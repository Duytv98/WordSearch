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


    // public void HintHighlightWord()
    // {
    //     var activeBoard = GameManager.Instance.ActiveBoard;
    //     var coins = GameManager.Instance.Coins;
    //     bool useBooter = false;
    //     string key = "Find-words";
    //     if (activeBoard == null || ScreenManager.Instance.IsActiveLoading()) return;

    //     List<string> nonFoundWords = new List<string>();

    //     // Lấy ra các từ chưa được tìm thấy
    //     for (int i = 0; i < activeBoard.words.Count; i++)
    //     {
    //         string word = activeBoard.words[i];
    //         if (!activeBoard.foundWords.Contains(word) && !wordListContainer.UnusedWords.Contains(word))  nonFoundWords.Add(word);
    //     }

    //     // Đảm bảo danh dách không âm
    //     if (nonFoundWords.Count == 0) return;
    //     if (GameManager.Instance.CheckBooterExist(key)) useBooter = true;

    //     if (coins < GameDefine.FIND_WORDS && !useBooter)
    //         PopupContainer.Instance.ShowNotEnoughCoinsPopup();
    //     else
    //     {
    //         // Pick a random word to show
    //         string wordToShow = nonFoundWords[Random.Range(0, nonFoundWords.Count)];

    //         // Set it as selected
    //         GameManager.Instance.OnWordSelected(wordToShow);
    //         // Highlight the word
    //         characterGrid.ShowWordHint(wordToShow);
    //         // Deduct the cost
    //         if (useBooter) SubtractionBooter(key, 1);
    //         else Coins -= GameDefine.FIND_WORDS;

    //         SaveableManager.Instance.SaveCoins(Coins);
    //         AudioManager.Instance.Play("hint-used");
    //     }
    // }











}
