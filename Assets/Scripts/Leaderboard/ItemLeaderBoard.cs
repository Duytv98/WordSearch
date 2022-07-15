using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnhancedUI.EnhancedScroller;
public class ItemLeaderBoard : EnhancedScrollerCellView
{
    private PlayerLB _data;

    [SerializeField] private Sprite[] spritesBorder = null;
    [SerializeField] private Sprite[] spritesMedal = null;
    [SerializeField] private Sprite[] spritesAvatar = null;
    [SerializeField] private Sprite[] spritesBG = null;
    // Start is called before the first frame update
    public int DataIndex { get; private set; }

    [SerializeField] private Image bg;
    [SerializeField] private Image medal;
    [SerializeField] private Image border;
    [SerializeField] private Image avatar;
    [SerializeField] private TextMeshProUGUI txtSTT;
    [SerializeField] private TextMeshProUGUI txtName;
    [SerializeField] private TextMeshProUGUI score;
    public void SetData(int index, PlayerLB data, int indexPlayer)
    {
        DataIndex = index;
        _data = data;
        SetActiveDefault();

        if (DataIndex < 3)
        {
            medal.sprite = spritesMedal[DataIndex];
            medal.SetNativeSize();
            border.sprite = spritesBorder[DataIndex];
            border.SetNativeSize();
            avatar.sprite = spritesAvatar[0];
            avatar.SetNativeSize();
        }
        else
        {
            medal.gameObject.SetActive(false);
            border.gameObject.SetActive(false);
            txtSTT.gameObject.SetActive(true);
            avatar.sprite = spritesAvatar[1];
            avatar.SetNativeSize();
            txtSTT.text = (DataIndex + 1).ToString();
        }

        

        if (DataIndex == indexPlayer)
        {
            bg.sprite = spritesBG[1];
            bg.SetNativeSize();
            
            avatar.sprite = spritesAvatar[0];
            avatar.SetNativeSize();
        }
        else
        {
            bg.sprite = spritesBG[0];
            bg.SetNativeSize();
        }
        txtName.text = _data.name;
        score.text = _data.score.ToString();
    }
    private void SetActiveDefault()
    {
        medal.gameObject.SetActive(true);
        border.gameObject.SetActive(true);
        avatar.gameObject.SetActive(true);
        txtSTT.gameObject.SetActive(false);
        txtName.gameObject.SetActive(true);
        score.gameObject.SetActive(true);
    }

}
