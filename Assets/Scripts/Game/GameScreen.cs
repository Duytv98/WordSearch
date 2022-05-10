using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScreen : MonoBehaviour
{
    [SerializeField] private string id = "game";
    [SerializeField] private Text wordHintCostText = null;
    [SerializeField] private Text letterHintCostText = null;

    public void Initialize()
    {
        wordHintCostText.text = "x" + GameManager.Instance.CoinCostWordHint;
        letterHintCostText.text = "x" + GameManager.Instance.CoinCostLetterHint;
    }
}
