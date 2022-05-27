using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
public class SaveableManager : SingletonComponent<SaveableManager>
{
    public void LoadDataOnline()
    {
        // Debug.Log("LoadDataOnline");
        GameManager.Instance.IdPlayer = PlayerPrefs.GetString("UserId");
        GameManager.Instance.IsLogIn = PlayerPrefs.GetString("isLogIn") == "true" ? true : false;
        string jsonString = PlayerPrefs.GetString("playerInfo");
        PlayerInfo playerLocal = JsonUtility.FromJson<PlayerInfo>(jsonString);
        // GameManager.Instance.IsLogIn = true;
        if (!GameManager.Instance.IsLogIn)
        {
            GameManager.Instance.ConfigData(playerLocal);
        }
        else
        {
            FireBaseController.Instance.Read_Data(playerLocal);
        }
    }
    public void LoadDataOffline()
    {
        // Debug.Log("LoadDataOffline");
        if (CheckExistData())
        {
            // Người chơi đã từng tham gia trờ chơi
            GameManager.Instance.IdPlayer = GetUserId();
            GameManager.Instance.IsLogIn = IsLogIn();
            string jsonString = GetString("playerInfo");
            PlayerInfo playerLocal = JsonUtility.FromJson<PlayerInfo>(jsonString);
            GameManager.Instance.ConfigData(playerLocal);

        }
        else
        {
            SetGameDefaut();
            LoadDataOffline();
        }
    }

    public void SaveData()
    {
        // Debug.Log("save Data: ");
        // Debug.Log(GameManager.Instance.GetPlayerInfo());
        GameManager.Instance.SetPlayerInfo();
        SetPlayerInfo(GameManager.Instance.GetPlayerInfo());
        if (GameManager.Instance.IsLogIn) FireBaseController.Instance.SaveData(GameManager.Instance.GetPlayerInfo());
    }



    public void SetUserId(string userId)
    {
        // Debug.Log("userId1");
        PlayerPrefs.SetString("UserId", userId);

        // Debug.Log("userId2");
    }
    public string GetUserId()
    {
        return PlayerPrefs.GetString("UserId");
    }
    public void SetLogIn(bool isLogIn)
    {
        PlayerPrefs.SetString("isLogIn", isLogIn == true ? "true" : "false");
    }
    public bool IsLogIn()
    {
        return PlayerPrefs.GetString("isLogIn") == "true" ? true : false;
    }


    public bool CheckExistData()
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
