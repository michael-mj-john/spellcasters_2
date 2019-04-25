using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.VR;

[RequireComponent(typeof(SteamVR_TrackedObject))]

public class PickupParent : MonoBehaviour
{
	SteamVR_TrackedObject trackedObj;
	SteamVR_Controller.Device device;

	GameObject[] Grabbables;
	GameObject grabbables;
	GameObject grabbed;
	HatLogic heldHat;
	GameObject head;
	GameObject ears;
	GameObject origin;
	GameObject target;
	public GameObject button;

	float pickupTime;

	public Animator buttonAnim;
	Animator whiteout;
	List<Collider> TriggerList;


	float audio2Volume;
	float audio1Volume;

	bool fadeIn;
	bool fadeOut;

	AudioSource ambient;
	AudioSource foley;

	AudioClip[] sounds;
	bool isOculus = false;

	public Transform ball;
	private bool endingPlayed;
	private bool releaseHat;
	private float releaseTime;
	float triggerR;

	public bool inHand = false;

	void Awake()
	{
		trackedObj = GetComponent<SteamVR_TrackedObject> ();

	}

	void Start()
	{
		if (VRDevice.model.ToLower ().Contains ("oculus"))
		{
			isOculus = true;
		}
	}


	void Update()
	{
		device = SteamVR_Controller.Input ((int)trackedObj.index);

		if(isOculus)
		{
			triggerR = Input.GetAxis ("OculusRightTrigger");

			if (triggerR < 0.35f && grabbed != null)
			{
				tossObject (grabbed.GetComponent<Rigidbody>());
			}
		}


		// Drop object
		if (Input.GetKeyUp("joystick button 15"))
		{
			if (grabbed != null) 
			{
				tossObject (grabbed.GetComponent<Rigidbody>());
			}
		}
	}


	// Pickup logic
	void OnTriggerStay(Collider col)
	{

		if ((Input.GetKeyDown("joystick button 15") && !isOculus) || (isOculus && triggerR > 0.35f))
		{
			if (grabbed == null)
			{
                if (col.GetComponent<HatLogic>())
				{
					heldHat = col.GetComponent<HatLogic> ();
					if (heldHat.onHead == false)
					{
						if (heldHat.held == false)
						{
							inHand = true;
							heldHat.held = true;
                            heldHat.GetComponent<PhotonView>().RequestOwnership();
                            heldHat.GetComponent<PhotonView>().RPC("onHeadTrue", PhotonTargets.AllBuffered, false);
                            heldHat.GetComponent<PhotonView>().RPC("onHandTrue", PhotonTargets.AllBuffered, true);
                            col.GetComponent<Rigidbody>().isKinematic = true;
							//col.gameObject.transform.SetParent(gameObject.transform);
							grabbed = col.gameObject;
							heldHat = col.GetComponent<HatLogic> ();
							heldHat.held = true;
                            heldHat.hand = this.gameObject.transform;
							pickupTime = Time.time;						
							col.GetComponent<HatLogic> ().takeOffHat();
						}
					}
				}
			}
		}
	}

	void tossObject(Rigidbody rigidBody)
	{
		//rigidBody.isKinematic = false;
		//grabbed.gameObject.transform.SetParent(null);
		grabbed = null;
		inHand = false;
		heldHat.GetComponent<PhotonView>().RPC("tossObject", PhotonTargets.AllBuffered);
        heldHat = null;
		if (device !=null)
			rigidBody.velocity = device.velocity * 1.2f;
			//GetComponent<Rigidbody> ().velocity;
	}
}