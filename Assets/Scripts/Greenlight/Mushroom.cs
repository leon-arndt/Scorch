using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mushroom : MonoBehaviour {
    [SerializeField]
    private string mushroomName, mushroomDescription;

    [SerializeField]
    private Sprite sprite;

    public string GetMushroomName()
    {
        return mushroomName;
    }

    public string GetMushroomDescription()
    {
        return mushroomDescription;
    }

    public Sprite GetMushroomSprite()
    {
        return sprite;
    }
}
