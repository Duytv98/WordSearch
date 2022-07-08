using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotEnoughCoinsPopup : MonoBehaviour
{

    public void ClickAD()
    {
        GameManager.Instance.Coins += GameDefine.COINS_REWARD_AD;
        PopupContainer.Instance.CloseCurrentPopup();

        SaveableManager.Instance.SaveCoins(GameManager.Instance.Coins);
    }
}
