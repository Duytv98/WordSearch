using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class HighlightLetterButton : MonoBehaviour
{

    [SerializeField] private Text letterText = null;
    private char letter;
    private bool isBooterUse = false;
    public void Setup(char letter, bool isBooterUse)
    {
        this.isBooterUse = isBooterUse;
        this.letter = letter;
        letterText.text = letter.ToString();
        Vector3 localPosition = transform.localPosition;
        transform.localPosition = new Vector3(localPosition.x, localPosition.y, 0f);
    }
    public void OnClick()
    {
        GameManager.Instance.OnChooseHighlightLetterPopupClosed(letter, isBooterUse);
        AudioManager.Instance.Play_Click_Button_Sound();
        AudioManager.Instance.Play("hint-used");
    }
}
