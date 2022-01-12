using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class TouchController : MonoBehaviour
{
    private float prevX;
    public static float touchMoveDirection = 0f;
    private float xThreshHold = 4f;
    private bool isFirstTouch = true;
    private bool initParameters = false;

    void Start()
    {
        prevX = Screen.width / 2;
    }

    void Update()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if(isFirstTouch == false)
            {
                float diff = touch.position.x - prevX;
                touchMoveDirection = (diff >= xThreshHold) ? 1f : touchMoveDirection;
                touchMoveDirection = (diff <= -xThreshHold) ? -1f : touchMoveDirection;
                touchMoveDirection = (Mathf.Abs(diff) < xThreshHold) ? 0f : touchMoveDirection;
            }

            prevX = touch.position.x;
            isFirstTouch = false;
            initParameters = false;
        }
        else
        {
            if(initParameters == false)
            {
                initParameters = true;
                Init();
            }
        }
    }

    private void Init()
    {
        touchMoveDirection = 0f;
        isFirstTouch = true;
    }
}
