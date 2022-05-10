using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
public class GameManager : SingletonComponent<GameManager>
{



    public enum GameMode
    {
        Casual,
        Progress
    }
    public enum GameState
    {
        None,
        GeneratingBoard,
        BoardActive
    }
    [Header("Data")]
    [SerializeField] private string characters = null;
    [SerializeField] private List<CategoryInfo> categoryInfos = null;

    [SerializeField] private List<DifficultyInfo> difficultyInfos = null;

    [Header("Values")]
    [SerializeField] private int startingCoins = 0;
    [SerializeField] private int startingKeys = 0;
    [SerializeField] private int numLevelsToAwardCoins = 0;
    [SerializeField] private int coinsToAward = 0;
    [SerializeField] private int coinCostWordHint = 0;
    [SerializeField] private int coinCostLetterHint = 0;

    [Header("Components")]
    [SerializeField] private CharacterGrid characterGrid = null;
    [SerializeField] private WordListContainer wordListContainer = null;
    [SerializeField] private GameObject loadingIndicator = null;
    [SerializeField] private ScreenManager screenManager = null;

    [SerializeField] private TopBar topBar = null;
    public int Coins { get; set; }
    public int Keys { get; set; }

    public List<CategoryInfo> CategoryInfos { get { return categoryInfos; } }
    public int StartingCoins { get { return startingCoins; } }
    public int StartingKeys { get { return startingKeys; } }
    public int CoinCostWordHint { get { return coinCostWordHint; } }
    public int CoinCostLetterHint { get { return coinCostLetterHint; } }
    public CategoryInfo ActiveCategoryInfo { get; set; }
    public int ActiveDifficultyIndex { get; private set; }
    public Board ActiveBoard { get; private set; }
    public int ActiveLevelIndex { get; private set; }
    public GameMode ActiveGameMode { get; private set; }
    public GameState ActiveGameState { get; private set; }




    public Dictionary<string, string> BoardsInProgress { get; private set; }
    public Dictionary<string, int> LastCompletedLevels = null;
    public List<string> UnlockedCategories { get; private set; }


    private PlayerInfo playerInfo = null;




    [Header("Debug / Testing")]
    [SerializeField] private bool awardKeyEveryLevel = false;
    [SerializeField] private bool awardCoinsEveryLevel = false;

    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;

        playerInfo = new PlayerInfo();
        BoardsInProgress = new Dictionary<string, string>();
        LastCompletedLevels = new Dictionary<string, int>();
        UnlockedCategories = new List<string>();

