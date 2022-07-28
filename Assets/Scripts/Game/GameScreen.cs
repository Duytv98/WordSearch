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


    [SerializeField] private TopBar topBar = null;
    [SerializeField] private ButtonController buttonController = null;

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

    private bool boosterFree = false;

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

        topBar.Initialize(dataController.ActiveCategoryInfo.icon, dataController.ActiveLevelIndex);
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
        buttonController.SetActiveEventButtonRecommendWord(GetNonFoundWords().Count == 0);
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

        // Debug.Log("BoardCompleted");
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

    // Action Booster
    public void HintHighlightWord()
    {
        if (!buttonController.IsActiveEvent) return;
        string key = "Find-words";
        CheckBooterExist(key);
        if (ActiveBoard == null || ScreenManager.Instance.IsActiveLoading()) return;

        List<string> nonFoundWords = new List<string>();
        foreach (var word in ActiveBoard.words)
        {
            if (!ActiveBoard.foundWords.Contains(word) && !wordListContainer.UnusedWords.Contains(word)) nonFoundWords.Add(word);
        }
        if (nonFoundWords.Count == 0) return;
        if (dataController.Coins < GameDefine.FIND_WORDS && !boosterFree)
            PopupContainer.Instance.ShowNotEnoughCoinsPopup();
        else
        {
            buttonController.SetActiveEventButtonInGame(false);

            string wordToShow = nonFoundWords[UnityEngine.Random.Range(0, nonFoundWords.Count)];
            OnWordSelected(wordToShow);
            characterGrid.ShowWordHint(wordToShow);

            BoosterPay(key, GameDefine.FIND_WORDS);
            AudioManager.Instance.Play("hint-used");
        }
    }
    private int countLetterHighlightExist = 0;
    public void HintHighlightLetter()
    {
        if (!buttonController.IsActiveEvent) return;
        string key = "Find-letters";
        CheckBooterExist(key);
        List<char> listLetterExist = characterGrid.GetListLetterExist();
        countLetterHighlightExist = listLetterExist.Count;
        if (ActiveBoard == null || ScreenManager.Instance.IsActiveLoading() || listLetterExist.Count <= 0) return;

        if (dataController.Coins < GameDefine.FIND_LETTERS && !boosterFree)
        {
            PopupContainer.Instance.ShowNotEnoughCoinsPopup();
        }
        else
        {
            PopupContainer.Instance.ShowHighlighLetterPopup(listLetterExist);
        }
    }

    //Nhan chu tu HighlightLetterPopupClosed
    public void OnChooseHighlightLetterPopupClosed(char letter)
    {
        string key = "Find-letters";
        ActiveBoard.letterHintsUsed.Add(letter);
        characterGrid.ShowLetterHint(letter);

        BoosterPay(key, GameDefine.FIND_LETTERS);
        PopupContainer.Instance.ClosePopup("ChooseHighlighLetterPopup");
        SetBoardInProgress();
        if (countLetterHighlightExist <= 1) buttonController.SetActiveEventButtonHighlightLetter(false);

    }

    public void SuggestManyWords()
    {
        if (!buttonController.IsActiveEvent) return;

        string key = "Suggest-many-words";
        CheckBooterExist(key);
        float timeMoveRocket = 1f;
        // Lấy ra các từ chưa được tìm thấy
        List<string> nonFoundWords = GetNonFoundWords();

        List<string> nonFoundWordsChoose = new List<string>();

        // Đảm bảo danh dách không âm
        if (nonFoundWords.Count == 0) return;
        if (dataController.Coins < GameDefine.SUGGEST_MANY_WORDS && !boosterFree) PopupContainer.Instance.ShowNotEnoughCoinsPopup();
        else
        {
            buttonController.SetActiveEventButtonInGame(false);
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


            BoosterPay(key, GameDefine.SUGGEST_MANY_WORDS);
            ActionButtonRecommendWord();

            effectContronler.PlayRocket(timeMoveRocket);
        }

    }

    public void RecommendWord()
    {
        if (!buttonController.IsActiveEvent) return;
        string key = "Recommend-word";
        CheckBooterExist(key);

        // Lấy ra các từ chưa được tìm thấy
        List<string> nonFoundWords = GetNonFoundWords();
        // Đảm bảo danh dách không âm
        if (nonFoundWords.Count == 0) return;
        if (dataController.Coins < GameDefine.RECOMMEND_WORD && !boosterFree) PopupContainer.Instance.ShowNotEnoughCoinsPopup();
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
            BoosterPay(key, GameDefine.RECOMMEND_WORD);

            AudioManager.Instance.Play("hint-used");

            ActionButtonRecommendWord();
        }
    }

    public void ClearWords()
    {
        if (!buttonController.IsActiveEvent) return;
        string key = "Clear-words";
        CheckBooterExist(key);
        if (dataController.Coins < GameDefine.CLEAR_WORDS && !boosterFree) PopupContainer.Instance.ShowNotEnoughCoinsPopup();
        else
        {
            buttonController.SetActiveEventButtonInGame(false);
            bool isClear = characterGrid.ClearWords();
            if (isClear)
            {
                BoosterPay(key, GameDefine.CLEAR_WORDS);
                SetBoardInProgress();
            }
        }
    }
    public void RotatingScreen()
    {
        if (!buttonController.IsActiveEvent) return;
        buttonController.SetActiveEventButtonInGame(false);
        characterGrid.Rotating();
    }

    private void BoosterPay(string key, int amount)
    {
        if (boosterFree)
        {
            ListBoosterInGame[key] -= 1;
            UpdateBooterInGame(key);
            dataController.SetListBooster(key, -1);
        }
        else
        {
            dataController.SetCoins(-amount);
        }

        dataToday.UpdateListBoosterUseToday(key, 1);
    }


    private void SubtractionBooster(string key, int amount)
    {
        ListBoosterInGame[key] -= amount;
        UpdateBooterInGame(key);
        dataController.SetListBooster(key, -amount);

    }

    public bool CheckBooterExist(string key)
    {
        if (ListBoosterInGame.ContainsKey(key) && ListBoosterInGame[key] > 0)
        {
            boosterFree = true;
            return true;
        }
        boosterFree = false;
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
