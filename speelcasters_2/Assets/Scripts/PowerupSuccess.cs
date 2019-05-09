using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupSuccess : MonoBehaviour {
    PhotonView pv;
	// Use this for initialization
	void Start () {
        pv = GetComponent<PhotonView>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(5);
        PhotonNetwork.Destroy(pv);
    }
}
