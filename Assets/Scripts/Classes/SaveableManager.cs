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


        GameManager.Instance.GetDataBackground();
        Debug.Log("GameManager.Instance.IsLogIn: " + GameManager.Instance.IsLogIn);
        if (!GameManager.Instance.IsLogIn)
        {
            Debug.Log("aaaaaaaaaaaa");
            GameManager.Instance.GetAllDataUser();
        }
        else
        {
            //////////////////////
            // FireBaseController.Instance.Read_Data(playerLocal);
            // DOVirtual.DelayedCall(2, );
        }
    }

    public void LoadDataOffline()
    {
        Debug.Log("LoadDataOffline");
        bool isPlay = IsActiveGame();
        if (isPlay)
        {
            // Người chơi đã từng tham gia trờ chơi
            GameManager.Instance.GetDataBackground();
            GameManager.Instance.GetAllDataUser();

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
            //////////////////////

            // if (GameManager.Instance.IsLogIn) FireBaseController.Instance.SaveData(GameManager.Instance.GetPlayerInfo());
        }
        else
        {
            // Debug.Log("ScreenManager.Instance.SaveLocalProgressCasual");
            ScreenManager.Instance.SaveLocalProgressCasual();
        }
    }


    public void SetLogIn(bool isLogIn)
    {
        Debug.Log("setLOgin: " + isLogIn);
        PlayerPrefs.SetInt("isLogIn", isLogIn == true ? 1 : 0);

        Debug.Log("get: " + PlayerPrefs.GetInt("isLogIn"));
    }
    public bool IsLogIn()
    {
        Debug.Log("get: " + PlayerPrefs.GetInt("isLogIn"));
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

    public void SetActiveGame()
    {
        PlayerPrefs.SetInt(GameDefine.KEY_GAME, 1);
    }
    public bool IsActiveGame()
    {
        return PlayerPrefs.HasKey(GameDefine.KEY_GAME);
    }

    public void SaveUserOff(User user)
    {
        PlayerPrefs.SetString(GameDefine.KEY_USER, JsonUtility.ToJson(user));
    }
    public User GetUserOff()
    {
        var strUser = PlayerPrefs.GetString(GameDefine.KEY_USER);
        if (string.IsNullOrEmpty(strUser)) return null;
        return JsonUtility.FromJson<User>(strUser);
    }

    public void SetGameDefaut()
    {
        SetActiveGame();

        SaveMusic(GameDefine.DEFAULT_MUSIC);
        SaveSound(GameDefine.DEFAULT_SOUND);
        SaveCoins(GameDefine.STARTING_COINS);
        SaveKeys(GameDefine.STARTING_KEYS);
        SaveListBooster(CreateListBooterDefaut());
        // PlayerInfo playerInfo = new PlayerInfo(GameDefine.STARTING_COINS, GameDefine.STARTING_KEYS);
        // playerInfo.listBooter = Utilities.ConvertToJsonString(CreateListBooterDefaut());
        // SetPlayerInfo(playerInfo);
    }




    public void SaveSound(bool isSound)
    {
        PlayerPrefs.SetInt("isSound", isSound ? 1 : -1);
    }
    public void SaveMusic(bool isMusic)
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

    public void SaveListBooster(Dictionary<string, int> listBooster)
    {
        var str = Utilities.ConvertToJsonString(listBooster);
        PlayerPrefs.SetString(GameDefine.KEY_LIST_BOOSTER, str);
    }
    public Dictionary<string, int> GetListBooster()
    {
        var str = PlayerPrefs.GetString(GameDefine.KEY_LIST_BOOSTER);
        return Convert.ToDictionarySI(str);
    }





    public void SaveCoins(int coins)
    {
        PlayerPrefs.SetInt(GameDefine.KEY_USER_COINS, coins);
    }
    public void SaveKeys(int keys)
    {
        PlayerPrefs.SetInt(GameDefine.KEY_USER_KEYS, keys);
    }

    public int GetCoins()
    {
        return PlayerPrefs.GetInt(GameDefine.KEY_USER_COINS);
    }

    public int GetKeys()
    {
        return PlayerPrefs.GetInt(GameDefine.KEY_USER_KEYS);
    }


    public void SaveLastCompletedLevels(Dictionary<string, int> lastCompletedLevels)
    {
        var str = Utilities.ConvertToJsonString(lastCompletedLevels);
        PlayerPrefs.SetString(GameDefine.KEY_LAST_COMPLETED_LEVELS, str);
    }
    public Dictionary<string, int> GetLastCompletedLevels()
    {
        var str = PlayerPrefs.GetString(GameDefine.KEY_LAST_COMPLETED_LEVELS);
        return Convert.ToDictionarySI(str);
    }

    public void SaveBoardsInProgress(Dictionary<string, string> boardsInProgress)
    {
        var str = Utilities.ConvertToJsonString(boardsInProgress);
        PlayerPrefs.SetString(GameDefine.KEY_BOARDS_IN_PROGRESS, str);
    }
    public Dictionary<string, string> GetBoardsInProgress()
    {
        var str = PlayerPrefs.GetString(GameDefine.KEY_BOARDS_IN_PROGRESS);
        return Convert.ToDictionarySS(str);
    }
    public void SaveUnlockedCategories(List<string> unlockedCategories)
    {
        var str = string.Join(",", unlockedCategories);
        PlayerPrefs.SetString(GameDefine.KEY_UNLOCKED_CATEGORIES, str);
    }
    public List<string> GetUnlockedCategories()
    {
        var str = PlayerPrefs.GetString(GameDefine.KEY_UNLOCKED_CATEGORIES);
        return Convert.ToListS(str);
    }

    private void SaveDisplayNameUser(string name)
    {
        PlayerPrefs.SetString(GameDefine.KEY_DISPLAY_NAME, name);
    }
    public string GetDisplayNameUser()
    {
        return PlayerPrefs.GetString(GameDefine.KEY_DISPLAY_NAME);
    }
    private void SaveUserId(string userId)
    {
        PlayerPrefs.SetString(GameDefine.KEY_USERID, userId);
    }
    public string GetUserId()
    {
        return PlayerPrefs.GetString(GameDefine.KEY_USERID);
    }

    public void SaveDataUser(string name, string userId)
    {
        SaveDisplayNameUser(name);
        SaveUserId(userId);
    }






}
