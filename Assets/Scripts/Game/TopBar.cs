using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopBar : MonoBehaviour
{
    [SerializeField] private Text txtLevel = null;
    [SerializeField] private Image iconCategoty = null;

    [SerializeField] private Text coinAmountText = null;
    [SerializeField] private Text keyAmountText = null;

    [SerializeField] private GameObject keyContainer = null;
    [SerializeField] private GameObject timeContainer = null;


    // Update is called once per frame
    public void Initialize(Sprite spIcon, int indexLevel)
    {
        txtLevel.text = string.Format("LEVEL {0}", indexLevel+1);
        iconCategoty.sprite = spIcon;
        iconCategoty.SetNativeSize();
    }
    public void UpdateCoinsAndKeys(int coins, int keys)
    {
        coinAmountText.text = coins.ToString();
        keyAmountText.text = keys.ToString();
    }

    public void UpdateCoins(int coins)
    {
        coinAmountText.text = coins.ToString();
    }
    public void UpdateKeys(int keys)
    {
        keyAmountText.text = keys.ToString();
    }

}
