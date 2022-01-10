﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// assign dictionaries for player and bots here, add bots here if needed
// max bot count: 3
// pause, resume actions
// parameters shared by many units

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public float wingCollectTime = 2f;
    [System.NonSerialized] public Dictionary<string, bool> mainWingsOnBack = new Dictionary<string, bool>();
    [System.NonSerialized] public Dictionary<string, bool> isLandingAvailable = new Dictionary<string, bool>();

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        // assign values to dictionaries
        isLandingAvailable.Add("Player", false);
        isLandingAvailable.Add("Bot1", false);
        isLandingAvailable.Add("Bot2", false);
        isLandingAvailable.Add("Bot3", false);
        isLandingAvailable.Add("Bot4", false);

        mainWingsOnBack.Add("Player", false);
        mainWingsOnBack.Add("Bot1", false);
        mainWingsOnBack.Add("Bot2", false);
        mainWingsOnBack.Add("Bot3", false);
        mainWingsOnBack.Add("Bot4", false);
    }

    // Get and Set isLandingAvailable bools
    public bool GetLandingValue(string name)
    {
        bool value;
        if (isLandingAvailable.TryGetValue(name, out value))
        {
            // success
            return value;
        }
        else
        {
            // failure
            Debug.Log("Couldnt get isLandingAvailable value from dictionary, returning false");
            return false;
        }
    }

    public void SetLandingValue(string name, bool value)
    {
        isLandingAvailable[name] = value;
    }


    // Get and Set mainWingsOnBack bools
    public bool GetMainWingsValue(string name)
    {
        bool value;
        if (mainWingsOnBack.TryGetValue(name, out value))
        {
            // success
            return value;
        }
        else
        {
            // failure
            Debug.Log("Couldnt get mainWingsOnBack value from dictionary, returning false");
            return false;
        }
    }

    public void SetMainWingsValue(string name, bool value)
    {
        mainWingsOnBack[name] = value;
    }
}
