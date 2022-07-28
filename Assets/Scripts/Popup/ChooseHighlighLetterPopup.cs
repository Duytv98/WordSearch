using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseHighlighLetterPopup : MonoBehaviour
{

    [SerializeField] private GameObject letterButtonPrefab = null;
    
    [SerializeField] private Transform letterButtonContainer = null;
    [SerializeField] private GameObject noLettersToShow = null;
    public void OnShowing(List<char> letters)
    {
        if (letters.Count == 0) noLettersToShow.SetActive(true);
        else
        {
            noLettersToShow.SetActive(false);
            letters.Sort();
            int oldChildCount = letterButtonContainer.transform.childCount;
            int count = Mathf.Max(oldChildCount, letters.Count);
            for (int i = 0; i < count; i++)
            {
                if (oldChildCount == 0) CreateHighlightLetter(letters[i], DataController.Instance.DicWord[letters[i]]);
                else
                {
                    if (letters.Count < oldChildCount)
                    {
                        if (i < letters.Count) ChangeHighlightLetter(i, letters[i], DataController.Instance.DicWord[letters[i]]);
                        else DeactivateHighlightLetter(i);
                    }
                    else
                    {
                        if (i < oldChildCount) ChangeHighlightLetter(i, letters[i], DataController.Instance.DicWord[letters[i]]);
                        else CreateHighlightLetter(letters[i], DataController.Instance.DicWord[letters[i]]);
                    }
                }
            }
        }
    }
    private void CreateHighlightLetter(char letter, Sprite sprite)
    {
        GameObject highlightLetter = Instantiate(letterButtonPrefab, Vector3.zero, Quaternion.identity, letterButtonContainer);
        HighlightLetterButton _highlightLetterButtonScript = highlightLetter.GetComponent<HighlightLetterButton>();
        _highlightLetterButtonScript.Setup(letter, sprite);
    }
    private void ChangeHighlightLetter(int index, char letter, Sprite sprite)
    {
        GameObject highlightLetter = letterButtonContainer.transform.GetChild(index).gameObject;
        HighlightLetterButton _highlightLetterButtonScript = highlightLetter.GetComponent<HighlightLetterButton>();
        _highlightLetterButtonScript.Setup(letter, sprite);
    }
    private void DeactivateHighlightLetter(int index)
    {
        GameObject highlightLetter = letterButtonContainer.transform.GetChild(index).gameObject;
        highlightLetter.SetActive(false);
    }

}
