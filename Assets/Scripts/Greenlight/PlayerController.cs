using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using Scorch_SceneObject;
using Scorch_Keybindings;


public class PlayerController : MonoBehaviour, VA_Observer
{
    [SerializeField]
    UIController uicontroller;

    [SerializeField]
    private HandsAnimationBehavior hands;

    private FirstPersonController fpsController;
    private KeyBinding keybindings;

    [SerializeField]
    private List<Evidence> importantEvidenceList; //required to solve the case

    private List<Evidence> evidenceList;
    private List<Interactable> interactableList; //contains items such as keys
    private List<Mushroom> mushroomList;

    private Transform camTransform;

    private bool paused = false;
    private bool memoboardOpen = false; // is the phone open?
    private bool canStartFinalCall = false; // can you call Gonzales?
    private bool gonzalesCanStartFinalCall = false; //can Gonzales call you?
    private bool answeredAllQuestions = false; //used to determine when the player has finished


    //For the observer pattern
    private bool callState = false;
    private bool questState = false;
    private bool amState = false;
    private bool commState = false;
    private bool songState = false;
    private bool subtitles = false;
    private string language;

    private int importantEvidenceFound;
    private const float interactionDistance = 2.0f; //was 2.5f before

    [SerializeField]
    private AudioManager audioManager;


    private bool crouching = false; // is the player currently crouching
    private Crouching crouch;

    TimeTrigger trigger;
    private VManager vManager;

    // The following is required to check the "vibrieren" audioSource
    [SerializeField]
    private Conversation conversation; //Bad OOD

