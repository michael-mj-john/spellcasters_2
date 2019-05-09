using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBall_2 : MonoBehaviour {
    public ParticleSystem ps;
    PhotonView photonView;
    bool doDamage = true;
    public bool blue;
	// Use this for initialization
	void Start () {
        //ps = GetComponentInChildren<ParticleSystem>();
        photonView = GetComponent<PhotonView>();
        StartCoroutine(AOE_Timer());
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
	}

    IEnumerator AOE_Timer()
    {
        Wave();
        yield return new WaitForSeconds(1);
        Wave();
        yield return new WaitForSeconds(1);
        Wave();
        yield return new WaitForSeconds(1);
        Wave();
        yield return new WaitForSeconds(1);
        ps.Stop();
        yield return new WaitForSeconds(4);
        PhotonNetwork.Destroy(photonView);
    }
    void Wave()
    {
        if (GetComponent<PhotonView>().isMine)
        {
            Collider[] hits;
            hits = Physics.OverlapSphere(transform.position, 5);
            foreach (Collider hit in hits)
            {
                if (hit.transform.tag == "Player")
                {
                    if (hit.transform.parent.GetComponent<TeamManager>().blue != blue)
                    {
                        hit.gameObject.GetPhotonView().RPC("TakeDamage", PhotonTargets.AllBuffered, 10f);
                    }
                    else
                    {
                    }
                }
            }
        }
    }
}
