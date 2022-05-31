using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using DG.Tweening;
public class SaveableManager : MonoBehaviour
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
    private void Start()
    {

    }
    public void LoadDataOnline()
    {
        Debug.Log("LoadDataOnline");


        GameManager.Instance.Update4variable(GetUserId(), IsLogIn(), IsMusic(), IsSound());
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
    public void LoadDataOffline()
    {
        bool isPlay = PlayerPrefs.HasKey("SonatGameStudio");
        if (isPlay)
        {
            // Người chơi đã từng tham gia trờ chơi
            GameManager.Instance.Update4variable(GetUserId(), IsLogIn(), IsMusic(), IsSound());
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
        Debug.Log(GameManager.Instance.GetPlayerInfo().ToString());
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
        PlayerPrefs.SetInt("isLogIn", isLogIn == true ? 1 : 0);
    }
    public bool IsLogIn()
    {
        return PlayerPrefs.GetInt("isLogIn") == 1 ? true : false;
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
        PlayerPrefs.SetInt("SonatGameStudio", 1);
        PlayerPrefs.SetInt("isMusic", 1);
        PlayerPrefs.SetInt("isSound", 1);
        PlayerInfo playerInfo = new PlayerInfo(GameManager.Instance.StartingCoins, GameManager.Instance.StartingKeys);
        SetPlayerInfo(playerInfo);

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
