using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScreen : MonoBehaviour
{
    [System.Serializable]
    private class Plus
    {
        public string id;
        public Transform transform;
        public Text txtPlus;
    };

    [SerializeField] private string id = "game";
    [SerializeField] private CharacterGrid characterGrid = null;
    [SerializeField] private WordListContainer wordListContainer = null;
    public static GameScreen Instance;
    [SerializeField] private Plus[] arrayPlus = null;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

    }
    public void Initialize()
    {
        Dictionary<string, int> ListBoosterInGame = GameManager.Instance.ListBoosterInGame;
        foreach (var plus in arrayPlus)
        {
            int amountBooter = ListBoosterInGame[plus.id];
            if (amountBooter <= 0)
            {
                plus.transform.gameObject.SetActive(false);
            }
            else
            {
                plus.transform.gameObject.SetActive(true);
                plus.txtPlus.text = "+ " + amountBooter;
            }
        }
    }
    public void UpdateBooterInGame(string key)
    {
        Dictionary<string, int> ListBoosterInGame = GameManager.Instance.ListBoosterInGame;
        int amountBooter = ListBoosterInGame[key];
        Plus plus = Array.Find(arrayPlus, plus => plus.id == key);
        plus.txtPlus.text = "+ " + amountBooter;
        if (amountBooter <= 0) plus.transform.gameObject.SetActive(false);
    }
    public void SetDefault()
    {
        characterGrid.Clear();
        wordListContainer.Clear();
    }


}
