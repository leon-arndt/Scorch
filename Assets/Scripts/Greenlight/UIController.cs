using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using UnityEngine.SceneManagement;
using Scorch_SceneObject;

public class UIController : MonoBehaviour
{
    private enum UIMode { Organize, Inspect, Connect };
    private UIMode uimode;
    private UIMode uimodePrevious;

    private enum MurderSuspect { Susan, Jason, Other }; //needs to be refactored : make more abstract
    private MurderSuspect murderSuspect;

    private enum Resources { AutopsyReport, AlcoholProblem };
    private Resources resources;


    //private enum GonzalesExpression { Normal, Happy, Concerned, Angry }; //needs to be refactored : make more abstract
    //private GonzalesExpression gonzalesExpression;

    //private enum UIHints { OpenInventory, AnswerCalls, InspectEvidence, ConnectEvidence};
    //private UIHints uihints;

    private int currentQuestion = 0;
    private int questionsAnsweredCorrectly = 0; //replace later (bad ugly code)

    private List<Evidence> selectedEvidence;
    private List<Image> inventoryIconList; //quite bulky (bad ugly code) ?
    private List<Image> disabledInventoryIconList; //quite bulky (bad ugly code) ?

    [SerializeField]
    private PlayerController playerController; //needs to be refactored : inheret reference

    [SerializeField]
    private UILineConnector uiLineConnector;

    [SerializeField]
    private EndScreenData endScreenData;

    [SerializeField]
    public UISounds uisounds;

    [SerializeField]
    private List<Question> questionsList;

    [SerializeField]
    private List<Image> selectorsList;

    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private GameObject selectorGroupForTwo, selectorGroupForThree, computer, loginScreen, emailsScreen, inputMurdererNameScreen, endScoreScreen, areYouReadyScreen, pauseScreen, optionsScreen, rebindScreen, introScreen, resourceScreen;

    [SerializeField]
    private Text evidenceText, evidenceHintText, uiHint, inventoryText, inspectModeText, inspectModeHintText, emailContents, wrongPasswordText, tryAgainText, resourceContentText, resourceNameText;

    [SerializeField]
    private InputField loginInputField, murdererNameInputField;

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private CanvasGroup inventoryPanel, callUiCanvasGroup;

    [SerializeField]
    private Image cursor, inventoryIconPrefab, callIcon, confirmIcon, inspectCloseup, closeupOnly, emailAttachment, gonzales, cutsceneTopBar, cutsceneBottomBar, coverImage; //phoneIcon //penTool

    [SerializeField]
    private Sprite cursorOff, cursorOn, confirmButtonDisabled, confirmButtonEnabled, confirmButtonPressed, gonzalesNormal, gonzalesHappy, gonzalesConcerned, gonzalesAngry;

    [SerializeField]
    private RectTransform cutsceneRectTransform;

    [Space(10)]

    [SerializeField]
    private Image inspectBackground, connectBackground, closeupBackground;

    [Space(10)]

    [SerializeField]
    private Image profileTab, evidenceTab, resourcesTab;

    [SerializeField]
    private Image[] resourceIcons;

    private bool hasOpenedMemoboardBefore = false;
    private bool hasInspectedBefore = false;
    private bool hasConnectedBefore = false;

    private Conversation conversation;
    private AnsweringMachine answeringMachine;

    // private Questions questionsClass;

    private bool helpvar = false;

    // Use this for initialization
    void Start()
    {
        //for keeping track of the resources and calls
        conversation = Scorch_SceneObject.SceneObject.conversation;
        answeringMachine = Scorch_SceneObject.SceneObject.answeringMachine;

        //lists for the inventory icons
        selectedEvidence = new List<Evidence>();
        inventoryIconList = new List<Image>();
        disabledInventoryIconList = new List<Image>();

        ReturnToOrganizeMode();

        Color col = evidenceText.color;
        col.a = 0;
        evidenceText.color = col;
        evidenceHintText.color = col;

        //Ensures proper enabling
        canvas.gameObject.SetActive(true);
        HideInventory();
        HideIntroScreen();
        HideOptionsMenu();
        HideRebindMenu();
        HidePauseMenu();
        HideCloseup();
        SetPenVisibility(false);
        SetPhoneVisibility(false);
        SetUIHintVisibility(false);
        SetWrongPasswordTextVisibility(false);
        SetAreYouReadyVisibility(false);
        SetCallUiVisibility(false);

        inputMurdererNameScreen.SetActive(false);
        endScoreScreen.SetActive(false);
        uiLineConnector.gameObject.SetActive(false);
        coverImage.gameObject.SetActive(false);

        GoToEvidenceTab(); //Ensure that the player sees their evidence first

        // make sure that the player can't call Gonzales to solve the case right away
        SetCallIconState(false);

    }

    private IEnumerator EvidenceUICoroutine;
    public void ShowEvidenceText(string newText, string newHintText)
    {
        if (EvidenceUICoroutine != null) StopCoroutine(EvidenceUICoroutine);

        evidenceText.text = newText;
        evidenceHintText.text = newHintText;

        Color col = evidenceText.color;
        col.a = 1;
        evidenceText.color = col;
        evidenceHintText.color = col;

        HideText(evidenceText, evidenceHintText);

    }


