using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorSpell : MonoBehaviour
{

	private SpellcastingGestureRecognition spellcastingGesture;
	private PhotonView photonView;
	private LineRenderer line;
	public GameObject reticle;
	GameObject reticleInstance;
	private Vector3 steeringDirection;
	private Vector3[] points = new Vector3[2];
	public bool mine = false;
	public GameObject explosion;
    private bool deflected;

    [HideInInspector]
    public SpellcastingGestureRecognition spellcast;

    public LayerMask targettable;
	public GameObject wand;
	private float damage = 20;
	private float castDist = 10;
	private float magCap = 10;
	private float skyCap = 15;
	private float groundCap = 40;
	public float duration = 12f;
	public Vector3 direction; //Sets the direction to where the fireball is traveling to.
	public float speed = 10f;
	public float startup = 0.25f;
	public float minLinearVelocity = 5f;
	public float minAngularVelocity = 20f;
	private float steeringForceSky = 10000;
	private float steeringForceGround = 15000;
    private float reflectForce = 100;
    private float initialForce = 500;
    private float initialForceDist = 2;
    public Rigidbody rb;
    bool first = true;
	public AudioSource audioSource;
	public AudioClip deflectAudio;
	[HideInInspector]
	public bool isMaster;
	[HideInInspector]
	public bool isSlave;

	[HideInInspector]
	public bool blue = false;

	//    [SerializeField]
	private float activeTimer = 0;
	//    [SerializeField]
	private SphereCollider fbCollider;
	// Use this for initialization
	void Start()
	{
		photonView = GetComponent<PhotonView> ();
		if (photonView.isMine) 
		{
			reticleInstance = Instantiate (reticle, transform.position, transform.rotation) as GameObject;
			mine = true;
			wand = GameObject.Find ("Controller (right)");
			spellcastingGesture = Camera.main.transform.parent.GetComponent<SpellcastingGestureRecognition> ();
			//spellcastingGesture.enabled = false;
			rb = GetComponent<Rigidbody> ();
			rb.AddForce (transform.forward * 1000);
			line = GetComponent<LineRenderer> ();
		}
		//Destroy(this.gameObject, duration);
		if (startup > 0)
			activeTimer = startup;
		if (fbCollider == null)
			fbCollider = this.GetComponent<SphereCollider> ();
		if (audioSource == null)
			audioSource = this.GetComponent<AudioSource> ();

        if (transform.rotation.x <= 45)
        {
            transform.rotation = Quaternion.Euler(45,0,0);
        }

	}

	// Update is called once per frame
	void Update()
	{
        if (mine == true) 
		{
            SteamVR_Controller.Input(spellcast.rightControllerIndex).TriggerHapticPulse(300);SteamVR_Controller.Input(spellcast.rightControllerIndex).TriggerHapticPulse(300);
        }

		//Timer to activate collider.
		if (activeTimer > 0)
			activeTimer -= Time.deltaTime;
		else if (!fbCollider.enabled)
			fbCollider.enabled = true;
	}
	private void FixedUpdate()
	{
		// If this fireball belongs to me
		if (mine == true)
		{
            if (deflected == false)
            {
                points[0] = transform.position;
                points[1] = wand.transform.position + wand.transform.forward * .15f;
                line.SetPositions(points);

                Vector3 fwd = wand.transform.TransformDirection(Vector3.forward);
                RaycastHit hit;

                if (Vector3.Distance(wand.transform.position, transform.position) < initialForceDist)
                {
                    rb.AddForce(Vector3.up * initialForce);
                }

                // If raycast hit
                if (Physics.Raycast(wand.transform.position, wand.transform.forward, out hit, 1000, targettable) && Vector3.Distance(wand.transform.position, hit.point) < 40)
                {

                    //print(Vector3.Distance(wand.transform.position, hit.point));
                    if (hit.transform.gameObject.tag == ("Spell"))
                    {
                        return;
                    }
                    Quaternion rotation = Quaternion.LookRotation(hit.point - transform.position);
                    steeringDirection = hit.point - transform.position;
                    magCap = groundCap;
                    rb.AddForce(steeringDirection.normalized * steeringForceGround * Time.smoothDeltaTime);
                    reticleInstance.transform.position = hit.point;
                }
                // If raycast didn't hit
                else
                {
                    steeringDirection = (wand.transform.position + (wand.transform.forward * 20 - transform.position));
                    magCap = skyCap;
                    rb.AddForce(steeringDirection.normalized * steeringForceSky * Time.smoothDeltaTime);
                    reticleInstance.transform.position = wand.transform.position + (wand.transform.forward * 20);
                }
                rb.velocity = clampVector(rb.velocity);

                Debug.DrawRay(wand.transform.position, fwd * 1000, Color.green);
            }
		}
	}

	Vector3 clampVector(Vector3 myVector)
	{
		if (myVector.magnitude > magCap) 
		{
			float multiplier = magCap / myVector.magnitude;
			myVector.x = myVector.x * multiplier;
			myVector.y = myVector.y * multiplier;
			myVector.z = myVector.z * multiplier;
		}

		return myVector;
	}

    private void OnCollisionEnter(Collision collision)
    {

        if (GetComponent<PhotonView>().isMine)
        {
            // If it's a shield, deflect
            if (collision.transform.tag == "Shield")
            {
                GameObject newExplosion = PhotonNetwork.Instantiate(explosion.name, this.transform.position, new Quaternion(), 0);
                DestroyNoDamage();
            }
            //    if (collision.transform.GetComponent<Shield>())
            //    {
            //        // If the shield we hit is the enemy's
            //        if (collision.transform.GetComponent<Shield>().GetBlue() != blue)
            //        {
            //            Reflect();
            //        }
            //        else
            //        {
            //            Physics.IgnoreCollision(GetComponent<SphereCollider>(), collision.transform.GetComponent<BoxCollider>(), true);
            //        }

            //    }
            //    else if (collision.transform.GetComponent<Pong_Shield>())
            //    {
            //        // If the shield we hit is the enemy's
            //        if (collision.transform.GetComponent<Pong_Shield>().GetBlue() != blue)
            //        {
            //            Reflect();
            //        }
            //        else
            //        {
            //            Physics.IgnoreCollision(GetComponent<SphereCollider>(), collision.transform.GetComponent<BoxCollider>(), true);
            //        }
            //    }
            else
            {
                GameObject newExplosion = PhotonNetwork.Instantiate(explosion.name, this.transform.position, new Quaternion(), 0);
                DestroyFireball();
            }
            //}
        }
    }
		


	private void OnTriggerEnter(Collider collider)
	{
	}
	
	void DestroyFireball()
	{
		//Destroy game object.
		//        PhotonNetwork.Destroy(this.gameObject);

//	
		Collider[] hits;
		hits = Physics.OverlapSphere(transform.position, 6, int.MaxValue, QueryTriggerInteraction.Collide);
		foreach (Collider hit in hits)
		{
			if (hit.transform.tag == "Player")
			{
                if (hit.transform.parent.GetComponent<TeamManager>().blue != blue)
                    hit.gameObject.GetPhotonView().RPC("TakeDamage", PhotonTargets.AllBuffered, damage);
                
			}
            else if(hit.transform.tag == "Curse")
            {
                if (hit.transform.GetComponent<VineTrap>().blue != blue)
                    hit.gameObject.GetPhotonView().RPC("DestroyVines", PhotonTargets.All);
            }

		}

        Destroy(reticleInstance);
        //spellcastingGesture.enabled = true;
        spellcast.Vibrate(.1f, 3999);
        PhotonNetwork.Destroy (this.GetComponent<PhotonView> ());
	}

    void DestroyNoDamage()
    {
        //Destroy game object.
        //        PhotonNetwork.Destroy(this.gameObject);

        //

        Destroy(reticleInstance);
        //spellcastingGesture.enabled = true;
        spellcast.Vibrate(.1f, 3999);
        PhotonNetwork.Destroy(this.GetComponent<PhotonView>());
    }

    void Reflect()
    {
        deflected = true;
        Destroy(reticle);
        line.enabled = false;
        rb.velocity = Vector3.zero;
        transform.LookAt(Camera.main.transform);
        blue = !blue;
        rb.AddForce((Camera.main.transform.position - transform.position) * reflectForce);
        if (deflectAudio != null) audioSource.PlayOneShot(deflectAudio);
    }
}