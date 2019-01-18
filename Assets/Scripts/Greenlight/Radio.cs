using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Radio : Interactables
{
    [SerializeField]
    private AudioClip[] radioSong;
    [SerializeField]
    private AudioSource Radio_AudioSource;
    [SerializeField]
    private AudioMixerGroup Radio_AudioMixerGroup;
   

    private bool radio;
    private int num = -1;

    private void Start()
    {
        radio = false;
    }


    public void SetRadioSong()
    {
        num = (num + 1) % (radioSong.Length + 1);
        if (num < radioSong.Length)
        {
            Radio_AudioSource.clip = radioSong[num];
            Radio_AudioSource.Play();
        }
        else
        {
            Radio_AudioSource.Stop();
        }
    }
}
