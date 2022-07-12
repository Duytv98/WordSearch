using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using SimpleJSON;
public class LeaderboardController : MonoBehaviour
{
    [System.Serializable]
    private class PlayerLB
    {

        public string avatar;
        public string name;
        public int score;

    };



    private int indexPlayer = 19;
    private int plus = 0;
    private List<PlayerLB> _data;
    private List<PlayerLB> _oldData;


    void Start()
    {
        if (!SaveableManager.Instance.IsActiveGame()) CreateDefaultData();
        // UpdateLeaderboard();

    }

    private List<PlayerLB> CreatePlayer()
    {
        List<PlayerLB> data = new List<PlayerLB>();
        data.Add(new PlayerLB() { name = "David Darwin", score = Random.Range(100, 300) });
        data.Add(new PlayerLB() { name = "Balthazar Jones", score = Random.Range(100, 300) });
        data.Add(new PlayerLB() { name = "James Chichester", score = Random.Range(100, 300) });
        data.Add(new PlayerLB() { name = "Mary Schooling", score = Random.Range(100, 300) });
        data.Add(new PlayerLB() { name = "Robert Powers", score = Random.Range(100, 300) });
        data.Add(new PlayerLB() { name = "Emily Jackson", score = Random.Range(100, 300) });
        data.Add(new PlayerLB() { name = "Nora Ingalls", score = Random.Range(100, 300) });
        data.Add(new PlayerLB() { name = "Emma Ford", score = Random.Range(100, 300) });
        data.Add(new PlayerLB() { name = "Tom Marsh", score = Random.Range(100, 300) });
        data.Add(new PlayerLB() { name = "Bianca Mitchell", score = Random.Range(100, 300) });
        data.Add(new PlayerLB() { name = "Callie Trujillo", score = Random.Range(100, 300) });
        data.Add(new PlayerLB() { name = "Alejandra Arnold", score = Random.Range(100, 300) });
        data.Add(new PlayerLB() { name = "Gary Holder", score = Random.Range(100, 300) });
        data.Add(new PlayerLB() { name = "Cyrus Powers", score = Random.Range(100, 300) });
        data.Add(new PlayerLB() { name = "Kenyon Stanley", score = Random.Range(100, 300) });
        data.Add(new PlayerLB() { name = "Waylon Larkin", score = Random.Range(100, 300) });
        data.Add(new PlayerLB() { name = "Marquise Watson", score = Random.Range(100, 300) });
        data.Add(new PlayerLB() { name = "Darrius McCullough", score = Random.Range(100, 300) });
        data.Add(new PlayerLB() { name = "Sergio Simmons", score = Random.Range(100, 300) });
        data.Add(new PlayerLB() { name = "Toiiiiii", score = 0 });


        return data.OrderByDescending(o => o.score)
        .ThenByDescending(x => x.name).ToList();

    }
    private void CreateDefaultData()
    {
        Debug.Log("LeaderboardController CreateDefaultData ");
        _data = CreatePlayer();
        _oldData = new List<PlayerLB>(_data);

        Debug.Log(JsonUtility.ToJson(_data[0]));
        Debug.Log(_data.Count);
        SaveListPlayer(_data);

        var new_oldData = GetListPlayer();
        Debug.Log(new_oldData.Count);
        Debug.Log(JsonUtility.ToJson(new_oldData[0]));
    }
    private List<PlayerLB> UpdateScorePlayer()
    {
        plus = Random.Range(100, 1000);
        _data[indexPlayer].score += plus;

        var user = _data[indexPlayer];
        _data[indexPlayer] = _data[0];
        _data[0] = user;

        for (int i = 1; i < _data.Count; i++)
        {
            var player = _data[i];
            if (i % 2 == 0)
            {
                var index = Random.Range(i, _data.Count);
                var temp1 = _data[index];
                temp1.score += (int)(((float)Random.Range(60, 150) / (float)100) * plus);
                _data[i] = temp1;
                _data[index] = player;
            }
        }
        foreach (var item in this._oldData)
        {
            var id = _data.IndexOf(item);
            if (id > 0) _data[id].score += (int)(plus * Random.Range(0.5f, 0.75f));
        }
        var newList = _data.OrderByDescending(o => o.score)
                            .ThenByDescending(x => x.name).ToList();
        indexPlayer = newList.IndexOf(user);

        // Debug.Log("new index Player: " + indexPlayer);
        return newList;
    }
    public void UpdateLeaderboard()
    {
        _oldData = UpdateScorePlayer();
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
        var str = FromString(_data);
        PlayerPrefs.SetString(GameDefine.KEY_LEADERBOARD_PLAYER, str);
    }
    private List<PlayerLB> GetListPlayer()
    {
        var str = PlayerPrefs.GetString(GameDefine.KEY_LEADERBOARD_PLAYER);
        return FromList(str);
    }
}

