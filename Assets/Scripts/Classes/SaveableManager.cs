using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase.Auth;
using SimpleJSON;
using DG.Tweening;
public class SaveableManager : MonoBehaviour
{

    public static SaveableManager Instance;
    [SerializeField] private FireBaseController fireBaseController = null;
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
    public void LoadDataOffline()
    {
        // Debug.Log("LoadDataOffline");
        bool isPlay = IsActiveGame();
        if (isPlay)
        {
            GameManager.Instance.GetDataBackground();
            DataController.Instance.GetAllDataUser();
        }
        else
        {
            SetGameDefaut();
            LoadDataOffline();
        }
    }



    //Login and Default
    private void SetActiveGame()
    {
        PlayerPrefs.SetInt(GameDefine.KEY_GAME, 1);
    }

    public bool IsActiveGame()
    {
        return PlayerPrefs.HasKey(GameDefine.KEY_GAME);
    }

    public void SetLogIn(bool isLogIn)
    {
        PlayerPrefs.SetInt("isLogIn", isLogIn == true ? 1 : 0);
        GameManager.Instance.IsLogIn = isLogIn;
    }

    public bool IsLogIn()
    {
        return PlayerPrefs.GetInt("isLogIn") == 1 ? true : false;
    }

    public void SaveProvidersLogin(string providers)
    {
        PlayerPrefs.SetString(GameDefine.KEY_PROVIDERS, providers);
    }

    public string GetProvidersLogin()
    {
        return PlayerPrefs.GetString(GameDefine.KEY_PROVIDERS);
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

    private void SetGameDefaut()
    {
        SetActiveGame();

        SaveMusic(GameDefine.DEFAULT_MUSIC);
        SaveSound(GameDefine.DEFAULT_SOUND);
        SaveCoins(GameDefine.STARTING_COINS);
        SaveKeys(GameDefine.STARTING_KEYS);
        SaveListBooster(CreateListBooterDefaut());
    }

    public void CheckAccount(FirebaseUser user, string providers, string avatar = null)
    {
        if (IsLogIn())
        {
            // Debug.Log("Tung login");
            var lastUseId = GetUserId();
            if (lastUseId.Equals(user.UserId)) Debug.Log("Tai Khoan Cu");
            else Debug.Log("Tai Khoan Moi");
        }
        else
        {
            // Debug.Log("chua tung log");
            SetLogIn(true);
            SaveProvidersLogin(providers);
            SaveDataUser(user.DisplayName, user.UserId, avatar);

            fireBaseController.Read_Data();
        }
    }




    //Sound
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




    //InProgress
    public void SaveCoins(int coins)
    {
        PlayerPrefs.SetInt(GameDefine.KEY_USER_COINS, coins);
        fireBaseController.SaveCoins();
    }

    public void SaveKeys(int keys)
    {
        PlayerPrefs.SetInt(GameDefine.KEY_USER_KEYS, keys);
        fireBaseController.SaveKeys();
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
        fireBaseController.SaveLastCompletedLevels();
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
        fireBaseController.SaveUnlockedCategories();
    }

    public List<string> GetUnlockedCategories()
    {
        var str = PlayerPrefs.GetString(GameDefine.KEY_UNLOCKED_CATEGORIES);
        return Convert.ToListS(str);
    }

    public void SaveListBooster(Dictionary<string, int> listBooster)
    {
        var str = Utilities.ConvertToJsonString(listBooster);
        PlayerPrefs.SetString(GameDefine.KEY_LIST_BOOSTER, str);
        fireBaseController.SaveListBooster();
    }

    public Dictionary<string, int> GetListBooster()
    {
        var str = PlayerPrefs.GetString(GameDefine.KEY_LIST_BOOSTER);
        return Convert.ToDictionarySI(str);
    }

    public void SaveTimeCompleteLevel(Dictionary<string, float> timeCompleteLevel)
    {
        var str = Utilities.ConvertToJsonString(timeCompleteLevel);
        PlayerPrefs.SetString(GameDefine.KEY_TIME_COMPLETE_LEVEL, str);
        fireBaseController.SaveTimeCompleteLevel();
    }


    public Dictionary<string, float> GetTimeCompleteLevel()
    {
        var str = PlayerPrefs.GetString(GameDefine.KEY_TIME_COMPLETE_LEVEL);
        return Convert.ToDictionarySF(str);
    }

    // public void


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
    private void SaveAvatar(string avatar)
    {
        PlayerPrefs.SetString(GameDefine.KEY_AVATAR, avatar);
    }
    public string GetAvatar()
    {
        return PlayerPrefs.GetString(GameDefine.KEY_AVATAR);
    }




    public void SaveDataUser(string name, string userId, string avatar)
    {
        PlayerPrefs.SetString(GameDefine.KEY_DISPLAY_NAME, name);
        PlayerPrefs.SetString(GameDefine.KEY_USERID, userId);
        PlayerPrefs.SetString(GameDefine.KEY_AVATAR, avatar);
    }

    public PlayerInfo GetPlayerLocal()
    {
        PlayerInfo playerInfo = new PlayerInfo();
        playerInfo.displayName = PlayerPrefs.GetString(GameDefine.KEY_DISPLAY_NAME);
        playerInfo.coins = PlayerPrefs.GetInt(GameDefine.KEY_USER_COINS);
        playerInfo.keys = PlayerPrefs.GetInt(GameDefine.KEY_USER_KEYS);
        playerInfo.lastCompletedLevels = PlayerPrefs.GetString(GameDefine.KEY_LAST_COMPLETED_LEVELS);
        playerInfo.unlockedCategories = PlayerPrefs.GetString(GameDefine.KEY_UNLOCKED_CATEGORIES);
        playerInfo.listBooster = PlayerPrefs.GetString(GameDefine.KEY_LIST_BOOSTER);
        playerInfo.timeCompleteLevel = PlayerPrefs.GetString(GameDefine.KEY_TIME_COMPLETE_LEVEL);
        return playerInfo;
    }

    public void SaveDataPlayerLocal(PlayerInfo playerInfo)
    {
        // Debug.Log(JsonUtility.ToJson(playerInfo));
        PlayerPrefs.SetString(GameDefine.KEY_DISPLAY_NAME, playerInfo.displayName);
        PlayerPrefs.SetInt(GameDefine.KEY_USER_COINS, playerInfo.coins);
        PlayerPrefs.SetInt(GameDefine.KEY_USER_KEYS, playerInfo.keys);
        PlayerPrefs.SetString(GameDefine.KEY_LAST_COMPLETED_LEVELS, playerInfo.lastCompletedLevels);
        PlayerPrefs.SetString(GameDefine.KEY_UNLOCKED_CATEGORIES, playerInfo.unlockedCategories);
        PlayerPrefs.SetString(GameDefine.KEY_LIST_BOOSTER, playerInfo.listBooster);
        PlayerPrefs.SetString(GameDefine.KEY_TIME_COMPLETE_LEVEL, playerInfo.timeCompleteLevel);
        PlayerPrefs.SetString(GameDefine.KEY_AVATAR, playerInfo.avatar);
    }



    


}
