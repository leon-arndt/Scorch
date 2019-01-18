using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to create scriptable objects for the resources found in the phone which contain some of the dialogs
/// </summary>



[CreateAssetMenu(fileName ="New Resource", menuName = "Scorch Custom/Resource")]
public class Resource : ScriptableObject {
    [SerializeField]
    private string resourceName;

    [SerializeField]
    private string content;

    public int test;


    public string GetResourceName()
    {
        return resourceName;
    }

    public string GetResourceContent()
    {
        return content;
    }
}
