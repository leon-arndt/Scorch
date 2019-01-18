using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using Scorch_SceneObject;

public class Conversation : VoiceManager
{
    //Observerpattern
    private VManager vManager;
    private static int observerIDTracker;
    private int observerID;

    //is true during vibration
    private bool pickUpAble = true, finalCallState = false;

    //With which GameState/Call should the Game start with?
    public int stateNumber;

    // Different States, More Calls were Intended than used
    private enum CallState
    {
        BlackScreen = 0,
        Introdialogue = 1,
        AutopsyReport = 2,
        AlcoholProblem = 3,
        GrowingImpatient = 4,
        FinalCall = 5
    }

    //private bool calls, currentPickUpState;
    private bool currentPickUpState;
    CallState currentState;
    private int iterator = 0;
    [SerializeField]
    public AudioSource vibrieren;
    private int convIter = 0, startWithThisClip = 0;

    //Subtitles
    public Queue<string> conversation, sentences;
    //Coroutines
    private IEnumerator coVoice;
    private IEnumerator coSubtitle;

    // How long should the vibration be until the player skips a call?
    [SerializeField]
    private int vibrationDuration;

    //Output of mainVoiceSource 
    [SerializeField]
    private AudioMixerGroup ConversationMixerGroup;

    //Referenced Scripts and Gameobjects
    [SerializeField]
    private Subtitles subs;
    [SerializeField]
    private PlayerController playercontroller;
    [SerializeField]
    private UIController uicontroller;
    [SerializeField]
    private HandsAnimationBehavior hands;

    // For Getters to know if introdialogue has already been played
    private bool introdialogue, autopsyreport, alcoholproblem, growingimpatient;

    void Start()
    {
        // Subtitles
        conversation = new Queue<string>();
        sentences = new Queue<string>();
        // Get Data from Subtitles
        for (int i = 0; i < voiceActing.Length; i++)
        { // Sentences
            foreach (string sub in subs.AllSentences(subs.IntToStringConverter(i)))
            {
                voiceActing[i].csvSentences.Add(sub);
            }
            subs.sentences.Clear();
            foreach (float time in subs.AllTimes(subs.IntToStringConverter(i)))
            {
                voiceActing[i].csvTime.Add(time);
            }
            subs.times.Clear();
        }

        // AudioOutput
        mainVoiceSource.outputAudioMixerGroup = ConversationMixerGroup;

        // Handanimation
        hands = FindObjectOfType<HandsAnimationBehavior>();

        // Observerpattern
        #region 

        vManager = SceneObject.VManager;
        observerID = ++observerIDTracker;
        //Debug.Log("New Observer " + this.observerID);
        try
        {
            vManager.Register(this);
        }
        catch (NullReferenceException ex)
        {
            Debug.Log("No vManager, maybe it has been destroyed:" + ex);
        }
        #endregion
    }

    //Voice Acting
    public IEnumerator PlayVoiceActing()
    {
        coVoice = PlayMultipleClips();
        StartCoroutine(coVoice);
        coSubtitle = GetNextLine();
        StartCoroutine(coSubtitle);

        yield return null;

    }
    public IEnumerator PlayMultipleClips()
    {
        playElement = (convIter % (voiceActing.Length)); // Iterating through Conversations e.g. introdialogue, autopsy,...
        clipCount = voiceActing[playElement].voiceClip.Length; //How many clips are in this conversation

        for (int i = startWithThisClip; i < clipCount; i++)
        {
            mainVoiceSource.clip = voiceActing[playElement].voiceClip[i]; //assign the right sourceClip
            mainVoiceSource.Play();

            yield return new WaitForSeconds(voiceActing[playElement].voiceClip[i].length);
            if (finalCallState)
            {

                yield break;
            }
        }

        switch (currentState)
        {
            case CallState.Introdialogue:
                playercontroller.EndCutscene();
                // UI Hint, Move, Sprint, Crouch, Camera can be added here
                introdialogue = true;
                // Debug.Log("Introdialogue" + introdialogue);
                break;
            case CallState.AutopsyReport:
                autopsyreport = true;
                //Debug.Log("AutopsyReport" + autopsyreport);
                break;
            case CallState.AlcoholProblem:
                alcoholproblem = true;
                //Debug.Log("AlcoholProblem" + alcoholproblem);
                break;
            case CallState.GrowingImpatient:
                growingimpatient = true;
                // Debug.Log("GrowingImpatient" + growingimpatient);
                break;
            default: break;

        }
        vManager.SetCallState(false);

    }

    //Subtitles
    private IEnumerator GetNextLine()
    {   //set sentence Iterator to 0, because each converstion starts with sentence 0 
        sentenceIterator = 0;
        // Iterate through conversations
        if (voiceActing.Length == 0) yield return null;

        // Iterate through all sentences of Conversation
        foreach (string sentence in voiceActing[playElement].csvSentences)
        {
            try
            {
                conversation.Enqueue(sentence);
                if (finalCallState)
                {
                    ResetSubtitles();
                    yield break;
                }
                DisplayNextSentence();

            }
            catch (NullReferenceException ex) { Debug.Log("Check if sentences available"); }


            yield return null; //If this Coroutine is stopped
            yield return new WaitForSeconds(voiceActing[playElement].csvTime[sentenceIterator]); //wait a special amount of time until next line is shown
            sentenceIterator++;
        }

        ResetSubtitles(); //Reset Subtitles

    }
    private void DisplayNextSentence()
    {
        if (conversation.Count == 0)
        {
            ResetSubtitles();
            return;
        }
        string sentence = conversation.Dequeue();
        dialogueText.text = sentence;
    }

