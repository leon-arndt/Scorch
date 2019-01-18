using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnsweringMachingBlinking : MonoBehaviour {
    [SerializeField]
    float blinkRate = 1.0f;

    bool state = false;
    bool blinking = true;

    [SerializeField]
    private Renderer targetRenderer;

    [SerializeField]
    private Material materialOff, materialOn;

    // Use this for initialization
    void Start () {
        InvokeRepeating("ToggleDisplay", 0f, blinkRate);
	}

    private void Update()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            TurnOff();
        }
    }
    void ToggleDisplay()
    {
        if (blinking)
        {
            state = !state;
            if (state)
            {
                targetRenderer.material = materialOn;
            }
            else if (!state)
            {
                targetRenderer.material = materialOff;
            }
        }
    }

    public void TurnOff()
    {
        Debug.Log("Answering Machine blinking was turned off");
        state = false;
        blinking = false;
        CancelInvoke();
        targetRenderer.material = materialOff;
    }
}
