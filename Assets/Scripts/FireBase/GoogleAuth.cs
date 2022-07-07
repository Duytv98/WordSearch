using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Google;

public class GoogleAuth : MonoBehaviour
{
    private FirebaseAuth auth;

    public void SetUp()
    {

        GoogleSignInConfiguration configuration = new GoogleSignInConfiguration { WebClientId = GameDefine.WEBCLIENTID, RequestEmail = true, RequestIdToken = true };

        auth = FirebaseAuth.DefaultInstance;

        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
    }

    public void Login()
    {
        GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(OnAuthenticationFinished);
    }

    public void LogOut()
    {
        OnSignOut();
    }

    private void OnAuthenticationFinished(Task<GoogleSignInUser> task)
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
        auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
        {
            AggregateException ex = task.Exception;
            if (ex != null)
            {
                if (ex.InnerExceptions[0] is FirebaseException inner && (inner.ErrorCode != 0))
                    Debug.Log("\nError code = " + inner.ErrorCode + " Message = " + inner.Message);
            }
            else
            {
                FirebaseUser newuser = task.Result;
                SaveableManager.Instance.CheckAccount(newuser, GameDefine.KEY_PROVIDERS_GG);
                CheckCurrentUser();
            }
        });
    }

    private void OnSignOut()
    {
        GoogleSignIn.DefaultInstance.SignOut();
        auth.SignOut();
    }
    private void CheckCurrentUser()
    {
        // Debug.Log(" ===== CheckCurrentUser");
        var user = auth.CurrentUser;
        if (user != null)
        {
            Debug.Log(string.Format("UserId: {0}\nProviderId: {1}\nDisplayName: {2}\nEmail: {3}\nPhotoUrl: {4}\n", user.UserId, user.ProviderId, user.DisplayName, user.Email, user.PhotoUrl));
        }
        else Debug.Log(null);
    }



}
