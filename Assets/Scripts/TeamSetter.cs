using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamSetter : MonoBehaviour {
    public Material blue;
    public Material red;
    private PhotonView photonView;
    private Renderer render;
    TargetablePlayer targetableScript;
    // Use this for initialization
    void Awake () {
        photonView = GetComponent<PhotonView>();
        render = GetComponent<Renderer>();
        targetableScript = GetComponent<TargetablePlayer>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void SetRed()
    {
        //Debug.Log("Test " + Time.time);
        photonView.RPC("SetRed2", PhotonTargets.AllBuffered, null);
        
    }
    public void SetBlue()
    {
        //Debug.Log("Test " + Time.time);
        photonView.RPC("SetBlue2", PhotonTargets.AllBuffered, null);
        
    }
    [PunRPC]
    public void SetRed2()
    {
        Material[] mats = render.materials;
        for (int i = 0; i < mats.Length; i++)
        {
            mats[i] = red;
        }
        render.materials = mats;

        if (targetableScript != null) targetableScript.UpdateMaterials(red);
    }
    [PunRPC]
    public void SetBlue2()
    {
        Material[] mats = render.materials;
        for (int i = 0; i < mats.Length; i++)
        {
            mats[i] = blue;
        }
        render.materials = mats;
        if (targetableScript != null) targetableScript.UpdateMaterials(blue);
    }
}
