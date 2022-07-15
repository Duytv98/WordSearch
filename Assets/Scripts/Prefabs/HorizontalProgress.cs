using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HorizontalProgress : MonoBehaviour
{
    [SerializeField] private Image mask;
    [SerializeField] private TextMeshProUGUI nameEvent;
    [SerializeField] private TextMeshProUGUI luong;

    private float maximum;
    private float current;

    public float Maximum { get => maximum; set => maximum = value; }


    public void SetUp(string name, string luong){
        nameEvent.text = name;
        this.luong.text = luong;
    }
    public void UpdateCureentFill(float current)
    {
        this.mask.fillAmount = GetCureentFill(current);
    }

    private float GetCureentFill(float current)
    {
        return current / Maximum;
    }
}
