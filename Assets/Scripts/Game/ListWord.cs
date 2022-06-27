using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListWord : MonoBehaviour
{
    [SerializeField] private Sprite[] arrSpire;
    private Dictionary<char, Sprite> dicWord = null;
    public Dictionary<char, Sprite> DicWord { get => dicWord; set => dicWord = value; }

    private void Start() {
        DicWord = new Dictionary<char, Sprite>();

        for (int i = 0; i < GameDefine.CHARACTERS.Length; i++)
        {
            DicWord.Add(GameDefine.CHARACTERS[i], arrSpire[i]);
        }
        Debug.Log("COunt: " + DicWord.Count);
    }
}
