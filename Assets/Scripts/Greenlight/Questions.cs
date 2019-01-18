using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scorch_SceneObject;
using System;
using UnityEngine.UI;

public class Questions : VoiceManager
{
    private Question[] questions;
    [SerializeField]
    private TimeTrigger[] Colliders;

    [SerializeField]
    Music music;
    bool lastQustionPlayed;

    //Observerpattern
    private static int observerIDTracker;
    private int observerID;
    private VManager vManager;

    EndScreenData endScreenData;

    // Use this for initialization
    void Start()
    {

        endScreenData = FindObjectOfType<EndScreenData>();
        questions = GetComponentsInChildren<Question>();

        //observerpattern
        #region 
        vManager = SceneObject.VManager;

        observerID = ++observerIDTracker;
        // Debug.Log("New Observer " + this.observerID);
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
    public IEnumerator SetClip(int i)
    {
        if (i < questions.Length)
        {

            for (int j = 0; j < questions[i].GetClip().Length; j++)
            {
                mainVoiceSource.clip = questions[i].GetClip()[j];
                mainVoiceSource.Play();
                //StartCoroutine(questions[i].GetNextLine());
                yield return new WaitForSeconds(questions[i].GetClip()[j].length);

                if (i == questions.Length - 1)
                {
                    yield return new WaitForSeconds(questions[i].GetClip()[j].length);
                    music.StartEndOfDayMusic(5, 5);// waiting time before playing coroutine, how long fade should take
                    ActivateEndTriggers();
                }
            }
        }
    }


    public IEnumerator PlayReaction(int questNumber, int score)
    {
        if (questNumber == questions.Length)
        {
            score = endScreenData.GetQuestionScore();
        }
        if (questions[questNumber].reaction.Length != 0)
        {
            switch (score)
            {
                case 0:
                    mainVoiceSource.clip = questions[questNumber].reaction[0];
                    dialogueText.text = questions[questNumber].GetReactionSubtitle(0);
                    Debug.Log("[QUESTIONS] case 1 - Fail ");
                    break;
                case 1:
                    mainVoiceSource.clip = questions[questNumber].reaction[1];
                    dialogueText.text = questions[questNumber].GetReactionSubtitle(1);
                    Debug.Log("[QUESTIONS] case 2 - Middle");
                    break;
                case 2:
                    mainVoiceSource.clip = questions[questNumber].reaction[2];
                    dialogueText.text = questions[questNumber].GetReactionSubtitle(1);
                    Debug.Log("[QUESTIONS] case 3 - Perfect");
                    break;
            }
            if (!commState || !amState)
            {
                mainVoiceSource.Play();
            }
            yield return new WaitForSeconds(mainVoiceSource.clip.length);
            ResetSubtitles();
        }
        //StopCoroutine(SetClip(questNumber));
        // StartCoroutine(SetClip(questNumber + 1));
        if (questNumber < questions.Length-1)
            questions[questNumber + 1].PlayQuestion(questNumber + 1);
    }

    private void ActivateEndTriggers()
    {
        Colliders[0].GetComponent<BoxCollider>().enabled = true;
        Colliders[0].SetActiveEndTrigger(true);
        Colliders[1].GetComponent<BoxCollider>().enabled = true;
        Colliders[1].SetActiveEndTrigger(true);
    }

    public Text GetSubtitlesTextbox()
    {
        return dialogueText;
    }


}

