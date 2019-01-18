using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureCycle : MonoBehaviour {
    [SerializeField]
    Texture defaultTexture;

    [SerializeField]
    Texture[] frames; //An array which holds all the textures to cycle through

    [SerializeField]
    Renderer targetRenderer;

    [SerializeField]
    Material defaulMaterial, cyclingMaterial;

    [SerializeField]
    float framesPerSecond = 10;
    bool state = false; //should we cycle?

    private void Start()
    {
        //targetRenderer = GetComponent<MeshRenderer>();
    }
    // Update is called once per frame
    void Update()
    {
    }


    // Toggle State
    public void ToggleState()
    {
        if (state)
        {
            StopCycling();
        }
        else
        {
            StartCycling();
        }
    }
    
    
    //Start cycling (turning the TV on)
    public void StartCycling()
    {
        state = true;
        InvokeRepeating("Cycle", 0f, 1f / framesPerSecond);
        targetRenderer.material = cyclingMaterial;
    }

    public void StopCycling()
    {
        state = false;
        CancelInvoke();
        targetRenderer.material.mainTexture = defaultTexture;
        targetRenderer.material = defaulMaterial;
    }

    // Iterate once
    private void Cycle()
    {
        int index = (int)(Time.time * framesPerSecond) % frames.Length;
        targetRenderer.material.mainTexture = frames[index];
    }
}
