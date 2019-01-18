using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Scorch_SceneObject;

//Monobehaviour for Coroutines
public class VoiceManager : MonoBehaviour, VA_Observer
{
    //Observerpattern
    protected bool callState, questState, amState, commState, songState, subtitles;
    protected string language; 

    //Subtitles
    [SerializeField]
    protected Text  dialogueText;

    //Sound
    public VoiceActing[] voiceActing;
    [NonSerialized]
    public int playElement, sentenceIterator, conversationIterator, clipCount;

    [SerializeField]
    public AudioSource mainVoiceSource;

     VManager vm;


    private void Start()
    {
        vm = SceneObject.VManager;
        vm.SetSubtitleState(true);

    }

    //Subtitles
    protected void ResetSubtitles()
    {
        dialogueText.text = null;
    }

    //Outdated
    #region
    /*  private IEnumerator GetNextLine()
      {
          //set sentence Iterator to 0, because each converstion starts with sentence 0 
          sentenceIterator = 0;
          // Iterate through conversations
          playElement = (conversationIterator % voiceActing.Length);
          // Iterate through all sentences of Conversation
          {
              foreach (string sentence in voiceActing[playElement].sentences)
              {
                 // try
                  {

                      conversation.Enqueue(sentence);
                      DisplayNextSentence();
                  }
                 // catch (NullReferenceException ex) { Debug.Log("Check if sentences available"); }
                  yield return null; //If this Coroutine is stopped
                  yield return new WaitForSeconds(voiceActing[playElement].time[sentenceIterator]); //wait a special amount of time until next line is shown
                  sentenceIterator++;
              }
          }
          ResetSubtitles(); //Reset Subtitles
      }*/
     
    //Coroutines for Subtitles and Audio
    //  private void StartSubtitles()
    //  {
    // StopCoroutine("GetNextLine");
    //  StartCoroutine("GetNextLine");
    //   }
    //   private void StartVoiceClips()
    //   {
    // StopCoroutine("PlayMultipleClips");
    // StartCoroutine("PlayMultipleClips");
    //    }
    #endregion

    public void StartVoiceActing()
    {
        StartCoroutine("PlayVoiceActing");
    }

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


}