    private void HideText(Text text, Text text2)
    {

        EvidenceUICoroutine = FadeText(text, text2);

        StartCoroutine(EvidenceUICoroutine);

    }

    public void HandleCloseup(Evidence evidence)
    {
        //The width and height standards are hardcoded for performance
        if (evidence.closeupSprite != null)
        {

            playerController.AllowMouseLook(false);
            closeupOnly.sprite = evidence.closeupSprite;
            //reformat
            closeupOnly.rectTransform.sizeDelta = new Vector2(960f * closeupOnly.sprite.bounds.size.x / closeupOnly.sprite.bounds.size.y, 960f);

            closeupBackground.gameObject.SetActive(true);
        }
        else
        {
            //square
            closeupOnly.rectTransform.sizeDelta = new Vector2(960f, 960f);
        }
    }

    public void HideCloseup()
    {
        closeupBackground.gameObject.SetActive(false);
        playerController.AllowMouseLook(true);

        // For objects which have a pop up right away, show the evidence name and hint after the pop is closed
        if (playerController.GetEvidenceList().Count > 0)
        {
            ShowEvidenceText(playerController.GetEvidenceLatest().GetEvidenceName(), playerController.GetEvidenceLatest().GetEvidenceHint());
        }
    }

    public bool GetCloseupState()
    {
        if (closeupBackground.gameObject.activeSelf) { return true; }
        else { return false; }

    }

    public void HideIntroScreen()
    {
        introScreen.SetActive(false);
    }

    public void ShowPauseMenu()
    {
        pauseScreen.SetActive(true);

        //Play sound to show pause menu
        //pauseScreen.gameObject.GetComponent<UISound>().PlaySound();
        uisounds.AssignUIAudio(pauseScreen);
    }

    public void HidePauseMenu()
    {
        //Play sound to hide pause menu
        //pauseScreen.gameObject.GetComponent<UISound>().PlaySoundOpposite();

        uisounds.AssignUIAudio(pauseScreen);

        pauseScreen.SetActive(false);
    }

    public void ShowOptionsMenu()
    {
        optionsScreen.SetActive(true);

        //Play sound to show options menu
        //optionsScreen.gameObject.GetComponent<UISound>().PlaySound(); //commented out
    }

    public void HideOptionsMenu()
    {
        //Play sound to hide options menu
       // optionsScreen.gameObject.GetComponent<UISound>().PlaySoundOpposite(); //commented out

        optionsScreen.SetActive(false);
    }

    public void ShowRebindMenu()
    {
        rebindScreen.SetActive(true);
    }

    public void HideRebindMenu()
    {
        rebindScreen.SetActive(false);
    }

    public void ShowInventory()
    {
        Debug.Log("Inventory should have been shown");
        inventoryPanel.alpha = 1.0f;
        hasOpenedMemoboardBefore = true;

        //Return to the evidence tab
        GoToEvidenceTab();

        if (!hasInspectedBefore)
        {
            SetUIHintVisibility(true);
            UpdateUIHint("Click on evidence to inspect it");
            hasInspectedBefore = true;
        }

        //Play sound to open phone
        //inventoryPanel.gameObject.GetComponent<UISound>().PlaySound();
        uisounds.AssignUIAudio(inventoryPanel.gameObject);


        //Decided whether new resources are available
        DecideResourceVisibility();
    }
    public void HideInventory()
    {
        Debug.Log("Inventory should have been hidden");
        inventoryPanel.alpha = 0.0f;

        //Play sound to close phone
        //inventoryPanel.gameObject.GetComponent<UISound>().PlaySoundOpposite();
        uisounds.AssignUIAudio(inventoryPanel.gameObject);
    }
    public void CloseInventoryWithButton()
    {
        HideInventory();
        playerController.ReturnToGame();
    }

    public void AddNewPieceOfEvidenceToMemoboard(Evidence evidenceToAdd)
    {
        //Image newPieceOfEvidence;

        Image newPieceOfEvidence = Instantiate(inventoryIconPrefab, Vector3.zero, transform.rotation) as Image; //transform.position
        newPieceOfEvidence.transform.SetParent(evidenceTab.gameObject.transform); //newPieceOfEvidence.transform.SetParent(inventoryPanel.gameObject.transform);
        newPieceOfEvidence.transform.localScale = new Vector3(1, 1, 1); //was 2.2 before, not sure why
                                                                        //newPieceOfEvidence.transform.position = new Vector3(Random.Range(-500f, 500f) + 500f, Random.Range(-120f, 120f) + 120f, 0);
        newPieceOfEvidence.transform.position = new Vector3(640, 360, 0);
        foreach (var invIcon in inventoryIconList)
        { //update the new icon's position so that it is not too close to the others
            if (Vector3.Distance(newPieceOfEvidence.transform.position, invIcon.transform.position) < 100f)
            {
                newPieceOfEvidence.transform.position = new Vector3(Random.Range(-400f, 400f) + 600f, Random.Range(-200f, 200f) + 360f, 0);
            }
        }
        CopyComponent(evidenceToAdd, newPieceOfEvidence.gameObject);

        endScreenData.AddToEvidenceStringList(evidenceToAdd.GetEvidenceName()); //Adds the name of the evience to the end screen data object

        inventoryIconList.Add(newPieceOfEvidence);
    }

