using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;
using Firebase.Auth;
using DG.Tweening;
public class SettingsPopup : MonoBehaviour
{

    // [SerializeField] private ToggleSlider musicToggle = null;
    // [SerializeField] private ToggleSlider soundToggle = null;
    [SerializeField] private CanvasGroup btnLogIn = null;
    [SerializeField] private CanvasGroup btnLogOut = null;

    [SerializeField] private Transform toggleMusic = null;
    [SerializeField] private Transform toggleSound = null;


    public void OnShowing()
    {

        bool isLogIn = GameManager.Instance.IsLogIn;
        toggleMusic.localPosition = new Vector3(GameManager.Instance.IsMusic ? 50f : -50f, toggleMusic.localPosition.y, toggleMusic.localPosition.z);
        toggleSound.localPosition = new Vector3(GameManager.Instance.IsSound ? 50f : -50f, toggleSound.localPosition.y, toggleSound.localPosition.z);
        SetButton(isLogIn);
        StartCoroutine(checkInternetConnection((isConnected) =>
        {
            SetButton(isLogIn, isConnected);
        }));

    }
    public void SetButton(bool isLogIn, bool isConnected = true)
    {
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
    IEnumerator checkInternetConnection(Action<bool> action)
    {
        UnityWebRequest request = new UnityWebRequest("http://google.com");
        yield return request.SendWebRequest();
        if (request.error != null) action(false);
        else action(true);
    }
    public void ClickMusic()
    {
        bool isMusic = GameManager.Instance.IsMusic;
        float x_toggleMusic = isMusic ? -50f : 50f;
        GameManager.Instance.IsMusic = !isMusic;
        toggleMusic.DOLocalMoveX(x_toggleMusic, 0.3f);


        if (isMusic)
        {
            AudioManager.Instance.PauseMusic();

        }
        else
        {
            AudioManager.Instance.PlayMusic();
        }

        SaveableManager.Instance.SetMusic(!isMusic);
    }
    public void ClickSound()
    {
        bool isSound = GameManager.Instance.IsSound;
        float x_toggleSound = isSound ? -50f : 50f;
        GameManager.Instance.IsSound = !isSound;
        toggleSound.DOLocalMoveX(x_toggleSound, 0.3f);

        SaveableManager.Instance.SetSound(!isSound);
    }


}
