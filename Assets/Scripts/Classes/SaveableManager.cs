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
        // Debug.Log("LoadDataOnline");


        GameManager.Instance.Update4variable(GetUserId(), IsLogIn(), IsMusic(), IsSound());
        string jsonString = PlayerPrefs.GetString("playerInfo");
        PlayerInfo playerLocal = JsonUtility.FromJson<PlayerInfo>(jsonString);
        // Debug.Log("loacal: ");
        // Debug.Log(playerLocal.ToString());
        // Debug.Log("islogin: " + GameManager.Instance.IsLogIn);
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
        Debug.Log("LoadDataOffline");
        bool isPlay = PlayerPrefs.HasKey("SonatGameStudio1");
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
        // Debug.Log("SaveableManager  SaveData()");
        // Debug.Log("GameManager.Instance.ActiveGameMode: " + GameManager.Instance.ActiveGameMode);
        if (GameManager.Instance.ActiveGameMode == GameManager.GameMode.Progress)
        {
            // Debug.Log("GameManager.Instance.ActiveGameMode == GameManager.GameMode.Progress");
            // Debug.Log("save Data: ");
            // Debug.Log(GameManager.Instance.GetPlayerInfo());
            GameManager.Instance.SetPlayerInfo();
            // Debug.Log(GameManager.Instance.GetPlayerInfo().ToString());
            SetPlayerInfo(GameManager.Instance.GetPlayerInfo());
            if (GameManager.Instance.IsLogIn) FireBaseController.Instance.SaveData(GameManager.Instance.GetPlayerInfo());
        }
        else
        {
            // Debug.Log("ScreenManager.Instance.SaveLocalProgressCasual");
            ScreenManager.Instance.SaveLocalProgressCasual();
        }
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
        PlayerPrefs.SetInt("SonatGameStudio1", 1);
        PlayerPrefs.SetInt("isMusic", GameDefine.DEFAULT_MUSIC);
        PlayerPrefs.SetInt("isSound", GameDefine.DEFAULT_SOUND);
        PlayerInfo playerInfo = new PlayerInfo(GameDefine.STARTING_COINS, GameDefine.STARTING_KEYS);
        playerInfo.listBooter = Utilities.ConvertToJsonString(CreateListBooterDefaut());
        SetPlayerInfo(playerInfo);
    }
    private Dictionary<string, int> CreateListBooterDefaut()
    {
        Dictionary<string, int> ListBooter = new Dictionary<string, int>();
        ListBooter.Add("Clear-words", 0);
        ListBooter.Add("Find-letters", 0);
        ListBooter.Add("Recommend-word", 0);
        ListBooter.Add("Find-words", 0);
        ListBooter.Add("Suggest-many-words", 0);
        return ListBooter;
    }
    public void SetSound(bool isSound)
    {
        PlayerPrefs.SetInt("isSound", isSound ? 1 : -1);
    }
    public void SetMusic(bool isMusic)
    {
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
