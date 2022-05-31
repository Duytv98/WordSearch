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
    public string webClientId = "<your client id here>";
    private GoogleSignInConfiguration configuration;
    private FirebaseAuth auth;
    private FirebaseUser user;

    private bool setUpFirebaseAuthSuccess = false;


    private DatabaseReference reference;


    public bool SetUpFirebaseAuthSuccess { get => setUpFirebaseAuthSuccess; set => setUpFirebaseAuthSuccess = value; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        configuration = new GoogleSignInConfiguration { WebClientId = webClientId, RequestEmail = true, RequestIdToken = true };
    }

    void Start()
    {
        bool isPlay = PlayerPrefs.HasKey("SonatGameStudio");

        ScreenManager.Instance.SetActiveFlashCanvas(true);
        StartCoroutine(checkInternetConnection((isConnected) =>
           {
               if (isConnected)
               {
                   CheckFirebaseDependencies(isPlay);
               }
               else
               {
                   SaveableManager.Instance.LoadDataOffline();
               }
           }));
    }


    //Authenticate
    public void SetUpFirebaseAuth(bool actLogin = false)
    {
        auth = FirebaseAuth.DefaultInstance;


        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;

        SetUpFirebaseAuthSuccess = true;


        user = auth.CurrentUser;


    }
    public FirebaseUser GetCurrentUser()
    {
        // Debug.Log("SetUpFirebaseAuthSuccess: " + SetUpFirebaseAuthSuccess);
        if (SetUpFirebaseAuthSuccess) return auth.CurrentUser;
        else
        {
            auth = FirebaseAuth.DefaultInstance;
            GoogleSignIn.Configuration = configuration;
            GoogleSignIn.Configuration.UseGameSignIn = false;
            GoogleSignIn.Configuration.RequestIdToken = true;


            SetUpFirebaseAuthSuccess = true;

            return auth.CurrentUser;
        }
    }
    public void SignInWithGoogle()
    {
        Debug.Log("============== Ckicl login");
        GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(OnAuthenticationFinished);
        Debug.Log(" ==================       SignInWithGoogle");
    }

    public void SignOutFromGoogle() { OnSignOut(); }
    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {

        Debug.Log(" ==================       OnAuthenticationFinished");
        if (task.IsFaulted)
        {
            Debug.Log(" ==================       task.IsFaulted: " + task.IsFaulted);

            using (IEnumerator<Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
            {
                Debug.Log(" ==================       enumerator.MoveNext(): " + enumerator.MoveNext());
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                    Debug.Log("Got Error: " + error.Status + " " + error.Message);
                }
                else
                {
                    Debug.Log("Got Unexpected Exception?!?" + task.Exception);
                }
            }
        }
        else if (task.IsCanceled)
        {
            Debug.Log("Canceled");
        }
        else
        {

            Debug.Log(" ==================       OnAuthenticationFinished           else");
            SignInWithGoogleOnFirebase(task.Result.IdToken);
        }
    }
    private void SignInWithGoogleOnFirebase(string idToken)
    {
        Debug.Log(" ==================       SignInWithGoogleOnFirebase");

        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);
        Debug.Log(" ==================       credential: " + credential);


        auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
        {

            Debug.Log(" ==================       SignInWithCredentialAsync:      end task");
            AggregateException ex = task.Exception;
            if (ex != null)
            {
                Debug.Log("===================== ex  != null");
                if (ex.InnerExceptions[0] is FirebaseException inner && (inner.ErrorCode != 0))
                    Debug.Log("\nError code = " + inner.ErrorCode + " Message = " + inner.Message);
            }
            else
            {
                Debug.Log("=========================  dang nhao thanh cong");
                // Debug.Log("Sign In Successful.");
                // User player = new User();
                // player.UserName = task.Result.DisplayName;
                // player.Email = task.Result.Email;
                // player.UserId = task.Result.UserId;


                // btnSignIn.interactable = false;
                // btnSignOut.interactable = true;
                // infoText.text = "Đăng nhập thành công";
                // Debug.Log("Đăng nhập thành công" + "\nUserName: " + task.Result.DisplayName + "  Email: " + task.Result.Email);

                // GameControler.Instance.SetLoadData(player);
                GameManager.Instance.IdPlayer = task.Result.UserId;
                SaveableManager.Instance.SetUserId(task.Result.UserId);
                SaveableManager.Instance.SetLogIn(true);
                GameManager.Instance.ConfigUserFireBase(task.Result.DisplayName, task.Result.Email);
                GameManager.Instance.IsLogIn = true;
                PopupContainer.Instance.SettingsPopupShowButton(true);

                reference.Child("User").Child(task.Result.UserId).Child("DisplayName").SetValueAsync(task.Result.DisplayName);
                reference.Child("User").Child(task.Result.UserId).Child("Email").SetValueAsync(task.Result.Email);


                string jsonString = PlayerPrefs.GetString("playerInfo");
                PlayerInfo playerLocal = JsonUtility.FromJson<PlayerInfo>(jsonString);

                Read_Data(playerLocal);

                // GameManager.Instance.SetPlayerInfo();
                // SaveableManager.Instance.SetPlayerInfo(GameManager.Instance.GetPlayerInfo());


                // GameManager.Instance.DeactivateLoading();


            }
        });
    }
    private void OnSignOut()
    {
        // Debug.Log("Calling SignOut");
        GoogleSignIn.DefaultInstance.SignOut();


        // btnSignIn.interactable = true;
        // btnSignOut.interactable = false;
        // infoText.text = "Đăng xuất Thành Công";


        // user = auth.CurrentUser;
        // Debug.Log(user.DisplayName);

        GameManager.Instance.IsLogIn = false;
        SaveableManager.Instance.SetLogIn(false);
        PopupContainer.Instance.SettingsPopupShowButton(false);

        // infoText.text += "\n Check tên đăng nhập:  " + user.DisplayName;

        // GameControler.Instance.SetLoadData();
    }

    private void CheckFirebaseDependencies(bool isPlay)
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                if (task.Result == DependencyStatus.Available)
                {
                    SetUpFirebaseAuth();
                    SetUpDataBaseReference();

                    Debug.Log("=========" + "IsMusic3: " + SaveableManager.Instance.IsMusic());
                    
                    if (!isPlay) SaveableManager.Instance.LoadDataOffline();
                    else SaveableManager.Instance.LoadDataOnline();
                    // Debug.Log("GameManager.Instance.IsLogIn: " + GameManager.Instance.IsLogIn);
                    // if (GameManager.Instance.IsLogIn) Read_Data("UserId4324", playerLocal);
                }

                else
                    Debug.Log("Could not resolve all Firebase dependencies: " + task.Result.ToString());
            }
            else
            {
                Debug.Log("Dependency check was not completed. Error : " + task.Exception.Message);
            }
        });
    }
    IEnumerator checkInternetConnection(Action<bool> action)
    {
        UnityWebRequest request = new UnityWebRequest("http://google.com");
        yield return request.SendWebRequest();
        if (request.error != null) action(false);
        else action(true);
    }
    //RealtimeDatabase


    public void SetUpDataBaseReference()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    public void SaveData(PlayerInfo user)
    {
        string UserId = GameManager.Instance.IdPlayer;
        // Debug.Log("id: " + UserId);
        Debug.Log(user.ToString());
        StartCoroutine(checkInternetConnection((isConnected) =>
          {
              if (isConnected)
              {
                  user.boardsInProgress = null;
                  string json = JsonUtility.ToJson(user);
                  reference.Child("User").Child(UserId).SetRawJsonValueAsync(json)
                  .ContinueWith(task =>
                  {
                      if (task.IsCompleted)
                      {
                          Debug.Log("successdully added data to firebase");
                      }
                      else Debug.Log("not successdully");
                  });
              };
          }));

    }

    public void Read_Data(PlayerInfo playerLocal)
    {
        Debug.Log("=========" + "Read_Data");
        string userId = GameManager.Instance.IdPlayer;
        reference.Child("User").Child(userId).GetValueAsync().ContinueWithOnMainThread(task =>
               {
                   if (task.IsCompleted)
                   {
                       DataSnapshot snapshot = task.Result;
                       //    Debug.Log("Get data successdully");
                       PlayerInfo playerFireBase = JsonUtility.FromJson<PlayerInfo>(snapshot.GetRawJsonValue());
                       //    Debug.Log("playerLocal: ");
                       //    Debug.Log(playerLocal.ToString());
                       //    Debug.Log("playerFireBase: ");
                       //    Debug.Log(playerFireBase.ToString());
                       PlayerInfo playerInfo = new PlayerInfo();
                       playerInfo.Union(playerLocal, playerFireBase);
                       //    Debug.Log(playerInfo.ToString());
                       GameManager.Instance.ConfigData(playerInfo);

                       GameManager.Instance.SetPlayerInfo();
                       SaveableManager.Instance.SetPlayerInfo(GameManager.Instance.GetPlayerInfo());


                   }
                   else Debug.Log("not successdully");
               });


    }
}
