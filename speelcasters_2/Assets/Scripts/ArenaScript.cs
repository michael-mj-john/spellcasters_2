using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnDestroy()
    {
        foreach(GameObject curse in GameObject.FindGameObjectsWithTag("Curse"))
        {
            PhotonNetwork.Destroy(curse.GetPhotonView());
        }
    }
}
