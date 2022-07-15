using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;
using SimpleJSON;

public class DailyGift : MonoBehaviour
{
    [System.Serializable]
    private class GiftDay
    {
        public string id;
        public Image image;
        public Text TextDay;
    };

    [SerializeField] private string id = "dailyGift";
    public Dictionary<string, string> HistoryCollection { get; private set; }
    [SerializeField] private GiftDay[] GiftDays = null;
    private string idLastCollectGift = null;
    [SerializeField] private Color[] highlightColors = null;
    private Dictionary<string, string> giftInfo;

    // private int sttCurrentCollectGift = 0;
    public void Initialize()
    {
        SetUp();
        SetColorGiftDay();
    }
    public void SetUp()
    {
        HistoryCollection = new Dictionary<string, string>();
        HistoryCollection = GetHistoryCollectionLocal();
        if (GetIdCurrentCollectGift() != null && !CheckCollectionConsecutiveGifts() ||
            HistoryCollection.Count == 7 && CheckCollectionNextDay())
        {
            HistoryCollection.Clear();
            SaveHistoryCollectionLocal();
        }
        if (GetIdCurrentCollectGift() == null)
        {
            if (GetStatusGiftFast() < 0) SetStatusGiftFast(1);
        }
        else
        {
            Debug.Log("CheckCollectionNextDay(): " + CheckCollectionNextDay());
            if (CheckCollectionNextDay() && GetStatusGiftFast() < 0) SetStatusGiftFast(1);
        }


        idLastCollectGift = GetIdCurrentCollectGift();
        if (idLastCollectGift == null)
        {
            giftInfo = CreateGiftEveryDay();
            SaveGiftInfoLocal();
        }
        else giftInfo = GetGiftInfoLocal();
        // string str = null;
        // foreach (var gift in giftInfo)
        // {
        //     str += gift.Key;
        //     str += (" --- " + gift.Value + ";   ");
        // }
        // Debug.Log(str);
    }
    public Tuple<string, Booter> GetGiftDay()
    {
        SetUp();
        int idIntCurr = HistoryCollection.Count;
        string idCollect = "Day-" + (idIntCurr + 1);

        if (GetStatusGiftFast() <= 0) return null;
        else if (idIntCurr == 0 || CheckCollectionNextDay())
        {
            return Tuple.Create(idCollect, GetBooter(idCollect));
        }
        return null;
    }
    public void CollectionFastGift(Tuple<string, Booter> tuple)
    {
        HistoryCollection.Add(tuple.Item1, GetStringDateTimeNow());
        SaveHistoryCollectionLocal();
        DataController.Instance.SetListBooster(tuple.Item2.id, tuple.Item2.amount);
    }

    private Dictionary<string, string> CreateGiftEveryDay()
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        string[] idGifts = new string[]
            {"Clear-words", "Find-letters", "Recommend-word", "Find-words", "Suggest-many-words"};
        for (int i = 1; i < 8; i++)
        {
            string idDay = "Day-" + i;
            if (!dictionary.ContainsKey(idDay))
            {
                int amount;
                string idBooter;
                if (i <= 3)
                {
                    idBooter = idGifts[UnityEngine.Random.Range(0, 3)];
                }
                else
                {
                    idBooter = idGifts[UnityEngine.Random.Range(3, 5)];
                }

                amount = UnityEngine.Random.Range(1, 3);
                Booter booter = new Booter(idBooter, amount);
                dictionary.Add(idDay, booter.GetString());
            }
        }

