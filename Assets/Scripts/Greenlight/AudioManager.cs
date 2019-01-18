using UnityEngine.Audio;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using System.Collections;
using System.Collections.Generic;
using Scorch_SceneObject;
using System;
using Random = UnityEngine.Random;

public class AudioManager : VoiceManager
{
    /// <summary>
    /// On PlayerController and manages Audio assignment (e.g. interaction sound, footsteps,...)
    /// UnityEngine.Random is for the case that more than 2 sounds exist, so the sound doesn't sound monotone / doesn't become annoying
    /// </summary>

    // Footsteps
    public List<GroundType> GroundTypes = new List<GroundType>();

    [SerializeField]
    private FirstPersonController FPC;
    public string currentground;

    // Terrain Footsteps
    private Vector3 textureCollission;
    private int terrainTextureIndex;
    private int currentindex;
    [SerializeField]
    Texture[] terrainTextures;

    //PickUpSound
    public List<PickUpType> PickUps = new List<PickUpType>();
    private AudioSource PU_AudioSource;
    [SerializeField]
    private AudioMixerGroup PickUpAudioMixer;
    public string currentpickup;

    //  Atmo
    Atmo go;
    [SerializeField]
    private AudioMixerGroup AtmoAudioMixerGroup;
    public string currentarea;

    public float musicVolume = 1f;
    [SerializeField]
    float seconds;


    //OPattern
    private static int observerIDTracker;
    private int observerID;
    private VManager vManager;

    //Inbetweens
    private AudioSource Comm_AudioSource;
    [SerializeField]
    private AudioMixerGroup CommMixerGroup;

    //Interactiontype
    public List<InteractableType> InteractableType = new List<InteractableType>();
    private AudioSource It_AudioSource;
    public string currentInteractableTypeName;
    [SerializeField]
    [Range(0f, 1f)]
    private AudioSource Ite_AudioSource;
    [SerializeField]
    private AudioMixerGroup InteractableMixerGroup;

    //Introsong
    private AudioSource Intro_AudioSource;
    [SerializeField]
    private AudioClip intro;
    [SerializeField]
    private AudioMixerGroup IntroAudioMixerGroup;

    //Pause song
    private AudioSource Pause_AudioSource;
    [SerializeField]
    private AudioClip pauseClip;
    [SerializeField]
    private AudioMixerGroup PauseSongMixerGroup;


    //Environment
    private AudioSource Environment_AudioSource;
    public string currentEnvironmentSound;
    public List<EnvironmentalSound> environmentSound = new List<EnvironmentalSound>();

    
    UIController ui; 

    [SerializeField]
    AtmoFade atmo;
    [SerializeField]
    private AudioMixer masterMixer;
    void Start()
    {

        PU_AudioSource = gameObject.AddComponent<AudioSource>();
        PU_AudioSource.outputAudioMixerGroup = PickUpAudioMixer;

        Comm_AudioSource = gameObject.AddComponent<AudioSource>();
        Comm_AudioSource.outputAudioMixerGroup = CommMixerGroup;

        It_AudioSource = gameObject.AddComponent<AudioSource>();
        It_AudioSource.outputAudioMixerGroup = InteractableMixerGroup;
        Ite_AudioSource = gameObject.AddComponent<AudioSource>();
        Ite_AudioSource.outputAudioMixerGroup = InteractableMixerGroup;

        #region //Observerpattern
        vManager = SceneObject.VManager;
        observerID = ++observerIDTracker;
        try
        {
            vManager.Register(this);
        }
        catch (NullReferenceException ex)
        {
            Debug.Log("No vManager, maybe it has been destroyed");
        }
        #endregion

        //CROSSREFERENCING
        // Would be better in Inspector
        ui = FindObjectOfType<UIController>();

        //Introsong
        Intro_AudioSource = gameObject.AddComponent<AudioSource>();
        Intro_AudioSource.clip = intro;
        Intro_AudioSource.outputAudioMixerGroup = IntroAudioMixerGroup;

        //Pause song
        Pause_AudioSource = gameObject.AddComponent<AudioSource>();
        Pause_AudioSource.outputAudioMixerGroup = PauseSongMixerGroup;
        Pause_AudioSource.clip = pauseClip;
        Pause_AudioSource.ignoreListenerPause = true;
        Pause_AudioSource.playOnAwake = false;

        //Environment
        Environment_AudioSource = gameObject.AddComponent<AudioSource>();
        Environment_AudioSource.outputAudioMixerGroup = AtmoAudioMixerGroup;
    }
    //Pick Up Sound
    #region
    private void SetPickupMaterial(PickUpType pickup)
    {
        if (currentpickup != pickup.name)
        {
            PU_AudioSource.clip = pickup.pickupSound;
        }

        PU_AudioSource.Play();


        currentpickup = pickup.name;
    }

