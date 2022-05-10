using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotEnoughCoinsPopup : MonoBehaviour
{

    public void ClickAD()
    {
        GameManager.Instance.Coins += 100;
        CloseNotEnoughCoinsPopup();
    }

    public void CloseNotEnoughCoinsPopup()
    {
        PopupContainer.Instance.ClosePopup();
    }
}
