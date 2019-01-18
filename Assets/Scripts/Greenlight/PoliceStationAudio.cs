using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class PoliceStationAudio : MonoBehaviour
{
    // Reaction dependant on achieved score 
    private EndScreenData endScreenData;
    int maximumScore = 5;

    private AudioSource reactionAudioSource;
    [SerializeField]
    private AudioClip[] reactionAudioClip;
    [SerializeField]
    private AudioSource confessionAudioSource;
    [SerializeField]
    private AudioMixerGroup reactionAudioMixerGroup;
    [SerializeField]
    private float waitingtimeBefore, waitingtimeAfter;

    [SerializeField]
    private Text dialogueText;

    //Subtitles
    public Queue<string> conversation, sentences;

    Subtitles subs;
    PoliceStationController policeStationController;

    [SerializeField]
    private List<string> reactionSubtitle = new List<string>();
    public List<float> csvTime = new List<float>();

    [SerializeField]
    private List<string> confessionSentences = new List<string>();
    [SerializeField]
    public List<float> confessionTime = new List<float>();
    private int sentenceIterator;


    // Use this for initialization
    void Start()
    {
        //Subtitle Container
        conversation = new Queue<string>();
        sentences = new Queue<string>();

        endScreenData = FindObjectOfType<EndScreenData>();
        reactionAudioSource = gameObject.AddComponent<AudioSource>();
        policeStationController = GetComponent<PoliceStationController>();
        subs = GetComponent<Subtitles>();


        StartCoroutine(ConfessionReaction());
        Invoke("Subs", 0.01f); //Delay is needed, because otherwise subtitles aren't found

    }
    private IEnumerator ConfessionReaction()
    {
        yield return new WaitForSeconds(waitingtimeBefore); // wait a little bit until confession starts
        confessionAudioSource.Play();  // Audio
        StartCoroutine(GetNextLine()); // Subtitles

        yield return new WaitForSeconds(confessionAudioSource.clip.length); //wait until confession has ended
        yield return new WaitForSeconds(waitingtimeAfter); // wait a little bit until the reaction of gonzales plays;


        policeStationController.UpdateAndShowEndScreen(); //Update and show Endscreen
        PlayReaction(endScreenData.GetQuestionScore()); // Play Gonzales' reaction according to score
    }


    public void PlayReaction(int gameScore)
    {//best score: e.g. 3 < gamescore <= 5
        if (((maximumScore / 3) * 2 < gameScore) && (gameScore <= maximumScore))
        {

            reactionAudioSource.clip = reactionAudioClip[2];
            dialogueText.text = reactionSubtitle[2];
        }
        // middle score: e.g. 1 < gamescore <= 3
        else if ((maximumScore / 3) < gameScore && ((maximumScore / 3) * 2 <= gameScore))
        {
            reactionAudioSource.clip = reactionAudioClip[1];
            dialogueText.text = reactionSubtitle[1];
        }

        // worst score: e.g. 1 <= gamescore
        else if ((gameScore <= (maximumScore / 3)))
        {
            reactionAudioSource.clip = reactionAudioClip[0];
            dialogueText.text = reactionSubtitle[0];

        }
        StartCoroutine(WaitUntilResettingSubtitles(reactionAudioSource.clip.length));
        reactionAudioSource.Play();

    }

    //Adding Subtitles to Lists
    private void Subs()
    {


        for (int i = 0; i <= 2; i++) //Add sentences to String List Reaction Subtitle
        {
            foreach (string reactionSub in subs.AllSentences("Endreaction" + i))
            {
                reactionSubtitle.Add(reactionSub);
            }
            subs.sentences.Clear();// needs to be done every time, so the sentences before won't added again
        }

        foreach (string sub in subs.AllSentences("Confession"))
        {
            confessionSentences.Add(sub);
        }
        subs.sentences.Clear();

    }
  
    //DisplaySubtitles 
    private IEnumerator GetNextLine()
    { //set sentence Iterator to 0, because each converstion starts with sentence 0 
        sentenceIterator = 0;
        // Iterate through conversations

        foreach (string sentence in confessionSentences)
        {
            try
            {
                conversation.Enqueue(sentence);
                DisplayNextSentence();
            }
            catch (NullReferenceException ex) { Debug.Log("Check if sentences available"); }


            yield return null; //If this Coroutine is stopped
            yield return new WaitForSeconds(confessionTime[sentenceIterator]); //wait a special amount of time until next line is shown
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

    //Resetting Subtitles after last Message
    private void ResetSubtitles()
    {
        dialogueText.text = null;
    }
    private IEnumerator WaitUntilResettingSubtitles(float time)
    {
        yield return new WaitForSeconds(time + 1);
        ResetSubtitles();
    }

}
