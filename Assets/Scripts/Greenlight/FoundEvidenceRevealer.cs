using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoundEvidenceRevealer : MonoBehaviour {
    EndScreenData endScreenData;
    List<string> EndScreenDataStringList;

    [SerializeField]
    List<GameObject> evidenceIcons;

    private void Start()
    {
        endScreenData = FindObjectOfType<EndScreenData>();
        EndScreenDataStringList = endScreenData.GetFoundEvidenceStringList();

        RevealFoundEvidence(); //might need to be moved somewhere else
    }


    public void RevealFoundEvidence()
    {
        foreach (GameObject evidenceIcon in evidenceIcons)
        {
            if (EndScreenDataStringList.Contains(evidenceIcon.name))
            {
                //Show the text and make the image visible
                Text text;
                text = evidenceIcon.GetComponentInChildren<Text>();
                text.color = Color.white;

                Image image;
                image = evidenceIcon.GetComponentInChildren<Image>();
                image.color = Color.white;
            }
            else
            {
                //Hide the text and make the image black
                Text text;
                text = evidenceIcon.GetComponentInChildren<Text>();
                text.color = Color.clear;

                Image image;
                image = evidenceIcon.GetComponentInChildren<Image>();
                image.color = Color.black;
            }
        }
     
    }
}
