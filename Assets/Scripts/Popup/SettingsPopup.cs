using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;
using Firebase.Auth;
public class SettingsPopup : MonoBehaviour
{

    // [SerializeField] private ToggleSlider musicToggle = null;
    // [SerializeField] private ToggleSlider soundToggle = null;
    [SerializeField] private CanvasGroup btnLogIn = null;
    [SerializeField] private CanvasGroup btnLogOut = null;

    public void OnShowing()
    {
       
        bool isLogIn = GameManager.Instance.IsLogIn;
        SetButton(isLogIn);
        StartCoroutine(checkInternetConnection((isConnected) =>
        {
            SetButton(isLogIn, isConnected);
        }));

    }
    public void SetButton(bool isLogIn, bool isConnected = true)
    {
        // Debug.Log("SetButton   isLogIn: " + isLogIn);
        btnLogIn.alpha = isLogIn ? 0 : isConnected ? 1 : 0.7f;
        btnLogIn.interactable = !isLogIn && isConnected;
        btnLogIn.blocksRaycasts = !isLogIn && isConnected;

        btnLogOut.alpha = isLogIn ? isConnected ? 1 : 0.7f : 0;
        btnLogOut.interactable = isLogIn && isConnected;
        btnLogOut.blocksRaycasts = isLogIn && isConnected;
    }
    public void SetActiveButtonLogin(bool active)
    {
        btnLogIn.alpha = active ? 1 : 0;
        btnLogIn.interactable = active;
        btnLogIn.blocksRaycasts = active;
    }
    public void SetActiveButtonLogout(bool active)
    {
        btnLogOut.alpha = active ? 1 : 0;
        btnLogOut.interactable = active;
        btnLogOut.blocksRaycasts = active;
    }



    public void CloseSettingsPopup()
    {
        PopupContainer.Instance.ClosePopup("SettingsPopup");
    }

    IEnumerator checkInternetConnection(Action<bool> action)
    {
        UnityWebRequest request = new UnityWebRequest("http://google.com");
        yield return request.SendWebRequest();
        if (request.error != null)
        {
            action(false);
            Debug.Log("Không có mạng");
        }
        else
        {
            action(true);
            Debug.Log("Có mạng");

        }
    }
}
