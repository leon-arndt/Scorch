using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Crouching : MonoBehaviour
{
    bool animationIsInProgress;

    [SerializeField]
    private float animationTime;
    [SerializeField]
    private Vector3 deltaPos;
    public FirstPersonController FPC;

    public void Start()
    {
        FPC = FindObjectOfType<FirstPersonController>();
        //animationTime /= 2f;
        //deltaPos *= 2f;
    }

    public void Crouch(bool isCrouching)
    {
        StartCoroutine(CrouchCoroutine(isCrouching));
    }

    IEnumerator CrouchCoroutine(bool isCrouching)
    {
        for (float f = 0f; f <= animationTime; f += Time.fixedDeltaTime)
        {
          //  Debug.Log("Forlooopcalled");
            float time = Time.fixedDeltaTime - 0.004f;
            animationIsInProgress = true;
            if (!isCrouching)
            {
                FPC.crouching = false;
                gameObject.transform.Translate(deltaPos / (1.0f / time), Space.World);

            }
            else if (isCrouching)
            {
                FPC.crouching = true;
                gameObject.transform.Translate(-deltaPos / (1.0f / time), Space.World);

            }
            yield return null;
            animationIsInProgress = false;
        }
    }

}
