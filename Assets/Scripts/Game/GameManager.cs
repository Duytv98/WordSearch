using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SimpleJSON;
public class GameManager : MonoBehaviour
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
    public static GameManager Instance;
    [Header("Data")]
    [SerializeField] private List<CategoryInfo> categoryInfos = null;

    [SerializeField] private List<DifficultyInfo> difficultyInfos = null;

    [Header("Values")]
    [SerializeField] private int numLevelsToAwardCoins = 0;
    [SerializeField] private int coinsToAward = 0;

    [Header("Components")]
    [SerializeField] private CharacterGrid characterGrid = null;
    [SerializeField] private WordListContainer wordListContainer = null;
    [SerializeField] private GameObject loadingIndicator = null;
    [SerializeField] private ScreenManager screenManager = null;

    [SerializeField] private Effect effectContronler = null;
    public int Coins { get; set; }
    public int Keys { get; set; }

    public List<CategoryInfo> CategoryInfos { get { return categoryInfos; } }
    public CategoryInfo ActiveCategoryInfo { get; set; }
    public int ActiveDifficultyIndex { get; private set; }
    public Board ActiveBoard { get; private set; }
    public int ActiveLevelIndex { get; private set; }
    public GameMode ActiveGameMode { get; private set; }
    public GameState ActiveGameState { get; private set; }

    public Dictionary<string, string> BoardsInProgress { get; private set; }
    public Dictionary<string, int> LastCompletedLevels = null;
    public Dictionary<string, int> ListBooter = null;
    public Dictionary<string, int> ListBooterInGame { get; private set; }
    public List<string> UnlockedCategories { get; private set; }

    private PlayerInfo playerInfo = null;


    [Header("Debug / Testing")]
    [SerializeField] private bool awardKeyEveryLevel = false;
    [SerializeField] private bool awardCoinsEveryLevel = false;


    private bool isLogIn = false;
    private string idPlayer;
    private bool isCompleted;
    private bool isMusic = true;
    private bool isSound = true;
    public bool IsLogIn { get => isLogIn; set => isLogIn = value; }
    public string IdPlayer { get => idPlayer; set => idPlayer = value; }
    public bool IsCompleted { get => isCompleted; set => isCompleted = value; }
    public bool IsMusic { get => isMusic; set => isMusic = value; }
    public bool IsSound { get => isSound; set => isSound = value; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        Application.targetFrameRate = 60;

        playerInfo = new PlayerInfo();
        BoardsInProgress = new Dictionary<string, string>();
        LastCompletedLevels = new Dictionary<string, int>();
        ListBooter = new Dictionary<string, int>();
        UnlockedCategories = new List<string>();
        // Debug.Log("ListBooter.Count: " + ListBooter.Count);

        characterGrid.Initialize();
        wordListContainer.Initialize();
    }

    public void Update4variable(string idPlayer, bool isLogIn, bool isMusic, bool isSound)
    {
        IdPlayer = idPlayer;
        IsLogIn = isLogIn;
        IsMusic = isMusic;
        IsSound = isSound;
    }
    // GỌi khi tất cả các data load hoàn tất
    public void ConfigData(PlayerInfo playerInfo)
    {
        Coins = playerInfo.coins;
        Keys = playerInfo.keys;
        LastCompletedLevels = Convert.ToDictionarySI(playerInfo.lastCompletedLevels);
        ListBooter = Convert.ToDictionarySI(playerInfo.listBooter);
        BoardsInProgress = Convert.ToDictionarySS(playerInfo.boardsInProgress);
        UnlockedCategories = Convert.ToListS(playerInfo.unlockedCategories);
        this.playerInfo.DisplayName = playerInfo.DisplayName;
        this.playerInfo.Email = playerInfo.Email;
        //check bật nhạc
        if (IsMusic) AudioManager.Instance.PlayMusic();
        ScreenManager.Instance.SetActiveFlashCanvas(false);
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

        // SetBoardInProgress(board, categoryInfo, levelIndex);
        SaveCurrentBoard();
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
        IsCompleted = false;
        ActiveBoard = board;
        // Debug.Log("Count: " + ActiveBoard.recommendWords.Count);
        SetUpListBooterUse();
        // Debug.Log("ListBooter: " + Utilities.ConvertToJsonString(ListBooter));
        // Debug.Log("ListBooterUse: " + Utilities.ConvertToJsonString(ListBooterInGame));
        if (levelIndex >= 0)
        {
            ScreenManager.Instance.Show("game");
            characterGrid.SetUp(board);
            wordListContainer.Setup(board);
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

        // ScreenManager.Instance.ShowScreenGame();
        ScreenManager.Instance.Show("game");
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
        var str = words.Aggregate("", (current, item) => current + (item + " , "));
        // Debug.Log(str);
        // Create the board settings that will be passed to BoardCreator.CreateBoard
        BoardCreator.BoardConfig boardConfig = new BoardCreator.BoardConfig();
        boardConfig.rows = difficultyInfo.boardRowSize;
        boardConfig.cols = difficultyInfo.boardColumnSize;
        boardConfig.words = words;
        boardConfig.randomCharacters = GameDefine.CHARACTERS;
        // Debug.Log(JsonUtility.ToJson(boardConfig));

        ActiveGameState = GameState.GeneratingBoard;
        // ScreenManager.Instance.ShowScreenGame();

        ScreenManager.Instance.Show("game");
        loadingIndicator.SetActive(true);

        // // Start the creation of the board
        BoardCreator.CreateBoard(boardConfig, OnCasualBoardCreated);
    }
    private void OnCasualBoardCreated(Board board)
    {
        // Debug.Log(32222222);
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
            if (ActiveBoard.foundWords.Contains(word) || wordListContainer.UnusedWords.Contains(word))
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
                // Debug.Log(" GameManaer ActiveBoard.foundWords.Count: " + ActiveBoard.foundWords.Count);
                if (ActiveBoard.recommendWords.Contains(word)) ActiveBoard.recommendWords.Remove(word);
                return word;
            }
        }

        // Debug.Log("OnWordSelected: " + ActiveBoard.letterHintsUsed.Count);

        return null;
    }
    public void CheckBoardCompleted()
    {
        if (ActiveBoard.foundWords.Count == ActiveBoard.words.Count)
        {
            if (!IsCompleted) BoardCompleted();
            Debug.Log("Thắng");
        }
        else
        {
            SaveCurrentBoard();
        }
    }
    private void BoardCompleted()
    {
        IsCompleted = true;
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

        AudioManager.Instance.Play("level-complete");
    }
    public Vector3 GetPositionWord(string word)
    {
        return wordListContainer.GetPositionWord(word);
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
    public int IsLevelPlayable(CategoryInfo categoryInfo)
    {
        // Debug.Log("IsLevelPlayable: " + categoryInfo.displayName);
        if (!LastCompletedLevels.ContainsKey(categoryInfo.saveId)) return 0;
        else return LastCompletedLevels[categoryInfo.saveId];
    }
    // Sử lý list category
    public bool UnlockCategory(CategoryInfo categoryInfo)
    {
        switch (categoryInfo.lockType)
        {
            case CategoryInfo.LockType.Coins:
                if (Coins < categoryInfo.unlockAmount) { }
                else { }

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
                    screenManager.RefreshLevelScreen();
                    PopupContainer.Instance.ClosePopup("UnlockCategoryPopup");
                    SaveableManager.Instance.SaveData();
                    return true;
                }

                break;
        }
        return false;

    }
    public void HintHighlightWord()
    {
        bool useBooter = false;
        string key = "Find-words";
        if (ActiveBoard == null || loadingIndicator.activeSelf)
        {
            return;
        }

        List<string> nonFoundWords = new List<string>();

        // Lấy ra các từ chưa được tìm thấy
        for (int i = 0; i < ActiveBoard.words.Count; i++)
        {
            string word = ActiveBoard.words[i];

            if (!ActiveBoard.foundWords.Contains(word) && !wordListContainer.UnusedWords.Contains(word))
            {
                nonFoundWords.Add(word);
            }
        }

        // Đảm bảo danh dách không âm
        if (nonFoundWords.Count == 0) return;
        if (CheckBooterExist(key)) useBooter = true;

        if (Coins < GameDefine.FIND_WORDS && !useBooter)
            PopupContainer.Instance.ShowNotEnoughCoinsPopup();
        else
        {
            // Pick a random word to show
            string wordToShow = nonFoundWords[Random.Range(0, nonFoundWords.Count)];

            // Set it as selected
            OnWordSelected(wordToShow);
            // Highlight the word
            characterGrid.ShowWordHint(wordToShow);
            // Deduct the cost
            if (useBooter) SubtractionBooter(key, 1);
            else Coins -= GameDefine.FIND_WORDS;


            AudioManager.Instance.Play("hint-used");
        }
    }
    public void HintHighlightLetter()
    {

        bool useBooter = false;
        string key = "Find-letters";
        if (ActiveBoard == null || loadingIndicator.activeSelf)
        {
            return;
        }

        if (CheckBooterExist(key)) useBooter = true;

        if (Coins < GameDefine.FIND_LETTERS && !useBooter)
        {
            PopupContainer.Instance.ShowNotEnoughCoinsPopup();
        }
        else
        {
            PopupContainer.Instance.ShowHighlighLetterPopup(useBooter);
        }
    }

    //Nhận chữ từ HighlightLetterPopupClosed
    public void OnChooseHighlightLetterPopupClosed(char letter, bool isBooterUse)
    {
        string key = "Find-letters";
        ActiveBoard.letterHintsUsed.Add(letter);
        characterGrid.ShowLetterHint(letter);
        if (isBooterUse) SubtractionBooter(key, 1);
        else Coins -= GameDefine.FIND_LETTERS;
        PopupContainer.Instance.ClosePopup("ChooseHighlighLetterPopup");
        SaveCurrentBoard();

    }



    public void SuggestManyWords()
    {
        bool useBooter = false;
        string key = "Suggest-many-words";
        float timeMoveRocket = 1f;
        List<string> nonFoundWords = new List<string>();

        List<string> nonFoundWordsChoose = new List<string>();
        // Lấy ra các từ chưa được tìm thấy
        for (int i = 0; i < ActiveBoard.words.Count; i++)
        {
            string word = ActiveBoard.words[i];

            if (!ActiveBoard.foundWords.Contains(word) && !ActiveBoard.recommendWords.Contains(word))
            {
                nonFoundWords.Add(word);
            }
        }
        // Đảm bảo danh dách không âm
        if (nonFoundWords.Count == 0) return;
        if (CheckBooterExist(key)) useBooter = true;
        if (Coins < GameDefine.SUGGEST_MANY_WORDS && !useBooter) PopupContainer.Instance.ShowNotEnoughCoinsPopup();
        else
        {
            if (nonFoundWords.Count > 3)
            {
                while (nonFoundWordsChoose.Count < 3)
                {
                    int index = Random.Range(0, nonFoundWords.Count);
                    nonFoundWordsChoose.Add(nonFoundWords[index]);
                    nonFoundWords.RemoveAt(index);
                }
            }
            else nonFoundWordsChoose = nonFoundWords;
            // ActiveBoard.recommendWords.Concat(nonFoundWordsChoose).ToList();
            ActiveBoard.recommendWords.UnionWith(nonFoundWordsChoose);
            SaveCurrentBoard();

            characterGrid.SuggestManyWords(timeMoveRocket, nonFoundWordsChoose);

            if (useBooter) SubtractionBooter(key, 1);
            else Coins -= GameDefine.SUGGEST_MANY_WORDS;
        }
        effectContronler.PlayRocket(timeMoveRocket);
    }
    public void ClearWords()
    {
        bool useBooter = false;
        string key = "Clear-words";
        if (CheckBooterExist(key)) useBooter = true;
        if (Coins < GameDefine.CLEAR_WORDS && !useBooter) PopupContainer.Instance.ShowNotEnoughCoinsPopup();
        else
        {
            bool isClear = characterGrid.ClearWords();
            if (isClear)
            {
                if (useBooter) SubtractionBooter(key, 1);
                else Coins -= GameDefine.CLEAR_WORDS;
            }
        }

    }
    public void RecommendWord()
    {
        bool useBooter = false;
        string key = "Recommend-word";

        List<string> nonFoundWords = new List<string>();
        // Lấy ra các từ chưa được tìm thấy
        for (int i = 0; i < ActiveBoard.words.Count; i++)
        {
            string word = ActiveBoard.words[i];

            if (!ActiveBoard.foundWords.Contains(word) && !ActiveBoard.recommendWords.Contains(word))
            {
                nonFoundWords.Add(word);
            }
        }
        // Đảm bảo danh dách không âm
        if (nonFoundWords.Count == 0) return;
        if (CheckBooterExist(key)) useBooter = true;
        if (Coins < GameDefine.RECOMMEND_WORD && !useBooter) PopupContainer.Instance.ShowNotEnoughCoinsPopup();
        else
        {
            // Pick a random word to show
            string wordToShow = nonFoundWords[Random.Range(0, nonFoundWords.Count)];

            ActiveBoard.recommendWords.Add(wordToShow);
            // Highlight the word
            characterGrid.ShowWordRecommend(wordToShow);
            // Debug.Log("recommendWords Count: " + ActiveBoard.recommendWords.Count);

            SaveCurrentBoard();

            // Set it as selected
            if (useBooter) SubtractionBooter(key, 1);
            else Coins -= GameDefine.RECOMMEND_WORD;
            AudioManager.Instance.Play("hint-used");
        }
    }
    public void RotatingScreen()
    {
        characterGrid.Rotating();
    }

    public void SetLocationUnusedInBoard(Position position)
    {
        ActiveBoard.locationUnuseds.Add(position);
    }

    public void WordListContainer_SetWordFound(string word)
    {
        wordListContainer.SetWordFound(word);
    }

    public void wordListContainer_SetWordRecommend(string word, Color color)
    {
        wordListContainer.SetWordRecommend(word, color);
    }

    public void WordListContainer_PlusWord()
    {
        wordListContainer.PlusWord(ActiveBoard.foundWords);
    }

    public PlayerInfo GetPlayerInfo()
    {
        return this.playerInfo;
    }
    public void SetPlayerInfo(string displayName = null, string email = null)
    {
        // Debug.Log("Coins: " + Coins + "       Keys: " + Keys + "     ListBooter: " + Utilities.ConvertToJsonString(ListBooter));
        playerInfo.coins = Coins;
        playerInfo.keys = Keys;
        if (displayName != null && email != null)
        {
            playerInfo.DisplayName = displayName;
            playerInfo.Email = email;
        }

        // playerInfo.keys = 0;
        // LastCompletedLevels["birds"] = 25;
        // playerInfo.activeBoard = ActiveBoard;
        playerInfo.lastCompletedLevels = Utilities.ConvertToJsonString(LastCompletedLevels);
        playerInfo.listBooter = Utilities.ConvertToJsonString(ListBooter);
        playerInfo.boardsInProgress = Utilities.ConvertToJsonString(BoardsInProgress);
        playerInfo.unlockedCategories = string.Join(",", UnlockedCategories);
    }

    public void SaveCurrentBoard()
    {
        SetBoardInProgress(ActiveBoard, ActiveCategoryInfo, ActiveLevelIndex);
    }
    private void SetBoardInProgress(Board board, CategoryInfo categoryInfo, int levelIndex = -1)
    {
        string saveKey = GetSaveKey(categoryInfo, levelIndex);
        string contentsBoard = Utilities.ConvertToJsonString(board.ToJson());
        // Debug.Log("contentsBoard: " + contentsBoard);
        BoardsInProgress[saveKey] = contentsBoard;
    }
    private Board GetSavedBoard(CategoryInfo categoryInfo, int levelIndex = -1)
    {
        string saveKey = GetSaveKey(categoryInfo, levelIndex);

        if (BoardsInProgress.ContainsKey(saveKey))
        {
            Board board = new Board();

            board.StringToJson(BoardsInProgress[saveKey]);
            return board;
        }
        return null;
    }
    private string GetSaveKey(CategoryInfo categoryInfo, int levelIndex = -1)
    {
        return string.Format("{0}_{1}", categoryInfo.saveId, levelIndex);
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

    public void AddWordDeleted(string word)
    {
        ActiveBoard.listWordDeleted.Add(word);
    }

    public void LogHashSetString(HashSet<string> list)
    {
        string a = "LogHashSetString số lượng từ:  " + list.Count + " :   ";
        foreach (string word in list)
        {
            a += ", ";
            a += word;
        }
        // Debug.Log(a);
    }

    public bool SetBooter(string key, int amount)
    {
        if (!ListBooter.ContainsKey(key)) return false;
        ListBooter[key] += amount;
        SaveableManager.Instance.SaveData();
        return true;
    }
    private void SetUpListBooterUse()
    {
        ListBooterInGame = new Dictionary<string, int>();
        foreach (var booter in ListBooter)
        {
            var key = booter.Key;
            var value = 0;
            value = booter.Value < 3 ? booter.Value : 3;
            ListBooterInGame.Add(key, value);
        }
    }
    private bool SubtractionBooter(string key, int amount)
    {
        if (ListBooter.ContainsKey(key) && ListBooterInGame.ContainsKey(key))
        {
            ListBooter[key] -= amount;
            ListBooterInGame[key] -= amount;
            GameScreen.Instance.UpdateBooterInGame(key);
            return true;
        }
        return false;
    }
    private bool CheckBooterExist(string key)
    {
        if (ListBooterInGame[key] > 0) return true;
        return false;
    }
}
