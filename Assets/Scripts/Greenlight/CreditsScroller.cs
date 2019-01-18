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

    // Use this for initialization
    private void Start()
    {
        continueText.gameObject.SetActive(false);
        Invoke("ShowContinueText", 102f);
    }

    // Update is called once per frame
    void Update () {
        if (credits.GetComponent<RectTransform>().sizeDelta.y < 5800f)
        {
            credits.transform.Translate(scrollSpeed * Vector3.up * Time.deltaTime);
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


            //Go to the credits
            SceneManager.LoadScene(0);
        }
	}

    private void ShowContinueText()
    {
        continueText.gameObject.SetActive(true);
    }
}
