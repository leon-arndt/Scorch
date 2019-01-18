using UnityEngine;
using System.Collections;

public class ForestAnimalBehavior : MonoBehaviour
{
    GameObject player;
    Animator animationController;
    
    //is the animal fleeing from the player
    bool fleeing = false;

    //How fast is the animal
    float speed = 3.7f; //was 4f

    //When does it start fleeing
    [SerializeField]
    float distanceThreshold = 7.0f;

    //animator


    private void Start()
    {
        player = FindObjectOfType<PlayerController>().gameObject;
        animationController = GetComponent<Animator>();
    }
    void Update()
    {
        //Start fleeing when the player gets too close
        if (Vector3.Distance(transform.position, player.transform.position) < distanceThreshold)
        {
            fleeing = true;

            //This ensure that birds fly with different animation cycles in swarms
            Invoke("StartFleeAnimation", Random.value);
        }
        else if (Vector3.Distance(transform.position, player.transform.position) > 100.0f && fleeing)
        {
            //Delete the gameObject when it is far away to save memory
            Destroy(gameObject);
        }


        //Move away from the player
        if (fleeing)
        {
            float step = speed * Time.deltaTime;

            //Moving forward and up constantly
            transform.Translate(step * ((0.5f * Vector3.up) + Vector3.forward));

            //Moving up and down to enhance realism
            transform.Translate(0.5f * step * Vector3.up * Mathf.Sin((5f) * Time.realtimeSinceStartup)); //1 step, 5f

            //Animator anim = transform.GetChild(0).GetComponent<Animator>();
        }
    }

    private void StartFleeAnimation()
    {
        animationController.Play("Flee");
    }
}