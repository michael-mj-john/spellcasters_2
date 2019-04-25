using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble_shield : MonoBehaviour, IShield
{
   
    public float Bubble_shieldDuration = 10f;
    private float Bubble_shieldTimer;
    Transform torso;
    Collider other;
    public bool blue { get; set; }
    public Transform owner { get; set; }
    public Transform hitSpark;

    //Transform Bubble_shieldSpot;

    // Use this for initialization
    void Start()
    {
        Bubble_shieldTimer = Bubble_shieldDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.GetComponent<PhotonView>().isMine)
        {
            if (torso == null)
            {
                return;
            }

            this.transform.position = torso.position;
            this.transform.rotation = torso.rotation;

            if (Bubble_shieldTimer <= 0)
                PhotonNetwork.Destroy(gameObject);
            else
            Bubble_shieldTimer -= Time.deltaTime;
        }
    }

    public void SetTorso(Transform torso_)
    {
        torso = torso_;
        torso.GetComponent<PhotonView>().RPC("set_BubbleShield", PhotonTargets.All, Bubble_shieldDuration);
        //Bubble_shieldSpot = torso.Find("Bubble_ShieldPt");
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
        Bubble_shieldTimer = 0;
        torso.GetComponent<PhotonView>().RPC("unset_BubbleShield", PhotonTargets.All);

        if (hitSpark != null)
            PhotonNetwork.Instantiate(hitSpark.name, transform.position, new Quaternion(), 0);
    }
}
