using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class DP_UI : MonoBehaviour {
    [SerializeField]
    DP_PlayerController playerController;

    [SerializeField]
    Text evidenceText, inventoryText;

    [SerializeField]
    CanvasGroup canvasGroup;

    [SerializeField]
    CanvasGroup inventoryPanel;

    private int numOfItemsInInventory;

    [SerializeField]
    private List<Image> imageList;

    [SerializeField]
    FirstPersonController fpsController;


    // Use this for initialization
    void Start () {
        //imageList = new List<Image>();

        canvasGroup.alpha = 0.0f;
        numOfItemsInInventory = 0;

        HideInventory();

        //firstPersonController = GetComponent<Light>();
    }
	
	// Update is called once per frame
	void Update () {

	}

    public void ShowEvidenceText(string newText) {
        canvasGroup.alpha = 1;
        evidenceText.text = newText;

        Invoke("HideEvidenceText", 2.0f); //Hides the text again after two seconds
    }

    private void HideEvidenceText() {
        canvasGroup.alpha = 0;
    }


    public void ShowInventory() {
        Debug.Log("Inventory should have been shown");
        inventoryPanel.alpha = 1.0f;
    }

    public void UpdateInventory() {
        //imageList[numOfItemsInInventory].sprite = newSprite; //
        imageList[numOfItemsInInventory].sprite = playerController.GetEvidenceLatest().GetComponent<DP_Evidence>().GetEvidenceSprite(); //
        numOfItemsInInventory++;

        Debug.Log("There are now " + numOfItemsInInventory + " in your inventory");
    }

    public void HideInventory() {
        Debug.Log("Inventory should have been hidden");
        inventoryPanel.alpha = 0.0f;
    }

    public void ChangeInventoryText(string question) {
        Cursor.lockState = CursorLockMode.None;
        inventoryText.text = question;
    }

    public void TestEvidence(int i) { //test 1, 2, 3, or 4
        Debug.Log("Beep beep I am testing the evidence");

        if (playerController.GetEvidenceList()[i].GetComponent<DP_Evidence>().GetEvidenceName() == "Matches") {
            inventoryText.text = "The matches were used!";
        } else {
            inventoryText.text = "I don't think so Gary.";
        }

        Debug.Log("These are " + playerController.GetEvidenceList()[i].GetComponent<DP_Evidence>().GetEvidenceName());
    }

    public void DisableFPSController() {
        //Disable first person controller
        fpsController.enabled = false;
    }
}
