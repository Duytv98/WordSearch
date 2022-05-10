using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class HighlightLetterButton : MonoBehaviour
{

    [SerializeField] private Text letterText = null;
    private char letter;
    public void Setup(char letter)
    {
        this.letter = letter;
        letterText.text = letter.ToString();
    }
    public void OnClick()
    {
        GameManager.Instance.OnChooseHighlightLetterPopupClosed(letter);
    }
}
