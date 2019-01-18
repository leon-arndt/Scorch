using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MainMenuAudio : MonoBehaviour {


    //PickUpSound
    [SerializeField]
    private AudioClip mainmenuAudioClip;
    [SerializeField]
    private AudioSource mainmenuAudioSource;
    [SerializeField]
    private AudioMixerGroup MainMenuMixerGroup;

    void Start () {

        mainmenuAudioSource = gameObject.AddComponent<AudioSource>();
        mainmenuAudioSource.clip = mainmenuAudioClip;
        mainmenuAudioSource.playOnAwake = true;
        mainmenuAudioSource.outputAudioMixerGroup = MainMenuMixerGroup;
        mainmenuAudioSource.loop = true;
        mainmenuAudioSource.enabled = true;

        mainmenuAudioSource.Play();
	}
	
}
