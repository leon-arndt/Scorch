using System.Collections;
using Scorch_SceneObject;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class AnsweringMachine : VoiceManager
{
    //Subtitles
    public Queue<string> conversation, sentences;


    [SerializeField]
    private int timeInbetween;   // offset of answering machine inbetween
                                 // actually not used in game any more because answering machine has just one clip. originally it had 3 clips 
    private int iter;            // iterates through answering machine clips

    [SerializeField]
    private AudioSource voiceSource;

    [SerializeField]
    private UIController uicontroller;


    private IEnumerator coVoice;
    private IEnumerator coSubtitle;
    private bool answeringmachinePlayedOnce;

    //Observerpattern
    private static int observerIDTracker;
    private int observerID;
    private VManager vManager;


    //csv
    Subtitles sub;

    public void Start()
    {
        conversation = new Queue<string>();
        sentences = new Queue<string>();
        vManager = SceneObject.VManager;

        observerID = ++observerIDTracker;

        try
        {
            vManager.Register(this);
        }
        catch (NullReferenceException ex)
        {
            Debug.Log("No vManager, maybe it has been destroyed:" + ex);
        }

        //csv languages
        sub = FindObjectOfType<Subtitles>();
        for (int i = 0; i < voiceActing.Length; i++)
        {
            {
                foreach (string sub in sub.AllSentences("Answeringmachine"))
                {
                    voiceActing[i].csvSentences.Add(sub);
                }
                sub.sentences.Clear();
                foreach (float time in sub.AllTimes("Answeringmachine"))
                {
                    voiceActing[i].csvTime.Add(time);
                }
                sub.times.Clear();
            }
        }

    }

    public IEnumerator PlayVoiceActing()
    {
        vManager.SetAmState();
        while (callState || questState) // avoid audio overlapping
        {
            uicontroller.SetUIHintVisibility(true); uicontroller.UpdateUIHint("Cannot Play while Talking");
            yield return new WaitForSeconds(2);
            uicontroller.UpdateUIHint("");
            yield break;
        }

        if (amState)
        {
            coVoice = PlayMultipleClips();
            StartCoroutine(coVoice);
            if (subtitles)
            {
                coSubtitle = GetNextLine();
                StartCoroutine(coSubtitle);
            }
        }
        else
        {
            if (subtitles)
            {
                StopCoroutine(coVoice);
                StopCoroutine(coSubtitle);
                ResetSubtitles();
            }
            voiceSource.Stop();

        }
    }

    // For the final game game it could actually just be Play(). but this code makes it more flexible for iterating through more audio clips   
    private IEnumerator PlayMultipleClips() 
    {

        playElement = (iter % (voiceActing.Length)); // Iterating through Conversations
        clipCount = voiceActing[playElement].voiceClip.Length;

        for (int i = 0; i < clipCount; i++)
        {
            voiceSource.clip = voiceActing[playElement].voiceClip[i]; //assign the right sourceClip
            voiceSource.Play();
            iter++;
            yield return new WaitForSeconds(voiceActing[playElement].voiceClip[i].length + timeInbetween);
        }

        answeringmachinePlayedOnce = true;

        vManager.SetAmState(false);
    }

    //Subtitles
    private IEnumerator GetNextLine()
    {
        //set sentence Iterator to 0, because each converstion starts with sentence 0 
        sentenceIterator = 0;
        // Iterate through conversations
        playElement = (conversationIterator % voiceActing.Length);
        // Iterate through all sentences of Conversation
        {
            foreach (string sentence in voiceActing[playElement].csvSentences)
            {
                try
                {
                    conversation.Enqueue(sentence);
                    DisplayNextSentence();
                }
                catch (NullReferenceException ex) { Debug.Log("Check if sentences available" + ex); }
                yield return null; //If this Coroutine is stopped
                yield return new WaitForSeconds(voiceActing[playElement].csvTime[sentenceIterator]); //wait a special amount of time until next line is shown
                sentenceIterator++;
            }
        }
        ResetSubtitles(); //Reset Subtitles
    }
    private void DisplayNextSentence()
    {
        if (conversation.Count == 0 || !amState)
        {
            ResetSubtitles();

            return;
        }
        string sentence = conversation.Dequeue();
        dialogueText.text = sentence;
    }

    // Is is the answering machine played once? -> For resources visibility
    public bool GetAnsweringMachinePlayedOnce()
    {
        return answeringmachinePlayedOnce;
    }
}