    public void ChangeInventoryText(string question)
    {
        inventoryText.text = question;
    }

    public void UpdateUIHint(string str)
    {
        uiHint.text = str;
    }

    public void UpdateInspectModeContent(Evidence evidence)
    {
        hasInspectedBefore = true;
        SetUIHintVisibility(false);

        //standard sizes are hardcoded for performance
        if (evidence.closeupSprite != null)
        {
            inspectCloseup.sprite = evidence.closeupSprite; //items with a close up
            //reformat
            inspectCloseup.rectTransform.sizeDelta = new Vector2(800f * inspectCloseup.sprite.bounds.size.x / inspectCloseup.sprite.bounds.size.y, 800f); //was 640
        }
        else
        {
            inspectCloseup.sprite = evidence.GetEvidenceSprite(); //normal items
            //square format
            inspectCloseup.rectTransform.sizeDelta = new Vector2(800f, 800f); //was 640
        }

        inspectModeText.text = evidence.GetEvidenceName();
        inspectModeHintText.text = evidence.GetEvidenceHint();
    }

    //Sets the cursor visibility
    public void SetCursorVisibility(bool visible)
    {
        cursor.gameObject.SetActive(visible);
    }


    //Sets the visibility the outside circle of the cursor
    public void SetCursorHighlight(bool highlight)
    {
        if (highlight)
        {
            cursor.sprite = cursorOn;
        }
        else cursor.sprite = cursorOff;
    }

    //Change the color of the cursor
    public void SetCursorColor(bool evidence)
    {
        if (evidence)
        {
            cursor.color = new Color(1f, 0.8f, 0.5f); //make the cursor orange
        }
        else cursor.color = Color.white;


    }


    public void SetPenVisibility(bool visible)
    {
        //penTool.gameObject.SetActive(visible);
    }

    public void SetPhoneVisibility(bool visible)
    {
        //phoneIcon.gameObject.SetActive(visible);
    }

    public void SetConfirmVisibility(bool visible)
    {
        confirmIcon.gameObject.SetActive(visible);
    }

    public void SetUIHintVisibility(bool visible)
    {
        uiHint.gameObject.SetActive(visible);
    }

    public void SetWrongPasswordTextVisibility(bool visible)
    {
        wrongPasswordText.gameObject.SetActive(visible);
    }

    public void SetAreYouReadyVisibility(bool visible)
    {
        areYouReadyScreen.SetActive(visible);
        if (!visible) //player presses no
        {
            //close the memoboard
            HideInventory();
            playerController.AllowMouseLook(true);
            playerController.SetMemoboardOpen(false);
        }
    }

    public void SetCallIconState(bool state)
    {
        if (!state)
        {
            callIcon.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            callIcon.GetComponent<Button>().interactable = false;
        }
        else
        {
            callIcon.color = new Color(1f, 1f, 1f, 1f);
            callIcon.GetComponent<Button>().interactable = true;
        }
    }


    public void SetConfirmState(int state)
    {
        switch (state)
        {
            case 0:
                confirmIcon.sprite = confirmButtonDisabled;
                break;

            case 1:
                confirmIcon.sprite = confirmButtonEnabled;
                break;

            case 2:
                confirmIcon.sprite = confirmButtonPressed;
                break;
        }
    }

    public void SetGonzalesExpression(int state)
    { //bad ugly code
        Debug.Log("Setting the gonzales facial expression");

        switch (state)
        {
            case 0: //Normal
                gonzales.sprite = gonzalesNormal;
                break;

            case 1: //Happy
                gonzales.sprite = gonzalesHappy;
                break;

            case 2: //Concerned
                gonzales.sprite = gonzalesConcerned;
                break;
            case 3: //Angry
                gonzales.sprite = gonzalesAngry;
                break;
        }
    }

    public void DisableConfirmButtonVisually()
    {
        SetConfirmState(0);
    }

    public bool SnapTransformIfNearSelector(Transform tranformToTest)
    {
        foreach (Image selector in selectorsList)
        {
            if (!selector.IsActive())
            {
                continue;
            }

            //See if there is an invIcon already there
            foreach (Image invIcon in inventoryIconList)
            {
                if (invIcon.rectTransform.position == selector.rectTransform.position)
                {
                    invIcon.GetComponent<InventoryIcon>().GoToSnapback();
                    Debug.Log("InvIcon snapped back");
                }
            }

            if (Vector3.Distance(selector.transform.position, tranformToTest.position) < 32.0f)
            {
                tranformToTest.position = selector.transform.position; //move the inventory icon
                return true;
            }
        }
        return false;
    }

    //Start the final call (originally in the player controller)
    public void StartFinalCall()
    {
        SetPhoneVisibility(false);
        SetAreYouReadyVisibility(false);
        playerController.AllowMouseLook(false);
        playerController.SetMemoboardOpen(true);
        ShowInventory();
        EnterConnectionMode();
        questionsList[currentQuestion].PlayQuestion(currentQuestion);
        SetUIHintVisibility(true);
        ParentInventoryIconsToConnectBackground();

        conversation.SetFinalCallState(true); // Notify Calls and Conversations that the player is already solving case
        conversation.SetVibration(false); // StopVibration

        //confirmIcon.gameObject.SetActive(false);
        SetConfirmVisibility(false);
        selectorGroupForThree.SetActive(false);


        //Invoke("SetInventoryText", questionsList[currentQuestion].GetClip()[0].length);
        //Invoke("MakeSelectorGroupThreeVisible", questionsList[currentQuestion].GetClip()[0].length);
        SetCallUiVisibility(false);

        StartCoroutine(AskFirstQuestion());

    }
    private void SetInventoryText()
    {
        inventoryText.text = questionsList[currentQuestion].GetQuestionName();
    }

