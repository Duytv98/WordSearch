using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DifficultyInfo
{
    public int boardRowSize;
    public int boardColumnSize;
    public int maxWords;
    public int maxWordLength;

    public DifficultyInfo(int boardRowSize, int boardColumnSize, int maxWords, int maxWordLength)
    {
        this.boardRowSize = boardRowSize;
        this.boardColumnSize = boardColumnSize;
        this.maxWords = maxWords;
        this.maxWordLength = maxWordLength;
    }
    public string Log()
    {
        return "boardRowSize: " + boardRowSize + "  boardColumnSize: " + boardColumnSize + "  maxWords: " + maxWords + "  maxWordLength: " + maxWordLength;
    }
}
