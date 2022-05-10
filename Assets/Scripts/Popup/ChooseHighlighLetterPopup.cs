using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseHighlighLetterPopup : MonoBehaviour
{

    [SerializeField] private GameObject letterButtonPrefab = null;
    [SerializeField] private Transform letterButtonContainer = null;
    [SerializeField] private GameObject noLettersToShow = null;
    public void OnShowing()
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

        if (letters.Count == 0)
        {
            noLettersToShow.SetActive(true);
        }
        else
        {
            noLettersToShow.SetActive(false);

            letters.Sort();

            // Debug.Log("letterButtonContainer.transform.childCount" + letterButtonContainer.transform.childCount);
            int oldChildCount = letterButtonContainer.transform.childCount;
            int count = oldChildCount > letters.Count ? oldChildCount : letters.Count;
            for (int i = 0; i < count; i++)
            {
                if (oldChildCount == 0)
                {
                    char letter = letters[i];

                    GameObject highlightLetter = Instantiate(letterButtonPrefab, Vector3.zero, Quaternion.identity, letterButtonContainer);
                    HighlightLetterButton _highlightLetterButtonScript = highlightLetter.GetComponent<HighlightLetterButton>();
                    _highlightLetterButtonScript.Setup(letter);
                }
                else
                {
                    if (letters.Count < oldChildCount)
                    {
                        if (i < letters.Count)
                        {
                            char letter = letters[i];
                            GameObject highlightLetter = letterButtonContainer.transform.GetChild(i).gameObject;
                            HighlightLetterButton _highlightLetterButtonScript = highlightLetter.GetComponent<HighlightLetterButton>();
                            _highlightLetterButtonScript.Setup(letter);
                        }
                        else
                        {
                            GameObject highlightLetter = letterButtonContainer.transform.GetChild(i).gameObject;
                            highlightLetter.SetActive(false);
                        }
                    }
                    else
                    {
                        if (i < oldChildCount)
                        {
                            char letter = letters[i];
                            GameObject highlightLetter = letterButtonContainer.transform.GetChild(i).gameObject;
                            HighlightLetterButton _highlightLetterButtonScript = highlightLetter.GetComponent<HighlightLetterButton>();
                            _highlightLetterButtonScript.Setup(letter);
                        }
                        else
                        {
                            char letter = letters[i];
                            GameObject highlightLetter = letterButtonContainer.transform.GetChild(i).gameObject;
                            HighlightLetterButton _highlightLetterButtonScript = highlightLetter.GetComponent<HighlightLetterButton>();
                            _highlightLetterButtonScript.Setup(letter);
                        }
                    }

                }



            }
        }
    }

}
