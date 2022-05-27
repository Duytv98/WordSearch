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
        else{
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
        // Play("bkg-music");
    }

    public void Play(string name)
    {
        Sound sound = Array.Find(soundInfos, sound => sound.name == name);
        if (sound == null)
        {
            {
                Debug.LogWarning("Sound: " + name + " not found!");
                return;
            }
        }
        sound.source.Play();
    }

    public void Play_Click_Button_Sound(){
        Play("btn-click");
    }
}
