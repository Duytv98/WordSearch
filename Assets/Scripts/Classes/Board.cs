using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
[System.Serializable]
public class Board
{
    public class WordPlacement
    {
        public string word;
        public Position startingPosition;
        public int verticalDirection;
        public int horizontalDirection;
    }
    public enum WordDirection
    {
        Up,
        UpRight,
        Right,
        DownRight,
        Down,
        DownLeft,
        Left,
        UpLeft,
        COUNT
    }
    public const char BlankChar = '\0';
    public int rows;
    public int cols;
    public List<string> words;
    public List<List<char>> boardCharacters;
    public List<WordPlacement> wordPlacements;
    public int difficultyIndex = -1;

    // tập hợp các từ đã được tìm thấy
    public HashSet<string> foundWords = new HashSet<string>();
    // Tập hợp các từ gợi ý
    public HashSet<string> recommendWords = new HashSet<string>();
    public HashSet<string> listWordDeleted = new HashSet<string>();
    public List<Position> locationUnuseds = new List<Position>();

    // Tập hợp vị trí các từ bị loại bỏ
    //
    public HashSet<char> letterHintsUsed = new HashSet<char>();
    public void FromJson(TextAsset levelFile)
    {
        string contents = levelFile.text;
        JSONNode json = JSON.Parse(contents);
        rows = json["rows"].AsInt;
        cols = json["cols"].AsInt;
        words = new List<string>();
        boardCharacters = new List<List<char>>();
        wordPlacements = new List<WordPlacement>();
        locationUnuseds = new List<Position>();

        for (int i = 0; i < json["words"].AsArray.Count; i++)
        {
            words.Add(json["words"].AsArray[i].Value);
        }
        for (int i = 0; i < json["boardCharacters"].AsArray.Count; i++)
        {
            boardCharacters.Add(new List<char>());

            for (int j = 0; j < json["boardCharacters"][i].AsArray.Count; j++)
            {
                char character = json["boardCharacters"][i][j].Value[0];
                boardCharacters[i].Add(character);
            }
        }

        for (int i = 0; i < json["wordPlacements"].AsArray.Count; i++)
        {
            JSONNode wordPlacementJson = json["wordPlacements"].AsArray[i];
            WordPlacement wordPlacement = new WordPlacement();

            wordPlacement.word = wordPlacementJson["word"].Value;
            wordPlacement.startingPosition = new Position(wordPlacementJson["row"].AsInt, wordPlacementJson["col"].AsInt);
            wordPlacement.horizontalDirection = wordPlacementJson["h"].AsInt;
            wordPlacement.verticalDirection = wordPlacementJson["v"].AsInt;

            wordPlacements.Add(wordPlacement);
        }
        for (int i = 0; i < json["locationUnuseds"].AsArray.Count; i++)
        {
            JSONNode locationUnusedJson = json["locationUnuseds"].AsArray[i];
            Position locationUnused = new Position(locationUnusedJson["row"].AsInt, locationUnusedJson["col"].AsInt);
            locationUnuseds.Add(locationUnused);
        }

        for (int i = 0; i < json["foundWords"].AsArray.Count; i++)
        {
            foundWords.Add(json["foundWords"].AsArray[i].Value);
            // Debug.Log(json["foundWords"].AsArray[i].Value);
        }
        for (int i = 0; i < json["recommendWords"].AsArray.Count; i++)
        {
            recommendWords.Add(json["recommendWords"].AsArray[i].Value);
        }
        for (int i = 0; i < json["listWordDeleted"].AsArray.Count; i++)
        {
            listWordDeleted.Add(json["listWordDeleted"].AsArray[i].Value);
        }
        for (int i = 0; i < json["letterHintsUsed"].AsArray.Count; i++)
        {
            letterHintsUsed.Add(json["letterHintsUsed"].AsArray[i].Value[0]);
        }

    }
    public void StringToJson(string contents)
    {
        JSONNode json = JSON.Parse(contents);
        rows = json["rows"].AsInt;
        cols = json["cols"].AsInt;
        words = new List<string>();
        boardCharacters = new List<List<char>>();
        wordPlacements = new List<WordPlacement>();
        locationUnuseds = new List<Position>();
        for (int i = 0; i < json["words"].AsArray.Count; i++)
        {
            words.Add(json["words"].AsArray[i].Value);
        }
        for (int i = 0; i < json["boardCharacters"].AsArray.Count; i++)
        {
            boardCharacters.Add(new List<char>());

            for (int j = 0; j < json["boardCharacters"][i].AsArray.Count; j++)
            {
                char character = json["boardCharacters"][i][j].Value[0];
                boardCharacters[i].Add(character);
            }
        }

        for (int i = 0; i < json["wordPlacements"].AsArray.Count; i++)
        {
            JSONNode wordPlacementJson = json["wordPlacements"].AsArray[i];
            WordPlacement wordPlacement = new WordPlacement();

            wordPlacement.word = wordPlacementJson["word"].Value;
            wordPlacement.startingPosition = new Position(wordPlacementJson["row"].AsInt, wordPlacementJson["col"].AsInt);
            wordPlacement.horizontalDirection = wordPlacementJson["h"].AsInt;
            wordPlacement.verticalDirection = wordPlacementJson["v"].AsInt;

            wordPlacements.Add(wordPlacement);
        }

        for (int i = 0; i < json["locationUnuseds"].AsArray.Count; i++)
        {
            JSONNode locationUnusedJson = json["locationUnuseds"].AsArray[i];
            Position locationUnused = new Position(locationUnusedJson["row"].AsInt, locationUnusedJson["col"].AsInt);
            locationUnuseds.Add(locationUnused);
        }
        for (int i = 0; i < json["foundWords"].AsArray.Count; i++)
        {
            foundWords.Add(json["foundWords"].AsArray[i].Value);
            // Debug.Log(json["foundWords"].AsArray[i].Value);
        }
        for (int i = 0; i < json["recommendWords"].AsArray.Count; i++)
        {
            recommendWords.Add(json["recommendWords"].AsArray[i].Value);
        }
        for (int i = 0; i < json["listWordDeleted"].AsArray.Count; i++)
        {
            listWordDeleted.Add(json["listWordDeleted"].AsArray[i].Value);
        }
        for (int i = 0; i < json["letterHintsUsed"].AsArray.Count; i++)
        {
            letterHintsUsed.Add(json["letterHintsUsed"].AsArray[i].Value[0]);
        }
    }


