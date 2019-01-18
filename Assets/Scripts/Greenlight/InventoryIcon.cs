using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryIcon : MonoBehaviour {
    private UIController uicontroller;

    private Evidence evidence; //the copied evidence component

    //UI fields
    private RectTransform boundsRectTransform;
    public RectTransform snapbackTransform;
    private RectTransform pickupTransform;
    private RectTransform rectTransform;
    private Text evidenceNameText;
    private Image iconImage, iconHighlight;

    //logic fields
    private bool pickedUp = false; //is the icon being moved
    private bool selected = false; //is this icon selected as a piece of evidence for the question

    //time fields (used to determine if it was a click)
    float clickTime;

    

	// Use this for initialization
	void Start () {
        uicontroller = GameObject.FindWithTag("UIController").GetComponent<UIController>();
        boundsRectTransform = GameObject.FindWithTag("RectBounds").GetComponent<RectTransform>();

        //rectTransform = GetComponent<RectTransform>();
        ClampPositionToBounds(); //asserts that the evidence starts inside the phone

        evidence = GetComponent<Evidence>(); 

        evidenceNameText = transform.GetChild(0).gameObject.GetComponent<Text>();
        evidenceNameText.text = evidence.GetEvidenceName();
        evidenceNameText.gameObject.SetActive(false);

        iconImage = transform.GetChild(1).gameObject.GetComponent<Image>();
        iconImage.sprite = evidence.GetEvidenceSprite();

        iconHighlight = GetComponent<Image>();
        iconHighlight.enabled = false;

        clickTime = Time.time;

        snapbackTransform = GetComponent<RectTransform>();
        //iconPin = transform.GetChild(2).gameObject.GetComponent<Image>();
    }
	
	// Update is called once per frame
	void Update () {
		if (pickedUp) {
            transform.position = Input.mousePosition + new Vector3(0, 0, 0); //needs the Canvas to be in ScreenSpaceOverlay, had Y offset with pin
            //transform.position = new Vector3(Mathf.Clamp(Input.mousePosition.x, -400f + 640, 400f + 640), Mathf.Clamp(Input.mousePosition.y, -250f + 360, 250f + 360), 0); //400 and 250

            ClampPositionToBounds();

            //maybe use boundsRectTransform.rect.x instead

        }

        //deselect and remove the highlight when not in connect mode
        if (!uicontroller.IsInConnectMode()) {
            if (selected) {
                ToggleSelect();
            }
        }

        //Hide or show the pins depending on the current mode
        //SetPinVisibility(!uicontroller.IsInConnectMode());

	}

    public void ShowText() {
        evidenceNameText.gameObject.SetActive(true);
    }

    public void HideText() {
        evidenceNameText.gameObject.SetActive(false);
    }

    public void Pickup() {
        //Check if we are in rganization mode first?
        pickedUp = true;
        clickTime = Time.time;

        pickupTransform = rectTransform;

        //Set snapback transform
        snapbackTransform = GetComponent<RectTransform>();

        //deselect it to disable the highlight
        Deselect();

        //Move the object to the top of the local hierarchy so that it is drawn above the others
        transform.SetAsLastSibling();
    }

    public void LetGo() {
        pickedUp = false;
        //See if it is above or near a thing
        if (uicontroller.SnapTransformIfNearSelector(transform)) {
            Debug.Log("inv icon should be selected");
            if (!selected) {
                ToggleSelect();
            }
        } else {
            Deselect();
            Debug.Log("inv icon should be deselected");
        }
    }

    public void ParseClick() { //Make inspect mode accesible later on even from connect mode
        /*if (uicontroller.IsInConnectMode()) {
            //ToggleSelect();
            Debug.Log("InventoryIcon click parsed in connect mode");
        } else*/ {
            Debug.Log("InventoryIcon click parsed outside connect mode");
            if (Time.time - clickTime < 0.12f) {
            //if (Vector2.Distance(pickupTransform.sizeDelta, rectTransform.sizeDelta) < 10) {
                Debug.Log("Seems like a click");
                //Enter Inspect Mode
                //Update the picture with this invIcons picture
                uicontroller.EnterInspectMode();
                uicontroller.UpdateInspectModeContent(evidence);
            }
        }
    }

    private void ToggleSelect() {
        selected = !selected;
        //Toggle the highlight
        iconHighlight.enabled = selected;

        //Add or remove from the selected evidence list
        if (selected) { //add to selected evidence list
            uicontroller.AddToSelectedEvidence(evidence);

            //Play Select Sound
            uicontroller.uisounds.AssignUIAudio(transform.gameObject);

        } else { //remove from selected evidence list
            uicontroller.RemoveFromSelectedEvidence(evidence);

            //Play Deselect Sound
            //uisound.PlaySoundOpposite();
            uicontroller.uisounds.AssignUIAudio(transform.gameObject);
        }
    }

    public void Deselect() { //used for external calling
        if (selected) {
            ToggleSelect();
        }
    }

    public bool GetSelected() {
        return selected;
    }

    private void ClampPositionToBounds() {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, boundsRectTransform.position.x - boundsRectTransform.rect.width / 2, boundsRectTransform.position.x + boundsRectTransform.rect.width / 2), Mathf.Clamp(transform.position.y, boundsRectTransform.position.y - boundsRectTransform.rect.height / 2, boundsRectTransform.position.y + boundsRectTransform.rect.height / 2), 0);
        //transform.position = new Vector3(Mathf.Clamp(transform.position.x, boundsRectTransform.rect.x - boundsRectTransform.rect.width / 2, boundsRectTransform.rect.x + boundsRectTransform.rect.width / 2), Mathf.Clamp(transform.position.y, boundsRectTransform.rect.y - boundsRectTransform.rect.height / 2, boundsRectTransform.rect.y + boundsRectTransform.rect.height / 2), 0);
    }
    /*private void SetPinVisibility(bool visible) {
        iconPin.gameObject.SetActive(visible);
    }*/

    public void GoToSnapback() {
        rectTransform = snapbackTransform;
    }
}
