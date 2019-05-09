using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBall_1 : MonoBehaviour {
    public GameObject IceBall_2;
    public bool blue;
    private float speed = 7.5f;
    private int damage = 10;
    private bool mine;
    private bool deflected;
    private float reflectForce = 100;
    public Rigidbody rb;
    public AudioClip deflectAudio;
    public AudioSource audioSource;

    [HideInInspector]
    public SpellcastingGestureRecognition spellcast;

    PhotonView photonView;
	// Use this for initialization
	void Start () {
        photonView = GetComponent<PhotonView>();
        StartCoroutine(lifetime(5));
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();

        if (photonView.isMine)
        {
            mine = true;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (mine == true)
            SteamVR_Controller.Input(spellcast.rightControllerIndex).TriggerHapticPulse(300);


        if (Input.GetKeyDown("joystick button 15") && photonView.isMine && !deflected) 
        {
            GameObject ib2 = PhotonNetwork.Instantiate(IceBall_2.name, transform.position, Quaternion.identity, 0);
            ib2.GetComponent<IceBall_2>().blue = blue;
            spellcast.Vibrate(.1f, 3999);
            PhotonNetwork.Destroy(photonView);
        }
	}
    private void FixedUpdate()
    {
        this.transform.Translate(this.transform.forward * speed * Time.deltaTime, Space.World);
    }
    private void OnTriggerEnter(Collider col)
    {
        print("entering trigger");
        if (col.gameObject.CompareTag("Shield"))
        {
            print("entering trigge2r");
            if (col.transform.GetComponent<Shield>())
            {
                print("entering trigger3");
                if (col.transform.GetComponent<Shield>().GetBlue() != blue)
                {
                    print("blue");
                    print("hit on shield");
                    //Apply damage to the shield.
                    Damageable damageScript = col.gameObject.GetComponent<Damageable>();
                    if (damageScript != null) damageScript.TakeDamage(damage);

                    if (GetComponent<PhotonView>().isMine)
                    {
                        Reflect();
                    }
                }

                else
                {

                }
            }
            else
            {
                //Instantiate new explosion.
                PhotonNetwork.Destroy(photonView);
            }
        }
            //Instantiate new explosion.
          //  PhotonNetwork.Destroy(photonView);
        
    }
    IEnumerator lifetime(float duration = 4)
    {
        yield return new WaitForSeconds(duration);
        GameObject ib2 = PhotonNetwork.Instantiate(IceBall_2.name, transform.position, Quaternion.identity, 0);
        ib2.GetComponent<IceBall_2>().blue = blue;
     //   print("BLUE:" + blue);
      //  print("HIS BLUE: " + ib2.GetComponent<IceBall_2>().blue);
        spellcast.Vibrate(.1f, 3999);

        if (mine == true)
            PhotonNetwork.Destroy(photonView);
    }

    void Reflect()
    {
        StopCoroutine("lifetime");
        StartCoroutine("lifetime", 2);
        deflected = true;
        rb.velocity = Vector3.zero;
        transform.LookAt(Camera.main.transform);
        speed *= 2; //Make reflected ball travel faster.
        blue = !blue;
        // rb.AddForce((Camera.main.transform.position - transform.position) * reflectForce);
        if (deflectAudio != null) audioSource.PlayOneShot(deflectAudio);
    }

}
