using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectsManager : MonoBehaviour {
    /* This script is used to randomly spawn leaves and to show the Fireflies at night 
     * It carries a reference to the dayNightCycle to figure out when to show the fireflies
     */

    // Empty game object which holds all the fireflies particle systems as children
    [SerializeField]
    DayNightCycle dayNightCycle;

    [SerializeField]
    GameObject firefliesGroup;

    // Stores the prefab for the leaves particle system
    [SerializeField]
    GameObject leavesParticlePrefab;

    // Direct reference to the player
    [SerializeField]
    GameObject player;


	// Use this for initialization
	void Start () {
        InvokeRepeating("CheckWhetherToSpawn", 0f, 10f); //spawn leaves every 10 seconds
        firefliesGroup.SetActive(false); //Hide the fireflies
	}

    /*void OnTriggerEnter(Collider other)
    {
        if (GetComponent<Collider>().bounds.Contains(player.transform.position))
        {
            print("player is inside the collider");
        }
    }*/


    void CheckWhetherToSpawn()
    {
        bool isNight = Time.timeSinceLevelLoad > 0.7f * dayNightCycle.GetGameDuration();
        Debug.Log("Night is" + isNight.ToString());

        Debug.Log("Checking whether to spawn");
        if (!GetComponent<Collider>().bounds.Contains(player.transform.position) && (!isNight))
        {
            Debug.Log("Leaves spawned");
            GameObject leaves = Instantiate(leavesParticlePrefab, player.transform.position + 10 * Vector3.up, Quaternion.Euler(new Vector3(-90, 0, 0))) as GameObject;
            leaves.transform.SetParent(firefliesGroup.transform.parent); //reparents it to the right group
        } else
        {
            Debug.Log("Player is inside the collider");
        }


        //Also check whether to turn on the fireflies
        if (isNight)
        {
            firefliesGroup.SetActive(true);

            //Delete the one set of permanent leaves
            if (transform.childCount > 0)
            {
                Destroy(gameObject.transform.GetChild(0).gameObject);
            }
        }
    }
}
