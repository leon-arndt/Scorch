using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VoiceActing
{
    public string name;
    public AudioClip[] voiceClip;
    [TextArea(3, 10)]
    public List<string> sentences;
    public List<float> time;

    //csv
    public List<string> csvSentences = new List<string>();
    public List<float> csvTime = new List<float>();


    
        //  csvSentences = subAM.AllSentences();
        //csvTime = subAM.AllTimes();
   
    
}



public interface VA_Observer
{
    void Refresh(bool callState, bool questState, bool amState, bool commState, bool songState,bool subtitles, string language );
    //  IEnumerator PlayMultipleClips();
}