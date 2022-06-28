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
    [SerializeField] private Sprite[] arrSpire;
    private Dictionary<char, Sprite> dicWord = null;
    public List<CategoryInfo> CategoryInfos { get { return categoryInfos; } }
    public Dictionary<char, Sprite> DicWord { get => dicWord; set => dicWord = value; }

    [Header("Values")]
    [SerializeField] private int numLevelsToAwardCoins = 0;
    [SerializeField] private int coinsToAward = 0;

    [Header("Components")]
    [SerializeField] private CharacterGrid characterGrid = null;
    [SerializeField] private WordListContainer wordListContainer = null;
    [SerializeField] private GameObject loadingIndicator = null;
    // [SerializeField] private ScreenManager screenManager = null;
    [SerializeField] private Effect effectContronler = null;
    public int Coins { get; set; }
    public int Keys { get; set; }

    public CategoryInfo ActiveCategoryInfo { get; set; }
    public int ActiveDifficultyIndex { get; private set; }
    public Board ActiveBoard { get; private set; }
    public int ActiveLevelIndex { get; private set; }

    private Board casualBoard = null;
    public Board CasualBoard { get => casualBoard; set => casualBoard = value; }

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
    private void Start()
    {
        DicWord = new Dictionary<char, Sprite>();

        for (int i = 0; i < GameDefine.CHARACTERS.Length; i++)
        {
            DicWord.Add(GameDefine.CHARACTERS[i], arrSpire[i]);
        }
    }

    public void GetAllDataUser()
    {
        
        Coins = SaveableManager.Instance.GetCoins();
        Keys = SaveableManager.Instance.GetKeys();

        LastCompletedLevels = SaveableManager.Instance.GetLastCompletedLevels();
        ListBooter = SaveableManager.Instance.GetListBooster();
        BoardsInProgress = SaveableManager.Instance.GetBoardsInProgress();
        UnlockedCategories = SaveableManager.Instance.GetUnlockedCategories();
        
        ScreenManager.Instance.SetActiveFlashCanvas(false);

    }
    public void GetDataBackground()
    {
        IsLogIn = SaveableManager.Instance.IsLogIn();
        IsMusic = SaveableManager.Instance.IsMusic();
        IsSound = SaveableManager.Instance.IsSound();

        
        if (IsMusic) AudioManager.Instance.PlayMusic();
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
        // Debug.Log("ConfigData");
        Coins = playerInfo.coins;
        Keys = playerInfo.keys;
        LastCompletedLevels = Convert.ToDictionarySI(playerInfo.lastCompletedLevels);
        ListBooter = Convert.ToDictionarySI(playerInfo.listBooter);
        // BoardsInProgress = Convert.ToDictionarySS(playerInfo.boardsInProgress);
        UnlockedCategories = Convert.ToListS(playerInfo.unlockedCategories);
        this.playerInfo.displayName = playerInfo.displayName;
        // this.playerInfo.Email = playerInfo.Email;
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
        SetupGame(board);

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
    // private void SetupGame(Board board, int levelIndex = -1)
    private void SetupGame(Board board)
    {
        IsCompleted = false;
        ActiveBoard = board;
        // Debug.Log("Count: " + ActiveBoard.recommendWords.Count);
        SetUpListBooterUse();
        // Debug.Log("ListBooter: " + Utilities.ConvertToJsonString(ListBooter));
        // Debug.Log("ListBooterUse: " + Utilities.ConvertToJsonString(ListBooterInGame));
        // if (levelIndex >= 0)
        // {
        //     ScreenManager.Instance.Show("game");
        //     characterGrid.SetUp(board);
        //     wordListContainer.Setup(board);
        // }

        if (ActiveGameMode == GameMode.Progress) ScreenManager.Instance.Show("game");
        else ScreenManager.Instance.InitializeGameScreen();
        characterGrid.SetUp(board);
        wordListContainer.Setup(board);


        ActiveGameState = GameState.BoardActive;

        SaveableManager.Instance.SaveData();
    }
    public bool AllLevelsComplete(CategoryInfo categoryInfo)
    {
        return LastCompletedLevels.ContainsKey(categoryInfo.saveId) && LastCompletedLevels[categoryInfo.saveId] >= categoryInfo.levelFiles.Count - 1;
    }

    public void StartCasual()
    {

        ActiveGameMode = GameMode.Casual;
        SetupGame(CasualBoard);
    }

    // public void ContinueCasual(CategoryInfo categoryInfo)
    // {
    //     Board savedBoard = GetSavedBoard(categoryInfo);

    //     if (savedBoard == null)
    //     {
    //         Debug.LogError("[GameManager] ContinueCasual: There is no saved casual board for category " + categoryInfo.saveId);

    //         return;
    //     }

    //     ActiveCategoryInfo = categoryInfo;
    //     ActiveDifficultyIndex = savedBoard.difficultyIndex;
    //     ActiveLevelIndex = -1;
    //     ActiveGameMode = GameMode.Casual;

    //     // characterGrid.Clear();
    //     // wordListContainer.Clear();
    //     SetupGame(savedBoard);

    //     // ScreenManager.Instance.ShowScreenGame();
    //     ScreenManager.Instance.Show("game");
    //     // ShowGameScreen();
    // }
    // public bool HasSavedCasualBoard(CategoryInfo categoryInfo)
    // {
    //     return GetSavedBoard(categoryInfo) != null;
    // }

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
        if (ActiveGameMode == GameMode.Progress) BoardsInProgress.Remove(GetSaveKey(ActiveCategoryInfo, ActiveLevelIndex));
        else ScreenManager.Instance.CompleteLevelCasual();

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
                    ScreenManager.Instance.RefreshLevelScreen();
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
        if (ActiveBoard == null || ScreenManager.Instance.IsActiveLoading())
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
        if (ActiveBoard == null || ScreenManager.Instance.IsActiveLoading())
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

    public void wordListContainer_SetWordRecommend(string word, int indexColor)
    {
        wordListContainer.SetWordRecommend(word, indexColor);
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
            playerInfo.displayName = displayName;
            // playerInfo.Email = email;
        }

        // playerInfo.keys = 0;
        // LastCompletedLevels["birds"] = 25;
        // playerInfo.activeBoard = ActiveBoard;
        playerInfo.lastCompletedLevels = Utilities.ConvertToJsonString(LastCompletedLevels);
        playerInfo.listBooter = Utilities.ConvertToJsonString(ListBooter);
        // playerInfo.boardsInProgress = Utilities.ConvertToJsonString(BoardsInProgress);
        playerInfo.unlockedCategories = string.Join(",", UnlockedCategories);
    }

    public void SaveCurrentBoard()
    {
        if (ActiveGameMode == GameMode.Progress) SetBoardInProgress(ActiveBoard, ActiveCategoryInfo, ActiveLevelIndex);
        else ScreenManager.Instance.SaveProgressCasual();
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
    // public void ActiveLoading()
    // {
    //     loadingIndicator.SetActive(true);
    // }
    // public void DeactivateLoading()
    // {
    //     loadingIndicator.SetActive(false);
    // }

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
