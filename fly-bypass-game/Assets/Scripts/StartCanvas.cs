using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCanvas : MonoBehaviour
{
    public void StartGameAfterAnim()
    {
        GameController.instance.ResumeGame();
        StartCoroutine(WaitAndDeactivate());
    }

    IEnumerator WaitAndDeactivate()
    {
        yield return new WaitForSeconds(.5f);
        transform.gameObject.SetActive(false);
    }
}
