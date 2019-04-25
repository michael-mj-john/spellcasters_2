using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banner : MonoBehaviour {
   
    public string role;
    private Dictionary<string, float> mapping = new Dictionary<string, float>();

	// Use this for initialization
	void Start () {
        mapping.Add("attacker", 0);
        mapping.Add("tank", 0.33333f);
        mapping.Add("healer", 0.666666f);

        this.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(mapping[role], 0);
	}
	
	// Update is called once per frame
	void Update () {
	    	
	}
}
