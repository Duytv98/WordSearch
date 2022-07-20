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
    // Start is called before the first frame update
    public void SetData(Quest data)
    {
        _data = data;
        icon.sprite = _data.icon;
        icon.SetNativeSize();

        nameQuest.text = _data.name;
        txtPro.text = string.Format("{0}/{1}", _data.current, _data.maximum);

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
