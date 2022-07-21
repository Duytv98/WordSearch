using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;

public class DailyGift : MonoBehaviour
{
    public static DailyGift Instance;
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

    [SerializeField] private CollectGift collectGift = null;

    [SerializeField] private Transform row1 = null;
    [SerializeField] private Transform row2 = null;
    [SerializeField] private Transform ItemGiftDay = null;
    [SerializeField] private ItemDailyGIft day7 = null;

    [SerializeField] private Sprite[] iconSelected = null;
    [SerializeField] private Sprite[] iconGift = null;
    [SerializeField] private Sprite[] iconGiftToday = null;
    [SerializeField] private Sprite oldCheck = null;
    [SerializeField] private Sprite currCheck = null;
    [SerializeField] private Sprite oldBg = null;
    [SerializeField] private Sprite newBg = null;
    [SerializeField] private Sprite newBgBig = null;
    [SerializeField] private Sprite oldHederBG = null;
    [SerializeField] private Sprite currHederBG = null;
    [SerializeField] private Sprite newHederBG = null;
    [SerializeField] private Color[] colorsText = null;


    private string idLastCollectGift = null;
    public Dictionary<string, string> HistoryCollection { get; private set; }

    private Dictionary<string, string> giftInfo;


    public Dictionary<string, ItemDailyGIft> UIDailyGift { get; private set; }

    public void OnShowing()
    {
        SetUp();
    }
    public void SetUp()
    {

        HistoryCollection = new Dictionary<string, string>();
        UIDailyGift = new Dictionary<string, ItemDailyGIft>();
        HistoryCollection = GetHistoryCollectionLocal();
        idLastCollectGift = GetIdCurrentCollectGift();
        // kiem tra user khong nhan qua lien tiep va vuot qua 7 ngay thi khoi tao cho nhan qua lai tu dau
        if (idLastCollectGift != null && !CheckCollectionConsecutiveGifts() ||
           HistoryCollection.Count == 7 && CheckCollectionNextDay())
        {
            HistoryCollection.Clear();
            SaveHistoryCollectionLocal();
        }
        if (idLastCollectGift == null)
        {
            giftInfo = CreateGiftEveryDay();
            SaveGiftInfoLocal();
        }
        else
        {
            giftInfo = GetGiftInfoLocal();
        }
        Debug.Log(Utilities.ConvertToJsonString(giftInfo));
        CreateItemDailyGIft();
        SetUI();
    }
    private void CreateItemDailyGIft()
    {
        if (row2.childCount != 3)
        {
            for (int i = 1; i < 7; i++)
            {
                var parent = i < 4 ? row1 : row2;
                Transform _itemDailyGIft = Instantiate(ItemGiftDay, Vector3.zero, Quaternion.identity, parent);
                _itemDailyGIft.name = "Day-" + i;
            }
        }
        foreach (Transform child in row1)
        {
            var itemDailyGIftScript = child.GetComponent<ItemDailyGIft>();
            itemDailyGIftScript.SetName(child.name);
            UIDailyGift.Add(child.name, itemDailyGIftScript);
        }
        foreach (Transform child in row2)
        {
            var itemDailyGIftScript = child.GetComponent<ItemDailyGIft>();
            itemDailyGIftScript.SetName(child.name);
            UIDailyGift.Add(child.name, itemDailyGIftScript);
        }
        UIDailyGift.Add("Day-7", day7);
        day7.SetName("Day-7");

    }
    private void SetUI()
    {
        int idIntCurr = HistoryCollection.Count;

        for (int i = 0; i < 7; i++)
        {
            ItemDailyGIft day = UIDailyGift["Day-" + (i + 1)];
            if (idIntCurr == 0)
            {
                if (i == idIntCurr) day.SetGiftNext(i == 6 ? newBgBig : newBg, newHederBG, iconGift[i], "00:00", colorsText[1]);
                else day.SetGiftFuture(i == 6 ? newBgBig : newBg, newHederBG, iconGift[i], colorsText[2]);
            }
            else
            {
                var key = "Day-" + (i + 1);
                string nowTimeString = GetStringDateTimeNow();
                if (HistoryCollection.ContainsKey(key))
                {
                    string lastTimeString = HistoryCollection[key];
                    var dayTime = StringToDateTime(nowTimeString).Subtract(StringToDateTime(lastTimeString));
                    // Debug.Log("dayTime: " + dayTime + "   days: " + dayTime.Days + "   minutes: " + dayTime.Seconds);
                    if (dayTime.Days >= 1 && dayTime.Seconds > 0)
                    {
                        // Debug.Log("cách ngày");
                        day.SetDayPassed(oldBg, oldHederBG, iconSelected[i], oldCheck, colorsText[0]);
                    }
                    else
                    {
                        // Debug.Log("hôm qua");
                        day.SetTodayCollected(iconGiftToday[i], currHederBG, currCheck, colorsText[1]);
                    }
                }
                else
                {
                    string lastTimeString = HistoryCollection["Day-" + idIntCurr];
                    int dayTime = SubtractDate(lastTimeString, nowTimeString);
                    if (i == idIntCurr)
                    {
                        var countdownTime = string.Empty;
                        if (dayTime < 1) countdownTime = CountdownTime(lastTimeString);
                        else countdownTime = "00:00";
                        day.SetGiftNext(i == 6 ? newBgBig : newBg, newHederBG, iconGift[i], countdownTime, colorsText[2]);
                    }
                    else day.SetGiftFuture(i == 6 ? newBgBig : newBg, newHederBG, iconGift[i], colorsText[2]);
                }
            }
        }
    }


