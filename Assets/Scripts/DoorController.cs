using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour {

    Animator animator;
    bool doorOpen;
    
    void Start()
    {
        doorOpen = false;
        animator = GetComponentInParent<Animator>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
     //       doorOpen = true;
   //         Doors("Open");
        }    
    }

    void OnTriggerExit(Collider col)
    {
        // Can close the doors if you want here.
    }

    void Doors(string direction)
    {
        //animator.SetTrigger(direction);
    }
}
