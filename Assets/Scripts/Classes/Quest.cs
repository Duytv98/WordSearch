using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{

    public string id;
    public Sprite icon;
    public string name;
    public int maximum;
    public int current;
    public int amountGift;
    public GiftType giftType;
    public bool isCompleted = false;
    public bool isCollect = false;

    public enum GiftType
    {
        Coins,
        Keys
    }

}
