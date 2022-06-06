using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

[System.Serializable]
public class Booter
{
    public string id;
    public int amount;

    public Booter(string id, int amount)
    {
        this.id = id;
        this.amount = amount;
    }
    public Booter()
    {
    }
    public string GetString()
    {
        return Utilities.ConvertToJsonString(this.ToJson());
    }
    public Dictionary<string, object> ToJson()
    {
        Dictionary<string, object> json = new Dictionary<string, object>();

        json["id"] = id;
        json["amount"] = amount;
        return json;
    }
    public void StringToJson(string contents)
    {
        JSONNode json = JSON.Parse(contents);
        id = json["id"];
        amount = json["amount"].AsInt;
    }
    public string Log()
    {
        return "id: " + id + "   amount: " + amount;
    }
}