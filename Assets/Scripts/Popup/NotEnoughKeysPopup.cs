using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotEnoughKeysPopup : MonoBehaviour
{
    public void CloseNotEnoughKeysPopup()
    {
        PopupContainer.Instance.ClosePopup();
    }

}
