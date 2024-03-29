﻿using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;
    [Range(.1f, 5f)]
    public float pitch;

    [HideInInspector]
    public float initialVolumeLevel;

    [HideInInspector]
    public AudioSource audioSource;

    public bool isLooping;
}
