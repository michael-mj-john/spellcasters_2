using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballSpawner : MonoBehaviour {

    public GameObject fireball;
    public float timeInt = 5f;
    float timeStack = 0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()

    {
        timeStack += Time.deltaTime;

        if (timeStack >= timeInt)
        {
            timeStack = 0;
            Instantiate(fireball, transform.position, transform.rotation);
        }

	}
}
