using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Google;
public class FireBaseController : MonoBehaviour
{

    public string webClientId = "<your client id here>";
    private GoogleSignInConfiguration configuration;
    private FirebaseAuth auth;
    private FirebaseUser user;


    private DatabaseReference reference;


    private void Awake()
    {
        configuration = new GoogleSignInConfiguration { WebClientId = webClientId, RequestEmail = true, RequestIdToken = true };
    }


    //Authenticate
    public void SetUpFirebaseAuth()
    {
        auth = FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
        if (user != null)
        {
            // btnSignOut.interactable = true;
            // infoText.text = user.DisplayName;
            // User player = new User();
            // player.UserName = user.DisplayName;
            // player.Email = user.Email;
            // player.UserId = user.UserId;
            // infoText.text += "\n Check tên đăng nhập:  " + user.DisplayName;
            // GameControler.Instance.SetLoadData(player);
        }
        else
        {
            // btnSignIn.interactable = true;
            // infoText.text = "Chưa đăng nhập";
        }

        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
    }
    public void SignInWithGoogle()
    {
        Debug.Log("Calling SignIn");
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

    public void SignOutFromGoogle() { OnSignOut(); }
    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using (IEnumerator<Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
            {
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
            SignInWithGoogleOnFirebase(task.Result.IdToken);
        }
    }
    private void SignInWithGoogleOnFirebase(string idToken)
    {
        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            AggregateException ex = task.Exception;
            if (ex != null)
            {
                if (ex.InnerExceptions[0] is FirebaseException inner && (inner.ErrorCode != 0))
                    Debug.Log("\nError code = " + inner.ErrorCode + " Message = " + inner.Message);
            }
            else
            {
                Debug.Log("Sign In Successful.");
                // User player = new User();
                // player.UserName = task.Result.DisplayName;
                // player.Email = task.Result.Email;
                // player.UserId = task.Result.UserId;


                // btnSignIn.interactable = false;
                // btnSignOut.interactable = true;
                // infoText.text = "Đăng nhập thành công";


                // GameControler.Instance.SetLoadData(player);

            }
        });
    }



    private void OnSignOut()
    {
        Debug.Log("Calling SignOut");
        GoogleSignIn.DefaultInstance.SignOut();


        // btnSignIn.interactable = true;
        // btnSignOut.interactable = false;
        // infoText.text = "Đăng xuất Thành Công";


        user = auth.CurrentUser;
        // infoText.text += "\n Check tên đăng nhập:  " + user.DisplayName;

        // GameControler.Instance.SetLoadData();
    }



    //RealtimeDatabase


}
