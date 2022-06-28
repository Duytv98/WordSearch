using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SimpleJSON;
[System.Serializable]
public class User
{
    public string DisplayName;
    public string Email;
    public string UserId;

    public User()
    {
    }
    public string SaveToString()
    {
        return JsonUtility.ToJson(this);
    }
}
