using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnhancedUI.EnhancedScroller;
public class SmallItemLeaderBoard : EnhancedScrollerCellView
{
    [SerializeField] private Sprite[] spritesAvatar = null;
    [SerializeField] private Image avatar;
    [SerializeField] private Image border;
    [SerializeField] private TextMeshProUGUI txtSTT;
    [SerializeField] private Image bgIndex;
    [SerializeField] private TextMeshProUGUI txtSTTDefault;
    [SerializeField] private TextMeshProUGUI txtName;
    [SerializeField] private VerticalLayoutGroup vlgContent;
    private PlayerLB _data;

    public void SetData(int index, PlayerLB data, int indexPlayer)
    {
        SetActiveDefault();

        if (index == indexPlayer)
        {
            avatar.sprite = spritesAvatar[1];
            avatar.SetNativeSize();
            txtSTT.text = (index + 1).ToString();
            txtSTTDefault.text = (index + 1).ToString();
            txtName.text = data.name;
            vlgContent.spacing = 12;
        }
        else
        {

            avatar.sprite = spritesAvatar[0];
            avatar.SetNativeSize();
            border.gameObject.SetActive(false);
            txtSTT.gameObject.SetActive(false);
            txtSTTDefault.text = (index + 1).ToString();
            txtName.text = data.name;
            vlgContent.spacing = 10;
        }
    }

    private void SetActiveDefault()
    {
        avatar.gameObject.SetActive(true);
        border.gameObject.SetActive(true);
        txtSTT.gameObject.SetActive(true);
        bgIndex.gameObject.SetActive(true);
        txtSTTDefault.gameObject.SetActive(true);
        txtName.gameObject.SetActive(true);
    }
}
