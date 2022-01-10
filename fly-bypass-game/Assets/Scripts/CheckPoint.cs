using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CheckPoint : MonoBehaviour
{
    private Transform pointT;
    
    void Start()
    {
        pointT = GetComponentsInChildren<Transform>(true).Where(x => x.gameObject.name == "point").SingleOrDefault();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" || other.tag =="Bot")
        {
            other.gameObject.GetComponent<CharController>().CharacterCheckPoint(pointT.position);
        }
    }
}
