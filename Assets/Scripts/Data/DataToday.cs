using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;

public class DataToday : MonoBehaviour
{

    public static DataToday Instance;

    [SerializeField] SaveableManager saveableManager = null;



    private DateTime timeStart;
    private TimeSpan timePlayCurrent;



    public Dictionary<string, int> lastCompletedLevels = null;
    public Dictionary<string, int> ListBoosterUse = null;
    private int keysCollect;
    private int keysUse;
    private int coinsCollect;
    private int coinsUse;

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

        keysCollect = 0;
        keysUse = 0;
        coinsCollect = 0;
        coinsUse = 0;

        PlayerPrefs.SetString(GameDefine.KEY_DATE_TIME_TODAY, GetStringDayNow());
        PlayerPrefs.SetString(GameDefine.KEY_TIME_PLAY_GAME_TODAY, "00:00:00");
        PlayerPrefs.SetString(GameDefine.KEY_LAST_COMPLETED_LEVELS_TODAY, Utilities.ConvertToJsonString(this.lastCompletedLevels));
        PlayerPrefs.SetString(GameDefine.KEY_LIST_BOOSTER_USE_TODAY, Utilities.ConvertToJsonString(this.ListBoosterUse));
        PlayerPrefs.SetInt(GameDefine.KEY_COINS_COLLECT_TODAY, 0);
        PlayerPrefs.SetInt(GameDefine.KEY_KEYS_COLLECT_TODAY, 0);
        PlayerPrefs.SetInt(GameDefine.KEY_COINS_USE_TODAY, 0);
        PlayerPrefs.SetInt(GameDefine.KEY_KEYS_USE_TODAY, 0);
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
    private void CallDataToDayLocal()
    {
        timePlayCurrent = TimeSpan.Parse(PlayerPrefs.GetString(GameDefine.KEY_TIME_PLAY_GAME_TODAY));

        keysCollect = PlayerPrefs.GetInt(GameDefine.KEY_KEYS_COLLECT_TODAY);
        keysUse = PlayerPrefs.GetInt(GameDefine.KEY_KEYS_USE_TODAY);
        coinsCollect = PlayerPrefs.GetInt(GameDefine.KEY_COINS_COLLECT_TODAY);
        coinsUse = PlayerPrefs.GetInt(GameDefine.KEY_COINS_USE_TODAY);

        this.lastCompletedLevels = Convert.ToDictionarySI(PlayerPrefs.GetString(GameDefine.KEY_LAST_COMPLETED_LEVELS_TODAY));
        this.ListBoosterUse = Convert.ToDictionarySI(PlayerPrefs.GetString(GameDefine.KEY_LIST_BOOSTER_USE_TODAY));
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

    private void Calculate_Time_Play()
    {
        var spaceTime = DateTime.Now.ToLocalTime().Subtract(timeStart);
        timePlayCurrent = timePlayCurrent.Add(spaceTime);
    }


    public void UpdateTimePlay()
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

    public int GetMinutePlay()
    {
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
}
