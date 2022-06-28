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

    public void CreateData()
    {
        Debug.Log("CreateData");
        var UserId = SaveableManager.Instance.GetUserId();
        // Debug.Log("id: " + UserId);

        PlayerInfo playerInfo = new PlayerInfo();
        playerInfo.displayName = PlayerPrefs.GetString(GameDefine.KEY_DISPLAY_NAME);
        playerInfo.coins =PlayerPrefs.GetInt(GameDefine.KEY_USER_COINS);
        playerInfo.keys = PlayerPrefs.GetInt(GameDefine.KEY_USER_KEYS);
        playerInfo.lastCompletedLevels = PlayerPrefs.GetString(GameDefine.KEY_LAST_COMPLETED_LEVELS);
        playerInfo.unlockedCategories = PlayerPrefs.GetString(GameDefine.KEY_UNLOCKED_CATEGORIES);
        playerInfo.listBooter = PlayerPrefs.GetString(GameDefine.KEY_LIST_BOOSTER);

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

    public void Read_Data()
    {
        Debug.Log("=========" + "Read_Data");
        string userId = SaveableManager.Instance.GetUserId();
        reference.Child("User").Child(userId).GetValueAsync().ContinueWithOnMainThread(task =>
                      {
                          if (task.IsCompleted)
                          {
                              DataSnapshot snapshot = task.Result;
                              Debug.Log(snapshot.GetRawJsonValue());

                              if (string.IsNullOrEmpty(snapshot.GetRawJsonValue()))
                              {
                                Debug.Log("aaaaaaaaa");
                                  SaveableManager.Instance.LoadDataOffline();
                                  CreateData();
                                  return;
                              }

                              PlayerInfo playerFireBase = JsonUtility.FromJson<PlayerInfo>(snapshot.GetRawJsonValue());
                              Debug.Log(playerFireBase.ToString());
                              //   PlayerInfo playerInfo = new PlayerInfo();
                              //   playerInfo.Union(playerLocal, playerFireBase);
                              //   Debug.Log(playerInfo.ToString());
                              //   GameManager.Instance.ConfigData(playerInfo);

                              //   GameManager.Instance.SetPlayerInfo();
                              //   SaveableManager.Instance.SetPlayerInfo(GameManager.Instance.GetPlayerInfo());


                          }
                          else Debug.Log("not successdully");
                      });

    }



}