        return dictionary;
    }

    private void SetColorGiftDay()
    {
        // Debug.Log("HistoryCollection.Count: " + HistoryCollection.Count);
        int idIntCurr = HistoryCollection.Count;
        for (int i = 0; i < GiftDays.Length; i++)
        {
            GiftDay giftDay = GiftDays[i];
            if (idIntCurr == 0)
            {
                if (i == idIntCurr) giftDay.image.color = highlightColors[1];
                else giftDay.image.color = highlightColors[3];
            }
            else
            {
                if (i < idIntCurr - 1) giftDay.image.color = highlightColors[0];
                else if (i == idIntCurr - 1)
                {
                    string lastTimeString = HistoryCollection[giftDay.id];
                    string nowTimeString = GetStringDateTimeNow();
                    int day = SubtractDate(lastTimeString, nowTimeString);
                    if (day <= 0) giftDay.image.color = highlightColors[2];
                    else giftDay.image.color = highlightColors[0];
                }
                else if (i == idIntCurr)
                {
                    GiftDay giftDaynew = GiftDays[i - 1];
                    string lastTimeString = HistoryCollection[giftDaynew.id];
                    string nowTimeString = GetStringDateTimeNow();
                    int day = SubtractDate(lastTimeString, nowTimeString);
                    if (day <= 0) giftDay.image.color = highlightColors[3];
                    else giftDay.image.color = highlightColors[1];
                }
                else giftDay.image.color = highlightColors[3];
            }
        }
    }

    private Dictionary<string, string> GetHistoryCollectionLocal()
    {
        return Convert.ToDictionarySS(PlayerPrefs.GetString("HistoryCollection"));
    }

    private void SaveHistoryCollectionLocal()
    {
        PlayerPrefs.SetString("HistoryCollection", Utilities.ConvertToJsonString(HistoryCollection));
    }

    private Dictionary<string, string> GetGiftInfoLocal()
    {
        return Convert.ToDictionarySS(PlayerPrefs.GetString("GiftInfo"));
    }

    private void SaveGiftInfoLocal()
    {
        PlayerPrefs.SetString("GiftInfo", Utilities.ConvertToJsonString(giftInfo));
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

    private bool CheckCollectionConsecutiveGifts()
    {
        var idLast = GetIdCurrentCollectGift();
        string lastTimeString = HistoryCollection[idLast];
        string nowTimeString = GetStringDateTimeNow();
        int day = SubtractDate(lastTimeString, nowTimeString);
        if (day > 1 || day < 0) return false;
        return true;
    }

    private bool CheckCollectionNextDay()
    {
        var idLast = GetIdCurrentCollectGift();
        string lastTimeString = HistoryCollection[idLast];
        string nowTimeString = GetStringDateTimeNow();
        int day = SubtractDate(lastTimeString, nowTimeString);
        if (day == 1) return true;
        return false;
    }

    private GiftDay GetGiftDay(string id)
    {
        return Array.Find(GiftDays, giftDay => giftDay.id == id);
    }

    private string GetIdCurrentCollectGift()
    {
        if (HistoryCollection.Count <= 0) return null;
        return "Day-" + HistoryCollection.Count;
    }

    public void Onclick(int idInt)
    {
        int idIntCurr = HistoryCollection.Count;
        // Debug.Log("idInt: " + idInt + "   idIntCurr: " + idIntCurr);
        GiftDay giftDayChoose = GiftDays[idInt - 1];
        if (idInt <= idIntCurr)
        {
            // Debug.Log("Bạn đã nhận quà");
            PopupContainer.Instance.ShowGiftPopup("DAILY GIFT", "You received a gift today");
        }
        else if (idInt == idIntCurr + 1)
        {
            // Debug.Log("vào else if");
            if (idIntCurr == 0)
            {
                CollectionGift(giftDayChoose);
            }
            else
            {
                if (CheckCollectionNextDay()) CollectionGift(giftDayChoose);
                else PopupContainer.Instance.ShowGiftPopup("DAILY GIFT", "Come back early tomorrow");

            }
        }
        else
        {
            PopupContainer.Instance.ShowGiftPopup("DAILY GIFT", "You need to receive the gift in the next " + (idInt - (idIntCurr + 1)) + " consecutive days to receive this gift.");
        }
        idLastCollectGift = GetIdCurrentCollectGift();
    }

    private void CollectionGift(GiftDay giftDay)
    {
        // Debug.Log("Nhận quà hôm nay");
        giftDay.image.color = highlightColors[2];
        HistoryCollection.Add(giftDay.id, GetStringDateTimeNow());
        SaveHistoryCollectionLocal();
        SetStatusGiftFast(-1);
        // Debug.Log("Day: " + giftDay.id);
        if (giftInfo.ContainsKey(giftDay.id))
        {
            Booter booter = GetBooter(giftDay.id);
            // Debug.Log("booter: " + booter.Log());
            bool test = DataController.Instance.SetListBooster(booter.id, booter.amount);
            // Debug.Log("Set booter: " + test);
            PopupContainer.Instance.ShowGiftPopup("DAILY GIFT", "YOU GET:\n" + booter.amount + " " + booter.id);
        }
    }

    public Booter GetBooter(string key)
    {
        Booter booter = new Booter();
        booter.StringToJson(giftInfo[key]);
        return booter;
    }
    private void SetStatusGiftFast(int status)
    {
        PlayerPrefs.SetInt("StatusGiftFast", status);
    }
    private int GetStatusGiftFast()
    {
        return PlayerPrefs.GetInt("StatusGiftFast");
    }
}