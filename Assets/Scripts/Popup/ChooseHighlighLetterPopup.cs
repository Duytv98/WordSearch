using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseHighlighLetterPopup : MonoBehaviour
{

    [SerializeField] private GameObject letterButtonPrefab = null;
    [SerializeField] private Transform letterButtonContainer = null;
    [SerializeField] private GameObject noLettersToShow = null;
    public void OnShowing(bool isBooterUse)
    {
        Board board = GameManager.Instance.ActiveBoard;

        List<char> letters = new List<char>();
        HashSet<char> lettersInList = new HashSet<char>();

        for (int row = 0; row < board.rows; row++)
        {
            for (int col = 0; col < board.cols; col++)
            {
                char letter = board.boardCharacters[row][col];
                if (!lettersInList.Contains(letter) && !board.letterHintsUsed.Contains(letter))
                {
                    letters.Add(letter);
                    lettersInList.Add(letter);
                }
            }
        }

        if (letters.Count == 0) noLettersToShow.SetActive(true);
        else
        {
            noLettersToShow.SetActive(false);
            letters.Sort();
            int oldChildCount = letterButtonContainer.transform.childCount;
            int count = oldChildCount > letters.Count ? oldChildCount : letters.Count;
            for (int i = 0; i < count; i++)
            {
                char letter = letters[i];
                var sprite = GameManager.Instance.DicWord[letter];
                if (oldChildCount == 0) CreateHighlightLetter(letter, sprite, isBooterUse);
                else
                {
                    if (letters.Count < oldChildCount)
                    {
                        if (i < letters.Count) ChangeHighlightLetter(i, letter, sprite, isBooterUse);
                        else DeactivateHighlightLetter(i);
                    }
                    else
                    {
                        if (i < oldChildCount) ChangeHighlightLetter(i, letter, sprite, isBooterUse);
                        else CreateHighlightLetter(letter, sprite, isBooterUse);
                    }
                }
            }
        }
    }
    private void CreateHighlightLetter(char letter, Sprite sprite, bool isBooterUse)
    {
        GameObject highlightLetter = Instantiate(letterButtonPrefab, Vector3.zero, Quaternion.identity, letterButtonContainer);
        HighlightLetterButton _highlightLetterButtonScript = highlightLetter.GetComponent<HighlightLetterButton>();
        _highlightLetterButtonScript.Setup(letter, sprite, isBooterUse);
    }
    private void ChangeHighlightLetter(int index, char letter, Sprite sprite, bool isBooterUse)
    {
        GameObject highlightLetter = letterButtonContainer.transform.GetChild(index).gameObject;
        HighlightLetterButton _highlightLetterButtonScript = highlightLetter.GetComponent<HighlightLetterButton>();
        _highlightLetterButtonScript.Setup(letter, sprite, isBooterUse);
    }
    private void DeactivateHighlightLetter(int index)
    {
        GameObject highlightLetter = letterButtonContainer.transform.GetChild(index).gameObject;
        highlightLetter.SetActive(false);
    }

}
