using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scenes
{
    Level01,
    Level02,
    Level03,
}


public static class SceneLoader
{
    
    public static void LoadNextLevel()
    {
        // if less than 3 load next level, if 3 load first level
        if (GameController.instance.currentLevel < 3)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        else
            SceneManager.LoadScene(Scenes.Level01.ToString());
    }
}
