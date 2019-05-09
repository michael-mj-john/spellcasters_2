using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Username : MonoBehaviour {

    public Material blue_mat;
    public Material red_mat;
    public Material default_mat;
    public GameObject head;

    // Use this for initialization
    void Start () {
        //if (this.GetComponent<PhotonView>().isMine)
        //{
        //    this.gameObject.SetActive(false);
        //}
    }
	
	// Update is called once per frame
	void Update () {

        // have usernames always be facing the local player
        if (this.GetComponent<PhotonView>().isMine)
        {
            this.transform.position = head.transform.position + 0.85f * Vector3.up + 0.45f * Camera.main.transform.forward;
        }
        else
        {
            this.transform.position = head.transform.position + 1.0f * Vector3.up;
        }
        this.transform.LookAt(Camera.main.transform);
        this.transform.RotateAround(this.transform.position, this.transform.up, 180f);
	}

    [PunRPC]
    public void SetUsername(string username)
    {
        this.GetComponent<TextMesh>().text = username;
    }

    [PunRPC]
    public void SetMaterial(int isBlue)
    {
        switch(isBlue)
        {
            case -1:
                this.GetComponent<Renderer>().material = default_mat;
                break;
            case 0:
                this.GetComponent<Renderer>().material = red_mat;
                break;
            case 1:
                this.GetComponent<Renderer>().material = blue_mat;
                break;
            default:
                this.GetComponent<Renderer>().material = default_mat;
                break;
        }
    }
}
