using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopBar : MonoBehaviour
{
    [Header("Top Bar In Game")]
    [SerializeField] private GameObject topBarInGame = null;
    [SerializeField] private GameObject keyContainer = null;
    [SerializeField] private GameObject timeContainer = null;
    [SerializeField] private Text txtCoinInGame = null;
    [SerializeField] private Text txtKeyInGame = null;
    [SerializeField] private Text txtLevel = null;
    [SerializeField] private Image iconCategoty = null;


    [Header("Top Bar In Home")]
    [SerializeField] private GameObject topBarInHome = null;
    [SerializeField] private Text txtCoinInHome = null;
    [SerializeField] private Text txtKeyInHome = null;

    [Header("Top Bar In Category")]
    [SerializeField] private GameObject topBarInCategory = null;

    public void OnSwitchingScreens(string toScreenId)
    {
        Debug.Log("OnSwitchingScreens: " + toScreenId);
        topBarInGame.SetActive(false);
        topBarInHome.SetActive(false);
        topBarInCategory.SetActive(false);
        switch (toScreenId)
        {
            case "home":
                topBarInHome.SetActive(true);
                break;
            case "game":
                topBarInGame.SetActive(true);
                break;
            case "category":
                topBarInCategory.SetActive(true);
                break;
            default: break;
        }
    }
    public void Initialize(Sprite spIcon, int indexLevel)
    {
        txtLevel.text = string.Format("LEVEL {0}", indexLevel + 1);
        iconCategoty.sprite = spIcon;
        iconCategoty.SetNativeSize();
    }

    public void UpdateCoins(int coins)
    {
        txtCoinInGame.text = coins.ToString();
        txtCoinInHome.text = coins.ToString();
    }
    public void UpdateKeys(int keys)
    {
        txtKeyInGame.text = keys.ToString();
        txtKeyInHome.text = keys.ToString();
    }

}
