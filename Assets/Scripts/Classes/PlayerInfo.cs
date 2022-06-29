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
    public string listBooster = null;

    public string ToString()
    {
        return "DisplayName: " + displayName +
               "\n coins: " + coins +
               "\n keys: " + keys +
               "\n lastCompletedLevels: " + lastCompletedLevels +
               "\n unlockedCategories: " + unlockedCategories +
               "\n listBooster: " + listBooster;
    }
    public void Union(PlayerInfo playerLocal, PlayerInfo playerFireBase)
    {
        displayName = playerFireBase.displayName;
        coins = playerLocal.coins >= playerFireBase.coins ? playerLocal.coins : playerFireBase.coins;
        keys = playerLocal.keys >= playerFireBase.keys ? playerLocal.keys : playerFireBase.keys;

        //lastCompletedLevels
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
                dictionary[key] = dictionary[key] >= lastCompletedLevelsFireBaseJson[key] ? dictionary[key] : (int)lastCompletedLevelsFireBaseJson[key];
            else dictionary.Add(key, lastCompletedLevelsFireBaseJson[key]);
        }
        lastCompletedLevels = Utilities.ConvertToJsonString(dictionary);

        //unlockedCategories
        string[] linesCategoriesLocal = playerLocal.unlockedCategories.Split(',');
        string[] linesFireBaseLocal = playerFireBase.unlockedCategories.Split(',');


        List<string> listCategories = new List<string>(linesCategoriesLocal);
        foreach (var categoryName in linesFireBaseLocal)
        {
            if (!listCategories.Contains(categoryName)) listCategories.Add(categoryName);
        }
        unlockedCategories = string.Join(",", listCategories);


        //listBooster
        JSONNode listBoosterLocalJson = JSON.Parse(playerLocal.listBooster);
        JSONNode listBoosterFireBaseJson = JSON.Parse(playerFireBase.listBooster);

        Dictionary<string, int> dicListBooster = new Dictionary<string, int>();
        foreach (var key in listBoosterLocalJson.Keys)
        {
            dicListBooster.Add(key, listBoosterLocalJson[key]);
        }
        foreach (var key in listBoosterFireBaseJson.Keys)
        {
            if (dicListBooster.ContainsKey(key))
                dicListBooster[key] = dicListBooster[key] >= listBoosterFireBaseJson[key] ? dicListBooster[key] : (int)listBoosterFireBaseJson[key];
            else dicListBooster.Add(key, listBoosterFireBaseJson[key]);
        }
        listBooster = Utilities.ConvertToJsonString(dicListBooster);


    }
    public PlayerInfo()
    {
    }
    public string SaveToString()
    {
        return JsonUtility.ToJson(this);
    }

}