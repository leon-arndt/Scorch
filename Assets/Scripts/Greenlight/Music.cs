using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    float val_a;
    float val_b;
    [SerializeField]
    AudioSource EndOfDayMusicAudioSource;

    private void Start()
    {
        EndOfDayMusicAudioSource.volume = 0;
    }
    public void StartEndOfDayMusic(float waitingTime, float time)  // waiting time before playing coroutine, how long fade should take
    {
        EndOfDayMusicAudioSource.Play(); 
        StartCoroutine(FadeIn(EndOfDayMusicAudioSource,waitingTime, time));
    }

    public void StopEndOfDayMusic(float waitingTime, float time)
    {
        //StopCoroutine(FadeIn(EndOfDayMusicAudioSource,0,time));
        StartCoroutine(FadeOut(EndOfDayMusicAudioSource,0, time));
    }
    private IEnumerator FadeIn(AudioSource source, float waitingTime, float time) // time = until full volume is reached
    {
        yield return new WaitForSeconds(waitingTime);
        while (source.volume < 1)
        {
            source.volume += 0.1f;
            yield return new WaitForSeconds(0.1f*time);
        }
    }
    private IEnumerator FadeOut(AudioSource source, float waitingtime, float time) // time = until  volume == 0 is reached
    {

        while (source.volume > 0)
        {
            source.volume -= 0.1f;
            yield return new WaitForSeconds(0.1f * time);
        }
        if (source.volume == 0)
            source.Stop();

    }

}
