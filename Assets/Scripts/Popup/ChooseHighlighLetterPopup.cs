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
        var characterItems = GameManager.Instance.CharacterItems;
        List<char> letters = new List<char>();
        foreach (var row in characterItems)
        {
            foreach (var item in row)
            {
                char letter = item.Text;
                if (!letters.Contains(letter) && !board.letterHintsUsed.Contains(letter) && item.IsActive)
                {
                    letters.Add(letter);
                }
            }
        }

        if (letters.Count == 0) noLettersToShow.SetActive(true);
        else
        {
            noLettersToShow.SetActive(false);
            letters.Sort();
            int oldChildCount = letterButtonContainer.transform.childCount;
            int count = Mathf.Max(oldChildCount, letters.Count);
            for (int i = 0; i < count; i++)
            {
                if (oldChildCount == 0) CreateHighlightLetter(letters[i], GameManager.Instance.DicWord[letters[i]], isBooterUse);
                else
                {
                    if (letters.Count < oldChildCount)
                    {
                        if (i < letters.Count) ChangeHighlightLetter(i, letters[i], GameManager.Instance.DicWord[letters[i]], isBooterUse);
                        else DeactivateHighlightLetter(i);
                    }
                    else
                    {
                        if (i < oldChildCount) ChangeHighlightLetter(i, letters[i], GameManager.Instance.DicWord[letters[i]], isBooterUse);
                        else CreateHighlightLetter(letters[i], GameManager.Instance.DicWord[letters[i]], isBooterUse);
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
