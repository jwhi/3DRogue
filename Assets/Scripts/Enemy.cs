using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	void Update () {
        GameObject playerIcon = GameObject.FindGameObjectWithTag("Player");
        //Debug.Log(playerIcon.name);
        gameObject.transform.GetChild(0).transform.rotation = playerIcon.transform.rotation;
    }
}
