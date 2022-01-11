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
                ActivateCollectWing();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isAvailable == false)
            return;

        if(other.tag == "Player" || other.tag == "Bot")
        {
            // if no collect wings on back, place them, happens only at first
            if(GameController.instance.GetMainWingsValue(other.gameObject.name) == false)
            {
                GameController.instance.SetMainWingsValue(other.gameObject.name, true);
                other.gameObject.GetComponent<CharController>().ShowCollectWings();
            }

            timeRemaining = updateTime;
            isAvailable = false;
            DeactivateCollectWing();

            // give +3 wings to character with an text animation and particle effect
            other.gameObject.GetComponent<CharController>().IncreaseWings(3);
        }
    }

    void DeactivateCollectWing()
    {
        collider.enabled = false;
        foreach (MeshRenderer m in meshRenderers)
            m.enabled = false;
    }

    void ActivateCollectWing()
    {
        collider.enabled = true;
        foreach (MeshRenderer m in meshRenderers)
            m.enabled = true;
    }

    private bool GetMainWingsValue(string name)
    {
        bool value;
        if (GameController.instance.mainWingsOnBack.TryGetValue(name, out value))
        {
            // success
            return value;
        }
        else
        {
            // failure
            Debug.Log("Couldnt get isLandingAvailable value from dictionary, returning false");
            return false;
        }
    }

    private void SetMainWingsValue(string name, bool value)
    {
        GameController.instance.mainWingsOnBack[name] = value;
    }
}
