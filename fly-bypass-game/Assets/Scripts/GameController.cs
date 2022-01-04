﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// pause, resume actions
// parameters shared by many units

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public float wingCollectTime = 2f;
    public bool collectWingsOnBack = false;         // might hide this on inspector put still public

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }
}
