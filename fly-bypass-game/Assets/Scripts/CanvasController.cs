using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// subcanvas type and canvas name should be the same
public enum CanvasType
{
    MainMenu,
    GameUI,
}

public class CanvasController : MonoBehaviour
{
    public static CanvasController instance;
    private List<SubCanvas> listSubCanvas = new List<SubCanvas>();
    
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        listSubCanvas = GetComponentsInChildren<SubCanvas>(false).ToList();
        SwitchCanvas(CanvasType.GameUI);
    }

    public void SwitchCanvas(CanvasType type)
    {
        listSubCanvas.ForEach(x => x.gameObject.SetActive(false));
        SubCanvas targetCanvas = listSubCanvas.Find(x => x.gameObject.name == type.ToString());
        if(targetCanvas != null)
        {
            targetCanvas.gameObject.SetActive(true);
        }
    }


}


