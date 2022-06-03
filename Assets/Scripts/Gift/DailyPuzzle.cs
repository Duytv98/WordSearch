using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;

public class DailyPuzzle : MonoBehaviour
{
    [System.Serializable]
    private class GiftDay
    {
        public string id;
        public Image image;
        public Text TextDay;
    };

    [SerializeField] private string id = "dailyPuzzle";
    public Dictionary<string, string> historyPuzzle { get; private set; }
    private string startDateTimePuzzle = null;
    [SerializeField] private GiftDay[] GiftDays = null;
    [SerializeField] private Color[] colors = null;
    private Dictionary<string, string> puzzleInfos;
    public void Initialize()
    {
        puzzleInfos = new Dictionary<string, string>();
        puzzleInfos = GetPuzzleInfosLocal();
        startDateTimePuzzle = GetStartDateTimePuzzle();
        if(String.IsNullOrEmpty(startDateTimePuzzle))
        Debug.Log(GetCurrentDayPuzzle());
        CreatepuzzleInfos();
    }
    private Dictionary<string, string> CreatepuzzleInfos()
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        for (int i = 1; i < 6; i++)
        {
            string idDay = "Day-" + i;
            if (!dictionary.ContainsKey(idDay))
            {
                LevelPuzzle levelPuzzle = new LevelPuzzle(idDay);
                Debug.Log(levelPuzzle.ToJson());
                string str = Utilities.ConvertToJsonString(levelPuzzle.ToJson());
                Debug.Log(str);
                dictionary.Add(idDay, str);
            }
        }
        return dictionary;
    }
    private void SetColorGiftDay()
    {

    }
    public DateTime StringToDateTime(string dateTimestring)
    {
        return DateTime.ParseExact(dateTimestring, "dd'/'MM'/'yyyy HH:mm:ss", CultureInfo.InvariantCulture);
    }
    public string GetStringDateTimeNow()
    {
        return DateTime.Now.ToLocalTime().ToString("dd'/'MM'/'yyyy HH:mm:ss");
    }
    public int SubtractDate(string start, string end)
    {
        return StringToDateTime(end).Subtract(StringToDateTime(start)).Days;
    }
    public int GetCurrentDayPuzzle()
    {
        if (String.IsNullOrEmpty(startDateTimePuzzle)) return 1;
        else
        {
            string strNowTime = GetStringDateTimeNow();
            int day = SubtractDate(startDateTimePuzzle, strNowTime);
            return (1 + day);
        }

    }

    public void OnClick(int idInt)
    {
        int idIntCurr = historyPuzzle.Count;
        int dayUse = GetCurrentDayPuzzle();
        Debug.Log("idInt: " + idInt + "     idIntCurr: " + idIntCurr + "   dayUse: " + dayUse);
        GiftDay giftDay = GiftDays[idInt - 1];
        if (idInt <= dayUse)
        {
            if (String.IsNullOrEmpty(startDateTimePuzzle))
            {
                Debug.Log("lần đầu nhậ quà");
                startDateTimePuzzle = GetStringDateTimeNow();
                SaveStartDateTimePuzzle();
            }
            Debug.Log("click đúng");
        }
        else
        {
            Debug.Log("Click sai");
        }
    }
    public void PlayGame(int id)
    {
        switch (id)
        {
            case 0:
                Debug.Log("click Easy");
                return;
            case 1:
                Debug.Log("click Medium");
                return;
            case 2:
                Debug.Log("click Hard");
                return;
        }
    }


    private void SaveStartDateTimePuzzle()
    {
        Debug.Log("SaveStartDateTimePuzzle");
        PlayerPrefs.SetString("StartDateTimePuzzle", startDateTimePuzzle);
    }
    private string GetStartDateTimePuzzle()
    {
        return PlayerPrefs.GetString("StartDateTimePuzzle");
    }

    private void SavePuzzleInfosLocal()
    {
        Debug.Log("SavePuzzleInfosLocal");
        PlayerPrefs.SetString("PuzzleInfos", Utilities.ConvertToJsonString(puzzleInfos));
    }
    private Dictionary<string, string> GetPuzzleInfosLocal()
    {
        Debug.Log("GetPuzzleInfosLocal");
        return Convert.ToDictionarySS(PlayerPrefs.GetString("PuzzleInfos")); ;
    }

}
