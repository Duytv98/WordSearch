using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SimpleJSON;
[System.Serializable]
public class PlayerInfo
{
    public string DisplayName;
    public string Email;
    public int coins = 0;
    public int keys = 0;
    // public CategoryInfo activeCategoryInfo = null;
    // public Board activeBoard = null;
    public string lastCompletedLevels = null;
    public string boardsInProgress = null;
    public string unlockedCategories = null;



    public string ToString()
    {
        return "DisplayName: " + DisplayName +
        "\n Email: " + Email +
        "\n coins: " + coins +
        "\n keys: " + keys +
        "\n lastCompletedLevels: " + lastCompletedLevels +
        "\n boardsInProgress: " + boardsInProgress +
        "\n unlockedCategories: " + unlockedCategories;
    }
    public void Union(PlayerInfo playerLocal, PlayerInfo playerFireBase)
    {
        DisplayName = playerFireBase.DisplayName;
        Email = playerFireBase.Email;
        coins = playerLocal.coins >= playerFireBase.coins ? playerLocal.coins : playerFireBase.coins;
        keys = playerLocal.keys >= playerFireBase.keys ? playerLocal.keys : playerFireBase.keys;

        JSONNode lastCompletedLevelsLocalJson = JSON.Parse(playerLocal.lastCompletedLevels);
        JSONNode lastCompletedLevelsFireBaseJson = JSON.Parse(playerFireBase.lastCompletedLevels);
        // Debug.Log("lastCompletedLevelsLocalJson: " + playerLocal.lastCompletedLevels);
        // Debug.Log("lastCompletedLevelsFireBaseJson: " + playerFireBase.lastCompletedLevels);

        Dictionary<string, int> dictionary = new Dictionary<string, int>();
        foreach (var key in lastCompletedLevelsLocalJson.Keys)
        {
            dictionary.Add(key, lastCompletedLevelsLocalJson[key]);
        }
        foreach (var key in lastCompletedLevelsFireBaseJson.Keys)
        {
            if (dictionary.ContainsKey(key)){
                // Debug.Log("key: " + key);
                // Debug.Log("Tồn tại");
                // Debug.Log("dictionary[key]:  " + dictionary[key] );
                // Debug.Log("lastCompletedLevelsFireBaseJson[key]: " + lastCompletedLevelsFireBaseJson[key]);
                dictionary[key] = dictionary[key] >= lastCompletedLevelsFireBaseJson[key] ? dictionary[key] : lastCompletedLevelsFireBaseJson[key];
            } 
            else dictionary.Add(key, lastCompletedLevelsFireBaseJson[key]);
        }
        lastCompletedLevels = Utilities.ConvertToJsonString(dictionary);
        boardsInProgress = "";

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


    public int GetCoins()
    {
        return this.coins;
    }

    public void SetCoins(int coins)
    {
        this.coins = coins;
    }
    public void DecreaseCoins(int coins)
    {
        this.coins -= coins;
    }
    public void IncreaseCoins(int coins)
    {
        this.coins += coins;
    }

    public int GetKeys()
    {
        return this.keys;
    }

    public void SetKeys(int keys)
    {
        this.keys = keys;
    }

    // public CategoryInfo GetActiveCategoryInfo()
    // {
    //     return this.activeCategoryInfo;
    // }

    // public void SetActiveCategoryInfo(CategoryInfo activeCategoryInfo)
    // {
    //     this.activeCategoryInfo = activeCategoryInfo;
    // }

    // public Board GetActiveBoard()
    // {
    //     return this.activeBoard;
    // }

    // public void SetActiveBoard(Board activeBoard)
    // {
    //     this.activeBoard = activeBoard;
    // }
    public string GetLastCompletedLevels()
    {
        return this.lastCompletedLevels;
    }

    public void SetLastCompletedLevels(string lastCompletedLevels)
    {
        this.lastCompletedLevels = lastCompletedLevels;
    }
    public string getUnlockedCategories()
    {
        return this.unlockedCategories;
    }

    public void setUnlockedCategories(string unlockedCategories)
    {
        this.unlockedCategories = unlockedCategories;
    }
    public static PlayerInfo CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PlayerInfo>(jsonString);
    }
    public string SaveToString()
    {
        return JsonUtility.ToJson(this);
    }

    public PlayerInfo GetPlayerInfo()
    {
        return this;
    }

}
