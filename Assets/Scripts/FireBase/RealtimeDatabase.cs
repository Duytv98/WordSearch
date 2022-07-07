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
public class RealtimeDatabase : MonoBehaviour
{
    private DatabaseReference reference;
    
    [SerializeField] private Image avatar = null;
    public void SetUp()
    {
        // Debug.Log("reference SetUp");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void Read_Data()
    {
        string userId = SaveableManager.Instance.GetUserId();
        // Debug.Log("userid: " + userId);
        reference.Child("User").Child(userId).GetValueAsync().ContinueWithOnMainThread(task =>
                      {
                          if (task.IsCompleted)
                          {
                            //   Debug.Log("aaaaaaaaaaaaaaaaaa");
                              DataSnapshot snapshot = task.Result;
                              if (string.IsNullOrEmpty(snapshot.GetRawJsonValue()))
                              {
                                //   Debug.Log(null);
                                  SaveableManager.Instance.LoadDataOffline();
                                  CreateData();
                                  return;
                              }
                              PlayerInfo playerFireBase = JsonUtility.FromJson<PlayerInfo>(snapshot.GetRawJsonValue());
                            //   Debug.Log("playerFireBase: " + JsonUtility.ToJson(playerFireBase));
                              PlayerInfo playerLocal = SaveableManager.Instance.GetPlayerLocal();

                            //   Debug.Log("playerLocal: " + JsonUtility.ToJson(playerLocal));
                              PlayerInfo playerInfo = new PlayerInfo();
                            //   Debug.Log(1111);
                              playerInfo.Union(playerLocal, playerFireBase);
                            //   Debug.Log(222222);
                            //   Debug.Log("playerInfo: " + JsonUtility.ToJson(playerInfo));
                              DisplayAvatar(playerInfo.avatar);

                              SaveableManager.Instance.SaveDataPlayerLocal(playerInfo);

                              SaveableManager.Instance.LoadDataOffline();
                          }
                          else Debug.Log("not successdully");
                      });

    }

    public void SaveCoins()
    {
        var userId = SaveableManager.Instance.GetUserId();

        var coins = PlayerPrefs.GetInt(GameDefine.KEY_USER_COINS);
        reference.Child("User").Child(userId).Child("coins").SetValueAsync(coins)
        .ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("successdully added data to firebase");
            }
            else Debug.Log("not successdully");
        });
    }

    public void SaveKeys()
    {
        var userId = SaveableManager.Instance.GetUserId();

        var keys = PlayerPrefs.GetInt(GameDefine.KEY_USER_KEYS);
        reference.Child("User").Child(userId).Child("keys").SetValueAsync(keys)
        .ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("successdully added data to firebase");
            }
            else Debug.Log("not successdully");
        });
    }

    public void SaveLastCompletedLevels()
    {
        var userId = SaveableManager.Instance.GetUserId();

        var lastCompletedLevels = PlayerPrefs.GetString(GameDefine.KEY_LAST_COMPLETED_LEVELS);
        reference.Child("User").Child(userId).Child("lastCompletedLevels").SetValueAsync(lastCompletedLevels)
        .ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("successdully added data to firebase");
            }
            else Debug.Log("not successdully");
        });
    }

    public void SaveUnlockedCategories()
    {
        var userId = SaveableManager.Instance.GetUserId();

        var unlockedCategories = PlayerPrefs.GetString(GameDefine.KEY_UNLOCKED_CATEGORIES);
        reference.Child("User").Child(userId).Child("unlockedCategories").SetValueAsync(unlockedCategories)
        .ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("successdully added data to firebase");
            }
            else Debug.Log("not successdully");
        });
    }

    public void SaveListBooster()
    {
        var userId = SaveableManager.Instance.GetUserId();

        var listBooster = PlayerPrefs.GetString(GameDefine.KEY_LIST_BOOSTER);
        reference.Child("User").Child(userId).Child("listBooster").SetValueAsync(listBooster)
        .ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("successdully added data to firebase");
            }
            else Debug.Log("not successdully");
        });
    }

    public void SaveTimeCompleteLevel()
    {
        var userId = SaveableManager.Instance.GetUserId();

        var timeCompleteLevel = PlayerPrefs.GetString(GameDefine.KEY_TIME_COMPLETE_LEVEL);
        reference.Child("User").Child(userId).Child("timeCompleteLevel").SetValueAsync(timeCompleteLevel)
        .ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("successdully added data to firebase");
            }
            else Debug.Log("not successdully");
        });
    }
    public void EditData(string keyOnline, string keyLocal)
    {
        var userId = SaveableManager.Instance.GetUserId();

        var value = PlayerPrefs.GetString(keyLocal);
        reference.Child("User").Child(userId).Child("keyOnline").SetValueAsync(value)
        .ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("successdully added data to firebase");
            }
            else Debug.Log("not successdully");
        });
    }

    public void CreateData()
    {
        Debug.Log("CreateData");
        var UserId = SaveableManager.Instance.GetUserId();

        PlayerInfo playerInfo = new PlayerInfo();
        playerInfo.displayName = PlayerPrefs.GetString(GameDefine.KEY_DISPLAY_NAME);
        playerInfo.coins = PlayerPrefs.GetInt(GameDefine.KEY_USER_COINS);
        playerInfo.keys = PlayerPrefs.GetInt(GameDefine.KEY_USER_KEYS);
        playerInfo.lastCompletedLevels = PlayerPrefs.GetString(GameDefine.KEY_LAST_COMPLETED_LEVELS);
        playerInfo.unlockedCategories = PlayerPrefs.GetString(GameDefine.KEY_UNLOCKED_CATEGORIES);
        playerInfo.listBooster = PlayerPrefs.GetString(GameDefine.KEY_LIST_BOOSTER);
        playerInfo.timeCompleteLevel = PlayerPrefs.GetString(GameDefine.KEY_TIME_COMPLETE_LEVEL);
        playerInfo.avatar = PlayerPrefs.GetString(GameDefine.KEY_AVATAR);

        string json = JsonUtility.ToJson(playerInfo);
        reference.Child("User").Child(UserId).SetRawJsonValueAsync(json)
        .ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("successdully added data to firebase");
            }
            else Debug.Log("not successdully");
        });
    }

    private void DisplayAvatar(string enc)
    {
        Texture2D tex = Convert.Base64ToTexture(enc);
        avatar.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        avatar.rectTransform.sizeDelta = new Vector2(tex.width, tex.height);
    }

}
