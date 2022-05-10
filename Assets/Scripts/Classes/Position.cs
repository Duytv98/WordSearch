using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Position
{
    public int row;
    public int col;

    public Position(int row, int col)
    {
        this.row = row;
        this.col = col;
    }
    public string Log()
    {
        return string.Format("row: {0}, col: {1}", row, col);
    }
}

