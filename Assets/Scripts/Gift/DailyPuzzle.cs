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
    [SerializeField] CasualGame casualGame = null;
    [SerializeField] ProgressPuzzle progressPuzzle = null;
    public Dictionary<string, string> HistoryPuzzle { get; private set; }
    private string startDateTimePuzzle = null;
    [SerializeField] private GiftDay[] GiftDays = null;
    [SerializeField] private Color[] colors = null;
    private Dictionary<string, string> puzzleInfos;

    private Dictionary<string, string> casualBoardsProgress;
    private string idDayChoose = null;
    private string levelChoose = null;
    [SerializeField] private LevelPuzzleInDay[] levelPuzzleInDays = null;
    public void Initialize()
    {
        puzzleInfos = new Dictionary<string, string>();
        casualBoardsProgress = new Dictionary<string, string>();
        puzzleInfos = GetPuzzleInfosLocal();
        casualBoardsProgress = GetLocalProgress();
        startDateTimePuzzle = GetStartDateTimePuzzle();

        progressPuzzle.Initialize(GetTotalLevelComplate(), 15);
        if (String.IsNullOrEmpty(startDateTimePuzzle))
        {
            puzzleInfos = CreatePuzzleInfos();
            casualBoardsProgress.Clear();
            SavePuzzleInfosLocal();
            SaveLocalProgress();
        }
        idDayChoose = "Day-" + GetCurrentDayPuzzle();
        SetBorderDayChoose();
        SetColorGiftDay();
        ShowLevelPlay();
    }

    private Dictionary<string, string> CreatePuzzleInfos()
    {
        // Debug.Log("Tạo Puzzle Infos");
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        for (int i = 1; i < 6; i++)
        {
            string idDay = "Day-" + i;
            if (!dictionary.ContainsKey(idDay))
            {
                LevelPuzzle levelPuzzle = new LevelPuzzle(idDay);
                // string str = Utilities.ConvertToJsonString(levelPuzzle.ToJson());
                dictionary.Add(idDay, Utilities.ConvertToJsonString(levelPuzzle.ToJson()));
            }
        }

        return dictionary;
    }

    private string GetKeyBoardsProgress(string idDay, string level)
    {
        return string.Format("{0}_board_{1}", idDay, level);
    }
    private void SetBoardInProgress(string idDay, string level)
    {
        string saveKey = GetKeyBoardsProgress(idDay, level);
        var board = GameScreen.Instance.ActiveBoard;
        string contentsBoard = Utilities.ConvertToJsonString(board.ToJson());
        // Debug.Log("contentsBoard: " + contentsBoard);
        casualBoardsProgress[saveKey] = contentsBoard;
    }
    private Board GetSavedBoard(string saveKey)
    {

        if (casualBoardsProgress.ContainsKey(saveKey))
        {
            Board board = new Board();

            board.StringToJson(casualBoardsProgress[saveKey]);
            return board;
        }
        return null;
    }

    public void SaveLocalProgress()
    {
        // Debug.Log("SaveLocalProgress");
        PlayerPrefs.SetString("casualBoardsProgress", Utilities.ConvertToJsonString(casualBoardsProgress));
    }
    private Dictionary<string, string> GetLocalProgress()
    {
        var content = PlayerPrefs.GetString("casualBoardsProgress");
        return Convert.ToDictionarySS(content);
    }
    public void SaveCurrentBoard()
    {
        SetBoardInProgress(idDayChoose, levelChoose);
    }

    public void LevelCasualSuccessful()
    {
        // Debug.Log("LevelCasualSuccessful");
        casualBoardsProgress.Remove(GetKeyBoardsProgress(idDayChoose, levelChoose));

        LevelPuzzle dayChoose = GetLevelPuzzle(idDayChoose);
        if (levelChoose == "Easy") dayChoose.easy = 1;
        else if (levelChoose == "Medium") dayChoose.medium = 1;
        else if (levelChoose == "Hard") dayChoose.hard = 1;

        Debug.Log("Easy: " + dayChoose.easy + "  Medium: " + dayChoose.medium + "   Hard: " + dayChoose.hard);

        puzzleInfos[idDayChoose] = Utilities.ConvertToJsonString(dayChoose.ToJson());
        SavePuzzleInfosLocal();
    }
    private void SetColorGiftDay()
    {
        // Debug.Log("SetColorGiftDay");
        // Debug.Log(puzzleInfos.Count);
        var currentDayPuzzle = GetCurrentDayPuzzle();
        // Debug.Log("currentDayPuzzle: " + currentDayPuzzle);
        int indexDay = 0;
        foreach (var puzzleInfo in puzzleInfos)
        {
            LevelPuzzle levelPuzzle = new LevelPuzzle();
            levelPuzzle.StringToJson(puzzleInfo.Value);
            GiftDay giftDay = GiftDays[indexDay];
            if (CheckContinueLevel(levelPuzzle.id, "Easy") || CheckContinueLevel(levelPuzzle.id, "Medium") || CheckContinueLevel(levelPuzzle.id, "Hard"))
            {
                giftDay.image.color = colors[1];
            }
            else if (indexDay < currentDayPuzzle - 1)
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

    private int GetTotalLevelComplate()
    {
        int totalLevel = 0;
        foreach (var puzzle in puzzleInfos)
        {
            LevelPuzzle levelPuzzle = new LevelPuzzle();
            levelPuzzle.StringToJson(puzzle.Value);
            if (levelPuzzle.easy > 0) totalLevel += 1;
            if (levelPuzzle.medium > 0) totalLevel += 1;
            if (levelPuzzle.hard > 0) totalLevel += 1;
        }
        return totalLevel;
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
        // Debug.Log("idInt: " + idInt + "   dayUse: " + dayUse);
        GiftDay giftDay = GiftDays[idInt - 1];
        if (idInt <= dayUse)
        {
            if (String.IsNullOrEmpty(startDateTimePuzzle))
            {
                // Debug.Log("lần đầu nhậ quà");
                startDateTimePuzzle = GetStringDateTimeNow();
                SaveStartDateTimePuzzle();
            }

            idDayChoose = "Day-" + idInt;

            SetBorderDayChoose();
            ShowLevelPlay();
            // Debug.Log("click đúng");
        }
        else
        {
            // Debug.Log("Click sai");
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
        // Debug.Log("ShowLevelPlay");
        LevelPuzzle dayChoose = GetLevelPuzzle(idDayChoose);
        // Debug.Log(dayChoose.Log());
        // Debug.Log(levelPuzzleInDays.Length);
        // foreach (var levelPuzzle in levelPuzzleInDays)
        // {
        //     levelPuzzle.SetUp(dayChoose.);
        // }
        for (int i = 0; i < levelPuzzleInDays.Length; i++)
        {
            var levelPuzzle = levelPuzzleInDays[i];
            switch (i)
            {
                case 0:
                    // Debug.Log("Level Easy");
                    levelPuzzle.SetUp(dayChoose.easy > 0, CheckContinueLevel(idDayChoose, "Easy"), colors[1]);
                    break;
                case 1:
                    // Debug.Log("Level Medium");
                    levelPuzzle.SetUp(dayChoose.medium > 0, CheckContinueLevel(idDayChoose, "Medium"), colors[2]);
                    break;
                case 2:
                    // Debug.Log("Level Hard");
                    levelPuzzle.SetUp(dayChoose.hard > 0, CheckContinueLevel(idDayChoose, "Hard"), colors[3]);
                    break;
            }
        }
    }
    private bool CheckContinueLevel(string idDay, string level)
    {
        var keyProgress = GetKeyBoardsProgress(idDay, level);
        return casualBoardsProgress.ContainsKey(keyProgress);
    }
    public bool NextLevelCasual()
    {
        LevelPuzzle dayChoose = GetLevelPuzzle(idDayChoose);
        if (levelChoose == "Easy")
        {
            if (dayChoose.medium < 0)
            {
                PlayGame(1);
                return true;
            }
            else if (dayChoose.hard < 0)
            {
                PlayGame(2);
                return true;
            }
        }
        else if (levelChoose == "Medium")
        {
            if (dayChoose.easy < 0)
            {
                PlayGame(0);
                return true;
            }
            else if (dayChoose.hard < 0)
            {
                PlayGame(2);
                return true;
            }

        }
        else if (levelChoose == "Hard")
        {
            if (dayChoose.easy < 0)
            {
                PlayGame(0);
                return true;
            }
            else if (dayChoose.medium < 0)
            {
                PlayGame(1);
                return true;
            }
        }
        return false;
    }
    public bool CheckCompletedAllLevel()
    {
        // Debug.Log("idDayChoose: " + idDayChoose);
        LevelPuzzle dayChoose = GetLevelPuzzle(idDayChoose);
        if (dayChoose.easy < 0) return false;
        else if (dayChoose.medium < 0) return false;
        else if (dayChoose.hard < 0) return false;
        return true;
    }

    public void PlayGame(int id)
    {
        if (String.IsNullOrEmpty(startDateTimePuzzle))
        {
            // Debug.Log("lần đầu nhậ quà");
            startDateTimePuzzle = GetStringDateTimeNow();
            SaveStartDateTimePuzzle();
        }

        // ScreenManager.Instance.ActiveDefaultGameScreen(id);
        ScreenManager.Instance.ActiveLoading();

        LevelPuzzle dayChoose = GetLevelPuzzle(idDayChoose);

        bool boardCompleted = false;
        switch (id)
        {
            case 0:
                // Debug.Log("click Easy");
                boardCompleted = dayChoose.easy > 0 ? true : false;
                levelChoose = "Easy";
                break;
            case 1:
                // Debug.Log("click Medium");
                boardCompleted = dayChoose.medium > 0 ? true : false;

                levelChoose = "Medium";

                break;
            case 2:
                // Debug.Log("click Hard");
                boardCompleted = dayChoose.hard > 0 ? true : false;
                levelChoose = "Hard";
                break;
        }

        string keyProgress = GetKeyBoardsProgress(idDayChoose, levelChoose);
        if (!boardCompleted && casualBoardsProgress.ContainsKey(keyProgress))
        {
            Board board = GetSavedBoard(keyProgress);
            
            
            // GameManager.Instance.CasualBoard = board;


            // Debug.Log(board.words.Count);
            // Debug.Log(" ====================================== ");
            // Debug.Log("Get Local board");
            // Debug.Log(Utilities.ConvertToJsonString(board.ToJson()));

            // GameManager.Instance.StartCasual();
            ScreenManager.Instance.DeactivateLoading();
        }
        else
        {
            casualGame.StartCasual(id);
        }
        // Debug.Log(dayChoose.Log());
        // dayChoose.CheckBoard();
        // Debug.Log(Utilities.ConvertToJsonString(dayChoose.ToJson()));
        // puzzleInfos[idDayChoose] = Utilities.ConvertToJsonString(dayChoose.ToJson());
        // Debug.Log(puzzleInfos[idDayChoose]);
        // SavePuzzleInfosLocal();

    }
    public void CreateLevelEnd()
    {
        // GameManager.Instance.StartCasual();
        ScreenManager.Instance.DeactivateLoading();
        // switch (id)
        // {
        //     case 0:
        //         Debug.Log("click Easy");
        //         SetBoardInProgress(idDayChoose, "Easy");
        //         break;
        //     case 1:
        //         Debug.Log("click Medium");
        //         SetBoardInProgress(idDayChoose, "Medium");
        //         break;
        //     case 2:
        //         Debug.Log("click Hard");
        //         SetBoardInProgress(idDayChoose, "Hard");
        //         break;
        // }
        SetBoardInProgress(idDayChoose, levelChoose);
        SaveLocalProgress();

    }


    private void SaveStartDateTimePuzzle()
    {
        // Debug.Log("SaveStartDateTimePuzzle");
        PlayerPrefs.SetString("StartDateTimePuzzle", startDateTimePuzzle);
    }

    private string GetStartDateTimePuzzle()
    {
        return PlayerPrefs.GetString("StartDateTimePuzzle");
    }

    private void SavePuzzleInfosLocal()
    {
        // Debug.Log("SavePuzzleInfosLocal");
        // Debug.Log(Utilities.ConvertToJsonString(puzzleInfos));
        PlayerPrefs.SetString("PuzzleInfos", Utilities.ConvertToJsonString(puzzleInfos));
    }

    private Dictionary<string, string> GetPuzzleInfosLocal()
    {
        // Debug.Log("GetPuzzleInfosLocal");
        // Debug.Log(PlayerPrefs.GetString("PuzzleInfos"));
        return Convert.ToDictionarySS(PlayerPrefs.GetString("PuzzleInfos"));
    }
}