using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreenData : MonoBehaviour {
    /* This class handles the data it receives from the uicontroller and the playercontroller in a persistant way
     * The gameObject is not destroyed on load
     * The data is shown at the end of the police station monolog in the police station scene
     */

    public List<string> foundEvidenceStringList; //list of the names of the evidence the payer found

    private int numEvidenceFound, questionScore; //made public to be accesible from external Objects


    // Use this for initialization
    void Awake()
    {
        DontDestroyOnLoad(this); //could also try (transform.gameObject) should be safer
        foundEvidenceStringList = new List<string>();
    }


    //Setters
    public void AddToEvidenceStringList(string newEvidenceName)
    {
        foundEvidenceStringList.Add(newEvidenceName);
    }

    public void SetNumEvidenceFound(int num)
    {
        numEvidenceFound = num;
    }

    public void SetNumQuestionsAnsweredCorrectly(int num)
    {
        //numQuestionsAnsweredCorrectly = num;
    }

    public void IncreaseQuestionScoreBy(int num)
    {
        questionScore += num;
        Debug.Log("The new question score is now" + questionScore);
    }



    //Getters
    public List<string> GetFoundEvidenceStringList()
    {
        return foundEvidenceStringList;
    }

    public int GetNumEvidenceFound()
    {
        //return numEvidenceFound;
        return foundEvidenceStringList.Count;
    }

    /*public int GetNumQuestionsAnsweredCorrectly()
    {
        //return numQuestionsAnsweredCorrectly;
    }*/

    public int GetQuestionScore()
    {
        return questionScore;
    }


    /*
    //Show found evidence objects at the end of the game
    public void ShowFoundEvidence()
    {
        foreach (var image in collection)
        {
            
        }
    }*/
}
