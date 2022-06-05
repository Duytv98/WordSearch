using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public static class Convert
{
    public static Dictionary<string, int> ToDictionarySI(string contents)
    {
        Dictionary<string, int> dictionary = new Dictionary<string, int>();
        JSONNode json = JSON.Parse(contents);
        foreach (var key in json.Keys)
        {
            dictionary.Add(key, json[key]);
        }
        return dictionary;
    }
    public static Dictionary<string, string> ToDictionarySS(string contents)
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        JSONNode json = JSON.Parse(contents);
        foreach (var key in json.Keys)
        {
            dictionary.Add(key, json[key]);
        }
        return dictionary;
    }
    public static List<string> ToListS(string contents)
    {
        string[] lines = contents.Split(',');
        List<string> someList = new List<string>(lines);
        return someList;
    }
}