        characterGrid.Initialize();
        wordListContainer.Initialize();
    }
    void Start()
    {
        SaveableManager.Instance.LoadSaveData();
        screenManager.Initialize();
    }

    public void ConfigData(PlayerInfo playerInfo)
    {
        Coins = playerInfo.coins;
        Keys = playerInfo.keys;
        ActiveBoard = playerInfo.activeBoard;
        LastCompletedLevels = ConvertToDictionaryLastCompletedLevels(playerInfo.lastCompletedLevels);
        BoardsInProgress = ConvertToDictionaryBoardsInProgress(playerInfo.boardsInProgress);
        UnlockedCategories = ConvertToListStringUnlockedCategories(playerInfo.unlockedCategories);
    }


    Board LoadLevelFile(CategoryInfo categoryInfo, int levelIndex)
    {
        TextAsset levelFile = categoryInfo.levelFiles[levelIndex];
        // Debug.Log(levelFile);
        Board board = new Board();
        board.FromJson(levelFile);
        // Debug.Log(board.foundWords);
        return board;
    }


    // Start Progress
    public void StartLevel(CategoryInfo categoryInfo, int levelIndex)
    {
        ActiveLevelIndex = levelIndex;
        ActiveGameMode = GameMode.Progress;
        Board board = GetSavedBoard(categoryInfo, levelIndex);
        // Debug.Log(board);
        if (board == null)
        {
            board = LoadLevelFile(categoryInfo, levelIndex);
        }
        SetupGame(board, levelIndex);

        SetBoardInProgress(board, categoryInfo, levelIndex);
    }
    public void StartNextLevel(CategoryInfo categoryInfo)
    {
        // Debug.Log(JsonUtility.ToJson(categoryInfo));
        int nextLevelIndex = LastCompletedLevels.ContainsKey(categoryInfo.saveId) ? LastCompletedLevels[categoryInfo.saveId] + 1 : 0;
        // Debug.Log("nextLevelIndex: " + nextLevelIndex);
        if (nextLevelIndex >= categoryInfo.levelFiles.Count)
        {
            nextLevelIndex = categoryInfo.levelFiles.Count - 1;
        }

        StartLevel(categoryInfo, nextLevelIndex);
    }
    private void SetupGame(Board board, int levelIndex = -1)
    {
        ActiveBoard = board;
        characterGrid.SetUp(board);
        wordListContainer.Setup(board);

        if (levelIndex >= 0)
        {
            ScreenManager.Instance.ShowScreenGame();
            topBar.SetTextLevel(levelIndex);
            topBar.SetCategoryName(ActiveCategoryInfo.displayName);
        }

        foreach (string foundWord in board.foundWords)
        {
            wordListContainer.SetWordFound(foundWord);
        }
        ActiveGameState = GameState.BoardActive;

        SaveableManager.Instance.SaveData();
    }
    public bool AllLevelsComplete(CategoryInfo categoryInfo)
    {
        return LastCompletedLevels.ContainsKey(categoryInfo.saveId) && LastCompletedLevels[categoryInfo.saveId] >= categoryInfo.levelFiles.Count - 1;
    }

    // Start Casual
    public void StartCasual(CategoryInfo categoryInfo, int difficultyIndex)
    {
        ActiveCategoryInfo = categoryInfo;
        ActiveDifficultyIndex = difficultyIndex;
        ActiveLevelIndex = -1;
        ActiveGameMode = GameMode.Casual;

        // Debug.Log("ActiveDifficultyIndex: " + ActiveDifficultyIndex);

        // Clear the board from any previous game
        characterGrid.Clear();
        wordListContainer.Clear();

        // Generate a new random board to use
        GenerateRandomBoard(difficultyInfos[difficultyIndex]);

        // ShowGameScreen();
    }
    public void ContinueCasual(CategoryInfo categoryInfo)
    {
        Board savedBoard = GetSavedBoard(categoryInfo);

        if (savedBoard == null)
        {
            Debug.LogError("[GameManager] ContinueCasual: There is no saved casual board for category " + categoryInfo.saveId);

            return;
        }

        ActiveCategoryInfo = categoryInfo;
        ActiveDifficultyIndex = savedBoard.difficultyIndex;
        ActiveLevelIndex = -1;
        ActiveGameMode = GameMode.Casual;

        // characterGrid.Clear();
        // wordListContainer.Clear();
        SetupGame(savedBoard);

        ScreenManager.Instance.ShowScreenGame();
        // ShowGameScreen();
    }
    private void GenerateRandomBoard(DifficultyInfo difficultyInfo)
    {
        // Load all the category words
        List<string> categoryWords = LoadWords(ActiveCategoryInfo, difficultyInfo.maxWordLength);
        // string str = "";
        // foreach (var item in categoryWords)
        // {
        //     str += (item + " , ");
        // }
        // Debug.Log(str);
        // Debug.Log("count: " + categoryWords.Count + "  difficultyInfo.maxWordLength: " + difficultyInfo.maxWordLength);

        List<string> words = new List<string>();

        // Randomly choose words to use
        for (int i = 0; i < categoryWords.Count && words.Count < difficultyInfo.maxWords; i++)
        {
            int randomIndex = Random.Range(i, categoryWords.Count);
            string randomWord = categoryWords[randomIndex];

            categoryWords[randomIndex] = categoryWords[i];
            categoryWords[i] = randomWord;

            words.Add(randomWord);
        }
        string str = "";
        foreach (var item in words)
        {
            str += (item + " , ");
        }
        Debug.Log(str);
        // Create the board settings that will be passed to BoardCreator.CreateBoard
        BoardCreator.BoardConfig boardConfig = new BoardCreator.BoardConfig();
        boardConfig.rows = difficultyInfo.boardRowSize;
        boardConfig.cols = difficultyInfo.boardColumnSize;
        boardConfig.words = words;
        boardConfig.randomCharacters = characters;
        Debug.Log(JsonUtility.ToJson(boardConfig));

        ActiveGameState = GameState.GeneratingBoard;
        ScreenManager.Instance.ShowScreenGame();
        loadingIndicator.SetActive(true);

        // // Start the creation of the board
        BoardCreator.CreateBoard(boardConfig, OnCasualBoardCreated);
    }
    private void OnCasualBoardCreated(Board board)
    {
        Debug.Log(32222222);
        board.difficultyIndex = ActiveDifficultyIndex;

        // Debug.Log(Utilities.ConvertToJsonString(board.ToJson()));

        SetupGame(board);

        SetBoardInProgress(board, ActiveCategoryInfo);

        loadingIndicator.SetActive(false);
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
    public bool HasSavedCasualBoard(CategoryInfo categoryInfo)
    {
        return GetSavedBoard(categoryInfo) != null;
    }






    public string OnWordSelected(string selectedWord)
    {

        string selectedWordReversed = "";

        // Đảo ngược chuỗi text
        for (int i = 0; i < selectedWord.Length; i++)
        {
            char character = selectedWord[i];

            selectedWordReversed = character + selectedWordReversed;
        }

        // Check if the selected word equals any of the hidden words
        for (int i = 0; i < ActiveBoard.words.Count; i++)
        {
            string word = ActiveBoard.words[i];

            // Kiểm tra từ đã được tìm thấy chưa
            if (ActiveBoard.foundWords.Contains(word) || wordListContainer.UnusedWord.Contains(word))
            {
                continue;
            }

            // Loại bỏ khoảng trắng
            string wordNoSpaces = word.Replace(" ", "");

            // kiểm tra sự trùng khớp
            if (selectedWord == wordNoSpaces || selectedWordReversed == wordNoSpaces)
            {
                // Thêm vào danh sách từ đã tìm thấy
                ActiveBoard.foundWords.Add(word);
                // Debug.Log("Số từ đã tìm thấy: " +  ActiveBoard.foundWords.Count);

                // Thông báo cho wordListContainer tiến hành đánh dấu word đã được chọn
                wordListContainer.SetWordFound(word);

                wordListContainer.PlusWord(ActiveBoard.foundWords);

                //kiểm tra đã tìm đủ word chưa
                if (ActiveBoard.foundWords.Count == ActiveBoard.words.Count)
                {
                    BoardCompleted();
                    Debug.Log("Thắng");
                }
                else
                {
                    SaveCurrentBoard();
                }

                // Return the word with the spaces
                return word;
            }
        }

        // Debug.Log("OnWordSelected: " + ActiveBoard.letterHintsUsed.Count);

        return null;
    }
    private bool CheckCoincident(string selectedWord, string selectedWordReversed, string wordNoSpaces)
    {
        bool result = true;

        return result;
    }
    private void BoardCompleted()
    {
        // Debug.Log("ActiveCategoryInfo: " + ActiveCategoryInfo + "  ActiveLevelIndex: " + ActiveLevelIndex + "  Key: " + GetSaveKey(ActiveCategoryInfo, ActiveLevelIndex));
        // Debug.Log(Utilities.ConvertToJsonString(BoardsInProgress));
        BoardsInProgress.Remove(GetSaveKey(ActiveCategoryInfo, ActiveLevelIndex));

        // Debug.Log("ActiveCategoryInfo: " + ActiveCategoryInfo + "  ActiveLevelIndex: " + ActiveLevelIndex + "  Key: " + GetSaveKey(ActiveCategoryInfo, ActiveLevelIndex));
        // Debug.Log(Utilities.ConvertToJsonString(BoardsInProgress));
        // Debug.Log(BoardsInProgress.Count);
        if (ActiveGameMode == GameMode.Progress && (!LastCompletedLevels.ContainsKey(ActiveCategoryInfo.saveId) || LastCompletedLevels[ActiveCategoryInfo.saveId] < ActiveLevelIndex))
        {
            LastCompletedLevels[ActiveCategoryInfo.saveId] = ActiveLevelIndex;
        }
        int coinsAwarded = (numLevelsToAwardCoins == 0 || (ActiveLevelIndex + 1) % numLevelsToAwardCoins == 0 || awardCoinsEveryLevel) ? coinsToAward : 0;
        int keysAwarded = (ActiveLevelIndex == ActiveCategoryInfo.levelFiles.Count - 1) ? 1 : 0;

        Coins += coinsAwarded;
        Keys += keysAwarded;


        SaveableManager.Instance.SaveData();
        PopupContainer.Instance.ShowLevelCompletePopup(coinsAwarded, keysAwarded);
    }
    public void HintHighlightWord()
    {

        if (ActiveBoard == null || loadingIndicator.activeSelf)
        {
            return;
        }

        List<string> nonFoundWords = new List<string>();

        // Lấy ra các từ chưa được tìm thấy
        for (int i = 0; i < ActiveBoard.words.Count; i++)
        {
            string word = ActiveBoard.words[i];

            if (!ActiveBoard.foundWords.Contains(word) || !wordListContainer.UnusedWord.Contains(word))
            {
                nonFoundWords.Add(word);
            }
        }

        // Đảm bảo danh dách không âm
        if (nonFoundWords.Count == 0)
        {
            Debug.Log("nonFoundWords.Count = 0 ");
            return;
        }

        // Check lượng coins Player có
        if (Coins < coinCostWordHint)
        {
            // Debug.Log("Coins < coinCostWordHint");
            PopupContainer.Instance.ShowNotEnoughCoinsPopup();
        }
        else
        {
            // Pick a random word to show
            string wordToShow = nonFoundWords[Random.Range(0, nonFoundWords.Count)];

            // Set it as selected
            OnWordSelected(wordToShow);

            var position = wordListContainer.GetPositionWord(wordToShow);

            // Highlight the word
            characterGrid.ShowWordHint(wordToShow);


            // Deduct the cost
            Coins -= coinCostWordHint;

            // SoundManager.Instance.Play("hint-used");
        }
    }
    public void HintHighlightLetter()
    {
        if (ActiveBoard == null || loadingIndicator.activeSelf)
        {
            return;
        }
        if (Coins < coinCostLetterHint)
        {
            PopupContainer.Instance.ShowNotEnoughCoinsPopup();
        }
        else
        {
            PopupContainer.Instance.ShowHighlighLetterPopup();
        }
    }

    //Nhận chữ từ HighlightLetterPopupClosed
    public void OnChooseHighlightLetterPopupClosed(char letter)
    {
        ActiveBoard.letterHintsUsed.Add(letter);
        characterGrid.ShowLetterHint(letter);
        Coins -= coinCostLetterHint;
        PopupContainer.Instance.ClosePopup();
        SaveCurrentBoard();

    }

    //trả về true nếu level completed
    public bool IsLevelCompleted(CategoryInfo categoryInfo, int levelIndex)
    {
        return LastCompletedLevels.ContainsKey(categoryInfo.saveId) && levelIndex <= LastCompletedLevels[categoryInfo.saveId];
    }

    // trả về true nếu level đang bị khóa
    public bool IsLevelLocked(CategoryInfo categoryInfo, int levelIndex)
    {
        return levelIndex > 0 && (!LastCompletedLevels.ContainsKey(categoryInfo.saveId) || levelIndex > LastCompletedLevels[categoryInfo.saveId] + 1);
    }


    // Sử lý list category

    public bool UnlockCategory(CategoryInfo categoryInfo)
    {
        switch (categoryInfo.lockType)
        {
            case CategoryInfo.LockType.Coins:
                if (Coins < categoryInfo.unlockAmount)
                {
                }
                else
                {
                }

                break;
            case CategoryInfo.LockType.Keys:
                if (Keys < categoryInfo.unlockAmount)
                {
                    PopupContainer.Instance.ShowNotEnoughKeysPopup();
                }
                else
                {
                    Keys -= categoryInfo.unlockAmount;

                    UnlockedCategories.Add(categoryInfo.saveId);
                    screenManager.RefreshMainScreen();
                    PopupContainer.Instance.ClosePopup();
                    SaveableManager.Instance.SaveData();
                    return true;
                }

                break;
        }
        return false;

    }












    public PlayerInfo GetPlayerInfo()
    {
        return this.playerInfo;
    }
    public void SetPlayerInfo()
    {
        playerInfo.coins = Coins;
        playerInfo.keys = Keys;
        playerInfo.activeBoard = ActiveBoard;
        playerInfo.lastCompletedLevels = Utilities.ConvertToJsonString(LastCompletedLevels);
        playerInfo.boardsInProgress = Utilities.ConvertToJsonString(BoardsInProgress);
        playerInfo.unlockedCategories = string.Join(",", UnlockedCategories);
    }
    public void SaveCurrentBoard()
    {
        SetBoardInProgress(ActiveBoard, ActiveCategoryInfo, ActiveLevelIndex);
        SaveableManager.Instance.SaveData();
    }
    private void SetBoardInProgress(Board board, CategoryInfo categoryInfo, int levelIndex = -1)
    {
        string saveKey = GetSaveKey(categoryInfo, levelIndex);
        string contentsBoard = Utilities.ConvertToJsonString(board.ToJson());
        BoardsInProgress[saveKey] = contentsBoard;
    }
    private Board GetSavedBoard(CategoryInfo categoryInfo, int levelIndex = -1)
    {
        string saveKey = GetSaveKey(categoryInfo, levelIndex);

        // Debug.Log("Get: " + BoardsInProgress.ContainsKey(saveKey));
        if (BoardsInProgress.ContainsKey(saveKey))
        {
            Board board = new Board();

            board.StringToJson(BoardsInProgress[saveKey]);
            // Debug.Log(board.foundWords.Count);
            return board;
        }
        return null;
    }
    private string GetSaveKey(CategoryInfo categoryInfo, int levelIndex = -1)
    {
        return string.Format("{0}_{1}", categoryInfo.saveId, levelIndex);
    }
    private Dictionary<string, int> ConvertToDictionaryLastCompletedLevels(string contents)
    {
        Dictionary<string, int> dictionary = new Dictionary<string, int>();
        JSONNode json = JSON.Parse(contents);
        foreach (var key in json.Keys)
        {
            // Debug.Log("key: " + key + "  value: " + json[key]);
            dictionary.Add(key, json[key]);
        }
        return dictionary;
    }
    private Dictionary<string, string> ConvertToDictionaryBoardsInProgress(string contents)
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        JSONNode json = JSON.Parse(contents);
        foreach (var key in json.Keys)
        {
            // Debug.Log("key: " + key + "  value: " + json[key]);
            dictionary.Add(key, json[key]);
        }
        return dictionary;
    }

    private List<string> ConvertToListStringUnlockedCategories(string contents)
    {
        string[] lines = contents.Split(',');
        List<string> someList = new List<string>(lines);
        return someList;
    }
    public void ActiveLoading()
    {
        loadingIndicator.SetActive(true);
    }
    public void DeactivateLoading()
    {
        loadingIndicator.SetActive(false);
    }

    public bool IsCategoryLocked(CategoryInfo categoryInfo)
    {
        if (categoryInfo.lockType == CategoryInfo.LockType.None || UnlockedCategories.Contains(categoryInfo.saveId))
        {
            return false;
        }
        return true;
    }

}
