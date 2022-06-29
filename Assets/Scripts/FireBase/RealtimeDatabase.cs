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
    public void SetUp()
    {
        Debug.Log("reference SetUp");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void Read_Data()
    {
        string userId = SaveableManager.Instance.GetUserId();
        reference.Child("User").Child(userId).GetValueAsync().ContinueWithOnMainThread(task =>
                      {
                          if (task.IsCompleted)
                          {
                              DataSnapshot snapshot = task.Result;
                              if (string.IsNullOrEmpty(snapshot.GetRawJsonValue()))
                              {
                                  SaveableManager.Instance.LoadDataOffline();
                                  CreateData();
                                  return;
                              }

                              PlayerInfo playerFireBase = JsonUtility.FromJson<PlayerInfo>(snapshot.GetRawJsonValue());
                              PlayerInfo playerLocal = SaveableManager.Instance.GetPlayerLocal();

                              PlayerInfo playerInfo = new PlayerInfo();
                              playerInfo.Union(playerLocal, playerFireBase);
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



}
