using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scorch_SceneObject;
using System;

public class Interactable : Interactables
{
    /* This class is added as a component to objects in the world which are interactables.
     * There should be different types such as doors and cupboards which can be opened and closed with animations.
     * There should also be a key which the player can pick up like evidence but which is not shown in the memo board.
     */

    private PlayerController playerController;
    private UIController uicontroller;

    private enum InteractionType { None, OpenAndClose, Pickup, AnsweringMachine, Computer, LightSwitch, React, Edible, Television, Radio }
    private enum AnimationType { Translate, Rotate };

    [SerializeField]
    private string interactableName;

    [SerializeField]
    private bool isALightSwitch, lightState;

    [SerializeField]
    private List<Light> targetLights;

    [SerializeField]
    private Renderer targetRenderer;

    [SerializeField]
    private Material lightbulbMaterialOff, lightbulbMaterialOn;

    [SerializeField]
    private InteractionType interactionType;

    [SerializeField]
    private AnimationType animationType;

    [SerializeField]
    private float animationTime;

    [SerializeField]
    private Vector3 deltaPos, deltaRot;

    [SerializeField]
    private Interactable requiredInteractable;



    AudioManager audioManager;

    AnsweringMachine answeringMachine;
    // VManager vmanager;

    bool state;
    public bool animationIsInProgress;
    bool firstTime = true;

    // Use this for initialization
    void Start()
    {


        Invoke("Subs", 1f);
        state = true;
        animationIsInProgress = false;
        playerController = FindObjectOfType<PlayerController>();
        uicontroller = FindObjectOfType<UIController>();
        answeringMachine = SceneObject.answeringMachine;
        audioManager = FindObjectOfType<AudioManager>();
        //vmanager = Scorch_SceneObject.SceneObject.VManager;

        float framerate = 1f / Time.deltaTime;
        float multiplier = 60f / framerate;
        multiplier *= multiplier;

        // Debug.Log("The " + gameObject.name + "'s multiplier is " + multiplier.ToString());
        //30 fps
        animationTime /= multiplier;
        deltaRot *= multiplier;
        deltaPos *= multiplier;
    }

    public void Interact(RaycastHit hit)
    {
        StartCoroutine(InteractCoroutine(hit));
    }

    public IEnumerator InteractCoroutine(RaycastHit hit)
    {
        //Adds a delay before the animation
        yield return new WaitForSeconds(0.4f);

        if (requiredInteractable == null || playerController.GetInteractableList().Contains(requiredInteractable))
        {//
            switch (interactionType)
            {
                case InteractionType.OpenAndClose:
                    OpenAndClose(hit);
                    break;
                case InteractionType.Pickup:
                    PickUp();
                    break;
                case InteractionType.Edible:
                    Eat();
                    break;
                case InteractionType.LightSwitch:
                    FlipLightswitch(hit);
                    break;
                case InteractionType.AnsweringMachine:
                    InteractAnsweringMachine();
                    break;
                case InteractionType.Computer:
                    InteractComputer();
                    break;
                case InteractionType.Television:
                    InteractTelevision();
                    break;
                case InteractionType.Radio:
                    InteractRadio(hit);
                    break;

            }
        }
        else
        { //locked
            uicontroller.ShowEvidenceText(interactableName, "seems to be locked");
            audioManager.assignInteractionSound(hit,3);
        }
    }

    private void PickUp()
    {
        playerController.AddToInteractableList(this); //adds the object to a list of interactables
        uicontroller.ShowEvidenceText("Found a " + interactableName, "");
        gameObject.SetActive(false);
    }

    private void Eat()
    {
        //playerController.AddToInteractableList(this); //adds the object to a list of interactables
        uicontroller.ShowEvidenceText("Yummy", "I'm sure he won't mind");
        gameObject.SetActive(false);
    }

    private void OpenAndClose(RaycastHit hit)
    {
        if (!animationIsInProgress)
        {
            state = !state;
            Debug.Log("The animation should have played");

            if (animationType == AnimationType.Translate)
            {
                StartCoroutine(Animate(hit));
            }

            if (animationType == AnimationType.Rotate)
            {
                StartCoroutine(OpenDoorSideways(hit));
            }
        }


    }

