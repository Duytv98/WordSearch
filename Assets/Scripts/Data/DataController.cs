using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataController : MonoBehaviour
{
    public static DataController Instance;

    [SerializeField] SaveableManager saveableManager = null;
    [SerializeField] LeaderboardController leaderboardController = null;
    [SerializeField] DataToday dataToday = null;
    private string keySave;


    [SerializeField] private List<CategoryInfo> categoryInfos = null;
    [SerializeField] private Sprite[] arrSquare = null;
    [SerializeField] private Sprite[] arrSpire;
    private Dictionary<char, Sprite> dicWord = null;
    public List<CategoryInfo> CategoryInfos { get { return categoryInfos; } }
    public Sprite[] ArrSquare { get => arrSquare; set => arrSquare = value; }
    public Dictionary<char, Sprite> DicWord { get => dicWord; set => dicWord = value; }




    public List<string> UnlockedCategories { get; private set; }
    public Dictionary<string, string> BoardsInProgress { get; set; }
    public Dictionary<string, float> TimeCompleteLevel { get; set; }
    public string KeySave { get => keySave; set => keySave = value; }
    public Dictionary<string, int> LastCompletedLevels = null;
    public Dictionary<string, int> ListBooster = null;




    public int Coins { get; set; }
    public int Keys { get; set; }
    public int TotalLevelCompleted { get; set; }


    public CategoryInfo ActiveCategoryInfo { get; set; }
    public int ActiveLevelIndex { get; set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }


        CreateDicWord();


    }
    private void CreateDicWord()
    {
        DicWord = new Dictionary<char, Sprite>();

        for (int i = 0; i < GameDefine.CHARACTERS.Length; i++)
        {
            DicWord.Add(GameDefine.CHARACTERS[i], arrSpire[i]);
        }
    }
    public void CreateKeySave()
    {
        KeySave = string.Format("{0}_{1}", ActiveCategoryInfo.saveId, ActiveLevelIndex);
    }
    public void SetTotalLevelCompleted()
    {
        TotalLevelCompleted = 0;
        // Debug.Log(Utilities.ConvertToJsonString(LastCompletedLevels));
        if (LastCompletedLevels.Count <= 0) TotalLevelCompleted = 0;
        else
        {
            foreach (var value in LastCompletedLevels.Values)
            {
                TotalLevelCompleted += (value + 1);
            }
        }
    }


    public void GetAllDataUser()
    {
        UnlockedCategories = SaveableManager.Instance.GetUnlockedCategories();
        LastCompletedLevels = SaveableManager.Instance.GetLastCompletedLevels();
        ListBooster = SaveableManager.Instance.GetListBooster();
        BoardsInProgress = SaveableManager.Instance.GetBoardsInProgress();
        TimeCompleteLevel = SaveableManager.Instance.GetTimeCompleteLevel();


        Coins = SaveableManager.Instance.GetCoins();
        Keys = SaveableManager.Instance.GetKeys();

        ScreenManager.Instance.UpdateCoinsAndKeys(Coins, Keys);
        ScreenManager.Instance.SetActiveFlashCanvas(false);

        // Debug.Log("LastCompletedLevels: "+ LastCompletedLevels);

        SetTotalLevelCompleted();
        leaderboardController.Initialize();
        dataToday.SetUp(LastCompletedLevels);
    }

    // UnlockedCategories
    public void SetUnlockedCategories(string id)
    {
        UnlockedCategories.Add(id);
        saveableManager.SaveUnlockedCategories(UnlockedCategories);
    }
    public bool IsCategoryLocked(CategoryInfo categoryInfo)
    {
        if (categoryInfo.lockType == CategoryInfo.LockType.None || UnlockedCategories.Contains(categoryInfo.saveId))
        {
            return false;
        }
        return true;
    }

    //Board
    public void SetBoardInProgress(string value)
    {
        BoardsInProgress[KeySave] = value;
        SaveBoardInProgress();
    }

    public void RemoveBoardUse()
    {
        BoardsInProgress.Remove(KeySave);
        SaveBoardInProgress();
    }

    public void SaveBoardInProgress()
    {
        saveableManager.SaveBoardsInProgress(BoardsInProgress);
    }

    public Board GetBoardUse()
    {
        if (BoardsInProgress.ContainsKey(KeySave))
        {
            Board board = new Board();

            board.StringToJson(BoardsInProgress[KeySave]);
            return board;
        }
        return null;
    }

    public Board GetBoardDefault()
    {
        TextAsset levelFile = ActiveCategoryInfo.levelFiles[ActiveLevelIndex];
        Board board = new Board();
        board.FromJson(levelFile);
        return board;
    }

    //TimeCompleteLevel
    public void SetTimeCompleteLevel(float time)
    {
        TimeCompleteLevel[KeySave] = time;
        SaveableManager.Instance.SaveTimeCompleteLevel(TimeCompleteLevel);
    }

    //LastCompletedLevels
    public bool SaveLastCompletedLevels()
    {
        if (!LastCompletedLevels.ContainsKey(ActiveCategoryInfo.saveId) || LastCompletedLevels[ActiveCategoryInfo.saveId] < ActiveLevelIndex)
        {
            LastCompletedLevels[ActiveCategoryInfo.saveId] = ActiveLevelIndex;
            SetTotalLevelCompleted();
            Debug.Log("TotalLevelCompleted: " + TotalLevelCompleted);
            leaderboardController.UpdateLeaderboard(TotalLevelCompleted);
            saveableManager.SaveLastCompletedLevels(LastCompletedLevels);
            return true;
        }
        return false;
    }

    public int GetIndexLevelNext(CategoryInfo categoryInfo)
    {

        int nextLevelIndex = LastCompletedLevels.ContainsKey(categoryInfo.saveId) ? LastCompletedLevels[categoryInfo.saveId] + 1 : 0;
        if (nextLevelIndex >= categoryInfo.levelFiles.Count) nextLevelIndex = categoryInfo.levelFiles.Count - 1;

        return nextLevelIndex;
    }

    //ListBooster
    public bool SetListBooster(string key, int amount)
    {
        if (!ListBooster.ContainsKey(key)) return false;

        ListBooster[key] += amount;
        saveableManager.SaveListBooster(ListBooster);
        return true;

    }

    public Dictionary<string, int> GetListBoosterInGame()
    {

        Dictionary<string, int> dic = new Dictionary<string, int>();
        foreach (var booter in ListBooster)
        {
            var key = booter.Key;
            var value = 0;
            value = booter.Value < 3 ? booter.Value : 3;
            dic.Add(key, value);
        }
        return dic;
    }

    //Coins
    public void SetCoins(int amount)
    {
        Coins += amount;
        dataToday.SetCoins(amount);
        saveableManager.SaveCoins(Coins);
        //update UI
        ScreenManager.Instance.UpdateCoinsAndKeys(Coins, Keys);
    }

    //Keys
    public void SetKeys(int amount)
    {
        Keys += amount;
        saveableManager.SaveKeys(Keys);
        dataToday.Setkeys(amount);
        //update UI
        ScreenManager.Instance.UpdateCoinsAndKeys(Coins, Keys);
    }




    void OnApplicationPause(bool pauseStatus)
    {
        // Debug.Log("pauseStatus ==========================  " + pauseStatus);
        if (!pauseStatus) dataToday.SetTimeStart();
        else
        {
            dataToday.UpdateTimePlay();
            // dataToday.log();
        }
    }
}
