using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnhancedUI.EnhancedScroller;

public delegate void CellButtonBoolClickedDelegate(int idQuest);
public class ItemQuest : EnhancedScrollerCellView
{
    private Quest _data;
    private int idQuest;

    public CellButtonBoolClickedDelegate buttonclicked;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameQuest;
    [SerializeField] private TextMeshProUGUI txtPro;
    [SerializeField] private Image btnCheck;
    [SerializeField] private Image mask;
    [SerializeField] private Text txtCollect;
    [SerializeField] private Image fill;
    // Start is called before the first frame update
    public void SetData(int idQuest, Quest data, Sprite spriteBtn, string status)
    {
        this.idQuest = idQuest;
        _data = data;
        icon.sprite = _data.icon;
        icon.SetNativeSize();

        nameQuest.text = _data.name;
        txtPro.text = string.Format("{0}/{1}", _data.current, _data.maximum);

        btnCheck.sprite = spriteBtn;
        btnCheck.SetNativeSize();
        if (status.Equals("Collect"))
        {
            var number = data.amountGift.ToString();
            var type = data.giftType == Quest.GiftType.Keys ? "K" : "C";
            txtCollect.gameObject.SetActive(true);
            if (data.giftType == Quest.GiftType.Keys)
            {
                txtCollect.text = string.Format("+ {0}  {1}", number, type);
            }
            else
            {
                txtCollect.text = string.Format("+ {0} {1}  {2}", number[0], number[1], type);
            }

        }
        else txtCollect.gameObject.SetActive(false);

        var button = btnCheck.GetComponent<Button>();
        if (status.Equals("Collected"))
        {
            fill.gameObject.SetActive(true);
            button.interactable = false;
            var newColorBlock = button.colors;
            newColorBlock.disabledColor = Color.white;
            button.colors = newColorBlock;
        }
        else
        {
            fill.gameObject.SetActive(false);
            button.interactable = true;
        }
        mask.fillAmount = GetCureentFill();
    }
    private float GetCureentFill()
    {
        return (float)_data.current / (float)_data.maximum;
    }
    public void CollectedGift()
    {
        if (buttonclicked != null) buttonclicked(idQuest);
    }

}
