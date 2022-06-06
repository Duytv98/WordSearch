using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System;

[System.Serializable]
public class LevelPuzzle
{
    public string id;
    public int easy;
    public int medium;
    public int hard;
    public string boardEasy = "";
    public string boardMedium = "";
    public string boardHard = "";

    public LevelPuzzle(string id)
    {
        this.id = id;
        this.easy = -1;
        this.medium = -1;
        this.hard = -1;
    }
    public LevelPuzzle()
    {
    }

    public void CheckBoard()
    {
        if (String.IsNullOrEmpty(boardEasy)) boardEasy = "";
        if (String.IsNullOrEmpty(boardMedium)) boardMedium = "";
        if (String.IsNullOrEmpty(boardHard)) boardHard = "";
    }
    public string GetString()
    {
        return Utilities.ConvertToJsonString(this.ToJson());
    }

    public Dictionary<string, object> ToJson()
    {
        var json = new Dictionary<string, object>();

        json["id"] = id;
        json["easy"] = easy;
        json["medium"] = medium;
        json["hard"] = hard;
        json["boardEasy"] = boardEasy;
        json["boardMedium"] = boardMedium;
        json["boardHard"] = boardHard;
        return json;
    }

    public void StringToJson(string contents)
    {
        JSONNode json = JSON.Parse(contents);
        id = json["id"];
        easy = json["easy"].AsInt;
        medium = json["medium"].AsInt;
        hard = json["hard"].AsInt;
        boardEasy = json["boardEasy"];
        boardMedium = json["boardMedium"];
        boardHard = json["boardHard"];
    }

    public string Log()
    {
        return "id: " + id + "\n   easy: " + easy + "   medium: " + medium + "   hard: " + hard + "\n   boardEasy: " + boardEasy + "\n   boardMedium: " + boardMedium + "\n   boardHard: " + boardHard;
    }
}