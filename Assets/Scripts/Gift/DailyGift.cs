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
        public string idGift;
        public int amountGift;
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
        HistoryCollection = new Dictionary<string, string>();
        HistoryCollection = GetHistoryCollectionLocal();
        if (GetIdCurrentCollectGift() != null && !CheckCollectionConsecutiveGifts())
        {
            HistoryCollection.Clear();
            SaveHistoryCollectionLocal();
        }
        idLastCollectGift = GetIdCurrentCollectGift();
        if (idLastCollectGift == null)
        {
            giftInfo = CreateGiftEveryDay();
            SaveGiftInfoLocal();
        }
        else giftInfo = GetGiftInfoLocal();
        var testGiftInfo = GetGiftInfoLocal();
        foreach (var item in testGiftInfo)
        {
            Debug.Log(item.Key + "   " + item.Value);
        }
        Debug.Log("giftInfo.Count: " + giftInfo.Count);
        SetColorGiftDay();
    }
    private Dictionary<string, string> CreateGiftEveryDay()
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        string[] idGifts = new string[] { "Clear-words", "Find-letters", "Recommend-word", "Find-words", "Suggest-many-words" };
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
                Debug.Log(idDay + "     " + booter.GetString());
            }
        }
        return dictionary;
    }

    private void SetColorGiftDay()
    {
        Debug.Log("HistoryCollection.Count: " + HistoryCollection.Count);
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
        return Convert.ToDictionarySS(PlayerPrefs.GetString("GiftInfo")); ;
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
        if (day > 1) return false;
        return true;
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
        GiftDay giftDayChoose = GiftDays[idInt - 1];
        if (idInt <= idIntCurr)
        {
            Debug.Log("Bạn đã nhận quà");
        }
        else if (idInt == idIntCurr + 1)
        {
            if (idIntCurr == 0)
            {
                CollectionGift(giftDayChoose);
            }
            else
            {
                string lastTimeString = HistoryCollection[idLastCollectGift];
                string nowTimeString = GetStringDateTimeNow();
                int day = SubtractDate(lastTimeString, nowTimeString);
                if (day == 0) Debug.Log("Ngay mai quay lai nhan qua nha!");
                else if (day == 1)
                {
                    CollectionGift(giftDayChoose);
                }
            }
            Debug.Log("Nhận quà hôm nay");
        }
        else
        {
            Debug.Log("Bạn cần nhận quà liên tiếp " + (idInt - (idIntCurr + 1)) + " ngày tiếp theo để nhận được phần quà này.");
        }

        idLastCollectGift = GetIdCurrentCollectGift();
    }
    private void CollectionGift(GiftDay giftDay)
    {
        Debug.Log(" ====================== ");
        giftDay.image.color = highlightColors[2];
        HistoryCollection.Add(giftDay.id, GetStringDateTimeNow());
        SaveHistoryCollectionLocal();
        Debug.Log("Day: " + giftDay.id);
        if (giftInfo.ContainsKey(giftDay.id))
        {
            string strBooter = giftInfo[giftDay.id];
            Debug.Log("strBooter: " + strBooter);
            Booter booter = JsonUtility.FromJson<Booter>(strBooter);
            bool test = GameManager.Instance.SetBooter(booter.id, booter.amount);
            Debug.Log(test);
        }

    }

}
