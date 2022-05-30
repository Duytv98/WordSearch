using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Sound[] soundInfos = null;

    public static AudioManager Instance;
    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        foreach (var sound in soundInfos)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            sound.source.playOnAwake = sound.playOnAwake;

        }
    }
    private void Start()
    {
        // if(GameManager.Instance.IsMusic) PlayMusic();
        // Play("bkg-music");
        
        Debug.Log("CheckExistData()  1   :" + PlayerPrefs.HasKey("Used_to_play"));
        Debug.Log("Used_to_play  1 :" + PlayerPrefs.GetString("Used_to_play"));
        Debug.Log(PlayerPrefs.GetString("Used_to_play"));
        Debug.Log("Test  1   :" + PlayerPrefs.HasKey("abvdd"));

        Debug.Log(PlayerPrefs.GetInt("abvdd"));
        Debug.Log("11111111");
        // ScreenManager.Instance.SetActiveFlashCanvas(true)
    }

    public void Play(string name)
    {
        // Debug.Log("play sound: " + name);
        Sound sound = Array.Find(soundInfos, sound => sound.name == name);
        if (sound == null)
        {
            {
                Debug.LogWarning("Sound: " + name + " not found!");
                return;
            }
        }
        if(GameManager.Instance.IsSound) sound.source.Play();
    }

    public void Play_Click_Button_Sound()
    {
        Play("btn-click");
    }
    public void PauseMusic(){
        soundInfos[0].source.Pause();
    }
    public void PlayMusic(){
        soundInfos[0].source.Play();
    }
}
