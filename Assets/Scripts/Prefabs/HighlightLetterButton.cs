using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class HighlightLetterButton : MonoBehaviour
{
    [SerializeField] private Image imgText = null;
    private char letter;
    public void Setup(char letter, Sprite sprite)
    {   
        gameObject.SetActive(true);
        this.letter = letter;
        imgText.sprite = sprite;
        
        imgText.SetNativeSize();
        // Vector3 localPosition = transform.localPosition;
        // transform.localPosition = new Vector3(localPosition.x, localPosition.y, 0f);
    }
    public void OnClick()
    {
        // Destroy(gameObject);
        GameScreen.Instance.OnChooseHighlightLetterPopupClosed(letter);
        AudioManager.Instance.Play_Click_Button_Sound();
        AudioManager.Instance.Play("hint-used");
    }
}
