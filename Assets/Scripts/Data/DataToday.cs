using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using System.Linq;

public class DataToday : MonoBehaviour
{

    public static DataToday Instance;

    [SerializeField] SaveableManager saveableManager = null;

    [SerializeField] private List<Quest> listQuest = null;


    private DateTime timeStart;
    private TimeSpan timePlayCurrent;

    public Dictionary<string, int> lastCompletedLevels = null;
    public Dictionary<string, int> ListBoosterUse = null;
    public List<Quest> listQuestUseToday = null;


    private int keysCollect;
    private int keysUse;
    private int coinsCollect;
    private int coinsUse;
    private int amountCategoryNew;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private string GetStringDayNow()
    {
        return DateTime.Now.ToLocalTime().ToString("dd'/'MM'/'yyyy");
    }
    public void SetUp(Dictionary<string, int> lastCompletedLevels)
    {
        if (!PlayerPrefs.HasKey(GameDefine.KEY_DATE_TIME_TODAY)) RestoreData(lastCompletedLevels);
        else
        {
            var strOldDate = PlayerPrefs.GetString(GameDefine.KEY_DATE_TIME_TODAY);
            var strTimeNow = GetStringDayNow();
            if (strOldDate.Equals(strTimeNow)) CallDataToDayLocal();
            else RestoreData(lastCompletedLevels);
        }
        SetTimeStart();
    }