    public void assignPickUpSound(RaycastHit hit)
    {
        if (hit.collider.gameObject.GetComponent<Interactables>() != null)
        {

            if (hit.collider.gameObject.GetComponent<Interactables>().material == "porcelain")
                SetPickupMaterial(PickUps[1]);
            else if (hit.collider.gameObject.GetComponent<Interactables>().material == "paper")
                SetPickupMaterial(PickUps[2]);
            else if (hit.collider.gameObject.GetComponent<Interactables>().material == "plastic")
                SetPickupMaterial(PickUps[3]);
            else if (hit.collider.gameObject.GetComponent<Interactables>().material == "packet")
                SetPickupMaterial(PickUps[4]);
            if (!(callState || amState))
            {
                if (hit.collider.gameObject.GetComponent<Interactables>().commentClip != null)
                    setInbetweenClip(hit);
            }
            else SetPickupMaterial(PickUps[0]);
        }
    }

    #endregion

    //Comments and Pick Up Comments
    #region
    private IEnumerator pickupCommentCo;
    private IEnumerator triggerCo;
    private void setInbetweenClip(RaycastHit hit) //for collected objects
    {
        pickupCommentCo = PickUpCommentCoroutine(hit);
        StartCoroutine(pickupCommentCo);
    }
    private IEnumerator PickUpCommentCoroutine(RaycastHit hit)
    {
        if (PU_AudioSource.clip != null) //Just play when there's an audioclip available, otherwise nullpointer exception 
        {
            yield return new WaitForSeconds(PU_AudioSource.clip.length);
        }

        bool closeup;
        closeup = ui.GetCloseupState();
        while (closeup) // play comment after closeup
        {
            closeup = ui.GetCloseupState(); yield return null;
        }

        if (!commState) // only play comment when there's no other comment already playing
        {
            vManager.SetCommState(true);
            if (subtitles && hit.collider.gameObject.GetComponent<Interactables>().sentence != null)
            {
                dialogueText.text = hit.collider.gameObject.GetComponent<Interactables>().sentence;
            }
            Comm_AudioSource.clip = hit.collider.gameObject.GetComponent<Interactables>().commentClip;
            Comm_AudioSource.Play();


            yield return new WaitForSeconds(Comm_AudioSource.clip.length + 1f);
            ResetSubtitles();
            vManager.SetCommState(false);
        }
    }
    public void setInbetweenClip(GameObject collGO) //for scene collission
    {
        if (pickupCommentCo != null)
        {
            StopCoroutine(pickupCommentCo);
        }
        if (collGO != null)
        {
            triggerCo = TriggerCoroutine(collGO);
            StartCoroutine(triggerCo);
        }
    }
    #endregion

    //Footsteps
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Footsteps
        if (hit.gameObject.GetComponent<Footsteps>() != null)
        {
            if (hit.gameObject.GetComponent<Footsteps>().material == "hardwood")
                SetGroundType(GroundTypes[1]);
            else if (hit.gameObject.GetComponent<Footsteps>().material == "lightwood")
                SetGroundType(GroundTypes[2]);
            else if (hit.gameObject.GetComponent<Footsteps>().material == "creaky")
                SetGroundType(GroundTypes[3]);
            else if (hit.gameObject.GetComponent<Footsteps>().material == "stairs")
                SetGroundType(GroundTypes[5]);
            else if (hit.gameObject.GetComponent<Footsteps>().material == "ladder")
                SetGroundType(GroundTypes[6]);
        }

        //Footsteps Terrain
        if (hit.gameObject.GetComponent<Terrain>() != null)
        {

            Terrain t = hit.gameObject.GetComponent<Terrain>();
            SplatPrototype[] ttex = t.terrainData.splatPrototypes;
       

            textureCollission = hit.point;
            terrainTextureIndex = TerrainSurface.GetMainTexture(hit.point);

            if (t.terrainData.splatPrototypes[terrainTextureIndex].texture.name == terrainTextures[0].name)
            {

                SetGroundType(GroundTypes[0]);

            }
            else if (t.terrainData.splatPrototypes[terrainTextureIndex].texture.name == terrainTextures[1].name)
            {
                SetGroundType(GroundTypes[4]);

            }

        }


