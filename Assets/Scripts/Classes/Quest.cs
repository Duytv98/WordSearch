using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{

    public string id;
    public string name;
    public int maximum;
    public int current;
    public int amountGift;

    public enum GiftType
    {
        Coins,
        Keys
    }

}
