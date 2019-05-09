using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalTracker : MonoBehaviour {

    public Transform guide;
	// Use this for initialization
	void Start () {
        if (guide == null) guide = this.transform.parent;
	}
	
	// Update is called once per frame
	void Update () {
        this.GetComponent<Rigidbody>().MovePosition(guide.position);
        this.GetComponent<Rigidbody>().MoveRotation(guide.rotation);
        //print(this.GetComponent<Rigidbody>().velocity + " : " + this.GetComponent<Rigidbody>().velocity.magnitude);
	}
}
