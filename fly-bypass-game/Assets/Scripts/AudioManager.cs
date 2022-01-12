using UnityEngine.Audio;
using UnityEngine;
using System;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public Sound[] sounds;
    private string currentlyPlaying;

    public string[] presses = { "ButtonPress1", "ButtonPress2", "ButtonPress3" };
    public string[] wingcollects = { "WingCollect1", "WingCollect2"};
    public string running = "Running";
    public string flying = "Flying";

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.audioSource = gameObject.AddComponent<AudioSource>();
            s.audioSource.clip = s.clip;
            s.audioSource.volume = s.volume;
            s.audioSource.pitch = s.pitch;
            s.audioSource.loop = s.isLooping;
            s.initialVolumeLevel = s.volume;
        }
    }


    public void PlaySound(string name)
    {
        if (currentlyPlaying == name)
            return;

        Sound sound = Array.Find(sounds, s => s.name == name);
        if (sound == null)
        {
            Debug.Log("sound is not found!");
            return;
        }

        if (sound.isLooping)
        {
            StopCurrentSound();
            currentlyPlaying = sound.name;
        }

        sound.audioSource.volume = sound.volume;
        sound.audioSource.Play();
    }

    public void PlayRandomSound(string[] stringArray)
    {
        int size = stringArray.Length;
        int index = UnityEngine.Random.Range(0, size);
        string sw = stringArray[index];
        PlaySound(sw);
    }

    public void StopCurrentSound()
    {
        // stops the one that's looping currently
        if (currentlyPlaying == null)
            return;

        Sound currentSound = Array.Find(sounds, s => s.name == currentlyPlaying);
        if (currentSound.isLooping)
            currentSound.audioSource.Stop();
    }

    // play this once in gamecontroller
    public void PlayThemeSong()
    {
        Sound sound = Array.Find(sounds, s => s.name == "Theme1");
        if (sound == null)
        {
            Debug.Log("sound is not found!");
            return;
        }

        sound.audioSource.volume = sound.volume;
        sound.audioSource.Play();
    }
}
