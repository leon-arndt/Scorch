using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceDisplay : MonoBehaviour {
    [SerializeField]
    private Resource resource;

    [SerializeField]
    private Image resourceIcon;

    [SerializeField]
    private Text nameText;

    private bool found = false;
        
    //private Text contentText;

	// Use this for initialization
	void Start () {
        resourceIcon.color = new Color(0f, 0f, 0f, 0f); //transparent
        resourceIcon.transform.GetChild(0).gameObject.SetActive(false); //hides the text for the resource

        //retrieves the strings from the Resource object
        nameText.text = resource.GetResourceName();
        //contentText.text = resource.GetResourceContent();
	}

    public string GetResourceName()
    {
        return resource.GetResourceName();
    }

    public string GetResourceContent()
    {
        //mit string replace?
        string newString = resource.GetResourceContent().Replace("\\n", "\n");
        return newString; 
      //  return resource.GetResourceContent();
    }

    public void MakeResourceVisible()
    {
        found = true;
        resourceIcon.color = new Color(1, 1, 1, 1f); //opaque
        resourceIcon.transform.GetChild(0).gameObject.SetActive(true); //shows the text for the resource
    }

    public bool WasFound()
    {
        return found;
    }
}