    // Logic for the different modes
    #region modeLogic
    public void EnterConnectionMode()
    {

        //   questionsClass.PlayClips(0);
        /*if (!hasConnectedBefore) {
            SetUIHintVisibility(true);
            UpdateUIHint("Drag into the connectors to select evidence");
            hasConnectedBefore = true;
        }*/

        uimode = UIMode.Connect;
        inspectBackground.gameObject.SetActive(false);
        connectBackground.gameObject.SetActive(true);

        //Hide try again text
        tryAgainText.gameObject.SetActive(false);

        //update the confirm button
        SetConfirmState(0);

        //Give the player a UI Hint
        SetUIHintVisibility(true);
        UpdateUIHint("Drag evidence into the circle to connect it");
    }

    public void EnterInspectMode()
    {
        uimodePrevious = uimode;
        uimode = UIMode.Inspect;
        inspectBackground.gameObject.SetActive(true);
        inspectBackground.transform.SetAsLastSibling(); //ensure that the inspect mode is on top
        //connectBackground.gameObject.SetActive(false); //not needed?

        //Play sound to open inspect mode
        //inspectBackground.gameObject.GetComponent<UISounds>().PlaySound();
        uisounds.AssignUIAudio(inspectBackground.gameObject);
    }

    public void ReturnToPreviousMode()
    {
        uimode = uimodePrevious;

        //Play sound to leave inspect mode
        //inspectBackground.gameObject.GetComponent<UISounds>().PlaySoundOpposite();
        uisounds.AssignUIAudio(inspectBackground.gameObject);

        inspectBackground.gameObject.SetActive(false);

        //Hide the connection background while organizing
        if (uimode == UIMode.Organize)
        {
            connectBackground.gameObject.SetActive(false);
        }
    }

    public void ReturnToOrganizeMode()
    {
        uimode = UIMode.Organize;
        inspectBackground.gameObject.SetActive(false);
        connectBackground.gameObject.SetActive(false);

        if (!hasConnectedBefore)
        {
            //SetUIHintVisibility(true);
            //UpdateUIHint("Drag evidence into the circle to connect it");
            hasConnectedBefore = true;
        }
    }

    public bool IsInConnectMode()
    {
        return uimode == UIMode.Connect;
    }

    public bool IsInCloseup()
    {
        return closeupBackground.IsActive();
    }

    #endregion

    #region tabLogic
    public void GoToProfileTab()
    {
        profileTab.transform.SetAsLastSibling();

        //Play sound to change tabs
        //profileTab.gameObject.GetComponent<UISounds>().PlaySound();
        uisounds.AssignUIAudio(profileTab.gameObject);

        Debug.Log("should have shown the profile tab");
    }

    public void GoToEvidenceTab()
    {
        evidenceTab.transform.SetAsLastSibling();

        //Play sound to change tabs
        //evidenceTab.gameObject.GetComponent<UISounds>().PlaySound();
        uisounds.AssignUIAudio(evidenceTab.gameObject);
    }

    public void GoToResourceTab()
    {
        resourcesTab.transform.SetAsLastSibling();

        //Play sound to change tabs
        //resourcesTab.gameObject.GetComponent<UISounds>().PlaySound();
        uisounds.AssignUIAudio(resourcesTab.gameObject);
    }

    #endregion

    //Logic for the selected evidence list
    public void AddToSelectedEvidence(Evidence evidenceToAdd)
    {
        selectedEvidence.Add(evidenceToAdd);
        Debug.Log("UIC selected evidence called: " + evidenceToAdd.GetEvidenceName());

        //Make the confirm button visible (called redundantly?)
        SetConfirmVisibility(true);

        if (selectedEvidence.Count == questionsList[currentQuestion].GetNumOfRequiredAnswers())
        {
            SetConfirmState(1); //Enable the confirm Button
        }
    }

    public void RemoveFromSelectedEvidence(Evidence evidenceToRemove)
    {
        selectedEvidence.Remove(evidenceToRemove);
        if (selectedEvidence.Count != questionsList[currentQuestion].GetCorrectEvidenceList().Count)
        {
            SetConfirmState(0);
        }
    }

    public List<Transform> ConvertSelectedToTransformList()
    {
        List<Transform> invItemTransformList = new List<Transform>();
        foreach (Evidence evidence in selectedEvidence)
        {
            invItemTransformList.Add(evidence.gameObject.transform);
        }
        return invItemTransformList;
    }

