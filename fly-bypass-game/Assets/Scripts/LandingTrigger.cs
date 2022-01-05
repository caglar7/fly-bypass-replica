using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (GameController.instance.isLandingAvailable == true)
                return;

            GameController.instance.isLandingAvailable = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            if (GameController.instance.isLandingAvailable == false)
                return;

            GameController.instance.isLandingAvailable = false;
        }
    }
}
