using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PoliceStationController : MonoBehaviour {
    /* This script handles the User Interface and Interaction inside police station scene
     * It finds the endScreenData in the scene and then displays it after the monolog has finished playing
     */

    [SerializeField]
    private GameObject endScreen;

    [SerializeField]
    private Text numEvidenceFoundText, questionScoreText;


    private EndScreenData endScreenData;

	// Use this for initialization
	void Start () {
        //Find the persistent end screen data object
        endScreenData = FindObjectOfType<EndScreenData>();

        endScreen.SetActive(false);
        Invoke("UpdateAndShowEndScreen", 70f); //The end screen should appear after 70 seconds
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Return))
        {
            // Only go to the credits if the end screen is visible
            if (endScreen.activeSelf)
            {
                //Bring back the mouse
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                //Go to the credits
                SceneManager.LoadScene(3);
            }
        }
	}

    //Show the endScreen gameObject with the new and updated Text
    public void UpdateAndShowEndScreen()
    {
        endScreen.SetActive(true);
        numEvidenceFoundText.text = "You found " + endScreenData.GetNumEvidenceFound().ToString() + " pieces of evidence";
        questionScoreText.text = endScreenData.GetQuestionScore().ToString() + " / 5";
    }
}
