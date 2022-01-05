using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class WingController : MonoBehaviour
{
    [SerializeField] private GameObject wingPrefab;

    private Transform mainLeftT, mainRightT;

    private List<Transform> leftWingsTList;
    private List<GameObject> leftWingObjects = new List<GameObject>(); 

    private List<Transform> rightWingsTList;
    private List<GameObject> rightWingObjects = new List<GameObject>();

    private float wingDestroyTime = .35f;

    void Start()
    {
        // find transforms for each wing side, 2 parent objects for wings
        Transform leftWingsTransform = GetComponentsInChildren<Transform>(true).Where(x => x.gameObject.name == "LeftWings").SingleOrDefault();
        Transform rightWingsTransform = GetComponentsInChildren<Transform>(true).Where(x => x.gameObject.name == "RightWings").SingleOrDefault();
        mainLeftT = GetComponentsInChildren<Transform>(true).Where(x => x.gameObject.name == "MainWingLeft").SingleOrDefault();
        mainRightT = GetComponentsInChildren<Transform>(true).Where(x => x.gameObject.name == "MainWingRight").SingleOrDefault();
        if (leftWingsTransform == null || rightWingsTransform == null)
            Debug.Log("LeftWings or RightWings Object null");
        if (mainLeftT == null || mainRightT == null)
            Debug.Log("Main wings one or two null");


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
        instantiateCount += ((CharController.wingCount -2) - (leftWingObjects.Count * 2)) / 2;

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
    }

    public void LoseWings()
    {
        if (leftWingObjects.Count == 0)
            return;

        int index = leftWingObjects.Count - 1;

        // left wing
        GameObject wing1 = leftWingObjects[index];
        leftWingObjects.RemoveAt(index);
        wing1.AddComponent<Rigidbody>();
        Destroy(wing1, wingDestroyTime);

        // right wing
        GameObject wing2 = rightWingObjects[index];
        rightWingObjects.RemoveAt(index);
        wing2.AddComponent<Rigidbody>();
        Destroy(wing2, wingDestroyTime);
    }

    public void CloseWings()
    {
        for (int i = 0; i < leftWingObjects.Count; i++)
        {
            leftWingObjects[i].GetComponent<MeshRenderer>().enabled = false;
            rightWingObjects[i].GetComponent<MeshRenderer>().enabled = false;
        }
    }

    public void ShowMainWings()
    {
        mainLeftT.gameObject.GetComponent<MeshRenderer>().enabled = true;
        mainRightT.gameObject.GetComponent<MeshRenderer>().enabled = true;
    }

    public void LoseMainWings()
    {
        // disable renderer, later might be with animation and effect
        mainLeftT.gameObject.GetComponent<MeshRenderer>().enabled = false;
        mainRightT.gameObject.GetComponent<MeshRenderer>().enabled = false;
    }
}
