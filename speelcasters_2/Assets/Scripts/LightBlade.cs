using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBlade : MonoBehaviour {

    public GameObject hitSpark;
	public Transform wand;
	private bool blue;
    private bool isDecaying = false;
    public float duration = 4;
	public float destroyTime = 5;
    public float hitBonusTime = 0.05f;
    private float durationTimer = 0;
	private float startTime;
	public float damage = 100;


	// Use this for initialization
	void Start () 
	{
		startTime = Time.time;	
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
			this.transform.position = wand.position;
			this.transform.rotation = wand.rotation;

            //Check if sword has lasted 
            if (isDecaying)
            {
                if (durationTimer > 0)
                    durationTimer -= Time.deltaTime;
                else
                    PhotonNetwork.Destroy(GetComponent<PhotonView>());
            }
            else
            {
                if((Time.time - startTime) > destroyTime)
                {
                    PhotonNetwork.Destroy(GetComponent<PhotonView>());
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
			if (GetComponent<PhotonView> ().isMine) 
			{
				if (other.transform.parent.GetComponent<TeamManager> ().blue != blue) 
				{
					other.gameObject.GetPhotonView().RPC("TakeDamage", PhotonTargets.AllBuffered, damage);
					PhotonNetwork.Instantiate (hitSpark.name, other.transform.position, new Quaternion (), 0);

                    if (!isDecaying)
                    { 
                        durationTimer = duration;
                        isDecaying = true;
                    }
				}
			}
        }
    }

	public void SetWand(Transform wand_)
	{
		wand = wand_;
	}

	public void SetBlue(bool blue_)
	{
		blue = blue_;
	}
}
