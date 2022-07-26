using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Google;
using TMPro;
using UnityEngine.Networking;

public class FireBaseController : MonoBehaviour
{
    public static FireBaseController Instance;
    [SerializeField] private FacebookAuth facebookAuth = null;
    [SerializeField] private GoogleAuth googleAuth = null;
    [SerializeField] private RealtimeDatabase realtimeDatabase = null;

    [SerializeField] private LoginPopup loginPopup = null;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        Input.multiTouchEnabled = false;
        ScreenManager.Instance.SetActiveFlashCanvas(true);

        StartCoroutine(checkInternetConnection((isConnected) =>
              {
                  if (isConnected) CheckFirebaseDependencies();
                  else SaveableManager.Instance.LoadDataOffline();
              }));

        // Debug.Log("isPlay: " + SaveableManager.Instance.IsActiveGame());
        // Debug.Log("IsLogIn: " + SaveableManager.Instance.IsLogIn());
        if (!SaveableManager.Instance.IsActiveGame() || !SaveableManager.Instance.IsLogIn()) SaveableManager.Instance.LoadDataOffline();

    }

    private void CheckFirebaseDependencies()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                if (task.Result == DependencyStatus.Available)
                {
                    facebookAuth.SetUp();
                    googleAuth.SetUp();
                    realtimeDatabase.SetUp();

                    if (!SaveableManager.Instance.IsActiveGame() || !SaveableManager.Instance.IsLogIn()) return;

                    realtimeDatabase.Read_Data();
                    // SaveableManager.Instance.LoadDataOnline();
                }
                else Debug.Log("Could not resolve all Firebase dependencies: " + task.Result.ToString());
            }
            else Debug.Log("Dependency check was not completed. Error : " + task.Exception.Message);
        });
    }





    IEnumerator checkInternetConnection(Action<bool> action)
    {
        UnityWebRequest request = new UnityWebRequest("http://google.com");
        yield return request.SendWebRequest();
        if (request.error != null) action(false);
        else action(true);
    }


    public void Facebook_Login()
    {
        // Debug.Log("Facebook_Login");
        if (!SaveableManager.Instance.IsLogIn()) facebookAuth.Login();
    }
    public void Google_Login()
    {
        // Debug.Log("Google_Login");
        if (!SaveableManager.Instance.IsLogIn()) googleAuth.Login();
    }
    public void LogOut()
    {
        SaveData();
        var providers = SaveableManager.Instance.GetProvidersLogin();
        if (providers.Equals(GameDefine.KEY_PROVIDERS_FB)) facebookAuth.LogOut();
        else if (providers.Equals(GameDefine.KEY_PROVIDERS_GG)) googleAuth.LogOut();

        SaveableManager.Instance.SetLogIn(false);
        UpdatePopupProfile();
    }

    public void UpdatePopupProfile()
    {
        loginPopup.OnShowing();
    }




    public void Read_Data()
    {
        realtimeDatabase.Read_Data();
    }

    public void SaveCoins()
    {
        // if (SaveableManager.Instance.IsLogIn()) realtimeDatabase.SaveCoins();
    }
    public void SaveKeys()
    {
        // if (SaveableManager.Instance.IsLogIn()) realtimeDatabase.SaveKeys();
    }
    public void SaveLastCompletedLevels()
    {
        // if (SaveableManager.Instance.IsLogIn()) realtimeDatabase.SaveLastCompletedLevels();
    }
    public void SaveUnlockedCategories()
    {
        // if (SaveableManager.Instance.IsLogIn()) realtimeDatabase.SaveUnlockedCategories();
    }
    public void SaveListBooster()
    {
        // if (SaveableManager.Instance.IsLogIn()) realtimeDatabase.SaveListBooster();
    }
    public void SaveTimeCompleteLevel()
    {
        // if (SaveableManager.Instance.IsLogIn()) realtimeDatabase.SaveTimeCompleteLevel();
    }
    public void SaveData()
    {
        if (SaveableManager.Instance.IsLogIn()) realtimeDatabase.SaveData();
    }


    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) SaveData();
    }

}