        // Area
        if (hit.gameObject.GetComponent<Atmo>() != null)
        {
            go = hit.gameObject.GetComponent<Atmo>();
            if (go.area == "house") //hit gameobject == house
            {
                SetArea("houseVol");
            }
            else if (go.area == "tower")
            {
                SetArea("towerVol");
                //  StartCoroutine(CrossFadeDayNight(AtmoAudioMixerGroup,Atmo_AudioSourceA));

            }
            else if (go.area == "outside")
            {
                //  StartCoroutine(CrossFadeDayNight(AtmoAudioMixerGroup,Atmo_AudioSourceA));
                SetArea("daynightVol");
            }

        }
        else if (hit.gameObject.GetComponent<Terrain>()) // If outside
        {
            SetArea("daynightVol");

        }
    }

    // Atmo
    #region
    private void SetArea(string area)
    {
        if (currentarea != null)
        {
            if (currentarea != area)
            {
                atmo.PlayAtmo(currentarea, area);
                currentarea = area;
            }
        }
    }
    private void SetGroundType(GroundType ground)
    {
        if (currentground != ground.name)
        {
            // sorry for this - next time I will inherit FirstPersonController Script to a new one and make a getter, promised
            FPC.m_FootstepSounds = ground.footstepsounds;
            FPC.m_LandSound = ground.footstepsounds[0];
            currentground = ground.name;
        }


    } 
    
    //Colliders
    //bool playA = true; --> In AtmoFade.cs now
    /*  private IEnumerator PlayAtmo(AreaType area)
      {
         if (Atmo_AudioSourceB.volume == 0.0f) playA = false;
          if (area.name == "tower") seconds *= 3;

          if (playA)
          {
              Atmo_AudioSourceA.clip = area.areaSound;

              yield return StartCoroutine(CrossFade(Atmo_AudioSourceB, Atmo_AudioSourceA, seconds));
          }
          else
          {

              Atmo_AudioSourceB.clip = area.areaSound;
              yield return StartCoroutine(CrossFade(Atmo_AudioSourceA, Atmo_AudioSourceB, seconds));
          }

          yield return null;
      }
    /*  private IEnumerator CrossFade(AudioSource a, AudioSource b, float time)
      {
          //calculation 
          float stepInterval = time / 20.0f;
          float volInterval = musicVolume / 20.0f;
          b.Play();

          for (int i = 0; i < 20; i++)
          {
              a.volume -= volInterval;
              b.volume += volInterval;
              yield return new WaitForSeconds(stepInterval);

          }
          a.Stop();
          // Debug.Log("done");
      }*/
    #endregion

    //Environment
    public void AssignEnvironmentTriggerSound(GameObject collGo)
    {
        if (collGo.gameObject.GetComponent<TimeTrigger>().tag == "Leaves")
        {
            SetEnvironmentalTrigger(environmentSound[0]);
        }
    }
    private void SetEnvironmentalTrigger(EnvironmentalSound triggerObject)
    {
        int m = Random.Range(1, triggerObject.environmentalSounds.Length);
        Environment_AudioSource.clip = triggerObject.environmentalSounds[m];
        Environment_AudioSource.PlayOneShot(Environment_AudioSource.clip);
        triggerObject.environmentalSounds[m] = triggerObject.environmentalSounds[0];
        triggerObject.environmentalSounds[0] = Ite_AudioSource.clip;
        //currentInteractableTypeName = triggerObject.name;
    }
    private IEnumerator TriggerCoroutine(GameObject collGO)
    {
        if (!commState)
        {
            vManager.SetCommState(true);
            Comm_AudioSource.clip = collGO.gameObject.GetComponent<TimeTrigger>().commentClip;
            Comm_AudioSource.Play();

            yield return new WaitForSeconds(Comm_AudioSource.clip.length);
            vManager.SetCommState(false);

        }
    }

    //Interaction
    public void assignInteractionSound(RaycastHit hit, int order)
    {
        if (hit.collider.gameObject.GetComponent<Interactable>() != null && hit.collider.gameObject.GetComponent<Interactable>().interactableType != null)
        {
            // order 1  = play before animation, order 2 = play after animation, order 3 = special events
            Interactable interactable = hit.collider.gameObject.GetComponent<Interactable>();
            if (interactable.interactableType == "lightswitchsmall")
            {
                setInteractableType(InteractableType[0]);
            }
            else if (interactable.interactableType == "lightswitchmiddle")
            {
                setInteractableType(InteractableType[1]);
            }
            else if (interactable.interactableType == "lightswitchlarge")
            {
                setInteractableType(InteractableType[2]);
            }
            else if (interactable.interactableType == "toilet")
            {
                setInteractableType(InteractableType[3]);
            }
            else if (interactable.interactableType == "fridge")
            {
                setInteractableType(InteractableType[4]);
            }
            else if (interactable.interactableType == "drawer1")
            {
                if (order == 1) setInteractableTypeStart(InteractableType[5]);
                else if (order == 2) setInteractableType(InteractableType[5]);
            }
            else if (interactable.interactableType == "drawer2")
            {
                if (order == 1) setInteractableTypeStart(InteractableType[6]);
                else if (order == 2) setInteractableType(InteractableType[6]);
            }
            else if (interactable.interactableType == "drawer3")
            {
                if (order == 1) setInteractableTypeStart(InteractableType[7]);
                else if (order == 2) setInteractableType(InteractableType[7]);
            }
            else if (interactable.interactableType == "entrancedoor1")
            {
                if (order == 1) setInteractableTypeStart(InteractableType[8]);
                else if (order == 2) setInteractableType(InteractableType[8]);
            }
            else if (interactable.interactableType == "bathroomdoor")
            {
                if (order == 1) setInteractableTypeStart(InteractableType[9]);
                else if (order == 2) setInteractableType(InteractableType[9]);
            }
            else if (interactable.interactableType == "entrancedoor2")
            {
                if (order == 1) setInteractableTypeStart(InteractableType[10]);
                else if (order == 2) setInteractableType(InteractableType[10]);
            }
            else if (interactable.interactableType == "roomdoor")
            {
                if (order == 1) setInteractableTypeStart(InteractableType[11]);
                else if (order == 2) setInteractableType(InteractableType[11]);
                else if (order == 3)
                {
                    setInteractableType(InteractableType[12]);
                }
            }

            else Debug.Log("No interactableType found");
        }
    }
    private void setInteractableType(InteractableType interactable)
    {
        int n = Random.Range(1, interactable.interactionSounds.Length);
        It_AudioSource.clip = interactable.interactionSounds[n];
        It_AudioSource.PlayOneShot(It_AudioSource.clip);
        // move picked sound to index 0 so it's not picked next time
        interactable.interactionSounds[n] = interactable.interactionSounds[0];
        interactable.interactionSounds[0] = It_AudioSource.clip;
        currentInteractableTypeName = interactable.name;

    }
    private void setInteractableTypeStart(InteractableType interactable)
    {
        int m = Random.Range(1, interactable.interactionSoundsStart.Length);
        Ite_AudioSource.clip = interactable.interactionSoundsStart[m];
        Ite_AudioSource.PlayOneShot(Ite_AudioSource.clip);
        // move picked sound to index 0 so it's not picked next time
        interactable.interactionSoundsStart[m] = interactable.interactionSoundsStart[0];
        interactable.interactionSoundsStart[0] = Ite_AudioSource.clip;
        currentInteractableTypeName = interactable.name;
    }


    //Music Maybe in Music script
    public void PlayIntroSong()
    {
        Intro_AudioSource.Play();
        StartCoroutine(LinearFade());
    }
    public void SetPauseSong(bool pause)
    {
        if (pause)
        {
            Debug.Log("set pause true");

            Pause_AudioSource.enabled = true;
            Pause_AudioSource.loop = true;
            Pause_AudioSource.Play();
        }
        else
        {
            Debug.Log("set pause false");
            Pause_AudioSource.enabled = false;
        }
    }
    private IEnumerator LinearFade()
    {

        for (float f = Intro_AudioSource.volume; f <= 1; f += 0.1f)
        {
            yield return new WaitForSeconds(.3f);
            Intro_AudioSource.volume = f;
        }
    }
}

/* Classes so they're shown properly in Inspector -> Maybe make own script for those*/
[System.Serializable]
public class GroundType
{
    public string name;
    public AudioClip[] footstepsounds;
}

[System.Serializable]
public class PickUpType
{
    public string name;
    public AudioClip pickupSound;
}


[System.Serializable]
public class InteractableType
{
    public string name;
    public AudioClip[] interactionSoundsStart;
    public AudioClip[] interactionSounds;

}

[System.Serializable]
public class EnvironmentalSound
{
    public string name;
    public AudioClip[] environmentalSounds;
}





