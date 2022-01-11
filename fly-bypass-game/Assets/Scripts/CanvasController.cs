using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

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

    [Header("Distance UI")]
    [SerializeField] private RectTransform arrowRT;
    [SerializeField] private RectTransform finishRT;
    private float arrowStart, arrowEnd, arrowRange;


    void Awake()
    {
        Debug.Log("canvas controller awake()");

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        listSubCanvas = GetComponentsInChildren<SubCanvas>(true).ToList();
        SwitchCanvas(CanvasType.GameUI);
    }

    void Start()
    {
        Debug.Log("canvas controller start()");
        arrowStart = arrowRT.localPosition.x;
        arrowEnd = finishRT.localPosition.x;
        arrowRange = arrowEnd - arrowStart;
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

    public void AssignArrowUI(float characterZ, float startZ, float finishZ)
    {
        float range = finishZ - startZ;
        float position = (characterZ - startZ) / range;  // 0 to 1 range start finish
        float arrowLocalX = arrowStart + arrowRange * position;
        arrowRT.localPosition = new Vector3(arrowLocalX, 0f, 0f);
    }
}


