using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour, IShield
{

    public float shieldDuration = 10f;

    private float shieldTimer;
    Transform book;
    Transform shieldSpot;
    public bool blue { get; set; }
    public Transform owner { get; set; }
    public Transform hitSpark;


    // Use this for initialization
    void Start()
    {
        shieldTimer = shieldDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.GetComponent<PhotonView>().isMine)
        {
            if (book == null)
            {
                return;
            }

            this.transform.position = (shieldSpot.position);
            this.transform.rotation = shieldSpot.rotation;

            shieldTimer -= Time.deltaTime;

            if (shieldTimer <= 0)
                PhotonNetwork.Destroy(this.GetComponent<PhotonView>());
        }
    }

    public void SetBook(Transform book_)
    {
        book = book_;
        shieldSpot = book.Find("ShieldPt");
    }

    [PunRPC]
    public void SetBlue(bool blue_)
    {
        blue = blue_;
    }
    
    public bool GetBlue()
    {
        return blue;
    }

    [PunRPC]
    //Reduces the health by the damage received.
    public void DestroyShield()
    {
        shieldTimer = 0;

        if(hitSpark != null)
            PhotonNetwork.Instantiate(hitSpark.name, transform.position, new Quaternion(), 0);
    }
}
