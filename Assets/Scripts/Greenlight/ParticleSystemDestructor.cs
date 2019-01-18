using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemDestructor : MonoBehaviour
{
    [SerializeField]
    float destroyAfter;

    // Use this for initialization
    void Start()
    {
        Invoke("Destroy", destroyAfter);
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}