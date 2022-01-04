using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// wing collect script
// when object is triggered by the character
// object is deactivated then waits for some time, activates again
// wait time for the wing collect should be taken from a central game controller

public class WingCollect : MonoBehaviour
{
    private float updateTime = 1f;
    private bool isAvailable = true;
    private float timeRemaining;

    // parameters for activate and deactivate
    private BoxCollider collider;
    private MeshRenderer[] meshRenderers;


    // Start is called before the first frame update
    void Start()
    {
        updateTime = GameController.instance.wingCollectTime;
        collider = GetComponent<BoxCollider>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        timeRemaining = updateTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(isAvailable == false)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0f)
            {
                isAvailable = true;
                ActivateWing();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isAvailable == false)
            return;

        if(other.tag == "Player")
        {
            Debug.Log("Wings collected!!");
            timeRemaining = updateTime;
            isAvailable = false;
            DeactivateWing();

            // give +3 wings to character with an text animation and particle effect
            CharController.wingCount += 3;
        }
    }

    void DeactivateWing()
    {
        collider.enabled = false;
        foreach (MeshRenderer m in meshRenderers)
            m.enabled = false;
    }

    void ActivateWing()
    {
        collider.enabled = true;
        foreach (MeshRenderer m in meshRenderers)
            m.enabled = true;
    }
}