    //Logic for asking questions
    #region questions
    public void TestSelectedEvidence()
    {
        //disables the UI Hint
        SetUIHintVisibility(false);

        if (selectedEvidence.Count == questionsList[currentQuestion].GetNumOfRequiredAnswers()) //should be equal to the number of question's expected count
        {
            //Hide the UI Hint
            SetUIHintVisibility(false);


            //Handle the button visuals
            SetConfirmState(2);
            Invoke("SetConfirmState(0)", 1f); //change me
            Invoke("DisableConfirmButtonVisually", 1f);

            if (QuestionWasAnsweredPerfectly(selectedEvidence, questionsList[currentQuestion].GetCorrectEvidenceList()))
            {
                Debug.Log("The answer was perfect");

                //Set the question score to 2 (Answered perfectly)
                questionsList[currentQuestion].SetQuestionScore(1);

                //Update the End Screen Data Question Score
                endScreenData.IncreaseQuestionScoreBy(1);

                //Update Gonzales' face to be happy
                SetGonzalesExpression(1);

                //Hide the try again text
                //tryAgainText.gameObject.SetActive(false);

                ConnectEvidenceVisually();
                PlayNextQuestionOrEnding();
            }
            else if (TestMajorityOfElementsEqual(selectedEvidence, questionsList[currentQuestion].GetCorrectEvidenceList()))
            { //TestMajorityOfElementsEqual(selectedEvidence, questionsList[currentQuestion].GetCorrectEvidenceList())
                Debug.Log("The answer was correct");

                //Set the question score to 1 (Majority achieved)
                questionsList[currentQuestion].SetQuestionScore(1);

                //Update the End Screen Data Question Score
                endScreenData.IncreaseQuestionScoreBy(1);

                //Update Gonzales' face to normal
                SetGonzalesExpression(0);

                //Hide the try again text
                //tryAgainText.gameObject.SetActive(false);

                ConnectEvidenceVisually();
                PlayNextQuestionOrEnding();
            }
            else
            {
                //Answered incorrectly
                Debug.Log("The answer was incorrect");

                //Set the score for the question for the reaction to false
                questionsList[currentQuestion].SetQuestionScore(0);

                //Update the End Screen Data Question Score
                endScreenData.IncreaseQuestionScoreBy(0); //does nothing

                //Update Gonzales' face to concerned
                SetGonzalesExpression(2);

                //tryAgainText.gameObject.SetActive(true);

                PlayNextQuestionOrEnding();
            }
        }
    }

