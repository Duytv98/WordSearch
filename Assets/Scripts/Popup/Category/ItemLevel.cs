using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemLevel : MonoBehaviour
{

    [SerializeField] private Image bg;
    [SerializeField] private TextMeshProUGUI txtLevel;
    private int index;
    private bool isPlayLevel;

    public void SetUp(int index, Sprite spBg, bool isPlay)
    {
        gameObject.SetActive(true);
        this.index = index;
        txtLevel.text = (this.index + 1).ToString();
        bg.sprite = spBg;
        bg.SetNativeSize();
        isPlayLevel = isPlay;

        var button = bg.GetComponent<Button>();
        button.interactable = isPlay;
        var newColorBlock = button.colors;
        newColorBlock.disabledColor = Color.white;
        button.colors = newColorBlock;
    }
    public void Disable()
    {
        gameObject.SetActive(false);
    }
    public void Onclick()
    {
        if (!isPlayLevel) return;
        SelectCategoryPopup.Instance.SelectedLevel(index);
    }
}
