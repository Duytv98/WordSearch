using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance;
    [SerializeField] private HomeScreen homeScreen = null;
    [SerializeField] private GameScreen gameScreen = null;
    [SerializeField] private LevelScreen levelScreen = null;
    [SerializeField] private DailyGift dailyGift = null;
    [SerializeField] private DailyPuzzle dailyPuzzle = null;
    [SerializeField] private GameObject flashCanvas = null;
    [SerializeField] private TopBar topBar = null;
    private List<string> backStack;
    private GameObject currentScreen;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    void Start()
    {
        backStack = new List<string>();
        Show("home");
    }
    public void Show(string id, bool isVisible = true)
    {
        if (currentScreen) Close(currentScreen);
        GameObject screen = null;
        AddBackStack(id);
        switch (id)
        {
            case "home":
                screen = homeScreen.gameObject;
                currentScreen = screen;
                screen.SetActive(true);
                homeScreen.Initialize();
                break;
            case "levels":
                screen = levelScreen.gameObject;
                currentScreen = screen;
                SetVisibilityLevle(screen, true);
                levelScreen.Initialize();
                break;
            case "game":
                screen = gameScreen.gameObject;
                currentScreen = screen;
                screen.SetActive(true);
                gameScreen.Initialize();
                break;
            case "dailyGift":
                screen = dailyGift.gameObject;
                currentScreen = screen;
                screen.SetActive(true);
                dailyGift.Initialize();
                break;
            case "dailyPuzzle":
                screen = dailyPuzzle.gameObject;
                currentScreen = screen;
                screen.SetActive(true);
                dailyPuzzle.Initialize();
                break;
            default:
                return;
        }
        topBar.OnSwitchingScreens(id);
    }

    public void Close(GameObject screen)
    {
        if (screen.name == "CanvasLevels")
        {
            SetVisibilityLevle(screen, false);
        }
        else screen.SetActive(false);
    }

    private void AddBackStack(string nameScreen)
    {
        if (backStack.Count == 0) backStack.Add(nameScreen);
        if (nameScreen != backStack[backStack.Count - 1])
        {
            backStack.Add(nameScreen);
        }
    }
    public void BackToHome()
    {
        backStack.Clear();
        Show("home");
    }
    public void RefreshLevelScreen()
    {
        levelScreen.ReloadData();
    }
    public void BackScreen()
    {
        if (backStack.Count <= 0)
        {
            Debug.LogWarning("[ScreenController] There is no screen on the back stack to go back to.");

            return;
        }

        string screenId = backStack[backStack.Count - 2];
        backStack.RemoveAt(backStack.Count - 1);
        if (currentScreen) Close(currentScreen);

        switch (screenId)
        {
            case "home":
                backStack.Clear();
                currentScreen = homeScreen.gameObject;
                homeScreen.gameObject.SetActive(true);
                AddBackStack("home");
                break;
            case "levels":
                currentScreen = levelScreen.gameObject;
                SetVisibilityLevle(levelScreen.gameObject, true);
                levelScreen.ReloadData();
                AddBackStack("levels");
                break;
            case "game":
                currentScreen = gameScreen.gameObject;
                gameScreen.gameObject.SetActive(true);
                AddBackStack("game");
                SaveableManager.Instance.SaveData();
                /////////////////////////////
                break;
        }

        topBar.OnSwitchingScreens(screenId);
    }

    private void SetVisibilityLevle(GameObject screen, bool isVisible)
    {
        CanvasGroup screenCG = screen.GetComponent<CanvasGroup>();

        screenCG.alpha = isVisible ? 1f : 0f;
        screenCG.interactable = isVisible ? true : false;
        screenCG.blocksRaycasts = isVisible ? true : false;
    }
    public void SetActiveFlashCanvas(bool isActive)
    {
        flashCanvas.SetActive(isActive);
    }

}
