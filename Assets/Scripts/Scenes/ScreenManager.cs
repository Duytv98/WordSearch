using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance;

    [SerializeField] private GameObject flashCanvas = null;
    [SerializeField] private GameObject loadingIndicator = null;
    [SerializeField] private HomeScreen homeScreen = null;
    [SerializeField] private GameScreen gameScreen = null;
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
    }
    public void Close(GameObject screen)
    {
        screen.SetActive(false);
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

    public void Show(string id)
    {
        if (currentScreen) Close(currentScreen);
        AddBackStack(id);
        switch (id)
        {
            case "home":
                currentScreen = homeScreen.gameObject;
                homeScreen.gameObject.SetActive(true);
                homeScreen.Initialize();
                break;
            case "game":
                currentScreen = gameScreen.gameObject;
                gameScreen.gameObject.SetActive(true);
                // gameScreen.Initialize();
                break;
            default:
                return;
        }
        topBar.OnSwitchingScreens(id);
    }
    public void BackScreen()
    {
        if (backStack.Count <= 0)
        {
            Debug.LogWarning("[ScreenController] There is no screen on the back stack to go back to.");

            return;
        }

        string screenId = backStack[backStack.Count - 2];
        if (backStack[backStack.Count - 1] == "game") DataController.Instance.SaveBoardInProgress();
        backStack.RemoveAt(backStack.Count - 1);
        if (currentScreen) Close(currentScreen);

        switch (screenId)
        {
            case "home":
                backStack.Clear();
                currentScreen = homeScreen.gameObject;
                homeScreen.gameObject.SetActive(true);
                homeScreen.Initialize();
                AddBackStack("home");
                break;
            case "game":
                currentScreen = gameScreen.gameObject;
                gameScreen.gameObject.SetActive(true);
                AddBackStack("game");
                break;
        }

        topBar.OnSwitchingScreens(screenId);
    }


    public void SetActiveFlashCanvas(bool isActive)
    {
        flashCanvas.SetActive(isActive);
    }
    public void ActiveLoading()
    {
        loadingIndicator.SetActive(true);
    }
    public void DeactivateLoading()
    {
        loadingIndicator.SetActive(false);
    }
    public bool IsActiveLoading()
    {
        return loadingIndicator.activeSelf;
    }

}
