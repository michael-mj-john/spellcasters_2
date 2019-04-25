using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureRigEnabled : MonoBehaviour {
    private GameObject camRig;
    private SpellcastingGestureRecognition spellcast;
    private SteamVR_TrackedObject tracked;
    public bool left;
	// Use this for initialization
	void Start () {
        camRig = transform.parent.gameObject;
        spellcast = camRig.GetComponent<SpellcastingGestureRecognition>();
        tracked = GetComponent<SteamVR_TrackedObject>();

        if (left)
        {
            spellcast.leftControllerIndex = (int)tracked.index;
        }
        else
        {
            spellcast.rightControllerIndex = (int)tracked.index;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
    private void OnEnable()
    {
        //print("enabled");
        camRig = transform.parent.gameObject;
       camRig.GetComponent<Edwon.VR.VRGestureRig>().Init();
    }
}
