using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Booter
{
    public string id;
    public int amount;
    public Sprite sprite;

    public Booter(string id, int amount)
    {
        this.id = id;
        this.amount = amount;
    }

    public Booter(string id, int amount, Sprite sprite)
    {
        this.id = id;
        this.amount = amount;
        this.sprite = sprite;
    }

    public string GetString()
    {
        return JsonUtility.ToJson(this);
    }
}