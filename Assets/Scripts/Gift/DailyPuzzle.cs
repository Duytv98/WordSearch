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
        public Text textDay;
        public Image rounded;
    };

    [SerializeField] private string id = "dailyPuzzle";
    public Dictionary<string, string> HistoryPuzzle { get; private set; }
    private string startDateTimePuzzle = null;
    [SerializeField] private GiftDay[] GiftDays = null;
    [SerializeField] private Color[] colors = null;
    private Dictionary<string, string> puzzleInfos;
    private string idDayChoose = null;

    [SerializeField] private LevelPuzzleInDay[] levelPuzzleInDays = null;
    public void Initialize()
    {
        puzzleInfos = new Dictionary<string, string>();
        puzzleInfos = GetPuzzleInfosLocal();
        startDateTimePuzzle = GetStartDateTimePuzzle();
        if (String.IsNullOrEmpty(startDateTimePuzzle))
        {
            puzzleInfos = CreatePuzzleInfos();
            SavePuzzleInfosLocal();
        }
        idDayChoose = "Day-" + GetCurrentDayPuzzle();
        SetBorderDayChoose();
        // Debug.Log(GetCurrentDayPuzzle());
        SetColorGiftDay();
        ShowLevelPlay();
    }

    private Dictionary<string, string> CreatePuzzleInfos()
    {
        Debug.Log("Tạo Puzzle Infos");
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        for (int i = 1; i < 6; i++)
        {
            string idDay = "Day-" + i;
            if (!dictionary.ContainsKey(idDay))
            {
                LevelPuzzle levelPuzzle = new LevelPuzzle(idDay);
                string str = Utilities.ConvertToJsonString(levelPuzzle.ToJson());
                Debug.Log(str);
                dictionary.Add(idDay, str);
            }
        }

        return dictionary;
    }

    private void SetColorGiftDay()
    {
        // Debug.Log("SetColorGiftDay");
        // Debug.Log(puzzleInfos.Count);
        var currentDayPuzzle = GetCurrentDayPuzzle();
        Debug.Log("currentDayPuzzle: " + currentDayPuzzle);
        int indexDay = 0;
        foreach (var puzzleInfo in puzzleInfos)
        {
            LevelPuzzle levelPuzzle = new LevelPuzzle();
            levelPuzzle.StringToJson(puzzleInfo.Value);
            GiftDay giftDay = GiftDays[indexDay];
            if (indexDay < currentDayPuzzle - 1)
            {
                var totalLevel = 0;
                if (levelPuzzle.easy > 0) totalLevel++;
                if (levelPuzzle.medium > 0) totalLevel++;
                if (levelPuzzle.hard > 0) totalLevel++;

                switch (totalLevel)
                {
                    case 1:
                        giftDay.image.color = colors[1];
                        break;
                    case 2:
                        giftDay.image.color = colors[2];
                        break;
                    case 3:
                        giftDay.image.color = colors[3];
                        break;
                    default:
                        giftDay.image.color = colors[0];
                        break;
                }
            }
            else if (indexDay == currentDayPuzzle - 1)
            {
                giftDay.image.color = colors[4];
            }
            else if (!String.IsNullOrEmpty(levelPuzzle.boardEasy) || !String.IsNullOrEmpty(levelPuzzle.boardMedium) || !String.IsNullOrEmpty(levelPuzzle.boardHard))
            {
                giftDay.image.color = colors[1];
            }
            else
            {
                giftDay.image.color = colors[0];
            }
            indexDay++;
        }
        // Debug.Log(currentDayPuzzle);
    }


    private LevelPuzzle GetLevelPuzzle(string key)
    {
        LevelPuzzle levelPuzzle = new LevelPuzzle();

        levelPuzzle.StringToJson(puzzleInfos[key]);
        return levelPuzzle;
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
        //        int idIntCurr = HistoryPuzzle.Count;
        int dayUse = GetCurrentDayPuzzle();
        Debug.Log("idInt: " + idInt + "   dayUse: " + dayUse);
        GiftDay giftDay = GiftDays[idInt - 1];
        if (idInt <= dayUse)
        {
            if (String.IsNullOrEmpty(startDateTimePuzzle))
            {
                Debug.Log("lần đầu nhậ quà");
                startDateTimePuzzle = GetStringDateTimeNow();
                SaveStartDateTimePuzzle();
            }

            idDayChoose = "Day-" + idInt;

            SetBorderDayChoose();
            ShowLevelPlay();
            Debug.Log("click đúng");
        }
        else
        {
            Debug.Log("Click sai");
        }
    }

    public void SetBorderDayChoose()
    {

        foreach (var giftDay in GiftDays)
        {
            if (giftDay.id.Equals(idDayChoose)) giftDay.rounded.gameObject.SetActive(true);
            else giftDay.rounded.gameObject.SetActive(false);
        }
    }
    private void ShowLevelPlay()
    {
        Debug.Log("ShowLevelPlay");
        LevelPuzzle dayChoose = GetLevelPuzzle(idDayChoose);
        Debug.Log(dayChoose.Log());
        Debug.Log(levelPuzzleInDays.Length);
        // foreach (var levelPuzzle in levelPuzzleInDays)
        // {
        //     levelPuzzle.SetUp(dayChoose.);
        // }
        for (int i = 0; i < levelPuzzleInDays.Length; i++)
        {
            var levelPuzzle = levelPuzzleInDays[i];
            // switch (i)
            // {
            //     case 0:
            //         levelPuzzle.SetUp(dayChoose.easy > 0, !String.IsNullOrEmpty(dayChoose.boardEasy), colors);
            //         break;
            //     case 1:
            //         levelPuzzle.SetUp(dayChoose.medium > 0, !String.IsNullOrEmpty(dayChoose.boardMedium), colors);
            //         break;
            //     case 2:
            //         levelPuzzle.SetUp(dayChoose.hard > 0, !String.IsNullOrEmpty(dayChoose.boardHard), colors);
            //         break;
            // }
            switch (i)
            {
                case 0:
                    levelPuzzle.SetUp(dayChoose.easy > 0, !String.IsNullOrEmpty(dayChoose.boardEasy), colors[1]);
                    break;
                case 1:
                    levelPuzzle.SetUp(dayChoose.medium > 0, !String.IsNullOrEmpty(dayChoose.boardMedium), colors[2]);
                    break;
                case 2:
                    levelPuzzle.SetUp(dayChoose.hard > 0, !String.IsNullOrEmpty(dayChoose.boardHard), colors[3]);
                    break;
            }
        }
    }

    public void PlayGame(int id)
    {
        LevelPuzzle dayChoose = GetLevelPuzzle(idDayChoose);
        switch (id)
        {
            case 0:
                Debug.Log("click Easy");
                dayChoose.easy = 1;
                break;
            case 1:
                Debug.Log("click Medium");
                dayChoose.medium = 1;
                break;
            case 2:
                Debug.Log("click Hard");
                dayChoose.hard = 1;
                break;
        }
        Debug.Log(dayChoose.Log());
        dayChoose.CheckBoard();
        Debug.Log(Utilities.ConvertToJsonString(dayChoose.ToJson()));
        puzzleInfos[idDayChoose] = Utilities.ConvertToJsonString(dayChoose.ToJson());
        Debug.Log(puzzleInfos[idDayChoose]);
        SavePuzzleInfosLocal();

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
        return Convert.ToDictionarySS(PlayerPrefs.GetString("PuzzleInfos"));
        ;
    }
}