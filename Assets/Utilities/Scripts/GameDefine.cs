using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDefine
{
    public static int STARTING_COINS = 100;
    public static int STARTING_KEYS = 1;
    public static string CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    //swinging branch
    public static int CLEAR_WORDS = 10;
    public static int FIND_LETTERS = 15;
    public static int RECOMMEND_WORD = 15;
    public static int FIND_WORDS = 20;
    public static int SUGGEST_MANY_WORDS = 30;
    public static bool DEFAULT_MUSIC = true;
    public static bool DEFAULT_SOUND = true;

    //firebase controller
    public static string WEBCLIENTID = "494966323998-60ifa1jku7g4d21fg8ak8gghgehsf1kp.apps.googleusercontent.com";

    public static DifficultyInfo[] DIFFICULTYINFOS = new DifficultyInfo[]
    {
        new DifficultyInfo(7, 7, 10, 6),
        new DifficultyInfo(10, 10, 20, 9),
        new DifficultyInfo(13, 13, 30, 12)
    };
    // public static Dictionary<string, Color> COLORDEFAULT = new Dictionary<string, Color>()
    // {
    //     {"Blue", new Color(20, 88, 172)},
    //     {"Green", new Color(20,128, 15)},
    //     {"Red", new Color(139, 11, 17)},
    //     {"Yellow", new Color(178, 91, 24)}
    // };
    public static Color[] COLOR_TEXT_BOARD = new Color[]
    {
        new Color32(20, 88, 172,255),
        new Color32(20,128, 15,255),
        new Color32(139, 11, 17,255),
        new Color32(178, 91, 24, 255)
    };    public static Color[] COLOR_TEXT_HIGHLIGHT = new Color[]
    {
        new Color32(7, 68, 144,255),
        new Color32(9,99, 15,255),
        new Color32(139, 11, 17,255),
        new Color32(178, 91, 24, 255)
    };
    public static Color[] COLOR_BG = new Color[]
    {
        new Color32(51, 119, 170,255),
        new Color32(48,184, 97,255),
        new Color32(240, 113, 83,255),
        new Color32(241, 205, 42, 255)
    };
    public static Color[] COLOR_LINE = new Color[]
    {
        new Color32(53, 167, 230,255),
        new Color32(78,202, 106,255),
        new Color32(255, 0, 42,255),
        new Color32(239, 197, 39, 255)
    };



    public static string KEY_GAME = "KEY_GAME";
    public static string KEY_USER = "KEY_USER";
    public static string KEY_USER_COINS = "KEY_USER_COINS";
    public static string KEY_USER_KEYS = "KEY_USER_KEYS";
    public static string KEY_LIST_BOOSTER = "KEY_LIST_BOOSTER";
    public static string KEY_LAST_COMPLETED_LEVELS = "KEY_LAST_COMPLETED_LEVELS";
    public static string KEY_BOARDS_IN_PROGRESS = "KEY_BOARDS_IN_PROGRESS";
    public static string KEY_UNLOCKED_CATEGORIES = "KEY_UNLOCKED_CATEGORIES";

    public static string KEY_DISPLAY_NAME = "KEY_DISPLAY_NAME";
    public static string KEY_USERID = "KEY_USERID";





}
