using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class FinishPlace : MonoBehaviour
{
    [SerializeField] private int position;  // can be 1,2,3,4 and 5
    [SerializeField] private Color color;  // for vertex color of tmp
    [SerializeField] private Color waitingColor;

    private TextMeshProUGUI textPosition;
    private TextMeshProUGUI textName;
    private TextMeshProUGUI textScore;
    private string positionText;

    void Start()
    {
        List<TextMeshProUGUI> texts = GetComponentsInChildren<TextMeshProUGUI>().ToList();
        textPosition = texts.Where(x => x.gameObject.name == "position").SingleOrDefault();
        textName = texts.Where(x => x.gameObject.name == "name").SingleOrDefault();
        textScore = texts.Where(x => x.gameObject.name == "score").SingleOrDefault();
        positionText = textPosition.text;   // store 1st, 2nd etc.
    }

    public void InitialText()
    {
        if(position <= GameController.instance.numberOfBots+1)
        {
            textPosition.text = "WAITING...";
            textPosition.color = waitingColor;
            textName.text = "";
            textScore.text = "";
        }
        else
        {
            textPosition.text = "";
            textName.text = "";
            textScore.text = "";
        }
    }

    public void SetPosition(string name, string score)
    {
        if(position <= GameController.instance.numberOfBots+1)  
        {
            if (score == "0")
                return;

            textPosition.text = positionText;
            textName.text = name;
            textScore.text = score.ToString() + "x";

            textPosition.color = color;
            textName.color = color;
            textScore.color = color;
        }
    }

}
