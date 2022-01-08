using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" || other.tag == "Bot")
        {
            if (GameController.instance.GetLandingValue(other.gameObject.name) == true)
                return;

            GameController.instance.SetLandingValue(other.gameObject.name, true);
            //Debug.Log(other.gameObject.name + " landing value: true");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" || other.tag == "Bot")
        {
            if (GameController.instance.GetLandingValue(other.gameObject.name) == false)
                return;

            GameController.instance.SetLandingValue(other.gameObject.name, false);
            //Debug.Log(other.gameObject.name + " landing value: false");
        }
    }

}
