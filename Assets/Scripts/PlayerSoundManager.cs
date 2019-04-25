using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour {
    public AudioClip hurt;
    public AudioClip recharge;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
//        if (Input.anyKey)
//            PlayerHurt();
	}

    public void PlayerHurt()
    {
        if(!GetComponent<AudioSource>().isPlaying)
            GetComponent<AudioSource>().PlayOneShot(hurt);
    }

    public void PlayRechargeSound()
    {
        GetComponent<AudioSource>().PlayOneShot(recharge);
    }
}