    private void FlipLightswitch(RaycastHit hit)
    {
        lightState = !lightState;

        foreach (Light targetLight in targetLights)
        {
            targetLight.gameObject.SetActive((!targetLight.gameObject.activeSelf)); //toggles the light(s) on and off
        }

        if (targetRenderer != null)
        {
            targetRenderer.material = (lightState) ? lightbulbMaterialOn : lightbulbMaterialOff;
        }

        audioManager.assignInteractionSound(hit, 2);

    }

    //Animate the object with translation and rotation
    IEnumerator Animate(RaycastHit hit)
    { 
        if (isALightSwitch)
        { //For objects with animation and Light
            FlipLightswitch(hit);
        }

        audioManager.assignInteractionSound(hit, 1);

        for (float f = 0f; f <= animationTime; f += Time.fixedDeltaTime) //Time.fixedDeltaTime
        {
            float time = (Time.fixedDeltaTime - 0.004f); //Time.fixedDeltaTime
            animationIsInProgress = true;
            if (!state)
            {
                transform.Translate(deltaPos / (1.0f / time));
            }
            else if (state)
            {
                transform.Translate(-deltaPos / (1.0f / time));
            }

            yield return null;
            animationIsInProgress = false;

        }

        if (!isALightSwitch && state)
        {
            audioManager.assignInteractionSound(hit, 2);
        }
    }

    IEnumerator OpenDoorSideways(RaycastHit hit) //declares a coroutine
    {
        if (isALightSwitch) { FlipLightswitch(hit); }

        audioManager.assignInteractionSound(hit, 1);

        float time = Time.fixedDeltaTime - 0.004f; //Time.fixedDeltaTime
        for (float f = 0f; f <= animationTime; f += time) //while the current rotation is less than the end we will continue, was end rotation
        {
            animationIsInProgress = true; //new
            if (!state)
            {
                gameObject.transform.Rotate(deltaRot / (1.0f / time)); //works
            }
            else if (state)
            {
                gameObject.transform.Rotate(-deltaRot / (1.0f / time));
            }


            yield return null; //returns null and will start up here again next frame, yet it will just restart the while loop
            animationIsInProgress = false; //new
        }

        if (!isALightSwitch && state)
        {
            audioManager.assignInteractionSound(hit, 2);
        }
    }

    public void InteractAnsweringMachine()
    {
        answeringMachine.StartVoiceActing();
        if (gameObject.GetComponent<AnsweringMachingBlinking>() != null)
        {
            gameObject.GetComponent<AnsweringMachingBlinking>().TurnOff();
        } else
        {
            Debug.Log("Could not find Answering Machine Blinking Component");
        }
    }

    public void InteractComputer()
    {
        uicontroller.ShowComputer();
    }

    public void InteractTelevision()
    {
        GetComponent<TextureCycle>().ToggleState();
    }
    public void InteractRadio(RaycastHit hit)
    {
        hit.collider.gameObject.GetComponent<Radio>().SetRadioSong();
    }

    public bool ShouldChangeCursorColor()
    {
        if (interactionType == InteractionType.Pickup || interactionType == InteractionType.Edible)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    Subtitles sub;
    string sen;
    private void Subs()
    {

        sub = FindObjectOfType<Subtitles>();
        try 
        {
            sen = sub.Find_Name(interactableName).Sentence;
        }
        catch (NullReferenceException ex)
        {
            Debug.Log("[SUBTITLES] Couldnt find subtitles for " + interactableName);

        }
        sentence = sen;
    }


    // Getters to be used externally: safer than returning unidentifieable enums
    public bool IsAPickup()
    {
        if (interactionType == InteractionType.Pickup)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsALightswitch()
    {
        return isALightSwitch;
    }

    public bool IsAReactable()
    {
        return (interactionType == InteractionType.React);
    }

}