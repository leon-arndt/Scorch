using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VManager : MonoBehaviour, VA_Subject
{
    //Observer Pattern
    private List<VA_Observer> observers; //of type VoiceActing
    private bool callState, questState, amState, commState, songState, subtitles;
    private string language;
    public enum Languages { English, German };
    // private Languages language;

    public VManager()
    {
        observers = new List<VA_Observer>();
    }

    //ObserverPattern
    public void Register(VA_Observer va)
    {
        observers.Add(va);
    }
    public void Unregister(VA_Observer va)
    {
        int vaIndex = observers.IndexOf(va);
        Debug.Log("Observer " + (vaIndex + 1) + " deleted");
    }
    public void NotifyObserver()
    {
        foreach (VA_Observer va in observers)
        {
            va.Refresh(callState, questState, amState, commState, songState, subtitles, language);
        }
    }

    //States
    public void SetCallState(bool newCallState)
    {
        callState = newCallState;
        NotifyObserver();
    }

    public void SetQuestState()
    {
        questState = !questState;
        NotifyObserver();
    }
    public void SetAmState(bool newAmState)
    {
        amState = newAmState;
        NotifyObserver();
    }
    public void SetAmState()
    {
        amState = !amState;
        NotifyObserver();
    }

    public void SetCommState()
    {
        commState = !commState;
        NotifyObserver();
    }

    public void SetCommState(bool newCommState)
    {
        commState = newCommState;
        NotifyObserver();
    }
    public void SetSongState(bool newSongState)
    {
        songState = newSongState;
        NotifyObserver();
    }
    public void SetSubtitleState(bool newSubtitleState)
    {
        subtitles = newSubtitleState;
        NotifyObserver();
    }

    public void SetSubtitleLanguage(string newLanguage)
    {
        language = newLanguage;
        NotifyObserver();
    }

    public string GetSubtitleLanguage() {
        return language;
    }
    // Getters
    public bool GetCallState()
    {
        return callState;
    }

}