    //Vibration
    public void SetVibration(bool vibration)
    {
        vibrieren.enabled = vibration;
    }
    //Conversation
    public void ConversationIterator()
    {
        convIter++;
    }

    //Gamestates
    #region 
    private void BlackScreen() { }
    //Calls
    private void Introdialogue()
    {

        StartCoroutine(c(CallState.Introdialogue, 0, false));

    }
    private void AutopsyReport()
    {
        StartCoroutine(c(CallState.AutopsyReport, 1, false));
        wait = true;

    }
    private void AlcoholProblem()
    {
        StartCoroutine(c(CallState.AlcoholProblem, 2, false));
    }

    private void GrowingImpatient()
    {
        StartCoroutine(c(CallState.GrowingImpatient, 3, true));
    }

    public void SetFinalCallState(bool newFinalCallState)
    {
        finalCallState = newFinalCallState;
    }

    private void ChangeState(CallState newState)
    {
        currentState = newState;
        stateNumber++;
        StartCoroutine(newState.ToString());

    }

    bool wait; bool wait2;
    public IEnumerator SetState(int i)
    {

        while (callState)
        {
            wait = true;
            yield return null;
        } while (commState || amState)
        {
            wait2 = true; // in case comments or answering machine are played
            yield return null;
        }
        if (wait)
        {
            wait = false;
            // Debug.Log("Waiting for call");
            yield return new WaitForSeconds(120); // In case two calls are too fast after another 

        }
        if (wait2)
        {
            wait2 = false;
            yield return new WaitForSeconds(10); // wait for 10 seconds

        }


        ChangeState((CallState)i);

    }

    IEnumerator StartNewCall()
    {


        uicontroller.ShowCallUi(); // UI hint when gonzales is calling

        if (iterator == 0) // first call cannot be skipped, therefore vibration should loop infinitely
        {

            SetVibration(true);
            yield return null;
        }
        else // every other call can be skipped
        {
            pickUpAble = true; // only true during vibration
            SetVibration(true);
            yield return new WaitForSeconds(vibrationDuration); //Skip call
            SetVibration(false);
            pickUpAble = false;


            if (!currentPickUpState)
            {
                uicontroller.HideCallUI();
                vManager.SetCallState(false);
            }

        }

    }

    private void PickUpCall(int i) // can be picked up when the callstate is true --> Needed when calls are triggered solely by time; 
    {
        if (iterator == i) // only possible, when conversation state is the same as the iterator --> restricts player to press button multiple times
        {
            hands.PlayPhoneAnimation(); // handanimation
            currentPickUpState = true;
            SetVibration(false); //stop vibration 
            vManager.SetCallState(true); // notify subject 
            StartVoiceActing(); // start actual call
            uicontroller.HideCallUI(); // hide UI

        }
    }

    IEnumerator c(CallState CallState, int i, bool option)
    {
        // when player is solvng case, then don't let further calls come through
        if (finalCallState) { yield break; }

        yield return new WaitForSeconds(1.7f);

        StartCoroutine(StartNewCall());
        if (i == 0) //Constant vibration
        {
            while (currentState == CallState)
            {
                uicontroller.SetCallUiVisibility(true);
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.G))
                {
                    while (iterator == 0 && pickUpAble)
                    {
                        PickUpCall(iterator);
                        iterator++;
                        currentPickUpState = true;

                        uicontroller.HideCallUI();
                        StartDialogueAnimation();

                    }
                }

                yield return null;
            }
        }
        else if (option) //First is pickedup = true; then pickedup = false --> Reaction for skipped calls
        {
            while (currentState == CallState)
            {
                if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.G)) && pickUpAble)
                {
                    if (currentPickUpState) //true
                    {
                        while (iterator == i)
                        {
                            //  Debug.Log("CurrentPickupstate: " + currentPickUpState);
                            PickUpCall(iterator);
                            ConversationIterator();
                            iterator++;
                        }
                    }
                    else
                    {
                        while (iterator == i)
                        {
                            // Debug.Log("CurrentPickupstate: " + currentPickUpState);
                            ConversationIterator();
                            PickUpCall(iterator);
                            iterator++;
                        }
                    }
                }

                yield return null;
            }
            if (iterator == i)
            {
                iterator++; ConversationIterator();
                currentPickUpState = false;
            }
        }
        else //normal states , normal vibration
        {
            while (currentState == CallState)
            {
                //Debug.Log(CallState);
                if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.G)))
                {

                    while (iterator == i && pickUpAble)
                    {
                        PickUpCall(iterator);
                        iterator++;
                        currentPickUpState = true;
                    }
                }
                else
                {
                    if (iterator == i)
                    {
                        currentPickUpState = false;
                    }
                }

                yield return null;
            }
            if (iterator == i)
            {
                iterator++;
                currentPickUpState = false;
            }
        }
        ConversationIterator();
    }
    #endregion
    [SerializeField]
    public AudioSource walkingAudioSource;
    private void StartDialogueAnimation()
    {
        Animator animator = playercontroller.GetComponent<Animator>();
        animator.SetBool("callActive", true);
        walkingAudioSource.enabled = true;
        // playercontroller.EndCutscene();

    }

    // Getter
    public bool GetIntrodialogueState() { return introdialogue; }
    public bool GetAutopsyReportState() { return autopsyreport; }
    public bool GetAlcoholProblemState() { return alcoholproblem; }
    public bool GetGrowingImpatientState() { return growingimpatient; }
    public bool GetFinalCallState() { return finalCallState; }
}

