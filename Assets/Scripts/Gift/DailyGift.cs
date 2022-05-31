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
    }

    public Dictionary<string, string> HistoryCollection { get; private set; }
    [SerializeField] private GiftDay[] GiftDays = null;

    private string idLastCollectGift = null;

    // private int sttCurrentCollectGift = 0;

    private void Start()
    {
        HistoryCollection = new Dictionary<string, string>();
        HistoryCollection = GetHistoryCollectionLocal();
        idLastCollectGift = GetIdCurrentCollectGift();
    }
    private Dictionary<string, string> GetHistoryCollectionLocal()
    {
        string contents = PlayerPrefs.GetString("HistoryCollection");
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        JSONNode json = JSON.Parse(contents);
        foreach (var key in json.Keys)
        {
            dictionary.Add(key, json[key]);
        }
        return dictionary;
    }
    private void SaveHistoryCollectionLocal()
    {
        PlayerPrefs.SetString("HistoryCollection", Utilities.ConvertToJsonString(HistoryCollection));
    }
    public DateTime StringToDateTime(string dateTimestring)
    {
        return DateTime.ParseExact(dateTimestring, "dd'/'MM'/'yyyy HH:mm:ss", CultureInfo.InvariantCulture);
    }
    public string GetStringDateTimeNow()
    {
        return DateTime.Now.ToLocalTime().ToString("dd'/'MM'/'yyyy HH:mm:ss");
    }
    public int SubtractDate(DateTime start, DateTime end)
    {
        return end.Subtract(start).Days;
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
                Debug.Log("Lần đầu nhận quà");
                HistoryCollection.Add(giftDayChoose.id, GetStringDateTimeNow());
            }
            else
            {

            }
            Debug.Log("Nhận quà hôm nay");
        }
        else
        {
            Debug.Log("Bạn cần nhận quà liên tiếp " + (idInt - (idIntCurr + 1)) + " ngày tiếp theo để nhận được phần quà này.");
        }
    }

}
