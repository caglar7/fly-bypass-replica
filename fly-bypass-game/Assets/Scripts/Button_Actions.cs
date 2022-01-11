﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ButtonType
{ 
    PauseButton,
    ResumeButton,
    ExitButton,
}


[RequireComponent(typeof(Button))]
public class Button_Actions : MonoBehaviour
{
    [SerializeField] private ButtonType buttonType;
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ButtonAction);
    }

    private void ButtonAction()
    {
        switch(buttonType)
        {
            case ButtonType.PauseButton:
                GameController.instance.PauseGame();
                CanvasController.instance.SwitchCanvas(CanvasType.MainMenu);
                break;
            case ButtonType.ResumeButton:
                GameController.instance.ResumeGame();
                CanvasController.instance.SwitchCanvas(CanvasType.GameUI);
                break;
            case ButtonType.ExitButton:
                GameController.instance.ExitGame();
                break;
            default:
                Debug.Log("button actions, unknown button type!");
                break;
        }
    }
}