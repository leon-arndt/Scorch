using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class UISounds : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField]
    private UIAudio[] uiSound;
    private string currentUISound;
    [SerializeField]
    private AudioMixerGroup uiMixerGroup;
    private bool select = true;

    // Use this for initialization
    void Start()
    {
        //Adds an audio source to the game object at runtime
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.ignoreListenerPause = true;

        audioSource.outputAudioMixerGroup = uiMixerGroup;
        Debug.Log("A new audio source was created by UISounds");
    }


    //Plays the standard sound for the ui element
    public virtual void PlaySound()
    {
        Debug.Log("UI Sounds tried playing sound");
    }

    //For objects that have a "off" sounds
    public virtual void PlaySoundOpposite()
    {
        Debug.Log("UI Sounds tried playing opposite sound");
    }

    public void AssignUIAudio(GameObject go)
    {
        if (go.GetComponent<UISound>() != null)
        {
            if (go.GetComponent<UISound>().sound == "phone")
                SetUISound(uiSound[0]);
            else if (go.GetComponent<UISound>().sound == "evidence")
            {
                if (go.GetComponent<InventoryIcon>().GetSelected()) // If Selection is true
                {
                    SetUISound(uiSound[1]);
                }
                else {
                    SetUISound(uiSound[5]);
                }
            }
            else if (go.GetComponent<UISound>().sound == "inspect")
                SetUISound(uiSound[2]);
            else if (go.GetComponent<UISound>().sound == "pause")
                SetUISound(uiSound[3]);
            else if (go.GetComponent<UISound>().sound == "tab")
                SetUISound(uiSound[4]);
            Debug.Log(go.GetComponent<UISound>().sound);
        }

    }




    public void GetSelectionState(bool selected)
    {
        this.select = selected;
    }

    private void SetUISound(UIAudio uiaudio)
    {
        if (uiaudio.uiSoundClips.Length > 1)
        {
            int m = Random.Range(1, uiaudio.uiSoundClips.Length);
            //  int m = 0;
            audioSource.clip = uiaudio.uiSoundClips[m];
            audioSource.PlayOneShot(audioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            uiaudio.uiSoundClips[m] = uiaudio.uiSoundClips[0];
            uiaudio.uiSoundClips[0] = audioSource.clip;
            currentUISound = uiaudio.name;
        }
        else
        {
            audioSource.clip  = uiaudio.uiSoundClips[0];
            audioSource.PlayOneShot(audioSource.clip);
            currentUISound = uiaudio.name;
        }
    }
}
[System.Serializable]
public class UIAudio
{
    public string name;
    public AudioClip[] uiSoundClips;
}