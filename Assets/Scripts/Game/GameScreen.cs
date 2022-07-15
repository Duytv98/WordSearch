using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScreen : MonoBehaviour
{
    public static GameScreen Instance;

    [System.Serializable]
    private class Plus
    {
        public string id;
        public Transform transform;
        public Text txtPlus;
    };

    [SerializeField] private string id = "game";

    [SerializeField] private DataController dataController = null;
    [SerializeField] private DataToday dataToday = null;
    [SerializeField] private CharacterGrid characterGrid = null;
    [SerializeField] private WordListContainer wordListContainer = null;
    [SerializeField] private Effect effectContronler = null;
    [SerializeField] private Plus[] arrayPlus = null;



    public Board ActiveBoard { get; private set; }

    public List<List<CharacterGridItem>> CharacterItems;

    public Dictionary<string, int> ListBoosterInGame { get; private set; }




    private bool isCompleted;
    public bool IsCompleted { get => isCompleted; set => isCompleted = value; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        characterGrid.Initialize();
        wordListContainer.Initialize();
    }
    public void UpdateBooterInGame(string key)
    {
        int amountBooter = ListBoosterInGame[key];
        Plus plus = Array.Find(arrayPlus, plus => plus.id == key);
        plus.txtPlus.text = "+ " + amountBooter;
        if (amountBooter <= 0) plus.transform.gameObject.SetActive(false);
    }

    public void Play()
    {
        dataController.CreateKeySave();
        Board board = dataController.GetBoardUse();
        if (board == null) board = dataController.GetBoardDefault();

        Debug.Log(Utilities.ConvertToJsonString(board.ToJson()));
        SetUpListBooterUse();
        SetupGame(board);
        SetBoardInProgress();
    }

    public void SetBoardInProgress()
    {
        string contentsBoard = Utilities.ConvertToJsonString(ActiveBoard.ToJson());
        dataController.SetBoardInProgress(contentsBoard);
    }

    private void SetupGame(Board board)
    {
        IsCompleted = false;
        ActiveBoard = board;

        // ScreenManager.Instance.Show("game");
        characterGrid.SetUp(board, DataController.Instance.DicWord);
        wordListContainer.Setup(board);


    }

    private void SetUpListBooterUse()
    {
        ListBoosterInGame = dataController.GetListBoosterInGame();

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

    private string GetSaveKey(CategoryInfo categoryInfo, int levelIndex = -1)
    {
        return string.Format("{0}_{1}", categoryInfo.saveId, levelIndex);
    }

    private List<string> GetNonFoundWords()
    {
        List<string> nonFoundWords = new List<string>();
        // Lấy ra các từ chưa được tìm thấy
        foreach (var word in ActiveBoard.words)
        {
            if (!ActiveBoard.foundWords.Contains(word) && !ActiveBoard.recommendWords.Contains(word)) nonFoundWords.Add(word);

        }
        return nonFoundWords;
    }

    public void ActionButtonRecommendWord()
    {
        var status = GetNonFoundWords().Count == 0 ? false : true;
        characterGrid.SetInteractableButtonRecommendWord(status);
        characterGrid.SetInteractableButtonSuggestManyWords(status);
    }

    // Event
    public string OnWordSelected(string selectedWord)
    {

        string selectedWordReversed = "";
        foreach (var character in selectedWord)
        {
            selectedWordReversed = character + selectedWordReversed;
        }
        for (int i = 0; i < ActiveBoard.words.Count; i++)
        {
            string word = ActiveBoard.words[i];
            if (ActiveBoard.foundWords.Contains(word) || wordListContainer.UnusedWords.Contains(word)) continue;
            string wordNoSpaces = word.Replace(" ", "");

            // kiểm tra sự trùng khớp
            if (selectedWord == wordNoSpaces || selectedWordReversed == wordNoSpaces)
            {
                // Thêm vào danh sách từ đã tìm thấy
                ActiveBoard.foundWords.Add(word);
                // Debug.Log(" GameManaer ActiveBoard.foundWords.Count: " + ActiveBoard.foundWords.Count);
                if (ActiveBoard.recommendWords.Contains(word)) ActiveBoard.recommendWords.Remove(word);
                SetBoardInProgress();
                return word;
            }
        }
        return null;
    }

    public bool CheckBoardCompleted()
    {
        if (ActiveBoard.foundWords.Count == ActiveBoard.words.Count)
        {
            if (!IsCompleted) BoardCompleted();
            return true;
        }
        return false;
    }

    private void BoardCompleted()
    {

        Debug.Log("BoardCompleted");
        IsCompleted = true;

        dataController.RemoveBoardUse();
        dataController.SaveLastCompletedLevels();

        int coinsAwarded = 10;
        int keysAwarded = 1;


        dataController.SetCoins(coinsAwarded);
        dataController.SetKeys(keysAwarded);

        PopupContainer.Instance.ShowLevelCompletePopup(coinsAwarded, keysAwarded);

        AudioManager.Instance.Play("level-complete");
    }

    public void WordListContainer_PlusWord()
    {
        wordListContainer.PlusWord(ActiveBoard.foundWords);
    }

    public void wordListContainer_SetWordRecommend(string word, int indexColor)
    {
        wordListContainer.SetWordRecommend(word, indexColor);
    }

    public void WordListContainer_SetWordFound(string word)
    {
        wordListContainer.SetWordFound(word);
    }

    public void HintHighlightWord()
    {
        bool useBooter = false;
        string key = "Find-words";
        if (ActiveBoard == null || ScreenManager.Instance.IsActiveLoading()) return;

        // Lấy ra các từ chưa được tìm thấy
        List<string> nonFoundWords = new List<string>();
        foreach (var word in ActiveBoard.words)
        {
            if (!ActiveBoard.foundWords.Contains(word) && !wordListContainer.UnusedWords.Contains(word)) nonFoundWords.Add(word);
        }

        // Đảm bảo danh dách không âm
        if (nonFoundWords.Count == 0) return;
        if (CheckBooterExist(key)) useBooter = true;
        if (dataController.Coins < GameDefine.FIND_WORDS && !useBooter)
            PopupContainer.Instance.ShowNotEnoughCoinsPopup();
        else
        {
            // Pick a random word to show
            string wordToShow = nonFoundWords[UnityEngine.Random.Range(0, nonFoundWords.Count)];

            // Set it as selected
            OnWordSelected(wordToShow);
            // Highlight the word
            characterGrid.ShowWordHint(wordToShow);
            // Deduct the cost
            
            BoosterPay(key, useBooter ? 1 : GameDefine.FIND_WORDS, useBooter);
            AudioManager.Instance.Play("hint-used");
        }
    }
    public void HintHighlightLetter()
    {
        bool useBooter = false;
        string key = "Find-letters";
        List<char> listLetterExist = characterGrid.GetListLetterExist();
        if (ActiveBoard == null || ScreenManager.Instance.IsActiveLoading() || listLetterExist.Count <= 0) return;
        if (CheckBooterExist(key)) useBooter = true;

        if (dataController.Coins < GameDefine.FIND_LETTERS && !useBooter)
        {
            PopupContainer.Instance.ShowNotEnoughCoinsPopup();
        }
        else
        {
            PopupContainer.Instance.ShowHighlighLetterPopup(useBooter, listLetterExist);
        }
    }

    //Nhận chữ từ HighlightLetterPopupClosed
    public void OnChooseHighlightLetterPopupClosed(char letter, bool useBooter)
    {
        string key = "Find-letters";
        ActiveBoard.letterHintsUsed.Add(letter);
        characterGrid.ShowLetterHint(letter);

        BoosterPay(key, useBooter ? 1 : GameDefine.FIND_LETTERS, useBooter);
        PopupContainer.Instance.ClosePopup("ChooseHighlighLetterPopup");
        SetBoardInProgress();

    }

    public void SuggestManyWords()
    {
        bool useBooter = false;
        string key = "Suggest-many-words";
        float timeMoveRocket = 1f;
        // Lấy ra các từ chưa được tìm thấy
        List<string> nonFoundWords = GetNonFoundWords();

        List<string> nonFoundWordsChoose = new List<string>();

        // Đảm bảo danh dách không âm
        if (nonFoundWords.Count == 0) return;
        if (CheckBooterExist(key)) useBooter = true;
        if (dataController.Coins < GameDefine.SUGGEST_MANY_WORDS && !useBooter) PopupContainer.Instance.ShowNotEnoughCoinsPopup();
        else
        {
            if (nonFoundWords.Count > 3)
            {
                while (nonFoundWordsChoose.Count < 3)
                {
                    int index = UnityEngine.Random.Range(0, nonFoundWords.Count);
                    nonFoundWordsChoose.Add(nonFoundWords[index]);
                    nonFoundWords.RemoveAt(index);
                }
            }
            else nonFoundWordsChoose = nonFoundWords;
            // ActiveBoard.recommendWords.Concat(nonFoundWordsChoose).ToList();
            ActiveBoard.recommendWords.UnionWith(nonFoundWordsChoose);
            SetBoardInProgress();

            characterGrid.SuggestManyWords(timeMoveRocket, nonFoundWordsChoose);


            BoosterPay(key, useBooter ? 1 : GameDefine.SUGGEST_MANY_WORDS, useBooter);
            ActionButtonRecommendWord();

            effectContronler.PlayRocket(timeMoveRocket);
        }

    }

    public void ClearWords()
    {
        bool useBooter = false;
        string key = "Clear-words";
        if (CheckBooterExist(key)) useBooter = true;
        if (dataController.Coins < GameDefine.CLEAR_WORDS && !useBooter) PopupContainer.Instance.ShowNotEnoughCoinsPopup();
        else
        {
            bool isClear = characterGrid.ClearWords();
            if (isClear)
            {
                BoosterPay(key, useBooter ? 1 : GameDefine.CLEAR_WORDS, useBooter);
                SetBoardInProgress();
            }
        }
    }

    public void RecommendWord()
    {
        bool useBooter = false;
        string key = "Recommend-word";

        // Lấy ra các từ chưa được tìm thấy
        List<string> nonFoundWords = GetNonFoundWords();
        // Đảm bảo danh dách không âm
        if (nonFoundWords.Count == 0) return;
        if (CheckBooterExist(key)) useBooter = true;
        if (dataController.Coins < GameDefine.RECOMMEND_WORD && !useBooter) PopupContainer.Instance.ShowNotEnoughCoinsPopup();
        else
        {
            // Pick a random word to show
            string wordToShow = nonFoundWords[UnityEngine.Random.Range(0, nonFoundWords.Count)];

            ActiveBoard.recommendWords.Add(wordToShow);
            // Highlight the word
            characterGrid.ShowWordRecommend(wordToShow);
            // Debug.Log("recommendWords Count: " + ActiveBoard.recommendWords.Count);

            SetBoardInProgress();

            // Set it as selected
            BoosterPay(key, useBooter ? 1 : GameDefine.RECOMMEND_WORD, useBooter);

            AudioManager.Instance.Play("hint-used");

            ActionButtonRecommendWord();
        }
    }

    private void BoosterPay(string key, int amount, bool status)
    {
        if (status)
        {
            ListBoosterInGame[key] -= amount;
            UpdateBooterInGame(key);
            dataController.SetListBooster(key, -amount);
        }
        else
        {
            dataController.SetCoins(-amount);
        }

        dataToday.UpdateListBoosterUseToday(key, 1);
    }

    public void RotatingScreen()
    {
        characterGrid.Rotating();
    }

    private void SubtractionBooster(string key, int amount)
    {
        ListBoosterInGame[key] -= amount;
        UpdateBooterInGame(key);
        dataController.SetListBooster(key, -amount);

    }

    public bool CheckBooterExist(string key)
    {
        if (ListBoosterInGame.ContainsKey(key) && ListBoosterInGame[key] > 0) return true;
        return false;
    }

    public void SetLocationUnusedInBoard(Position position)
    {
        ActiveBoard.locationUnuseds.Add(position);
    }

    public Vector3 GetPositionWord(string word)
    {
        return wordListContainer.GetPositionWord(word);
    }
}
