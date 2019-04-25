using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *                                  !!!!!!!!!!!!    IMPORTANT   !!!!!!!!!!!!
 *                                  
 *      This script relies on the scene having a GameObject called "RightController". MAKE SURE THAT THE GAMEOBJECT EXISTS!  
 *          Also, this script relies on the assumption that the pong shield ONLY MOVES ALONG THE X-AXIS and CENTERED IN (0, 0, 0)!
 *              In addition, it requires a GameObject (in our use, we used a plane) with specified layer "PongShieldPlane"
 * 
 * */

public class Pong_Shield : MonoBehaviour {

    public Vector3 scale;
    public float x_clamp;
    public float y;
    public float duration;

    GameObject rightController;
    RaycastHit hit;

    bool blue = true;

    // Use this for initialization
    void Start () {
        // check scale input
        if (scale == null)
        {
            scale = new Vector3(2, 1.5f, 0.25f);
        }
        else
        {
            if (scale.x == 0)
            {
                scale.x = 3;
            }
            if (scale.y == 0)
            {
                scale.y = 2.5f;
            }
            if (scale.z == 0)
            {
                scale.z = 0.25f;
            }
        }
        this.transform.localScale = scale;

        // check x_clamp input
        if (x_clamp <= 0)
        {
            x_clamp = 4;
        }

        // check y_clamp_min input
        if (y <= 0)
        {
            y = 3;
        }

        // check duration input
        if (duration <= 0)
        {
            duration = 10;
        }

        // retrieve "Right Controller" GameObject
        rightController = GameObject.Find("RightController");
    }
	
	// Update is called once per frame
	void LateUpdate () {
        // if the player did not instantiate the pong shield (not the owner) then do not do anything
        if (PhotonNetwork.player != this.GetComponent<PhotonView>().owner)          // could be replaced with PhotonView.isMine?
        {
            return;
        }


        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            PhotonNetwork.Destroy(GetComponent<PhotonView>());
        }

        // in case the GameObject "Right Controller" was not found in Start(), look for it again
        //      if still not found, return and skip the RayCast
        if (rightController == null)
        {
            rightController = GameObject.Find("RightController");
            if (rightController == null)
            {
                return;
            }
        }        

		if (Physics.Raycast(rightController.transform.position, rightController.transform.forward, out hit, 100, LayerMask.GetMask("PongShieldPlane")))
        {
            if (hit.transform.tag == "Pong_Shield_Plane")
            {
                //               this.transform.position = new Vector3(Mathf.Max(-x_clamp, Mathf.Min(x_clamp, hit.point.x)), y, hit.point.z);
                this.transform.position = new Vector3(hit.point.x, y, hit.point.z);
            }
        }
	}

    public void SetBlue(bool blue_)
    {
        blue = blue_;
    }

    public bool GetBlue()
    {
        return blue;
    }
}
