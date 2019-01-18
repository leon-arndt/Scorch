using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraController : MonoBehaviour {
    public Shader replacementShader;

    // Use this for initialization
    private void OnEnable() {
        if (replacementShader != null) {
            GetComponent<Camera>().SetReplacementShader(replacementShader, "RenderType");
        }
    }

    private void OnDisable() {
        GetComponent<Camera>().ResetReplacementShader();
    }
}
