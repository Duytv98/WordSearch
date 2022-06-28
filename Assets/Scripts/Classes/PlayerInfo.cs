using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SimpleJSON;
[System.Serializable]
public class PlayerInfo
{
    public string displayName;
    public int coins = 0;
    public int keys = 0;
    public string lastCompletedLevels = null;
    public string unlockedCategories = null;
    public string listBooter = null;
    
    public string ToString()
    {
        return "DisplayName: " + displayName +
               "\n coins: " + coins +
               "\n keys: " + keys +
               "\n lastCompletedLevels: " + lastCompletedLevels +
               "\n unlockedCategories: " + unlockedCategories +
               "\n listBooter: " + listBooter;
    }
    public void Union(PlayerInfo playerLocal, PlayerInfo playerFireBase)
    {
        displayName = playerFireBase.displayName;
        coins = playerLocal.coins >= playerFireBase.coins ? playerLocal.coins : playerFireBase.coins;
        keys = playerLocal.keys >= playerFireBase.keys ? playerLocal.keys : playerFireBase.keys;

        JSONNode lastCompletedLevelsLocalJson = JSON.Parse(playerLocal.lastCompletedLevels);
        JSONNode lastCompletedLevelsFireBaseJson = JSON.Parse(playerFireBase.lastCompletedLevels);

        Dictionary<string, int> dictionary = new Dictionary<string, int>();
        foreach (var key in lastCompletedLevelsLocalJson.Keys)
        {
            dictionary.Add(key, lastCompletedLevelsLocalJson[key]);
        }
        foreach (var key in lastCompletedLevelsFireBaseJson.Keys)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = dictionary[key] >= lastCompletedLevelsFireBaseJson[key] ? dictionary[key] : (int) lastCompletedLevelsFireBaseJson[key];
            else dictionary.Add(key, lastCompletedLevelsFireBaseJson[key]);
        }
        lastCompletedLevels = Utilities.ConvertToJsonString(dictionary);
        listBooter = playerLocal.listBooter;

        string[] linesCategoriesLocal = playerLocal.unlockedCategories.Split(',');
        string[] linesFireBaseLocal = playerFireBase.unlockedCategories.Split(',');


        List<string> listCategories = new List<string>(linesCategoriesLocal);
        foreach (var categoryName in linesFireBaseLocal)
        {
            if (!listCategories.Contains(categoryName)) listCategories.Add(categoryName);
        }
        unlockedCategories = string.Join(",", listCategories);
    }
    public PlayerInfo()
    {
    }
    public PlayerInfo(int coins, int keys)
    {
        this.coins = coins;
        this.keys = keys;
    }
    public string SaveToString()
    {
        return JsonUtility.ToJson(this);
    }

}