using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotEnoughCoinsPopup : MonoBehaviour
{

    public void ClickAD()
    {
        DataController.Instance.SetCoins(GameDefine.COINS_REWARD_AD);
        PopupContainer.Instance.CloseCurrentPopup();
    }
}