    public Dictionary<string, object> ToJson()
    {
        Dictionary<string, object> json = new Dictionary<string, object>();

        json["rows"] = rows;
        json["cols"] = cols;
        json["words"] = words;
        json["boardCharacters"] = boardCharacters;

        List<object> wordPlacementsJson = new List<object>();
        List<object> locationUnusedsJson = new List<object>();

        for (int i = 0; i < wordPlacements.Count; i++)
        {
            WordPlacement wordPlacement = wordPlacements[i];
            Dictionary<string, object> wordPlacementJson = new Dictionary<string, object>();

            wordPlacementJson["word"] = wordPlacement.word;
            wordPlacementJson["row"] = wordPlacement.startingPosition.row;
            wordPlacementJson["col"] = wordPlacement.startingPosition.col;
            wordPlacementJson["h"] = wordPlacement.horizontalDirection;
            wordPlacementJson["v"] = wordPlacement.verticalDirection;

            wordPlacementsJson.Add(wordPlacementJson);
        }

        json["wordPlacements"] = wordPlacementsJson;

        for (int i = 0; i < locationUnuseds.Count; i++)
        {
            Position locationUnused = locationUnuseds[i];
            Dictionary<string, object> locationUnusedJson = new Dictionary<string, object>();

            locationUnusedJson["row"] = locationUnused.row;
            locationUnusedJson["col"] = locationUnused.col;

            locationUnusedsJson.Add(locationUnusedJson);
        }
        json["locationUnuseds"] = locationUnusedsJson;

        if (foundWords.Count > 0)
        {
            json["foundWords"] = new List<string>(foundWords);
        }
        if (recommendWords.Count > 0)
        {
            json["recommendWords"] = new List<string>(recommendWords);
        }
        if (listWordDeleted.Count > 0)
        {
            json["listWordDeleted"] = new List<string>(listWordDeleted);
        }
        if (letterHintsUsed.Count > 0)
        {
            json["letterHintsUsed"] = new List<char>(letterHintsUsed);
        }
        return json;
    }

    public void ShuffleListString()
    {
        for (int i = 0; i < words.Count; i++)
        {
            string temp = words[i];
            int randomIndex = Random.Range(i, words.Count);
            words[i] = words[randomIndex];
            words[randomIndex] = temp;
        }
    }



}
