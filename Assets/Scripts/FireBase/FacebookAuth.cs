using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using Firebase.Auth;
using Firebase.Extensions;
using Facebook.MiniJSON;
using System;
using UnityEngine.UI;
using System.IO;

public class FacebookAuth : MonoBehaviour
{
    [SerializeField] private Image avatar = null;
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
        // Debug.Log(result);
        // Debug.Log(FB.IsLoggedIn);
        if (FB.IsLoggedIn)
        {
            AccessToken aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Debug.Log("aToken.UserId: " + aToken.UserId);
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
            GetAvatar();

            CheckCurrentUser();
        });
    }

    private void CheckCurrentUser()
    {
        Debug.Log(" ===== CheckCurrentUser");
        FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            Debug.Log(string.Format("UserId: {0}\nProviderId: {1}\nDisplayName: {2}\nEmail: {3}\nPhotoUrl: {4}\n", user.UserId, user.ProviderId, user.DisplayName, user.Email, user.PhotoUrl));
        }
        else Debug.Log(null);
    }
    string text = string.Empty;
    public void GetFriendsPlayingThisGame()
    {
        string query = "me/friends";
        FB.API(query, HttpMethod.GET, result =>
        {
            var dictionary = (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(result.RawResult);
            var friendsList = (List<object>)dictionary["data"];
            Debug.Log("friendsList count: " + friendsList.Count);
            text = string.Empty;
            foreach (var dict in friendsList)
                text += ((Dictionary<string, object>)dict)["name"];
            Debug.Log(text);
        });
    }
    public void GetAvatar()
    {
        FB.API("me/picture?type=square&height=128&width=128", HttpMethod.GET, SetAvatarFirebase);
    }
    private void SetAvatarFirebase(IGraphResult data)
    {
        string enc = Convert.Base64Texture(data.Texture);
        FirebaseUser user = auth.CurrentUser;
        SaveableManager.Instance.CheckAccount(user, GameDefine.KEY_PROVIDERS_FB, enc);
    }

    private void DisplayAvatar(string enc)
    {
        Texture2D tex = Convert.Base64ToTexture(enc);
        avatar.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        avatar.rectTransform.sizeDelta = new Vector2(tex.width, tex.height);
    }


}
