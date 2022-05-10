using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveableManager : SingletonComponent<SaveableManager>
{
    public void LoadSaveData()
    {
        if (CheckExistData())
        {
            string jsonString = GetString("playerInfo");
            // Debug.Log(jsonString);
            // Người chơi đã từng tham gia trờ chơi
            PlayerInfo playerInfo = JsonUtility.FromJson<PlayerInfo>(jsonString);
            GameManager.Instance.ConfigData(playerInfo);
        }
        else
        {
            // Người chơi chưa từng tham gia trờ chơi
            // Dictionary<string, int> lastCompletedLevel = new Dictionary<string, int>();
            // foreach (var category in categoryInfos)
            // {
            //     lastCompletedLevel.Add(category.saveId, 0);
            // }
            // GameManager.Instance.LastCompletedLevels = lastCompletedLevel;
            SetGameDefaut();
            LoadSaveData();
        }

    }

    public void SaveData()
    {
        GameManager.Instance.SetPlayerInfo();
        SetPlayerInfo(GameManager.Instance.GetPlayerInfo());
    }






    private bool CheckExistData()
    {
        return PlayerPrefs.HasKey("Used_to_play");

    }

    public void SetString(string KeyName, string Value)
    {
        PlayerPrefs.SetString(KeyName, Value);
    }

    public string GetString(string KeyName)
    {
        return PlayerPrefs.GetString(KeyName);
    }
    public void SetPlayerInfo(PlayerInfo playerInfo)
    {
        PlayerPrefs.SetString("playerInfo", playerInfo.SaveToString());
    }
    public void SetGameDefaut()
    {
        PlayerPrefs.SetString("Used_to_play", "true");
        PlayerInfo playerInfo = new PlayerInfo(GameManager.Instance.StartingCoins, GameManager.Instance.StartingKeys);
        SetPlayerInfo(playerInfo);
    }
}
