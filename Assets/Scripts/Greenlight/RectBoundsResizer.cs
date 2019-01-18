using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectBoundsResizer : MonoBehaviour {
    private RectTransform rectBounds;
    private float hRatio, vRatio;

    // Use this for initialization
    void Start () {
        //The canvas resolution is hardcoded so we don't a reference (performance)
        hRatio = Screen.width / 1280f; //was 1200, 1280
        vRatio = Screen .height / 720f; //was 1080
        Debug.Log("The current window size appears to be " + Screen.width + " / " + Screen.height);

        //Resize
        rectBounds = GetComponent<RectTransform>();
        float originalWidth, originalHeight;
        originalWidth = rectBounds.sizeDelta.x;
        originalHeight = rectBounds.sizeDelta.y;
        rectBounds.sizeDelta = new Vector2(originalWidth * hRatio, originalHeight * vRatio);
	}
}
