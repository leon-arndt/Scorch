using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DP_Evidence : MonoBehaviour {
    // Fields which define the characterstics of the evidence
    [SerializeField]
    string evidenceName;

    [SerializeField]
    Sprite sprite;

    int evidenceValue; //positive or negative, used for evaluating


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public string GetEvidenceName() {
        return evidenceName;
    }

    public int GetEvidenceValue() {
        return evidenceValue;
    }

    public Sprite GetEvidenceSprite() {
        return sprite;
    }
}
