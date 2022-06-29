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
    [SerializeField] private RectTransform shineMusic = null;
    [SerializeField] private RectTransform shineSound = null;
    [SerializeField] private Image imgMusic = null;
    [SerializeField] private Image imgSound = null;
    [SerializeField] private Sprite bgOn = null;
    [SerializeField] private Sprite bgOff = null;
    [SerializeField] private GameObject txtMusicOn = null;
    [SerializeField] private GameObject txtMusicOff = null;
    [SerializeField] private GameObject txtSoundOn = null;
    [SerializeField] private GameObject txtSoundOff = null;


    public void OnShowing()
    {
        shineMusic.anchoredPosition = new Vector2(GameManager.Instance.IsMusic ? 54f : -54f, shineMusic.anchoredPosition.y);
        shineSound.anchoredPosition = new Vector2(GameManager.Instance.IsSound ? 54f : -54f, shineSound.anchoredPosition.y);
        if (GameManager.Instance.IsMusic) SetOnMusic();
        else SetOffMusic();
        if (GameManager.Instance.IsSound) SetOnSound();
        else SetOffSound();

    }
    public void ClickMusic()
    {
        bool isMusic = GameManager.Instance.IsMusic;
        float x_shineMusic = isMusic ? -54f : 54f;
        GameManager.Instance.IsMusic = !isMusic;
        if (GameManager.Instance.IsMusic)
        {
            SetOnMusic();
            AudioManager.Instance.PlayMusic();
        }
        else
        {
            SetOffMusic();
            AudioManager.Instance.PauseMusic();
        }
        shineMusic.DOAnchorPosX(x_shineMusic, 0.2f);

        SaveableManager.Instance.SaveMusic(!isMusic);
    }
    public void ClickSound()
    {
        bool isSound = GameManager.Instance.IsSound;
        float x_shineSound = isSound ? -54f : 54f;
        GameManager.Instance.IsSound = !isSound;

        if (GameManager.Instance.IsSound) SetOnSound();
        else SetOffSound();
        shineSound.DOAnchorPosX(x_shineSound, 0.2f);

        SaveableManager.Instance.SaveSound(!isSound);
    }

    public void SetOnSound()
    {
        imgSound.sprite = bgOn;
        imgSound.SetNativeSize();
        txtSoundOn.SetActive(true);
        txtSoundOff.SetActive(false);
    }
    public void SetOffSound()
    {
        imgSound.sprite = bgOff;
        imgSound.SetNativeSize();
        txtSoundOn.SetActive(false);
        txtSoundOff.SetActive(true);
    }
    public void SetOnMusic()
    {
        imgMusic.sprite = bgOn;
        imgMusic.SetNativeSize();
        txtMusicOn.SetActive(true);
        txtMusicOff.SetActive(false);
    }
    public void SetOffMusic()
    {
        imgMusic.sprite = bgOff;
        imgMusic.SetNativeSize();
        txtMusicOn.SetActive(false);
        txtMusicOff.SetActive(true);
    }


}
