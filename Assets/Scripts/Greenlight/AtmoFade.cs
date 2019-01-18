using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AtmoFade : MonoBehaviour
{
    //Atmo
    public List<AreaType> Atmo = new List<AreaType>(); // [0] == day, [1] == night, [2] == house, [3] == tower
    private AudioSource Atmo_AudioSourceHouse, Atmo_AudioSourceTower, Atmo_AudioSourceDay, Atmo_AudioSourceNight;

    //Audiomixer Groups
    [SerializeField]
    private AudioMixerGroup AtmoAudioMixerGroup, DayAudioMixerGroup, NightAudioMixerGroup, HouseAudioMixerGroup, TowerAudioMixerGroup;

    private float audioVolume = 1f;

    // Master Mixer needed for referencing
    [SerializeField]
    AudioMixer masterMixer;

    float val_a; //output value of mixer a / old atmo
    float val_b; // output value of mixer b / new atmo
    private IEnumerator LastFadeInCoroutine;
    private IEnumerator LastFadeOutCoroutine;

    private void Start()
    {
        // House Audio Source
        Atmo_AudioSourceHouse = gameObject.AddComponent<AudioSource>();
        Atmo_AudioSourceHouse.loop = true; ; //house
        Atmo_AudioSourceHouse.clip = Atmo[2].areaSound; //  house
        Atmo_AudioSourceHouse.Play();
        masterMixer.SetFloat("houseVol", -80);

        // Tower Audio Source
        Atmo_AudioSourceTower = gameObject.AddComponent<AudioSource>();
        Atmo_AudioSourceTower.loop = true; // tower
        Atmo_AudioSourceTower.clip = Atmo[3].areaSound; //  tower 
        Atmo_AudioSourceTower.Play();
        masterMixer.SetFloat("towerVol", -80);

        // Day Night Audio Sources and Cycle
        Atmo_AudioSourceDay = gameObject.AddComponent<AudioSource>();
        Atmo_AudioSourceDay.clip = Atmo[0].areaSound; //  day 
        Atmo_AudioSourceDay.loop = true;
        Atmo_AudioSourceDay.volume = 1;
        Atmo_AudioSourceNight = gameObject.AddComponent<AudioSource>();
        Atmo_AudioSourceNight.clip = Atmo[1].areaSound; //  night
        Atmo_AudioSourceNight.loop = true;
        Atmo_AudioSourceNight.volume = 0;


        Atmo_AudioSourceDay.outputAudioMixerGroup = DayAudioMixerGroup;
        Atmo_AudioSourceNight.outputAudioMixerGroup = NightAudioMixerGroup;
        Atmo_AudioSourceHouse.outputAudioMixerGroup = HouseAudioMixerGroup;
        Atmo_AudioSourceTower.outputAudioMixerGroup = TowerAudioMixerGroup;



        IEnumerator DayNightAudioCoroutine = CrossFade(Atmo_AudioSourceDay, Atmo_AudioSourceNight, 1800); //1800s = 30 min -> Day Night Cycle
        StartCoroutine(DayNightAudioCoroutine);
    }

    // For Fading between two Audio Sources, when Fading JUST happens between audio sources 
    // -> Because of day- night cycle- Atmo this is not possible
    /*  private IEnumerator LastAtmoCoroutine; 
    public void PlayAtmo(AreaType area)
     {
         if (LastAtmoCoroutine != null)
         {
             StopCoroutine(LastAtmoCoroutine);
         }

         if (playA)
         {
             Atmo_AudioSourceA.clip = area.areaSound;
             LastAtmoCoroutine = CrossFade(Atmo_AudioSourceB, Atmo_AudioSourceA, seconds);
             StartCoroutine(LastAtmoCoroutine);
         }
         else
         {

             Atmo_AudioSourceB.clip = area.areaSound;
             StartCoroutine(CrossFade(Atmo_AudioSourceA, Atmo_AudioSourceB, seconds));
             LastAtmoCoroutine = CrossFade(Atmo_AudioSourceA, Atmo_AudioSourceB, seconds);
             StartCoroutine(LastAtmoCoroutine);
         }
     }*/
    // Fade between two audiosources -> Day Night Cycle
    private IEnumerator CrossFade(AudioSource a, AudioSource b, float time)
    {

        float steps = 45f;

        //Calculation 
        float timeInterval = time / steps; // how much time should pass until the next in-/ decrease
        float volInterval = audioVolume / steps; // how much the volume should in-/ decrease per step

       // make sure that both audio sources are active
        a.Play();
        b.Play();

        //Fading steps
        for (int i = 0; i < steps; i++) // i = step
        {
            a.volume -= volInterval;
            b.volume += volInterval;
            yield return new WaitForSeconds(timeInterval);

        }
        //playA = !playA; --> Necessary when fading more often than just once
        a.Pause();
    }

    // Called by other classes, Fading between Audiomixer Sliders
    // strings must be exactly the same as the exposed volume values in mixer
    public void PlayAtmo(string oldArea, string newArea)
    {

        masterMixer.GetFloat(oldArea, out val_a);
        masterMixer.GetFloat(newArea, out val_b);

        if (LastFadeInCoroutine != null)
        {
            StopCoroutine(LastFadeInCoroutine);
        }
        if (LastFadeOutCoroutine != null)
        {
            StopCoroutine(LastFadeOutCoroutine);
        }
        LastFadeInCoroutine = Fade1(newArea);
        LastFadeOutCoroutine = Fade2(oldArea);
        StartCoroutine(LastFadeInCoroutine);
        StartCoroutine(LastFadeOutCoroutine);
    }

    // Fade In
    private IEnumerator Fade1(string newAtmo)
    {
        while (val_b < 0)
        {
            val_b += 6;
            if (val_b >= 0) { val_b = 0; }
            masterMixer.SetFloat(newAtmo, val_b);
            yield return new WaitForSeconds(0.1f);

        }
    }
    // Fade Out
    private IEnumerator Fade2(string oldAtmo)
    {
        while (val_a > -80)
        {
            val_a -= 6;
            masterMixer.SetFloat(oldAtmo, val_a);
            yield return new WaitForSeconds(0.5f);
        }

    }

}

[System.Serializable]
public class AreaType
{
    public string name;
    public AudioClip areaSound;
}
