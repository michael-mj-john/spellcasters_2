using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassHammer : MonoBehaviour
{

    public GameObject hitSpark;
    public Transform wand;
    public float distanceFromHand = 2;
    private bool blue;
    public float destroyTime = 15;
    private float startTime;
    public float damage = 20;
    public float hammerRadius = 5;
    private Rigidbody rb;


    // Use this for initialization
    void Start()
    {
        startTime = Time.time;
        rb = this.GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        if (this.GetComponent<PhotonView>().isMine)
        {
            if (wand == null)
            {
                return;
            }
            if ((Time.time - startTime) > destroyTime)
                PhotonNetwork.Destroy(GetComponent<PhotonView>());
        }
    }
    private void FixedUpdate()
    {
        if (this.GetComponent<PhotonView>().isMine)
        {
            if (wand == null)
            {
                return;
            }


            this.transform.position = wand.position + wand.forward * distanceFromHand;
            this.transform.rotation = wand.rotation;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        PhotonView otherOwner;
        ITeamOwned teamOwnedShield;
        /*
        if (other.gameObject.CompareTag("Shield"))
        {
            if (GetComponent<PhotonView>().isMine)
            {
                //Assign team owned object.
                teamOwnedShield = other.gameObject.GetComponent<ITeamOwned>();
                print("SHIELD? : " + teamOwnedShield);

                //Check if it's from a different our team color.
                if (teamOwnedShield.GetBlue() != blue)
                {
                    print(teamOwnedShield.GetBlue() + " | " + blue);
                    other.gameObject.GetPhotonView().RPC("DestroyShield", PhotonTargets.AllBuffered);
                    PhotonNetwork.Instantiate(hitSpark.name, other.transform.position, new Quaternion(), 0);
                    PhotonNetwork.Destroy(GetComponent<PhotonView>());

                    
                    //Find if it has an owner with a photon view.
                    print("SHIELD OWNER: " + teamOwnedShield.owner.gameObject);
                    if (teamOwnedShield.owner.gameObject.GetPhotonView() != null)
                    {
                        //Assign it to otherOwner.
                        otherOwner = teamOwnedShield.owner.gameObject.GetPhotonView();
                        print("Shield Owner is: " + otherOwner);

                        //RPC call to take damage and destroy shield.
                        otherOwner.RPC("TakeDamage", PhotonTargets.AllBuffered, damage);
                        other.gameObject.GetPhotonView().RPC("DestroyShield", PhotonTargets.AllBuffered);
                        PhotonNetwork.Instantiate(hitSpark.name, other.transform.position, new Quaternion(), 0);
                        PhotonNetwork.Destroy(GetComponent<PhotonView>());
                    }
                    else
                    {
                        return;
                    }
                    
                }
            }
        }*/
        //Check if it's a player.
        if (other.gameObject.CompareTag("Player"))
        {
            print("It's a player!");

            //Check if I own this hammer.
            if (GetComponent<PhotonView>().isMine)
            {
                //Check if the hammer and the player are on different teams.
                if (other.transform.parent.GetComponent<TeamManager>().blue != blue)
                {
                    //Find any nearby shields and destroy them.
                    Collider[] hits;
                    hits = Physics.OverlapSphere(other.transform.position, hammerRadius);
                    foreach (Collider hit in hits)
                    {
                        //If the hit is a Shield.
                        if (hit.transform.tag == "Shield")
                        {
                            if (hit.transform.GetComponent<IShield>().GetBlue() != this.blue)
                            {
                                hit.gameObject.GetPhotonView().RPC("DestroyShield", PhotonTargets.AllBuffered);
                            }
                            else
                            {

                            }
                        }
                    }

                    //Damage player.
                    other.gameObject.GetPhotonView().RPC("TakeDamage", PhotonTargets.AllBuffered, damage);
                    PhotonNetwork.Instantiate(hitSpark.name, other.transform.position, new Quaternion(), 0);
                    PhotonNetwork.Destroy(GetComponent<PhotonView>());
                }
            }
        }

    }


    public void SetWand(Transform wand_) 
    {
        wand = wand_;
        this.transform.position = wand.position + wand.forward * distanceFromHand;
        this.transform.rotation = wand.rotation;
    }

    [PunRPC]
    public void SetBlue(bool blue_)
    {
        blue = blue_;
    }
}
