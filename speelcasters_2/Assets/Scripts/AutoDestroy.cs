using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour {
    public float duration = 5f;
    public bool InPhoton;
    private float timer = 0;
	// Use this for initialization
	void Start () {
        timer = duration;           
    }
	
	// Update is called once per frame
	void Update () {
        if (timer > 0)
            timer -= Time.deltaTime;
        else
        {
            if(InPhoton)
                PhotonNetwork.Destroy(this.GetComponent<PhotonView>());
            else
                Destroy(this.gameObject);
        }
	}
}
