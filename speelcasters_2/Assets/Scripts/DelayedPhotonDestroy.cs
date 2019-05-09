using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedPhotonDestroy : MonoBehaviour {

	private float startTime;
	public float destroyTime = 5;

	// Use this for initialization
	void Start () {
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if ((Time.time - startTime) > destroyTime)
			PhotonNetwork.Destroy(GetComponent<PhotonView>());
	}
}
