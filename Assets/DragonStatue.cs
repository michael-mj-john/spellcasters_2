using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonStatue : MonoBehaviour {

    public ParticleSystem flameThrower;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Spell") || other.gameObject.CompareTag("ShieldBreaker"))
        {
            ToggleFlame(true);
        }
    }

    void ToggleFlame(bool on)
    {
        if(on)
        {
            flameThrower.Stop(true);
            flameThrower.Play(true);
        }
        else
        {
            flameThrower.Stop(true);
        }

        flameThrower.gameObject.SetActive(on);
    }
}