    //Loads the Police Station scene after 3 seconds
    private IEnumerator LoadEndScreen()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(2);
        StopCoroutine(LoadEndScreen());
    }

    public void ReturnToMainMenu()
    {
        //Deletes the endScreenData Object so that there is only ever one in the scene
        EndScreenData endScreenData = FindObjectOfType<EndScreenData>();
        Destroy(endScreenData.gameObject);

        SceneManager.LoadScene(0);
    }
    //This is the method which handles the input the player selects as evidence
    public void TestSelectedEvidenceInput()
    {
        //Handle the button visuals
        SetConfirmState(2);
        Invoke("SetConfirmState(0)", 1f);

        if (murdererNameInputField.text.Contains("Jason") || murdererNameInputField.text.Contains("Jason Parker"))
        {
            murderSuspect = MurderSuspect.Jason;
            questionsList[currentQuestion].SetQuestionScore(2);

            //Update the End Screen Data Question Score, Jason was indeed the murderer
            endScreenData.IncreaseQuestionScoreBy(1); //does nothing

            if (currentQuestion < questionsList.Count - 1)
            { //Go to the next question
                Invoke("AskNextQuestion", 0.5f);
            }
        }
        else if (murdererNameInputField.text.Contains("Susan") || murdererNameInputField.text.Contains("Susan Baker"))
        {
            murderSuspect = MurderSuspect.Susan;
            questionsList[currentQuestion].SetQuestionScore(1);

            //Update the End Screen Data Question Score, Susan does not score points since she was not the murderer
            endScreenData.IncreaseQuestionScoreBy(0); //does nothing

            if (currentQuestion < questionsList.Count - 1)
            { //Go to the next question
                Invoke("AskNextQuestion", 0.5f);
            }
        }
        else
        {
            murderSuspect = MurderSuspect.Other;
            questionsList[currentQuestion].SetQuestionScore(0);
            if (currentQuestion < questionsList.Count - 1)
            { //Go to the next question
                Invoke("AskNextQuestion", 0.5f);
            }
        }
    }

    public void PlayNextQuestionOrEnding()
    {
        if (currentQuestion < questionsList.Count - 1)
        { //Go to the next question
            Invoke("AskNextQuestion", 1.0f);
        }
        else
        { //All questions have been answered

            HideInventory();
            playerController.SetMemoboardOpen(false);
            playerController.AllowMouseLook(true);
            playerController.SetAnsweredAllQuestions(true);

            // SetCursorVisibility(false);
            uiHint.text = "Go back to the police station";
            SetUIHintVisibility(true);
            
        }
    }

    //Asks the first Question
    private IEnumerator AskFirstQuestion()
    {
        yield return new WaitForSeconds(questionsList[currentQuestion].GetClip()[0].length);
        SetConfirmVisibility(true);
        SetInventoryText();
        EnableSuitableSelectorGroup();
        UpdateUIHint("Drag evidence into the connectors to select it");

        StopCoroutine(AskFirstQuestion());
    }

    public void AskNextQuestion()
    {

        foreach (Image invIcon in inventoryIconList)
        {
            if (invIcon.GetComponent<InventoryIcon>().GetSelected())
            { //deactivated the selected invIcons
                Debug.Log("This inventory Icon was deactivated: " + invIcon.name);
                invIcon.gameObject.SetActive(false);
            }

            //invIcon.GetComponent<InventoryIcon>().Deselect(); //deselect all invIcons
        }

        selectedEvidence.Clear(); //resets the player's evidence selection
        uiLineConnector.gameObject.SetActive(false);

        if (currentQuestion != questionsList.Count)
        {


            currentQuestion++; //Go to the next question (works as an index variable)
                               //  questionsClass.PlayClips(currentQuestion);


            inventoryText.text = questionsList[currentQuestion].GetQuestionName();
            Debug.Log("The number of expected pieces of evidence is: " + questionsList[currentQuestion].GetNumOfRequiredAnswers());

            EnableSuitableSelectorGroup();


            //overlay the input screen if it is an input question such as who is the murderer
            if (questionsList[currentQuestion].RequiresInput())
            {
                inputMurdererNameScreen.SetActive(true);
                //HideAllInventoryIcons();

                //Hide unused
                foreach (Image invIcon in inventoryIconList)
                {
                    if (invIcon.gameObject.activeSelf)
                    { //should be shown again later
                        disabledInventoryIconList.Add(invIcon);
                        invIcon.gameObject.SetActive(false);
                        helpvar = true;
                    }
                }

            }
            else
            {
                if (helpvar == true)
                {
                    inputMurdererNameScreen.SetActive(false);
                    //ShowAllInventoryIcons();

                    //Reshow unused
                    foreach (Image invIcon in disabledInventoryIconList)
                    {
                        invIcon.gameObject.SetActive(true);
                    }
                    helpvar = false;
                }
            }

            //Add here


            //Audio
            /* if (currentQuestion >= 1)
             {
                 audioManager.PlaySound("Question" + (currentQuestion+1));
             }*/
        }
    }

    private void ConnectEvidenceVisually()
    {
        //Visually connect the selected inventory icons
        List<RectTransform> inventoryIconRectTransformList = new List<RectTransform>();
        foreach (var invIcon in selectedEvidence)
        {
            inventoryIconRectTransformList.Add(invIcon.gameObject.GetComponent<RectTransform>());
        }

        //use the UILineConenctor to connect the pieces of evidence
        uiLineConnector.gameObject.SetActive(true);
        uiLineConnector.SetTransforms(inventoryIconRectTransformList);
    }

    /*private void HideAllInventoryIcons()
    {
        foreach (Image invIcon in inventoryIconList) {
            invIcon.gameObject.SetActive(false);
        }
    }

    private void ShowAllInventoryIcons()
    {
        foreach (Image invIcon in inventoryIconList) {
            invIcon.gameObject.SetActive(true); //could be problematic. it could re-enable already used icons
        }
    }*/

    public bool IsMuderSuspectJason()
    {
        return (murderSuspect == MurderSuspect.Jason);
    }

    public void ParentInventoryIconsToConnectBackground()
    {
        foreach (Image invIcon in inventoryIconList)
        {
            invIcon.transform.SetParent(connectBackground.transform);
        }
        Debug.Log("UIC attempted to reparent the inventory icons");
    }

    #endregion


    #region computer
    public void ShowComputer()
    {
        Debug.Log("The computer is being shown");
        playerController.AllowMouseLook(false);
        playerController.SetMemoboardOpen(true);
        computer.SetActive(true);
        loginScreen.SetActive(true);
        emailsScreen.SetActive(false);
        SetWrongPasswordTextVisibility(false);
    }

    public void HideComputer()
    {
        computer.SetActive(false);
        playerController.AllowMouseLook(true);
        playerController.SetMemoboardOpen(false);
    }

    public bool IsUsingComputer()
    {
        return computer.activeSelf;
    }

    public void ShowEmailsIfPasswordIsCorrect()
    {
        if (loginInputField.text == "1234")
        {
            emailsScreen.SetActive(true);
        }
        else
        {
            SetWrongPasswordTextVisibility(true);
        }
    }


    public void UpdateEmailContents(string str)
    {
        //used for newlines
        string newString = str.Replace("\\n", "\n");

        emailContents.text = newString;
        //reset the email attachment
        emailAttachment.sprite = null;
        emailAttachment.color = Color.clear;
    }

    public void UpdateEmailAttachment(Sprite spr)
    {
        if (spr != null)
        {
            emailAttachment.color = Color.white;
            emailAttachment.sprite = spr;
        }
    }


    #endregion


    //Logic for resources
    #region resources
    private void DecideResourceVisibility()
    {
        // For deciding whether a resource should be accesible, hardcoded for performance, hastily implemented because suddenly requested by players
        for (int i = 0; i < resourceIcons.Length; i++)
        {
            switch (i)
            {
                case 0: //intro dialogue
                    if (conversation.GetIntrodialogueState())
                    {
                        resourceIcons[0].GetComponent<ResourceDisplay>().MakeResourceVisible();
                    }
                    break;

                case 1: //autopsy report
                    if (conversation.GetAutopsyReportState())
                    {
                        resourceIcons[1].GetComponent<ResourceDisplay>().MakeResourceVisible();
                    }
                    break;

                case 2: //answering machine
                    if (answeringMachine.GetAnsweringMachinePlayedOnce())
                    {
                        resourceIcons[2].GetComponent<ResourceDisplay>().MakeResourceVisible();
                    }
                    break;
                case 3: //emails
                    resourceIcons[3].GetComponent<ResourceDisplay>().MakeResourceVisible();
                    break;
                default:
                    break;
            }
        }
    }

    public void ShowResource()
    {
        resourceScreen.SetActive(true);
    }

    public void HideResource()
    {
        resourceScreen.SetActive(false);
    }

    public void UpdateResourceText(int i)
    {
        if (resourceIcons[i].GetComponent<ResourceDisplay>().WasFound())
        {
            string name = "";
            string content = "";

            //Find the name of the conversation and its contents
            name = resourceIcons[i].GetComponent<ResourceDisplay>().GetResourceName();
            //name = name.Replace("\\n", "\n"); //Attempts to replace new line characters
            content = resourceIcons[i].GetComponent<ResourceDisplay>().GetResourceContent();

            //Update the resource UI window and update it
            resourceNameText.text = name;
            resourceContentText.text = content;

            ShowResource();
        }
    }

    #endregion

    //Tests if the question was answered perfectly
    private bool QuestionWasAnsweredPerfectly(List<Evidence> selectedEvidenceList, List<Evidence> correctEvidenceList)
    {
        //selectedEvidence, questionsList[currentQuestion].GetCorrectEvidenceList()
        int requiredForMajority = questionsList[currentQuestion].GetNumOfRequiredAnswers() / 2 + 1; //more than half
        int numEqual = 0;

        //Debug.Log(requiredForMajority.ToString() + "pieces of evidence need to be selected correctly");


        foreach (Evidence selectedEvidenceElement in selectedEvidence)
        {
            foreach (Evidence correctEvidenceElement in correctEvidenceList)
            {
                if (selectedEvidenceElement.GetEvidenceName() == correctEvidenceElement.GetEvidenceName()) //(selectedEvidenceElement.Equals(correctEvidenceElement)
                {
                    numEqual++;
                    //Debug.Log("A matching element was found");
                }
                if (numEqual >= requiredForMajority)
                {
                    // Debug.Log("A correct majority exists");
                    return true;
                }
            }
        }
        //    return false;
        return false;


        /*List<string> correctEvidenceListName = correctEvidenceList.ConvertAll<string>(delegate (Evidence e)
        {
            Debug.Log("correctEvidencelist" + e);
            return e.ToString();
        });
        List<string> selectedEvidenceListName = selectedEvidenceList.ConvertAll<string>(delegate (Evidence e)
        {
            Debug.Log("selectedEvidence" + e);
            return e.ToString();
        });*/


        /* foreach (Evidence c in correctEvidenceList) {
             Debug.Log("correct evidence" + c);
         }
         Debug.Log("correct evidence list" + correctEvidenceList);
         Debug.Log("selected evidence list" + selectedEvidenceList);
         */
        /*foreach (Evidence e in selectedEvidenceList) {
            if (!correctEvidenceList.Contains(e)) { Debug.Log("evidence e" + e); return false; }
        
        }*/
        /* for (int i = 0; i < selectedEvidenceList.Count; i++)
         {
             // if (selectedEvidenceList[i].GetEvidenceName() != correctEvidenceList[i].GetEvidenceName())
             Debug.Log("correvidence list" + selectedEvidenceList);
             Debug.Log("correvidence list" +correctEvidenceList[i]);
             if (!selectedEvidenceList.Contains(correctEvidenceList[i]))
                 return false;
         }*/
    }

    //Tests if the majority of two lists is the same
    private bool TestMajorityOfElementsEqual(List<Evidence> selectedEvidenceList, List<Evidence> correctEvidenceList)
    {
        //selectedEvidence, questionsList[currentQuestion].GetCorrectEvidenceList()
        int requiredForMajority = questionsList[currentQuestion].GetNumOfRequiredAnswers() / 2 + 1; //more than halfs
        int numEqual = 0;

        //Debug.Log(requiredForMajority.ToString() + "pieces of evidence need to be selected correctly");


        foreach (Evidence selectedEvidenceElement in selectedEvidence)
        {
            foreach (Evidence correctEvidenceElement in correctEvidenceList)
            {
                if (selectedEvidenceElement.GetEvidenceName() == correctEvidenceElement.GetEvidenceName()) //(selectedEvidenceElement.Equals(correctEvidenceElement)
                {
                    numEqual++;
                    //Debug.Log("A matching element was found");
                }
                if (numEqual >= requiredForMajority)
                {
                    // Debug.Log("A correct majority exists");
                    return true;
                }
            }
        }
        //    return false;
        return false;

        /*
            foreach (Evidence element in selectedEvidence) {
            if (correctEvidenceList.Contains(element)) {
                numEqual++;
                Debug.Log("A matching element was found");
            }
            if (numEqual >= requiredForMajority) {
                return true;
            }
        }
        return false;*/
    }



    //This is a script which is used to copy a GameComponent from one Object to another
    Component CopyComponent(Component original, GameObject destination)
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        // Copied fields can be restricted with BindingFlags
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy;
    }


    //Bug, coroutine is called incorrectly when called multiple times in quick succession

    //Coroutines
    /*IEnumerator FadeText()
    { //could allow for more than text fading
        yield return new WaitForSeconds(2.5f);
        for (float f = 1f; f > -0.1f; f -= 0.1f)
        {
            Color col = evidenceText.color;
            col.a = f;
            evidenceText.color = col;
            evidenceHintText.color = col;

           yield return null;
        }
    }*/

    IEnumerator FadeText(Text text, Text text2)
    { //could allow for more than text fading

        yield return new WaitForSeconds(2.5f);
        for (float f = 1f; f > -0.1f; f -= 0.1f)
        {
            yield return new WaitForSeconds(.001f);
            Color col = text.color;
            col.a = f;
            text.color = col;
            text2.color = col;

            yield return null;
        }

    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cvGroup)
    {
        Image[] imgs = cvGroup.GetComponentsInChildren<Image>();
        Text[] txts = cvGroup.GetComponentsInChildren<Text>();
        Color col;

        for (float f = 1f; f > -0.1f; f -= 0.1f)
        {
            yield return new WaitForSeconds(.00001f);
            foreach (Image i in imgs)
            {
                col = i.color;
                col.a = f;
                i.color = col; ;
            }
            foreach (Text t in txts)
            {
                col = t.color;
                col.a = f;
                t.color = col;
            }
        }
        yield return null;
    }

    //These methods are used to reference the coroutines outside the class
    public void StartShowingBars()
    {
        StartCoroutine(ShowBars());
    }

    public void StartHidingBars()
    {
        StartCoroutine(HideBars());
    }

    public void StartFadingWholeScreenToBlack()
    {
        coverImage.gameObject.SetActive(true);
        StartCoroutine(FadeImageIn(coverImage));
    }

    //Coroutines for showing and hiding the cutscene bars, require separate images to guarantee filling the screen with parent stretch rect transform
    private IEnumerator HideBars()
    {
        Color col;

        for (float f = 1f; f > -0.1f; f -= 0.1f)
        {
            yield return new WaitForSeconds(.00001f);
            //foreach (Image i in imgs)
            {
                //float topStartY = cutsceneTopBar.GetComponent<RectTransform>().anchoredPosition.y;
                //float bottomStartY = cutsceneBottomBar.GetComponent<RectTransform>().anchoredPosition.y;

                //cutsceneRectTransform.sizeDelta = new Vector2(10, 10);
                cutsceneTopBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 50 - 100f * f);
                cutsceneBottomBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -50 + 100f * f);

                //Fade
                col = cutsceneTopBar.color;
                col.a = f;
                cutsceneTopBar.color = col;
                cutsceneBottomBar.color = col;
            }
        }
        yield return null;
    }

    private IEnumerator ShowBars()
    {
        Color col;

        for (float f = 1f; f > -0.1f; f -= 0.1f)
        {
            yield return new WaitForSeconds(.00001f);
            cutsceneTopBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -50 + 100f * f);
            cutsceneBottomBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 50 - 100f * f);

            //Fade
            col = cutsceneTopBar.color;
            col.a = 1 - f;
            cutsceneTopBar.color = col;
            cutsceneBottomBar.color = col;
        }
        yield return null;
    }

    // Used for when the player walks out of the park to go to the police station
    IEnumerator FadeImageIn(Image image)
    {
        Color col;

        for (float f = 0f; f < 1.1f; f += 0.015f) // f < 1.1f ensures that the image fades completly, 1f is not enough because of float inaccuracies
        {
            yield return new WaitForSeconds(.00001f);


            //Fade
            col = coverImage.color;
            col.a = f;
            coverImage.color = col;
        }
        yield return null;

    }



    public void HideCallUI()
    {
        StopCoroutine(FadeCanvasGroup(callUiCanvasGroup));
        StartCoroutine(FadeCanvasGroup(callUiCanvasGroup));

    }

    private void ShowCanvasGroup(CanvasGroup cvGroup)
    {
        StopCoroutine(FadeCanvasGroup(callUiCanvasGroup));
        Image[] imgs = cvGroup.GetComponentsInChildren<Image>();
        Text[] txts = cvGroup.GetComponentsInChildren<Text>();
        Color col;

        foreach (Image i in imgs)
        {
            col = i.color;
            col.a = 1;
            i.color = col;
        }

        foreach (Text t in txts)
        {
            col = t.color;
            col.a = 1;
            t.color = col;
        }

    }

    public void ShowCallUi()
    {
        //SetCallUiVisibility(true);
        ShowCanvasGroup(callUiCanvasGroup);
    }

    public void SetCallUiVisibility(bool visible)
    {
        callUiCanvasGroup.gameObject.SetActive(visible);
    }


    //Is is a question which accepts two, or three pieces of evidence?
    public void EnableSuitableSelectorGroup()
    {
        switch (questionsList[currentQuestion].GetNumOfRequiredAnswers()) //was GetCorrectEvidenceList().Count
        {
            case 0: //hide both
                selectorGroupForTwo.SetActive(false);
                selectorGroupForThree.SetActive(false);
                break;
            case 2:
                selectorGroupForTwo.SetActive(true);
                selectorGroupForThree.SetActive(false);
                break;
            case 3:
                selectorGroupForTwo.SetActive(false);
                selectorGroupForThree.SetActive(true);
                break;
            default:
                Debug.Log("UIC can't handle correct question evidence list count");
                break;
        }
    }
}



