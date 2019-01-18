using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsAnimationBehavior : MonoBehaviour
{
    Animator animationController;

    // Use this for initialization
    void Start()
    {
        animationController = GetComponent<Animator>();
        PlayIdleAnimation();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    /// <summary>
    /// Methods for playing the different aimations
    /// </summary> 

    public void PlayGrabAnimation()
    {
        Debug.Log("Playing Grab Animation");
        animationController.Play("Grab");
    }


    public void PlayPhoneAnimation()
    {
        Debug.Log("Playing Phone Animation");
        animationController.Play("Phone");
    }


    public void PlayIdleAnimation()
    {
        Debug.Log("Playing Idle Animation");
        animationController.Play("Idle");
    }

    public void PlayInteractAnimation()
    {
        Debug.Log("Playing Interact Animation");
        animationController.Play("Interact");
    }

    public void PlayPushAnimation()
    {
        Debug.Log("Playing Push Animation");
        animationController.Play("Push");
    }
}