    private void RestoreData(Dictionary<string, int> lastCompletedLevels)
    {
        Debug.Log("RestoreData");
        this.lastCompletedLevels = new Dictionary<string, int>(lastCompletedLevels);
        this.ListBoosterUse = CreateListBooterDefaut();
        this.listQuestUseToday = CreateListRequest();

        keysCollect = 0;
        keysUse = 0;
        coinsCollect = 0;
        coinsUse = 0;

        amountCategoryNew = 0;

        PlayerPrefs.SetString(GameDefine.KEY_DATE_TIME_TODAY, GetStringDayNow());
        PlayerPrefs.SetString(GameDefine.KEY_TIME_PLAY_GAME_TODAY, "00:00:00");
        PlayerPrefs.SetString(GameDefine.KEY_LAST_COMPLETED_LEVELS_TODAY, Utilities.ConvertToJsonString(this.lastCompletedLevels));
        PlayerPrefs.SetString(GameDefine.KEY_LIST_BOOSTER_USE_TODAY, Utilities.ConvertToJsonString(this.ListBoosterUse));
        PlayerPrefs.SetString(GameDefine.KEY_QUEST_USE_TODAY, FromString(listQuestUseToday));
        PlayerPrefs.SetInt(GameDefine.KEY_COINS_COLLECT_TODAY, 0);
        PlayerPrefs.SetInt(GameDefine.KEY_KEYS_COLLECT_TODAY, 0);
        PlayerPrefs.SetInt(GameDefine.KEY_COINS_USE_TODAY, 0);
        PlayerPrefs.SetInt(GameDefine.KEY_KEYS_USE_TODAY, 0);
        PlayerPrefs.SetInt(GameDefine.KEY_NEW_CATEGORY_TODAY, 0);
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
    private List<Quest> CreateListRequest()
    {
        List<int> list = new List<int>();
        List<Quest> listQ = new List<Quest>();

        while (list.Count < 5)
        {
            var i = UnityEngine.Random.Range(0, listQuest.Count);
            if (!list.Contains(i))
            {
                list.Add(i);
                listQ.Add(listQuest[i]);
            }
        }
        return listQ;
    }

    private List<Quest> FromList(string contents)
    {
        return contents.Split(" ; ").ToList().ConvertAll(s => JsonUtility.FromJson<Quest>(s));
    }
    private string FromString(List<Quest> data)
    {
        var str = string.Empty;
        for (int i = 0; i < data.Count; i++)
        {
            str += JsonUtility.ToJson(data[i]);
            if (i == data.Count - 1) return str;
            str += " ; ";
        }
        return str;
    }
    private void CallDataToDayLocal()
    {
        timePlayCurrent = TimeSpan.Parse(PlayerPrefs.GetString(GameDefine.KEY_TIME_PLAY_GAME_TODAY));

        keysCollect = PlayerPrefs.GetInt(GameDefine.KEY_KEYS_COLLECT_TODAY);
        keysUse = PlayerPrefs.GetInt(GameDefine.KEY_KEYS_USE_TODAY);
        coinsCollect = PlayerPrefs.GetInt(GameDefine.KEY_COINS_COLLECT_TODAY);
        coinsUse = PlayerPrefs.GetInt(GameDefine.KEY_COINS_USE_TODAY);
        amountCategoryNew = PlayerPrefs.GetInt(GameDefine.KEY_NEW_CATEGORY_TODAY);

        this.lastCompletedLevels = Convert.ToDictionarySI(PlayerPrefs.GetString(GameDefine.KEY_LAST_COMPLETED_LEVELS_TODAY));
        this.ListBoosterUse = Convert.ToDictionarySI(PlayerPrefs.GetString(GameDefine.KEY_LIST_BOOSTER_USE_TODAY));
        this.listQuestUseToday = FromList(PlayerPrefs.GetString(GameDefine.KEY_QUEST_USE_TODAY));
    }

    public void Setkeys(int amount)
    {
        if (amount <= 0)
        {
            keysUse += Mathf.Abs(amount);
            PlayerPrefs.SetInt(GameDefine.KEY_KEYS_USE_TODAY, keysUse);
        }
        else
        {
            keysCollect += Mathf.Abs(amount);
            PlayerPrefs.SetInt(GameDefine.KEY_KEYS_COLLECT_TODAY, keysCollect);
        }
    }
    public void SetCoins(int amount)
    {
        if (amount <= 0)
        {
            coinsUse += Mathf.Abs(amount);
            PlayerPrefs.SetInt(GameDefine.KEY_COINS_USE_TODAY, coinsUse);
        }
        else
        {
            coinsCollect += Mathf.Abs(amount);
            PlayerPrefs.SetInt(GameDefine.KEY_COINS_COLLECT_TODAY, coinsCollect);
        }
    }
    public void UpdateAmountCategoryNew(int amount)
    {
        amountCategoryNew += amount;
        PlayerPrefs.SetInt(GameDefine.KEY_NEW_CATEGORY_TODAY, amountCategoryNew);
    }
    private void Calculate_Time_Play()
    {
        var spaceTime = DateTime.Now.ToLocalTime().Subtract(timeStart);
        timePlayCurrent = timePlayCurrent.Add(spaceTime);
    }


    private void UpdateTimePlay()
    {
        Calculate_Time_Play();
        PlayerPrefs.SetString(GameDefine.KEY_TIME_PLAY_GAME_TODAY, timePlayCurrent.ToString());
    }
    public void UpdateListBoosterUseToday(string key, int amount)
    {
        if (!ListBoosterUse.ContainsKey(key)) return;
        ListBoosterUse[key] += amount;
        PlayerPrefs.SetString(GameDefine.KEY_LIST_BOOSTER_USE_TODAY, Utilities.ConvertToJsonString(ListBoosterUse));
    }
    public int GetTimesUseBossterWithId(string key)
    {
        if (!ListBoosterUse.ContainsKey(key)) return 0;
        return ListBoosterUse[key];
    }
    public int GetTimesUseAnyBosster()
    {
        int total = 0;
        foreach (var value in ListBoosterUse.Values)
        {
            total += value;
        }
        return total;
    }

    public int GetMinutePlay()
    {
        UpdateTimePlay();
        return (int)timePlayCurrent.TotalMinutes;
    }

    public int GetTotalLevelCompletedInCategory(string idCategory)
    {
        if (lastCompletedLevels.ContainsKey(idCategory)) return 0;
        else return lastCompletedLevels[idCategory];
    }

    public int GetTotalLevelCompletedAnyCategory()
    {
        int total = 0;
        var currentTotalLevelCompleted = DataController.Instance.LastCompletedLevels;
        foreach (var item in currentTotalLevelCompleted)
        {
            if (lastCompletedLevels.ContainsKey(item.Key))
            {
                var startlv = lastCompletedLevels[item.Key];
                if (startlv == 0) total += (item.Value + 1);
                else total += (item.Value - startlv);
            }
            else total += (item.Value + 1);
        }
        return total;
    }
    public List<Quest> GetListQuestToday()
    {
        foreach (var item in listQuestUseToday)
        {
            var current = GetCurrentProgressWithQuest(item.id);
            item.current = current >= item.maximum ? item.maximum : current;
            if (current >= item.maximum)
            {
                item.current = item.maximum;
                item.isCompleted = true;
            }
            else
            {
                item.current = current;
                item.isCompleted = false;
            }
        }
        return listQuestUseToday;
    }
    public void UpdateListQuestToday()
    {
        Debug.Log("UpdateListQuestToday: " + FromString(listQuestUseToday));
        PlayerPrefs.SetString(GameDefine.KEY_QUEST_USE_TODAY, FromString(listQuestUseToday));
    }
    public void logListQuestToday()
    {
        Debug.Log(FromString(listQuestUseToday));
    }

    public void log()
    {
        Debug.Log("timePlayCurrent: " + timePlayCurrent);
        Debug.Log(GetMinutePlay());
        Debug.Log(Utilities.ConvertToJsonString(ListBoosterUse));
        Debug.Log("keysCollect: " + keysCollect + "   keysUse: " + keysUse);
    }
    public void SetTimeStart()
    {
        timeStart = DateTime.Now.ToLocalTime();
    }

    private int GetCurrentProgressWithQuest(string idQuest)
    {
        switch (idQuest)
        {
            case "play30":
                // Debug.Log("time Play: " + GetMinutePlay());
                return GetMinutePlay();
            case "coinCollect250":
                // Debug.Log("coinsCollect: " + coinsCollect);
                return coinsCollect;
            case "keyCollect15":
                // Debug.Log("keysCollect: " + keysCollect);
                return keysCollect;
            case "clearWords5":
                // Debug.Log("clearWords5: " + GetTimesUseBossterWithId("Clear-words"));
                return GetTimesUseBossterWithId("Clear-words");
            case "findLetters5":
                // Debug.Log("findLetters5: " + GetTimesUseBossterWithId("Find-letters"));
                return GetTimesUseBossterWithId("Find-letters");
            case "recommendWord5":
                // Debug.Log("recommendWord5: " + GetTimesUseBossterWithId("Recommend-word"));
                return GetTimesUseBossterWithId("Recommend-word");
            case "findWord5":
                // Debug.Log("findWord5: " + GetTimesUseBossterWithId("Find-words"));
                return GetTimesUseBossterWithId("Find-words");
            case "suggestManyWords5":
                // Debug.Log("suggestManyWords5: " + GetTimesUseBossterWithId("Suggest-many-words"));
                return GetTimesUseBossterWithId("Suggest-many-words");
            case "boosteruse5":
                // Debug.Log("boosteruse5: " + GetTimesUseAnyBosster());
                return GetTimesUseAnyBosster();
            case "levels15Complete":
                // Debug.Log("levels15Complete: " + GetTotalLevelCompletedAnyCategory());
                return GetTotalLevelCompletedAnyCategory();
            case "newCategories":
                // Debug.Log("newCategories: " + amountCategoryNew);
                return amountCategoryNew;
            case "keyUse25":
                // Debug.Log("keyUse25: " + keysUse);
                return keysUse;
            case "coinUse150":
                // Debug.Log("coinUse150: " + coinsUse);
                return coinsUse;
            case "watchVideos5":
                // Debug.Log("watchVideos5: " + 0);
                return 0;
            default: return 0;

        }
    }
}
