using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CasualGame : MonoBehaviour
{
    private List<CategoryInfo> categoryInfos = null;
    private int activeDifficultyIndex;
    private CategoryInfo activeCategoryInfo = null;
    // private int idLevel;
    private void Start()
    {
        this.categoryInfos = GameManager.Instance.CategoryInfos;
    }
    public void StartCasual(int difficultyIndex)
    {
        // idLevel = difficultyIndex;

        // Debug.Log(" StartCasual CasualGame");
        activeCategoryInfo = categoryInfos[Random.Range(0, categoryInfos.Count)];
        GameManager.Instance.ActiveCategoryInfo = activeCategoryInfo;
        activeDifficultyIndex = difficultyIndex;
        GenerateRandomBoard(GameDefine.DIFFICULTYINFOS[difficultyIndex]);
    }
    private void GenerateRandomBoard(DifficultyInfo difficultyInfo)
    {
        // Load all the category words
        List<string> categoryWords = LoadWords(activeCategoryInfo, difficultyInfo.maxWordLength);
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

    private List<string> LoadWords(CategoryInfo categoryInfo, int maxLength)
    {
        string contents = categoryInfo.wordFile.text;
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
        board.difficultyIndex = activeDifficultyIndex;
        GameManager.Instance.CasualBoard = board;
        // Debug.Log(Utilities.ConvertToJsonString(board.ToJson()));
        // ScreenManager.Instance.DailyPuzzleChooseLevel(idLevel);
        ScreenManager.Instance.DailyPuzzleChooseLevel();
        // GameManager.Instance.StartCasual();


    }
}
