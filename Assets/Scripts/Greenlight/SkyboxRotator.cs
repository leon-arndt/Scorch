using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxRotator : MonoBehaviour {
    [SerializeField]
    private Material skybox;
    private float rotation;
    private float exposure;

	// Use this for initialization
	void Start () {
        exposure = skybox.GetFloat("_Exposure");
	}
	
	// Update is called once per frame
	void Update () {
        rotation += 0.1f;
        /*if (exposure > 0) {
            exposure -= 0.01f;
        }*/
        skybox.SetFloat("_Rotation", rotation);
        skybox.SetFloat("_Exposure", exposure);
	}
}
