
using System.Dynamic;
using System.IO.Pipes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using SimpleJSON;
public class LeaderboardController : MonoBehaviour
{
    // [System.Serializable]
    // private class PlayerLB
    // {

    //     public string avatar;
    //     public string name;
    //     public int score;

    // };

    private int indexPlayer = 19;
    private int plus = 0;
    private List<PlayerLB> _data;

    private int old_score = 0;

    public List<PlayerLB> Data { get => _data; private set => _data = value; }
    public int IndexPlayer { get => indexPlayer; private set => indexPlayer = value; }

    public void Initialize()
    {
        // Debug.Log(SaveableManager.Instance.IsActiveGame());
        if (!PlayerPrefs.HasKey(GameDefine.KEY_LEADERBOARD_PLAYER)) Data = CreatePlayer();
        else Data = UpdateScorePlayer();
        SaveOldScore(old_score);
        SaveListPlayer(Data);

    }
    private List<PlayerLB> CreatePlayer()
    {
        var namePlayer = SaveableManager.Instance.GetDisplayNameUser();
        List<PlayerLB> data = new List<PlayerLB>();
        data.Add(new PlayerLB() { name = "David Darwin", score = Random.Range(5, 10) });
        data.Add(new PlayerLB() { name = "Balthazar Jones", score = Random.Range(5, 15) });
        data.Add(new PlayerLB() { name = "James Chichester", score = Random.Range(5, 15) });
        data.Add(new PlayerLB() { name = "Mary Schooling", score = Random.Range(10, 25) });
        data.Add(new PlayerLB() { name = "Robert Powers", score = Random.Range(10, 25) });
        data.Add(new PlayerLB() { name = "Emily Jackson", score = Random.Range(10, 25) });
        data.Add(new PlayerLB() { name = "Nora Ingalls", score = Random.Range(25, 50) });
        data.Add(new PlayerLB() { name = "Emma Ford", score = Random.Range(25, 50) });
        data.Add(new PlayerLB() { name = "Tom Marsh", score = Random.Range(25, 50) });
        data.Add(new PlayerLB() { name = "Bianca Mitchell", score = Random.Range(25, 50) });
        data.Add(new PlayerLB() { name = "Callie Trujillo", score = Random.Range(5, 70) });
        data.Add(new PlayerLB() { name = "Alejandra Arnold", score = Random.Range(5, 70) });
        data.Add(new PlayerLB() { name = "Gary Holder", score = Random.Range(5, 70) });
        data.Add(new PlayerLB() { name = "Cyrus Powers", score = Random.Range(5, 70) });
        data.Add(new PlayerLB() { name = "Kenyon Stanley", score = Random.Range(5, 70) });
        data.Add(new PlayerLB() { name = "Waylon Larkin", score = Random.Range(5, 70) });

        data.Add(new PlayerLB() { name = namePlayer, score = 0 });

        data.Add(new PlayerLB() { name = "Marquise Watson", score = 0 });
        data.Add(new PlayerLB() { name = "Darrius McCullough", score = 0 });
        data.Add(new PlayerLB() { name = "Sergio Simmons", score = 0 });

        IndexPlayer = data.Count - 4;
        SaveIndexPlayer(IndexPlayer);
        return SortLeaderboard(data);

    }
    private List<PlayerLB> UpdateScorePlayer()
    {
        Data = GetListPlayer();
        List<PlayerLB> _oldData = new List<PlayerLB>(Data);
        IndexPlayer = GetIndexPlayer();
        old_score = GetOldScore();



        plus = Data[IndexPlayer].score - old_score;

        // Debug.Log(Data[IndexPlayer].score + " - " + old_score + " = " + plus);

        var user = Data[IndexPlayer];
        Data[IndexPlayer] = Data[0];
        Data[0] = user;

        for (int i = 1; i < Data.Count; i++)
        {
            var player = Data[i];
            if (i % 2 == 0)
            {
                var index = Random.Range(i, Data.Count);
                var temp1 = Data[index];
                temp1.score += (int)(((float)Random.Range(60, 150) / (float)100) * plus);
                Data[i] = temp1;
                Data[index] = player;
            }
        }
        foreach (var item in _oldData)
        {
            var id = Data.IndexOf(item);
            if (id > 0 && id != IndexPlayer) Data[id].score += (int)(plus * Random.Range(0.5f, 0.75f));
        }
        var newList = SortLeaderboard(Data);
        IndexPlayer = newList.IndexOf(user);
        old_score = newList[IndexPlayer].score;
        SaveIndexPlayer(IndexPlayer);
        return newList;
    }
    public void UpdateLeaderboard(int score)
    {
        Data[IndexPlayer].score = score;
        var user = Data[IndexPlayer];

        Data = SortLeaderboard(Data);
        IndexPlayer = Data.IndexOf(user);

        SaveIndexPlayer(IndexPlayer);
        SaveListPlayer(Data);
    }
    private List<PlayerLB> SortLeaderboard(List<PlayerLB> data)
    {
        return data.OrderByDescending(o => o.score).ToList();
                                    //  .ThenByDescending(x => x.name).ToList();
    }

    private List<PlayerLB> FromList(string contents)
    {
        return contents.Split(" ; ").ToList().ConvertAll(s => JsonUtility.FromJson<PlayerLB>(s));
    }
    private string FromString(List<PlayerLB> data)
    {
        var str = string.Empty;
        for (int i = 0; i < data.Count; i++)
        {
            str += JsonUtility.ToJson(data[i]);
            if (i == data.Count - 1) return str;
            str += " ; ";
        }
        return str;
    }

    private void SaveListPlayer(List<PlayerLB> data)
    {
        var str = FromString(Data);
        PlayerPrefs.SetString(GameDefine.KEY_LEADERBOARD_PLAYER, str);
    }
    private List<PlayerLB> GetListPlayer()
    {
        var str = PlayerPrefs.GetString(GameDefine.KEY_LEADERBOARD_PLAYER);
        return FromList(str);
    }
    private void SaveOldScore(int score)
    {
        PlayerPrefs.SetInt("OLD_SCORE", score);
    }
    private int GetOldScore()
    {
        return PlayerPrefs.GetInt("OLD_SCORE");
    }
    private void SaveIndexPlayer(int index)
    {
        PlayerPrefs.SetInt("INDEX_PLAYER", index);
    }
    private int GetIndexPlayer()
    {
        return PlayerPrefs.GetInt("INDEX_PLAYER");
    }

}