    // Use this for initialization
    void Start()
    {
        evidenceList = new List<Evidence>();
        interactableList = new List<Interactable>();
        mushroomList = new List<Mushroom>();

        camTransform = transform.GetChild(0);
        fpsController = GetComponent<FirstPersonController>();
        keybindings = GetComponent<KeyBinding>();
        audioManager = FindObjectOfType<AudioManager>();
        crouch = FindObjectOfType<Crouching>();
        trigger = FindObjectOfType<TimeTrigger>();

        Camera.main.fieldOfView = PlayerPrefs.GetFloat("FOV", 60f);

        vManager = SceneObject.VManager;

        //Starts the cutscene right at the beginning of the game
        Invoke("StartCutscene", 0.01f);

        //Updates the mouse sensitivity using PlayerPrefs
        fpsController.m_MouseLook.XSensitivity = PlayerPrefs.GetFloat("MouseSens", 2f);
        fpsController.m_MouseLook.YSensitivity = PlayerPrefs.GetFloat("MouseSens", 2f);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug
        if (Input.GetKeyDown(KeyCode.T))
        {
            AllowMouseLook(true);
        }
        
        
        //Pausing the game
        if (Input.GetKeyDown(keybindings.pauseKey) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (uicontroller.IsUsingComputer()) //Hide the computer and emails screen
            {
                uicontroller.HideComputer();
                AllowMouseLook(true);
            }
            else if (uicontroller.IsInCloseup()) //Exit Closeup
            {
                uicontroller.HideCloseup();
            }
            else if (memoboardOpen && !uicontroller.IsInConnectMode()) //Exit Phone, removed because it caused unsolvable problems
            {
                //uicontroller.ReturnToPreviousMode();
                //ReturnToGame();
            }
            else //Pause Game
            {
                if (!paused && !answeredAllQuestions) //only pausing not unpausing, causing similar unsolvable problems as with phone
                {
                    HandlePause();
                }
            }
        }


        //Opening the Memoboard
        if (Input.GetKeyDown(keybindings.memoKey) || Input.GetKeyDown(KeyCode.Tab))
        {
            uicontroller.SetUIHintVisibility(false);

            if (uicontroller.IsInCloseup()) //Hide any closeups
            {
                uicontroller.HideCloseup();
            }
            else if (memoboardOpen == false) // Open the memoboard
            {
                if (!answeredAllQuestions) { //Can't open the memoboard at the end of the game anymore
                    uicontroller.ShowInventory();
                    memoboardOpen = true;

                    //Disable mouse look
                    AllowMouseLook(false);
                }
            }
            else if (!uicontroller.IsInConnectMode()) // Close the memoboard
            {
                uicontroller.ReturnToPreviousMode();
                ReturnToGame();
            }
        }

        //Submit password
        if (Input.GetKeyDown(keybindings.confirmKey) || Input.GetKeyDown(KeyCode.Return))
        {
            uicontroller.ShowEmailsIfPasswordIsCorrect();
            Debug.Log("The current call state is " + vManager.GetCallState().ToString());
            Debug.Log("Vibrieren playing is " + conversation.vibrieren.isPlaying.ToString());
        }

        uicontroller.SetCursorHighlight(false); //bad ugly code
        uicontroller.SetCursorColor(false); // change color to white again



        // if (!memoboardIsOpen)
        if (!memoboardOpen)
        {
            //The player's ability to crouch
            if ((Input.GetKeyDown(keybindings.crouchKey) || Input.GetKeyDown(KeyCode.LeftControl) && !crouching))
            {
                fpsController.crouching = true;
                crouching = true;
                crouch.Crouch(crouching);
            }
            if ((Input.GetKeyUp(keybindings.crouchKey) || Input.GetKeyUp(KeyCode.LeftControl) && crouching))
            {
                fpsController.crouching = false;
                crouching = false;
                crouch.Crouch(crouching);
            }


            /// <summary>
            /// This section handles a raycast used for picking up evidence and interacting with the world
            /// </summary> 
            RaycastHit hit;
            Debug.DrawRay(camTransform.position, camTransform.forward * interactionDistance);

            if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, interactionDistance))
            {
                //Is it evidence?
                if (hit.collider.gameObject.GetComponent<Evidence>() != null)
                {
                    if (hit.collider.gameObject.GetComponent<Evidence>().GetFound() == false)
                    {
                        //Change the cursor
                        uicontroller.SetCursorHighlight(true);
                        uicontroller.SetCursorColor(true);

                        //Debug.Log("I see an object we can pick up");

                        if (Input.GetKeyDown(keybindings.interactKey) || Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
                        { //Pick it up
                            Debug.Log("New Evidence has been found and added");

                            Evidence newEvidence = hit.collider.gameObject.GetComponent<Evidence>();

                            evidenceList.Add(newEvidence); //evidenceList.Add(hit.collider.gameObject);


                            //Add it to the memo board
                            uicontroller.AddNewPieceOfEvidenceToMemoboard(newEvidence); //error

                            //show UI text
                            if (newEvidence.GetShouldPopup() == false)
                            {
                                Debug.Log("New Evidence's name is " + newEvidence.GetEvidenceName());
                                uicontroller.ShowEvidenceText(newEvidence.GetEvidenceName(), newEvidence.GetEvidenceHint());
                            }

                            //Show UI Tooltip, if it is the first piece of evidence
                            if (evidenceList.Count == 1)
                            {
                                uicontroller.SetUIHintVisibility(true);
                                uicontroller.UpdateUIHint("Press Tab");
                            }


                            //show an UI Close up if it has a close up sprite and should pop-up
                            if (newEvidence.GetShouldPopup())
                            {
                                uicontroller.HandleCloseup(newEvidence);
                            }
                            

                            //hit.collider.gameObject.SetActive(false); //deactivate the evidence object in the world
                            StartCoroutine(SetGameObjectInactive(hit.collider.gameObject, 0.4f));
                            hit.collider.gameObject.GetComponent<Evidence>().SetFound(true);

                            //have we found all the important evidence? (when you are prepared and won't get stuck on questions)
                            //Show UI Hint that you can call Gonzales (when you are prepared and won't get stuck on questions)
                            if (CountImportantEvidenceFound() == importantEvidenceList.Count && evidenceList.Count >= 12) //should be changed to evidenceList.Contains(important evidence)
                            {
                                Debug.Log("Gonzales can call you now");
                                uicontroller.SetPhoneVisibility(true);
                                uicontroller.SetPenVisibility(true);
                                //canStartFinalCall = true;
                                gonzalesCanStartFinalCall = true;
                                //conversation.StartCoroutine(conversation.PlayVoiceActing());

                                //pasted
                                uicontroller.SetUIHintVisibility(true);
                                uicontroller.UpdateUIHint("Call Gonzales from your phone when you're ready");
                                uicontroller.SetCallIconState(true);
                            }

                            /*//Show UI Hint that you can call Gonzales (when you are prepared and won't get stuck on questions)
                            if (CountImportantEvidenceFound() == importantEvidenceList.Count && evidenceList.Count >= 12)
                            {

                            }*/

                            //Hands animations
                            hands.PlayGrabAnimation();

                        }
                    }
                }
                else if (hit.collider.gameObject.GetComponent<Interactable>() != null)
                //is it an interactable?
                {
                    //Change the cursor
                    uicontroller.SetCursorHighlight(true);
                    //uicontroller.SetCursorColor(false);
                    if (hit.collider.gameObject.GetComponent<Interactable>().ShouldChangeCursorColor())
                    {
                        uicontroller.SetCursorColor(true);
                    }

                    if (Input.GetKeyDown(keybindings.interactKey) || Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
                    {
                        Debug.Log("The player has found an interactable");
                        hit.collider.gameObject.GetComponent<Interactable>().Interact(hit);


                        //Hands animations: key is picked up
                        if (hit.collider.gameObject.GetComponent<Interactable>().IsAPickup() || hit.collider.gameObject.GetComponent<Interactable>().IsALightswitch())
                        {
                            hands.PlayGrabAnimation();
                        }
                        else if (!hit.collider.gameObject.GetComponent<Interactable>().IsAReactable()) //if not a reactable such as the map
                        {
                            hands.PlayInteractAnimation();
                        }
                    }
                }
                else if (hit.collider.gameObject.GetComponent<Mushroom>() != null)
                //is it a mushroom?
                {
                    Mushroom mushroom = hit.collider.gameObject.GetComponent<Mushroom>();
                    //Change the cursor
                    uicontroller.SetCursorHighlight(true);
                    uicontroller.SetCursorColor(true);
                    if (Input.GetKeyDown(keybindings.interactKey) || Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
                    {
                        Debug.Log("The player has found a mushroom");
                        uicontroller.ShowEvidenceText(mushroom.GetMushroomName(), mushroom.GetMushroomDescription());

                        hit.collider.gameObject.SetActive(false); //deactivate the mushroom

                        //Hands animations
                        hands.PlayGrabAnimation();
                    }

                }
                else if (hit.collider.gameObject.GetComponent<TimeTrigger>() != null)
                {
                    trigger.SetRayCastHit(hit);
                }
                if ((hit.collider.gameObject.GetComponent<Interactables>() != null))
                {
                    if (Input.GetKeyDown(keybindings.interactKey) || Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
                    { //Pick it up 
                        audioManager.assignPickUpSound(hit);

                    }
                }
            }

        }

    }

    public List<Evidence> GetEvidenceList()
    {
        return evidenceList;
    }

    public Evidence GetEvidenceLatest()
    {
        return evidenceList[evidenceList.Count - 1];//Added a -1
    }

    //Count how much important evidence the player has collected
    private int CountImportantEvidenceFound()
    {
        int num = 0;

        foreach (Evidence evidenceElement in evidenceList)
        {
            foreach (Evidence importantEvidenceElement in importantEvidenceList)
            {
                if (evidenceElement.GetEvidenceName() == importantEvidenceElement.GetEvidenceName()) //(selectedEvidenceElement.Equals(correctEvidenceElement)
                {
                    num++;
                }
            }
        }

        Debug.Log(num + " important pieces of evidence found");
        return num;
    }

    //Can Gonzales Start the Final Call with the player
    public bool GetGonzalesCanStartFinalCall()
    {
        return gonzalesCanStartFinalCall;
    }



    public List<Interactable> GetInteractableList()
    {
        return interactableList;
    }

    public void AddToInteractableList(Interactable interactable)
    {
        interactableList.Add(interactable);
        Debug.Log("Something was picked up as an interactable");
    }

    public void AddToMushroomList(Mushroom mushroom)
    {
        mushroomList.Add(mushroom);
    }

    public void AllowMouseLook(bool enabled)
    {
        fpsController.enabled = enabled;
        Cursor.visible = !enabled;
        Cursor.lockState = enabled ? CursorLockMode.Locked : CursorLockMode.None;
    }

    //Without parameters so that it can be invoked
    private void RefocusMouse()
    {
        AllowMouseLook(true);
    }

    public void HandlePause()
    {
        if (paused) //return to the game
        {
            if (!uicontroller.IsInConnectMode())
            {
                ReturnToGame();
                AllowMouseLook(true);
                Invoke("RefocusMouse", 0.1f); //workaround
            }

            uicontroller.HidePauseMenu();
            paused = false;
            Time.timeScale = 1f;

            AudioListener.pause = false;
            audioManager.SetPauseSong(false);
        }
        else
        {
            paused = true;
            uicontroller.ShowPauseMenu();
            AllowMouseLook(false);
            Time.timeScale = 0f;

            AudioListener.pause = true;
            audioManager.SetPauseSong(true);
        }
    }
    public void ReturnToGame()
    {
        uicontroller.HideInventory();
        memoboardOpen = false;

        AllowMouseLook(true);
    }

    public void SetMemoboardOpen(bool set)
    {
        memoboardOpen = set;
    }

    public bool getMemoboardOpen()
    {
        return memoboardOpen;
    }

    // Called by the UIController
    public void SetAnsweredAllQuestions (bool set)
    {
        answeredAllQuestions = set;
    }

    //For Handling Cutscenes
    //This is called at the beginning of the game
    public void StartCutscene()
    {
        Debug.Log("Started the cutscene");
        uicontroller.StartShowingBars();
        uicontroller.SetCursorVisibility(false);

        AllowMouseLook(true); //was false

        //Make sure that the player can't turn around
        fpsController.m_MouseLook.XSensitivity = 0f;
        fpsController.m_MouseLook.YSensitivity = 0f;

        //Invoke("EndCutscene", 5f);

    }


    //Called by the Scene Object Namespace
    public void EndCutscene()
    {
        Debug.Log("Ended the cutscene");
        uicontroller.StartHidingBars();
        uicontroller.SetCursorVisibility(true);

        AllowMouseLook(true);
        Animator animator = gameObject.GetComponent<Animator>();
        //animator.SetBool("Cutscene", false);
        animator.enabled = false;
        Cursor.visible = false;
        conversation.walkingAudioSource.enabled = false;

        //Reset the mouse sensitivity
        fpsController.m_MouseLook.XSensitivity = PlayerPrefs.GetFloat("MouseSens", 2f);
        fpsController.m_MouseLook.YSensitivity = PlayerPrefs.GetFloat("MouseSens", 2f);

        //Make sure the player is looking in the right direction
        transform.rotation = Quaternion.Euler(0, 0, 0);


        gameObject.AddComponent<Animator>();
    }

    //For the voice acting observer pattern
    public void Refresh(bool callState, bool questState, bool amState, bool commState, bool songState, bool subtitles, string language)
    {
        this.callState = callState;
        this.questState = questState;
        this.amState = amState;
        this.commState = commState;
        this.songState = songState;
        this.subtitles = subtitles;
        this.language = language;
    }

    //Needs to be a coroutine to be cleanly called with a delay: used to hide evidence after it is picked up
    IEnumerator SetGameObjectInactive(GameObject go, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        go.SetActive(false);
    }
}