using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DP_PlayerController : MonoBehaviour {
    [SerializeField]
    DP_UI uicontroller;

    [SerializeField]
    private float interactionDistance = 3.0f;


    private List<GameObject> evidenceList;
    private Transform camTransform;

    // Use this for initialization
    void Start () {
        evidenceList = new List<GameObject>();
        camTransform = transform.GetChild(0);
    }
	
	// Update is called once per frame
	void Update () {
        //Opening the Inventory
        if (Input.GetKeyDown(KeyCode.Tab)) {
            uicontroller.ShowInventory(); //
        }

        if (Input.GetKeyUp(KeyCode.Tab)) {
            uicontroller.HideInventory();
        }


        //Calling Gonzales
        if (Input.GetKeyDown(KeyCode.Return)) {
            uicontroller.ShowInventory();
            uicontroller.ChangeInventoryText("What was the murder weapon");
            uicontroller.DisableFPSController();
        }


        //Picking up Evidence
        RaycastHit hit;

        Debug.DrawRay(camTransform.position, camTransform.forward * interactionDistance);

        if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, interactionDistance)) {
            if (hit.collider.gameObject.GetComponent<DP_Evidence>() != null) { //Is it evidence
                //Debug.Log("I see an object we can pick up");

                if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)) { //Pick it up
                    Debug.Log("New Evidence has been found and added");

                    evidenceList.Add(hit.collider.gameObject);

                    //Add it to the inventory
                    //uicontroller.UpdateInventory(hit.collider.gameObject.GetComponent<DP_Evidence>().GetEvidenceSprite());
                    uicontroller.UpdateInventory();

                    //show UI text
                    Debug.Log("New Evidence's name is " + hit.collider.gameObject.GetComponent<DP_Evidence>().GetEvidenceName());
                    uicontroller.ShowEvidenceText(hit.collider.gameObject.GetComponent<DP_Evidence>().GetEvidenceName());

                    hit.collider.gameObject.SetActive(false); //disable the evidence object
                }
            }
        }
    }

    public List<GameObject> GetEvidenceList() {
        return evidenceList;
    }

    public GameObject GetEvidenceLatest() {
        return evidenceList[evidenceList.Count - 1];//Added a -1
    }
}
