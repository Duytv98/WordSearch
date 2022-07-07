using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotEnoughCoinsPopup : MonoBehaviour
{

    public void ClickAD()
    {
        GameManager.Instance.Coins += 100;
        PopupContainer.Instance.CloseCurrentPopup();

        SaveableManager.Instance.SaveCoins(GameManager.Instance.Coins);
        // CloseNotEnoughCoinsPopup();
    }

    // public void CloseNotEnoughCoinsPopup()
    // {
    //     PopupContainer.Instance.ClosePopup("NotEnoughCoinsPopup");
    // }
}
