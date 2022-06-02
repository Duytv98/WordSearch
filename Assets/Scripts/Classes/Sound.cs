using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{

    public string name;
    public AudioClip clip;
    public SoundType type = SoundType.SoundEffect;
    public bool playOnAwake = false;
    public bool loop = false;


    [Range(0, 1)] public float volume = 1;
    [Range(0, 1)] public float pitch = 1;

    [HideInInspector]
    public AudioSource source;
    public enum SoundType
    {
        SoundEffect,
        Music
    }

}
