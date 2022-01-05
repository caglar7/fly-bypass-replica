using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class WingController : MonoBehaviour
{
    [SerializeField] private GameObject wingPrefab;

    private List<Transform> leftWingsTList;
    private List<GameObject> leftWingObjects = new List<GameObject>(); 

    private List<Transform> rightWingsTList;
    private List<GameObject> rightWingObjects = new List<GameObject>();

    void Start()
    {
        // find transforms for each wing side, 2 parent objects for wings
        Transform leftWingsTransform = GetComponentsInChildren<Transform>(true).Where(x => x.gameObject.name == "LeftWings").SingleOrDefault();
        Transform rightWingsTransform = GetComponentsInChildren<Transform>(true).Where(x => x.gameObject.name == "RightWings").SingleOrDefault();
        if(leftWingsTransform == null || rightWingsTransform == null)
            Debug.Log("LeftWings or RightWings Object null");


        // get transforms for both sides, these store the position for each indiv. wing
        leftWingsTList = leftWingsTransform.gameObject.GetComponentsInChildren<Transform>(true).ToList();
        rightWingsTList = rightWingsTransform.gameObject.GetComponentsInChildren<Transform>(true).ToList();
        leftWingsTList.RemoveAt(0);
        rightWingsTList.RemoveAt(0);
    }

    public void OpenWings()
    {
        // check wing count first
        if (CharController.wingCount < 2)
            return;

        int showCount = 0;
        int instantiateCount = 0;
        showCount += leftWingObjects.Count;
        instantiateCount += (CharController.wingCount - (leftWingObjects.Count * 2)) / 2;

        // first show the ones that are already created
        for(int i=0; i<leftWingObjects.Count; i++)
        {
            leftWingObjects[i].GetComponent<MeshRenderer>().enabled = true;
            rightWingObjects[i].GetComponent<MeshRenderer>().enabled = true;
        }
        // instantiate more wings, add them to the object list
        for(int i=0; i<instantiateCount; i++)
        {
            // spawn left wing
            GameObject wing1 = Instantiate(wingPrefab, leftWingsTList[showCount + i]);
            wing1.transform.localPosition = Vector3.zero;
            leftWingObjects.Add(wing1);
            
            // spawn right wing
            GameObject wing2 = Instantiate(wingPrefab, rightWingsTList[showCount + i]);
            wing2.transform.localPosition = Vector3.zero;
            rightWingObjects.Add(wing2);
        }

        Debug.Log("WINGS OPENING");
        Debug.Log("Wing Count: " + CharController.wingCount);
        Debug.Log(leftWingObjects.Count + "Pairs");

        Debug.Log("COUNTS");
        Debug.Log("Show Count: " + showCount);
        Debug.Log("Insta Count: " + instantiateCount);


    }

    public void LoseWings()
    {
        Debug.Log("Wing count in wing controlelr: " + CharController.wingCount);

        // check if possible
        if (CharController.wingCount < 2)
            return;

        // remove the wings on last index
        int index = leftWingObjects.Count - 1;

        // left wing
        GameObject wing1 = leftWingObjects[index];
        leftWingObjects.RemoveAt(index);
        wing1.AddComponent<Rigidbody>();
        Destroy(wing1, 1f);

        // right wing
        GameObject wing2 = rightWingObjects[index];
        rightWingObjects.RemoveAt(index);
        wing2.AddComponent<Rigidbody>();
        Destroy(wing2, 1f);
    }

    public void CloseWings()
    {
        for (int i = 0; i < leftWingObjects.Count; i++)
        {
            leftWingObjects[i].GetComponent<MeshRenderer>().enabled = false;
            rightWingObjects[i].GetComponent<MeshRenderer>().enabled = false;
        }
    }

}
