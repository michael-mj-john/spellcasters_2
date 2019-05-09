using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum PlayerClass {none, attack, heal, support, all}; 

public class HatLogic : MonoBehaviour {


	public Material attackerMat;
	public Material healerMat;
	public Material supportMat;

	public PlayerClass playerClass = PlayerClass.none;
    public GameObject initparent;
    public Transform hand;
    public Transform head;
	public GameObject torso;
	public bool onHead = false;
	public bool touchingHead = false;
	public bool held = false;

	public bool resettable = false;
	public bool releaseHat = false;
	public float releaseTime = 0f;
	float timer;
    float resetDist = .6f;
    float resetTime = .5f;
	Vector3 startPosition;
	Quaternion startRotation;

	Transform hatSpot;

	PickupParent wand;

	PhotonView photonView;

	private Renderer rend;

	// Use this for initialization
	public void Start()
	{
		timer = 0;

		startPosition = gameObject.transform.position;
		startRotation = gameObject.transform.rotation;
		// print("start position is: " + startPosition);
	}

    // Update is called once per frame
    void Update()
    {
        if (onHead && GetComponent<PhotonView>().isMine)
        {
            if (head != null)
            {
                transform.SetPositionAndRotation(head.position, head.rotation);
            }
        }

        if (held == true && GetComponent<PhotonView>().isMine)
        {

            if (hand != null)
            {
                transform.position = hand.position + 0.4f * hand.forward;
                transform.rotation = hand.rotation;
            }
        }

        if (releaseHat == true)
        {
            if ((Time.time - releaseTime) > .5f)
            {
                held = false;
                releaseHat = false;
            }
        }

        if (GameObject.Find("Controller (right)") == null)
            return;

        wand = GameObject.Find("Controller (right)").GetComponent<PickupParent>();
        resettable = !onHead && !held;

        if (resettable && timer < resetTime && Vector3.Distance(startPosition, transform.position) > resetDist)
        {
            timer += Time.deltaTime;
        }


        if (held == false && timer >= resetTime)
        {
            resetHat();
            resettable = false;
        }

        if (!resettable)
        {
            timer = 0;
        }
    }

    // Detect the collision of hat and head
    void OnCollisionEnter(Collision other)
    {
		if (other.gameObject.tag == "put" && this.GetComponent<PhotonView>().isMine && other.gameObject.GetComponent<PhotonView>().isMine)
		{
            //Debug.Log("HATLOGIC photon view is mine");
			if (held == true) 
			{
				//print ("touching head");
				hatSpot = other.transform.Find("hatSpot");
				putOnHat ();
			}
		}
    }

	void OnCollisionExit(Collision other)
	{
		if (other.gameObject.tag == "put") 
		{
			touchingHead = false;
			hatSpot = null;
		}
	}

	public void putOnHat()
	{
        if (hatSpot.transform.parent.GetComponent<PhotonView>().isMine)
        {
            GameObject.Find("Announcer").GetComponent<AnnouncerEvents>().PlaySound("putOnHat");
            //Debug.Log("should be speaking");

            photonView.RPC("onHandTrue", PhotonTargets.AllBuffered, false);
            photonView.RPC("onHeadTrue", PhotonTargets.AllBuffered, true);
            head = hatSpot;
            torso = hatSpot.parent.parent.Find("Torso").gameObject;

            //            torso.GetComponent<PlayerStatus>().RemoveHat();

            // remove previous hat (if at all) and then set this as new hat
            PlayerStatus ps = torso.GetComponent<PlayerStatus>();
            ps.RemoveHat();
            ps.hat = this.gameObject;

            
            this.GetComponent<Rigidbody>().isKinematic = true;

            torso.GetComponent<PhotonView>().RPC("SetClass", PhotonTargets.AllBuffered, playerClass);

            // Search for the child hat in player
            foreach (Transform child in hatSpot)
                if (child.CompareTag("findHat"))
                {
                    this.transform.position = child.transform.position;
                    this.transform.rotation = child.transform.rotation;
                    onHead = true;
                }
        }
	}

	public void takeOffHat()
	{
		onHead = false;
        head = null;
		//torso.GetComponent<PlayerStatus> ().setClass(PlayerClass.none);
	}
		
	public void callSetClass(PlayerClass pc)
	{
		photonView.RPC("setClass", PhotonTargets.AllBuffered, pc);
	}

    [PunRPC]
    public void onHeadTrue(bool _onHead)
    {
        onHead = _onHead;

        if (onHead == true)
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    [PunRPC]
    public void onHandTrue(bool _held)
    {
        held = _held;

        if (held == true)
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
        else
        {
            //GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    [PunRPC]
	public void setClass(PlayerClass pc)
	{
		playerClass = pc;

		if (playerClass == PlayerClass.heal) {
			if (rend != null)
				rend.material = healerMat;
		}
		else if (playerClass == PlayerClass.attack) {
			if (rend != null)
			rend.material = attackerMat;
		}
		else if (playerClass == PlayerClass.support) {
			if (rend != null)
			rend.material = supportMat;
		}
	}

    [PunRPC]
    void tossObject()
    {
        GetComponent<Rigidbody>().isKinematic = false;
        releaseHat = true;
        releaseTime = Time.time;
        hand = null;
        //GetComponent<Rigidbody> ().velocity;
    }

    void Awake()
	{
		photonView = GetComponent<PhotonView> ();
		rend = GetComponent<Renderer> ();
	}

	public void resetHat()
	{
        //this.GetComponent<Rigidbody>().isKinematic = false;
        //print(onHead +" " + wand.inHand);

        photonView.RPC("onHeadTrue", PhotonTargets.AllBuffered, false);

        // may not be necessary
        resettable = true;
        gameObject.transform.position = startPosition;
		gameObject.transform.rotation = startRotation;

		this.GetComponent<Rigidbody> ().velocity = new Vector3 (0, 0, 0);
	}

    public void SetVisible(bool isVisible)
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).gameObject.SetActive(isVisible);
        }
        this.GetComponent<MeshRenderer>().enabled = isVisible;

        if (isVisible)
        {
            resetHat();
        }
    }
}
