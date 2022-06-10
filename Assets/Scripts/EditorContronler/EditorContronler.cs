using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using SFB;
using SimpleJSON;
public class EditorContronler : MonoBehaviour
{

    [SerializeField] Button btnLoadFile = null;
    [SerializeField] Button btnGenerateBoard = null;
    [SerializeField] Button btnSaveBoard = null;
    [SerializeField] Text txtCategory = null;

    private DifficultyInfo difficultyInfo = null;
    private string categoryName = null;
    private string listWord;

    private Board board = null;
    private string txtBorad = null;


    private bool isLoad = false;
    private bool isGenerate = false;

    private void Start()
    {
        difficultyInfo = GameDefine.DIFFICULTYINFOS[0];
        SetActiveGenerateBoard();
        SetActiveSaveBoard();
    }
    public void OnEndEdit(string str)
    {
        listWord = str;
    }
    public void OnChangeDifficulty(int val)
    {
        if (val >= 0 && val <= 2) difficultyInfo = GameDefine.DIFFICULTYINFOS[val];
        Debug.Log(difficultyInfo.Log());

    }

    public void SaveFile()
    {
        var defaultName = categoryName + "_" + difficultyInfo.boardColumnSize + "x" + difficultyInfo.boardRowSize;
        var path = StandaloneFileBrowser.SaveFilePanel("Save File", "", defaultName, "json");

        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, txtBorad);
        }
    }


    public void GenerateBoard()
    {
        if (string.IsNullOrEmpty(listWord)) return;
        List<string> categoryWords = LoadWords(listWord, difficultyInfo.maxWordLength);
        // string str = "";
        // foreach (var item in listWord)
        // {
        //     str += (item );
        // }
        // Debug.Log("load word: " + str);
        List<string> words = new List<string>();
        for (int i = 0; i < categoryWords.Count && words.Count < difficultyInfo.maxWords; i++)
        {
            int randomIndex = Random.Range(i, categoryWords.Count);
            string randomWord = categoryWords[randomIndex];

            categoryWords[randomIndex] = categoryWords[i];
            categoryWords[i] = randomWord;

            words.Add(randomWord);
        }
        BoardCreator.BoardConfig boardConfig = new BoardCreator.BoardConfig();
        boardConfig.rows = difficultyInfo.boardRowSize;
        boardConfig.cols = difficultyInfo.boardColumnSize;
        boardConfig.words = words;
        boardConfig.randomCharacters = GameDefine.CHARACTERS;
        BoardCreator.CreateBoard(boardConfig, OnCasualBoardCreated);

    }

    private List<string> LoadWords(string contents, int maxLength)
    {
        string[] lines = contents.Split('\n');

        List<string> words = new List<string>();
        HashSet<string> seenWords = new HashSet<string>();

        for (int i = 0; i < lines.Length; i++)
        {
            string word = lines[i].TrimEnd('\r', '\n');

            if (!string.IsNullOrEmpty(word) && !seenWords.Contains(word) && word.Length <= maxLength)
            {
                words.Add(word);
                seenWords.Add(word);
            }
        }

        return words;
    }
    private void OnCasualBoardCreated(Board board)
    {
        isGenerate = true;
        SetActiveSaveBoard();
        this.board = board;
        txtBorad = Utilities.ConvertToJsonString(board.ToJson());
        // Debug.Log(Utilities.ConvertToJsonString(board.ToJson()));
    }

    public void LoadFile()
    {
        isLoad = false;
        SetActiveGenerateBoard();
        SetActiveSaveBoard();
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "txt", false);

        if (paths.Length > 0)
        {
            StartCoroutine(OutputRoutine(new System.Uri(paths[0]).AbsoluteUri));
        }
    }

    private IEnumerator OutputRoutine(string url)
    {
        var loader = new UnityWebRequest(url);
        loader.downloadHandler = new DownloadHandlerBuffer();
        yield return loader.SendWebRequest();
        isLoad = true;
        SetActiveGenerateBoard();
        categoryName = GetNameFile(url);
        txtCategory.text = categoryName;
        listWord = loader.downloadHandler.text;

    }
    private string GetNameFile(string url)
    {
        var start = url.LastIndexOf("/") + 1;
        var end = url.LastIndexOf(".");
        var name = url.Substring(start, (end - start));
        return name.Replace(" ", "");
    }


    private void SetActiveGenerateBoard()
    {
        btnGenerateBoard.interactable = isLoad;
    }
    private void SetActiveSaveBoard()
    {
        btnSaveBoard.interactable = isGenerate;

    }
}