    private Dictionary<string, string> CreateGiftEveryDay()
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        string[] idGifts = new string[]
            {"Coins", "Clear-words", "Find-letters", "Recommend-word", "Find-words", "Suggest-many-words", "Keys"};
        for (int i = 0; i < idGifts.Length; i++)
        {
            string idDay = "Day-" + (i + 1);
            var indexKey = UnityEngine.Random.Range(i, idGifts.Length);
            var temp = idGifts[indexKey];
            idGifts[indexKey] = idGifts[i];
            idGifts[i] = temp;
            Booter booter = new Booter(idGifts[i], 1);
            dictionary.Add(idDay, booter.GetString());
        }
        return dictionary;
    }
    private Sprite idGiftToSprite(string idGift)
    {
        switch (idGift)
        {
            case "Coins":
                return iconSelected[0];
            case "Clear-words":
                return iconSelected[1];
            case "Find-letters":
                return iconSelected[2];
            case "Recommend-word":
                return iconSelected[3];
            case "Find-words":
                return iconSelected[4];
            case "Suggest-many-words":
                return iconSelected[5];
            case "Keys":
                return iconSelected[6];
            default: return null;
        }
    }

    private string GetIdCurrentCollectGift()
    {
        if (HistoryCollection.Count <= 0) return null;
        return "Day-" + HistoryCollection.Count;
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
        // Debug.Log("HistoryCollection.Count: " + HistoryCollection.Count);
        var idLast = GetIdCurrentCollectGift();
        // Debug.Log(idLast);
        string lastTimeString = HistoryCollection[idLast];
        string nowTimeString = GetStringDateTimeNow();
        int day = SubtractDate(lastTimeString, nowTimeString);
        if (day == 1) return true;
        return false;
    }
    public string GetStringDateTimeNow()
    {
        return DateTime.Now.ToLocalTime().ToString("dd'/'MM'/'yyyy HH:mm:ss");
    }

    public DateTime StringToDateTime(string dateTimestring)
    {
        return DateTime.ParseExact(dateTimestring, "dd'/'MM'/'yyyy HH:mm:ss", CultureInfo.InvariantCulture);
    }
    public int SubtractDate(string start, string end)
    {
        return StringToDateTime(end).Subtract(StringToDateTime(start)).Days;
    }
    private string CountdownTime(string oldTime)
    {
        var newTimeString = StringToDateTime(oldTime).AddDays(1).ToString("dd'/'MM'/'yyyy HH:mm:ss");
        string nowTimeString = GetStringDateTimeNow();
        return StringToDateTime(newTimeString).Subtract(StringToDateTime(nowTimeString)).ToString();
    }








    private Dictionary<string, string> GetHistoryCollectionLocal()
    {
        return Convert.ToDictionarySS(PlayerPrefs.GetString("HistoryCollection"));
    }
    private void SaveHistoryCollectionLocal()
    {
        PlayerPrefs.SetString("HistoryCollection", Utilities.ConvertToJsonString(HistoryCollection));
    }
    private void SetStatusGiftFast(int status)
    {
        PlayerPrefs.SetInt("StatusGiftFast", status);
    }
    private int GetStatusGiftFast()
    {
        return PlayerPrefs.GetInt("StatusGiftFast");
    }
    private Dictionary<string, string> GetGiftInfoLocal()
    {
        return Convert.ToDictionarySS(PlayerPrefs.GetString("GiftInfo"));
    }

    private void SaveGiftInfoLocal()
    {
        PlayerPrefs.SetString("GiftInfo", Utilities.ConvertToJsonString(giftInfo));
    }
    public void OnClick(string id)
    {
        var idIntCurr = "Day-" + (HistoryCollection.Count + 1);
        // Debug.Log("count: " + HistoryCollection.Count);
        if (HistoryCollection.ContainsKey(id))
        {
            // Debug.Log("Đã từng nhận quà");
        }
        else if (idIntCurr.Equals(id))
        {
            if (HistoryCollection.Count == 0) CollectionGift(id);
            else if (CheckCollectionNextDay()) CollectionGift(id);
            else Debug.Log("chưa đến thời gian nhận quà");
        }
        else
        {
            Debug.Log("chưa đến thời gian nhận quà");
        }
    }
    private void CollectionGift(string id)
    {
        HistoryCollection.Add(id, GetStringDateTimeNow());
        SaveHistoryCollectionLocal();
        // Debug.Log("count: " + HistoryCollection.Count);
        // Debug.Log(id[id.Length - 1]);
        int index = int.Parse(id[id.Length - 1].ToString()) - 1;
        ItemDailyGIft day = UIDailyGift[id];
        day.SetTodayCollected(iconGiftToday[index], currHederBG, currCheck, colorsText[1]);

        if (giftInfo.ContainsKey(id))
        {
            Booter booter = GetBooter(id);
            if (booter.id.Equals("Coins")) DataController.Instance.SetCoins(GameDefine.COIN_DAILY_GIFT);
            if (booter.id.Equals("Keys")) DataController.Instance.SetKeys(GameDefine.KEYS_DAILY_GIFT);
            else DataController.Instance.SetListBooster(booter.id, booter.amount);
            collectGift.ShowGift();
            // PopupContainer.Instance.ShowGiftPopup("DAILY GIFT", "YOU GET:\n" + booter.amount + " " + booter.id);
        }
    }

    public Booter GetBooter(string key)
    {
        Booter booter = new Booter();
        booter.StringToJson(giftInfo[key]);
        return booter;
    }


}
