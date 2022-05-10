using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScreenManager : SingletonComponent<ScreenManager>
{
    [SerializeField] private MainScreen mainScreen = null;
    [SerializeField] private GameScreen gameScreen = null;
    [SerializeField] private LevelScreen levelScreen = null;
    [SerializeField] private TopBar topBar = null;
    private List<string> backStack;
    private GameObject currentScreen;

    public void Initialize()
    {
        // MainScreen _mainScreenScript = mainScreen.GetComponent<MainScreen>();
        mainScreen.Initialize();
    }
    public void ShowScreenMain()
    {
        AddBackStack("main");
        if (currentScreen) SetVisibility(currentScreen, false);
        SetVisibility(mainScreen.gameObject, true);
        currentScreen = mainScreen.gameObject;
    }
    public void ShowScreenGame()
    {
        AddBackStack("game");
        if (currentScreen) SetVisibility(currentScreen, false);
        SetVisibility(gameScreen.gameObject, true);
        currentScreen = gameScreen.gameObject;

        // GameScreen _gameScript = gameScreen.GetComponent<GameScreen>();
        gameScreen.Initialize();

    }

    public void ShowScreenLevel()
    {
        AddBackStack("levels");
        if (currentScreen) SetVisibility(currentScreen, false);
        SetVisibility(levelScreen.gameObject, true);
        currentScreen = levelScreen.gameObject;

        // LevelScreen _levelScript = levelScreen.GetComponent<LevelScreen>();
        levelScreen.Initialize();
    }

    private void AddBackStack(string nameScreen)
    {
        if (backStack.Count == 0) backStack.Add(nameScreen);
        if (nameScreen != backStack[backStack.Count - 1])
        {
            backStack.Add(nameScreen);
        }
    }
    private void SetVisibility(GameObject screen, bool isVisible)
    {
        CanvasGroup screenCG = screen.GetComponent<CanvasGroup>();
        screenCG.alpha = isVisible ? 1f : 0f;
        screenCG.interactable = isVisible ? true : false;
        screenCG.blocksRaycasts = isVisible ? true : false;
        if (backStack.Count > 1) topBar.SetAlphaBackButton(true);
        else topBar.SetAlphaBackButton(false);
        if (isVisible)
        {
            topBar.OnSwitchingScreens(backStack[backStack.Count - 1]);
        }
    }
    void Start()
    {
        backStack = new List<string>();
        ShowScreenMain();
    }
    public void BackToHome()
    {
        backStack.Clear();
        ShowScreenMain();
        // mainScreen.ReloadData();
    }

    public void RefreshMainScreen(){
        // mainScreen.ReloadData();
    }




    // void HideCurrentScreen()
    // {
    //     if (currentScreen) currentScreen.SetActive(false);
    // }
    // GameObject GetScreenById(string id)
    // {
    //     GameObject result = null;
    //     switch (id)
    //     {
    //         case "main":
    //             result = mainScreen;
    //             break;
    //         case "levels":
    //             result = levelScreen;
    //             break;
    //         case "game":
    //             result = gameScreen;
    //             break;
    //     }
    //     return result;
    // }


    public void BackScreen()
    {
        if (backStack.Count <= 0)
        {
            Debug.LogWarning("[ScreenController] There is no screen on the back stack to go back to.");

            return;
        }

        string screenId = backStack[backStack.Count - 2];
        backStack.RemoveAt(backStack.Count - 1);
        if (currentScreen) Hide(currentScreen);
        switch (screenId)
        {
            case "main":
                currentScreen = mainScreen.gameObject;
                SetVisibility(mainScreen.gameObject, true);
                MainScreen _mainScreenScript = mainScreen.GetComponent<MainScreen>();
                // _mainScreenScript.ReloadData();
                break;
            case "levels":
                currentScreen = levelScreen.gameObject;
                SetVisibility(levelScreen.gameObject, true);

                LevelScreen _levelScript = levelScreen.GetComponent<LevelScreen>();
                _levelScript.ReloadData();
                break;
            case "game":
                currentScreen = gameScreen.gameObject;
                SetVisibility(gameScreen.gameObject, true);
                break;
        }

    }
    private void Hide(GameObject screen)
    {
        CanvasGroup screenCG = screen.GetComponent<CanvasGroup>();
        screenCG.alpha = 0f;
        screenCG.interactable = false;
        screenCG.blocksRaycasts = false;
    }
    // private void Show()
    // {

    // }
}
