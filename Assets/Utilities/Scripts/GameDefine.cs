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
    public static int DEFAULT_MUSIC = 1;
    public static int DEFAULT_SOUND = 1;

    //firebase controller
    public static string WEBCLIENTID = "494966323998-60ifa1jku7g4d21fg8ak8gghgehsf1kp.apps.googleusercontent.com";

    public static DifficultyInfo[] DIFFICULTYINFOS = new DifficultyInfo[]
                                            {
                                                new DifficultyInfo(7, 7, 10, 6),
                                                new DifficultyInfo(10, 10, 20, 9),
                                                new DifficultyInfo(13, 13, 30, 12)
                                            };



}
