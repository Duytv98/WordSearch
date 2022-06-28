using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using Firebase.Auth;
using Firebase.Extensions;
using System;
using UnityEngine.UI;
public class FacebookAuth : MonoBehaviour
{
    FirebaseAuth auth;
    void Awake()
    {
        if (!FB.IsInitialized) FB.Init(this.InitCallback, this.OnHideUnity);
        else FB.ActivateApp();
    }

    private void InitCallback()
    {
        if (FB.IsInitialized) FB.ActivateApp();
        else Debug.Log("Failed to Initialize the Facebook SDK");
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown) Time.timeScale = 0;
        else Time.timeScale = 1;
    }

    public void SetUp()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    public void LogOut()
    {
        FB.LogOut();
        auth.SignOut();
    }
    public void Login()
    {
        var permission = new List<string>() { "public_profile", "email", "user_friends" };
        FB.LogInWithReadPermissions(permission, AuthCallBack);
    }

    private void AuthCallBack(ILoginResult result)
    {
        Debug.Log(result);
        Debug.Log(FB.IsLoggedIn);
        if (FB.IsLoggedIn)
        {
            AccessToken aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            Debug.Log(aToken.UserId);
            foreach (string perm in aToken.Permissions)
            {
                Debug.Log(perm);
            }
            authwithfirebase(aToken.TokenString);
        }
        else
        {
            Debug.Log("User Cancelled login");
        }
    }
    public void authwithfirebase(string accesstoken)
    {
        Firebase.Auth.Credential credential = Firebase.Auth.FacebookAuthProvider.GetCredential(accesstoken);
        auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("singin encountered error" + task.Exception);
            }
            FirebaseUser newuser = task.Result;
            SaveableManager.Instance.SetLogIn(true);
            SaveableManager.Instance.SaveDataUser(newuser.DisplayName, newuser.UserId);
            CheckCurrentUser();
        });
    }



    private void CheckCurrentUser()
    {
        Debug.Log(" ===== CheckCurrentUser");
        var user = auth.CurrentUser;
        if (user != null)
        {
            Debug.Log(string.Format("UserId: {0}\nProviderId: {1}\nDisplayName: {2}\nEmail: {3}\nPhotoUrl: {4}\n", user.UserId, user.ProviderId, user.DisplayName, user.Email, user.PhotoUrl));
        }
        else Debug.Log(null);
    }

}
