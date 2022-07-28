using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDefine
{
    public static int STARTING_COINS = 999;
    public static int STARTING_KEYS = 5;
    public static int COINS_REWARD_AD = 50;
    public static string CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    //swinging branch
    public static int COIN_DAILY_GIFT = 25;
    public static int KEYS_DAILY_GIFT = 5;
    public static int CLEAR_WORDS = 10;
    public static int FIND_LETTERS = 15;
    public static int RECOMMEND_WORD = 15;
    public static int FIND_WORDS = 20;
    public static int SUGGEST_MANY_WORDS = 30;
    public static bool DEFAULT_MUSIC = false;
    public static bool DEFAULT_SOUND = false;

    //firebase controller
    public static string WEBCLIENTID = "206344601832-j5diib78fqe87bnkpfahbajrveqqqsvp.apps.googleusercontent.com";

    public static DifficultyInfo[] DIFFICULTYINFOS = new DifficultyInfo[]
    {
        new DifficultyInfo(7, 7, 10, 6),
        new DifficultyInfo(10, 10, 20, 9),
        new DifficultyInfo(13, 13, 30, 12)
    };
    public static Color[] COLOR_TEXT_BOARD = new Color[]
    {
        new Color32(20, 88, 172,255),   //#1458ac
        new Color32(167, 55, 11,255),   //#A7370B
        new Color32(20,128, 15,255),    //#14800f
        new Color32(164,46, 84,255),    //#A42E54
        new Color32(139, 11, 17,255),   //#8b0b11
        new Color32(163, 53, 119,255),  //#A33577
        new Color32(54, 33, 142,255),   //#36218E
        new Color32(34, 105, 174,255),  //#2269AE
        new Color32(20, 124, 111,255),  //#147C6F
        new Color32(178, 91, 24, 255)   //#b25b18
    }; public static Color[] COLOR_TEXT_HIGHLIGHT = new Color[]
    {
        new Color32(7, 68, 144,255),    //#074490
        new Color32(167, 55, 11,255),   //#A7370B
        new Color32(9,99, 15,255),      //#096305
        new Color32(164,46, 84,255),    //#A42E54
        new Color32(139, 11, 17,255),   //#8b0b11
        new Color32(163, 53, 119,255),  //#A33577
        new Color32(54, 33, 142,255),   //#36218E
        new Color32(34, 105, 174,255),  //#2269AE
        new Color32(20, 124, 111,255),  //#147C6F
        new Color32(178, 91, 24, 255)   //#b25b18
    };
    public static Color[] COLOR_BG = new Color[]
    {
        new Color32(51, 119, 170,255), //#37adff
        new Color32(248, 151, 74,255), //#F8974A
        new Color32(48,184, 97,255),   //#30b861
        new Color32(254,131, 185,255), //#FE83B9
        new Color32(240, 113, 83,255), //#f07153
        new Color32(237, 123, 241,255),//#ED7BF1
        new Color32(141, 121, 252,255),//#8D79FC
        new Color32(115, 220, 241,255),//#73DCF1
        new Color32(76, 190, 178,255), //#4CBEB2
        new Color32(241, 205, 42, 255) //#f1cd2a
    };
    public static Color[] COLOR_LINE = new Color[]
    {
        new Color32(53, 167, 230,255),  //#35a7e6
        new Color32(248, 151, 74,255),  //#F8974A
        new Color32(78,202, 106,255),   //#4eca6a
        new Color32(254,131, 185,255),  //#FE83B9
        new Color32(255, 0, 42,255),    //#ff002a
        new Color32(237, 123, 241,255), //#ED7BF1
        new Color32(141, 121, 252,255), //#8D79FC
        new Color32(115, 220, 241,255), //#73DCF1
        new Color32(76, 190, 178,255),  //#4CBEB2
        new Color32(239, 197, 39, 255)  //#efc527
    };



    public static string KEY_GAME = "KEY_GAME";
    public static string KEY_USER = "KEY_USER";
    public static string KEY_USER_COINS = "KEY_USER_COINS";
    public static string KEY_USER_KEYS = "KEY_USER_KEYS";
    public static string KEY_AVATAR = "KEY_AVATAR";
    public static string KEY_LIST_BOOSTER = "KEY_LIST_BOOSTER";
    public static string KEY_LAST_COMPLETED_LEVELS = "KEY_LAST_COMPLETED_LEVELS";
    public static string KEY_BOARDS_IN_PROGRESS = "KEY_BOARDS_IN_PROGRESS";
    public static string KEY_UNLOCKED_CATEGORIES = "KEY_UNLOCKED_CATEGORIES";
    public static string KEY_TIME_COMPLETE_LEVEL = "KEY_TIME_COMPLETE_LEVEL";
    public static string KEY_LEADERBOARD_PLAYER = "KEY_LEADERBOARD_PLAYER";
    public static string KEY_TOTAL_COMPLEMENT_LEVEL = "KEY_TOTAL_COMPLEMENT_LEVEL";


    public static string KEY_DISPLAY_NAME = "KEY_DISPLAY_NAME";
    public static string KEY_USERID = "KEY_USERID";
    public static string KEY_PROVIDERS = "KEY_PROVIDERS";
    public static string KEY_PROVIDERS_GG = "KEY_PROVIDERS_GG";
    public static string KEY_PROVIDERS_FB = "KEY_PROVIDERS_FB";


    public static string KEY_DATE_TIME_TODAY = "KEY_DATE_TIME_TODAY";
    public static string KEY_TIME_PLAY_GAME_TODAY = "KEY_TIME_PLAY_GAME_TODAY";
    public static string KEY_LAST_COMPLETED_LEVELS_TODAY = "KEY_LAST_COMPLETED_LEVELS_TODAY";
    public static string KEY_LIST_BOOSTER_USE_TODAY = "KEY_LIST_BOOSTER_USE_TODAY";
    public static string KEY_COINS_COLLECT_TODAY = "KEY_COINS_COLLECT_TODAY";
    public static string KEY_KEYS_COLLECT_TODAY = "KEY_KEYS_COLLECT_TODAY";
    public static string KEY_COINS_USE_TODAY = "KEY_COINS_USE_TODAY";
    public static string KEY_KEYS_USE_TODAY = "KEY_KEYS_USE_TODAY";
    public static string KEY_QUEST_USE_TODAY = "KEY_QUEST_USE_TODAY";
    public static string KEY_NEW_CATEGORY_TODAY = "KEY_NEW_CATEGORY_TODAY";



    public static string ADMOB_APP_ID = "ca-app-pub-8652816628411018~4107832360";
    public static string ADMOB_BANNER_ID = "ca-app-pub-8652816628411018/6172826721";
    public static string ADMOB_INTERS_ID = "ca-app-pub-8652816628411018/3892209941";
    public static string ADMOB_REWARDED_ID = "ca-app-pub-8652816628411018/2579128273";
    public static string APPSFLYER_DEV_KEY = "Ry3zd9x7iEihJdcAdRb7PX";
}
