using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PlayerInfo
{
    public int coins = 0;
    public int keys = 0;
    public CategoryInfo activeCategoryInfo = null;
    public Board activeBoard = null;
    public string lastCompletedLevels = null;
    public string boardsInProgress = null;
    public string unlockedCategories = null;







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

    public CategoryInfo GetActiveCategoryInfo()
    {
        return this.activeCategoryInfo;
    }

    public void SetActiveCategoryInfo(CategoryInfo activeCategoryInfo)
    {
        this.activeCategoryInfo = activeCategoryInfo;
    }

    public Board GetActiveBoard()
    {
        return this.activeBoard;
    }

    public void SetActiveBoard(Board activeBoard)
    {
        this.activeBoard = activeBoard;
    }
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
