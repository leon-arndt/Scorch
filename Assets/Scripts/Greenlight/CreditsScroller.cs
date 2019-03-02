using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditsScroller : MonoBehaviour {
    /* Does exactly what you would expect.
     * Uses the transform instead of the rectTransform to translate.
     */ 

    [SerializeField]
    private GameObject credits;

    [SerializeField]
    private Text continueText;

    [SerializeField]
    private float scrollSpeed = 3.0f;
    private float distanceMoved = 0f;

    private bool continueTextVisible = false;

    // Use this for initialization
    private void Start()
    {
        continueText.gameObject.SetActive(false);
        Invoke("ShowContinueText", 102f);
    }

    // Update is called once per frame
    void Update () {
            credits.transform.Translate(scrollSpeed * Vector3.up * Time.deltaTime);
            distanceMoved += scrollSpeed * Time.deltaTime;
        if (distanceMoved > 3000 & !continueTextVisible)
        {
            ShowContinueText();
        }


        //Return to the main menu
        if ( Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
        {
            //Find and Delete the persistent End Screen Data to prevent problems
            if (FindObjectOfType<EndScreenData>() != null)
            {
                EndScreenData endScreenData = FindObjectOfType<EndScreenData>();
                Destroy(endScreenData.gameObject);
            }


            //Go to the main menu
            SceneManager.LoadScene(0);
        }
	}

    private void ShowContinueText()
    {
        continueText.gameObject.SetActive(true);
        continueTextVisible = true;
        Debug.Log("continue text should now be visible");
    }
}
