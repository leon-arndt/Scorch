using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Evidence : Interactables
{
    /* This class is added as a component to objects in the world which are evidence
     * it is copied and used for inventory icons at runtime
     * The fields need to public for reflection which is used when they are copied
     */



    // Fields which define the characterstics of the evidence


    [SerializeField]
    public string evidenceName;
    public string evidenceHint;

    [SerializeField]
    public Sprite sprite, closeupSprite;
    Subtitles sub;
    string sen;

    //has the evidence already been found by the player?
    private bool found;

    //Define whether the evidence should pop-up right after being picked up (such as for newspapers)
    [SerializeField]
    private bool shouldPopup;

    // Use this for initialization
    public void Start()
    {
        Invoke("Subs",1f);
    }

    public string GetEvidenceName()
    {
        return evidenceName;
    }

    public string GetEvidenceHint()
    {
        return evidenceHint;
    }

    public Sprite GetEvidenceSprite()
    {
        return sprite;
    }

    private void Subs()
    {

        sub = FindObjectOfType<Subtitles>();
        try
        {
            sen = sub.Find_Name(evidenceName).Sentence;
        }
        catch (NullReferenceException ex)
        {
            Debug.Log("[SUBTITLES] Couldnt find subtitles for " + evidenceName);

        }
        sentence = sen;
    }


    //Getters and Setters (now required so that they aren't picked up multiple times while the animation is playing
    public bool GetFound()
    {
        return found;
    }

    public void SetFound(bool foundNew)
    {
        found = foundNew;
    }

    public bool GetShouldPopup()
    {
        return shouldPopup;
    }

}
