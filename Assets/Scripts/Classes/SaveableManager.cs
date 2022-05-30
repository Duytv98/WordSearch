using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using DG.Tweening;
public class SaveableManager  : MonoBehaviour
{
    
    public static SaveableManager Instance;
     private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start() {
        
    }
    public void LoadDataOnline()
    {
        Debug.Log("LoadDataOnline");

        Debug.Log("CheckExistData(): " + CheckExistData());

        Debug.Log("=========" + "IsMusic4: " + SaveableManager.Instance.IsMusic());
        if (CheckExistData())
        {
            GameManager.Instance.IdPlayer = PlayerPrefs.GetString("UserId");
            GameManager.Instance.IsLogIn = PlayerPrefs.GetString("isLogIn") == "true" ? true : false;
            GameManager.Instance.IsMusic = IsMusic();
            GameManager.Instance.IsSound = IsSound();
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
                // DOVirtual.DelayedCall(2, );
            }
        }
        else
        {
            Debug.Log("=========" + "IsMusic2: " + SaveableManager.Instance.IsMusic());
            Debug.Log("=========" + "May chua Tung choi");
            SaveableManager.Instance.LoadDataOffline();
        }

    }
    public void LoadDataOffline()
    {
        Debug.Log("LoadDataOffline");
        Debug.Log(" CheckExistData(): " + CheckExistData());
        if (CheckExistData())
        {
            // Người chơi đã từng tham gia trờ chơi
            GameManager.Instance.IdPlayer = GetUserId();
            GameManager.Instance.IsLogIn = IsLogIn();
            GameManager.Instance.IsMusic = IsMusic();
            GameManager.Instance.IsSound = IsSound();
            Debug.Log("=========" + "IsMusic(): " + IsMusic());
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
        Debug.Log("SetGameDefaut");
        PlayerPrefs.SetString("Used_to_play", "true1");
        PlayerInfo playerInfo = new PlayerInfo(GameManager.Instance.StartingCoins, GameManager.Instance.StartingKeys);
        SetPlayerInfo(playerInfo);

        Debug.Log("=========" + "SetGameDefaut");
        PlayerPrefs.SetInt("isMusic", 1);
        PlayerPrefs.SetInt("isSound", 1);
    }
    public void SetSound(bool isSound)
    {
        Debug.Log("SetSound: " + isSound);
        PlayerPrefs.SetInt("isSound", isSound ? 1 : -1);
    }
    public void SetMusic(bool isMusic)
    {
        Debug.Log("SetMusic: " + isMusic);
        PlayerPrefs.SetInt("isMusic", isMusic ? 1 : -1);
    }
    public bool IsSound()
    {
        return PlayerPrefs.GetInt("isSound") > 0 ? true : false;
    }
    public bool IsMusic()
    {
        return PlayerPrefs.GetInt("isMusic") > 0 ? true : false;
    }
}
