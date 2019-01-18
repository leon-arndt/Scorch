using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scorch_SceneObject;
using System;
using UnityEngine.SceneManagement;


public class TimeTrigger : VoiceManager
{
    // TimeTrigger should be renamed to Trigger.cs

    Conversation conversation;
    AudioManager audiomanager;
    RaycastHit ray;

    [SerializeField]
    public AudioClip commentClip;

    //Observerpattern
    private static int observerIDTracker;
    private int observerID;
    private VManager vManager;

    private UIController uicontroller;
    private Music music;

    private bool activateEndTrigger;

    private void Start()
    {
        conversation = SceneObject.conversation;
        audiomanager = SceneObject.audioManager;
        vManager = SceneObject.VManager;

        uicontroller = FindObjectOfType<UIController>();
        music = FindObjectOfType<Music>();

        observerID = ++observerIDTracker;
        try
        {
            vManager.Register(this);
        }
        catch (NullReferenceException ex)
        {
            Debug.Log("No vManager, maybe it has been destroyed:" + ex);
        }


    }

    // Activate Audio Sources /Assign audio clips when entering Trigger Collider
    private void OnTriggerEnter(Collider other)
    {
        if (tag == "Leaves")
        {
            audiomanager.AssignEnvironmentTriggerSound(gameObject);

        }
        else if (tag == "CallTrigger" && conversation.stateNumber == 1)
        {
            StartCoroutine(conversation.SetState(conversation.stateNumber));
            Debug.Log("Statenumber" + conversation.stateNumber);

            gameObject.GetComponent<BoxCollider>().enabled = false;

        }
        else if (tag == "Watchtower")
        {

            if (commentClip != null)
            {
                if (!callState) { audiomanager.setInbetweenClip(gameObject); }
                gameObject.GetComponent<BoxCollider>().enabled = false;
            }

        }
        else if (tag == "Introsong")
        {
            audiomanager.PlayIntroSong();
            gameObject.GetComponent<BoxCollider>().enabled = false;
        }
        else if ((tag == "Endtrigger") && activateEndTrigger)
        {

            //Expensive but only called once so it is okay
            uicontroller.StartFadingWholeScreenToBlack();
            music.StopEndOfDayMusic(0, 7); // waiting time before, how long the Audio fade should be
            Invoke("LoadPoliceStation", 7f);

        }
        else if ((tag == "StartWall" || tag == "SideWalls") && !callState && !commState)
        {
            if (activateEndTrigger || commentClip == null)
            {
                return;
            }
            else
            {
                audiomanager.setInbetweenClip(gameObject);
            }

        }

        //Debug.Log("  [TIMETRIGGER] " + tag);

        // vManager.SetCommState(false); // why do i have to set any state, so callState shows the right value?
    }

    // Activate Audio Sources /Assign audio clips when exiting Trigger Collider
    private void OnTriggerExit(Collider other)
    {
        if (tag == "CallTrigger" && conversation.stateNumber != 1)
        {
            StartCoroutine(conversation.SetState(conversation.stateNumber));
            Debug.Log("Statenumber" + conversation.stateNumber);

            gameObject.GetComponent<BoxCollider>().enabled = false;

        }
    }

    // For Playing Audio when Ray cast hits as well -> Checking if player looks into this direction
    public void SetRayCastHit(RaycastHit hit)
    {
        ray = hit;
    }
    private RaycastHit GetRayCastHit()
    {
        return ray;
    }

    // For Fading into Police Station
    public void SetActiveEndTrigger(bool active)
    {
        activateEndTrigger = active;
    }
    private void LoadPoliceStation()
    {
        SceneManager.LoadScene(2);
    }
}




