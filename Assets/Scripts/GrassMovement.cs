using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// http://answers.unity3d.com/questions/1137004/rotate-rigidbody-on-collision.html

public class GrassMovement : MonoBehaviour {
    public bool coll;
    Vector3 grassFall = new Vector3();
    Rigidbody rb;
    public float speed = 10f;

    private Unit.Direction direction = Unit.Direction.North;


    // Use this for initialization
    void Start () {
        coll = false;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag.Equals("Player"))
        {
            Unit player = col.transform.parent.gameObject.GetComponent<Unit>();
            direction = player.lastDirection;
            switch(direction)
            {
                case Unit.Direction.North:
                    grassFall = Vector3.right * 30;
                    break;
                case Unit.Direction.South:
                    grassFall = Vector3.left * 30;
                    break;
                case Unit.Direction.East:
                    grassFall = Vector3.forward * 30;
                    break;
                case Unit.Direction.West:
                    grassFall = Vector3.back * 30;
                    break;
                case Unit.Direction.NE:
                    grassFall = new Vector3(1, 0, -1);
                    break;
                case Unit.Direction.NW:
                    grassFall = new Vector3(1, 0, 1);
                    break;
                case Unit.Direction.SE:
                    grassFall = new Vector3(-1, 0, -1);
                    break;
                case Unit.Direction.SW:
                    grassFall = new Vector3(-1, 0, 1);
                    break;

                default:
                    break;
            }
            coll = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        coll = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (coll)
        {
            if (transform.GetChild(0).Find("grass").transform.eulerAngles.x < 40 &&
                transform.GetChild(0).Find("grass").transform.eulerAngles.x > -40 &&
                transform.GetChild(0).Find("grass").transform.eulerAngles.z < 40 &&
                transform.GetChild(0).Find("grass").transform.eulerAngles.z > -40)
            {
                //Quaternion deltaRotation = Quaternion.Euler(grassFall * Time.deltaTime);
                // transform.GetChild(0).FindChild("grass").Rotate(grassFall);
                transform.GetChild(0).Find("grass").rotation = Quaternion.AngleAxis(40, grassFall);
            }
        }
        else
        {
            transform.GetChild(0).Find("grass").transform.rotation = Quaternion.Slerp(transform.GetChild(0).Find("grass").transform.rotation, Quaternion.identity, Time.deltaTime*15);
        }
    }
}
