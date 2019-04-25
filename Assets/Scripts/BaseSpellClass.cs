using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSpellClass : MonoBehaviour {
    private GameObject _myAvatar;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void SetOwner(GameObject avatar)
    {
        _myAvatar = avatar;
    }
    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        if (other.CompareTag("Player"))
        {
            //Apply damage to object if it has the Player tag and implements the PlayerStatus script.
            //PlayerStatus statusScript = other.GetComponent<PlayerStatus>();
            //if (statusScript != null) statusScript.takeDamage(damage);
            //else
            //{
            //    print("statusscript is null");
            //}
            //Instantiate new explosion.
            //GameObject newExplosion = PhotonNetwork.Instantiate(explosion.name, this.transform.position, new Quaternion(), 0);
        }
        else if (other.CompareTag("put"))
        {
            print("hit on head");
            //Apply damage to object if it has the Player tag and implements the PlayerStatus script.
            //PlayerStatus statusScript = other.transform.parent.GetComponentInChildren<PlayerStatus>();
            //if (statusScript != null) statusScript.takeDamage(damage);
            //Instantiate new explosion.
            //GameObject newExplosion = PhotonNetwork.Instantiate(explosion.name, this.transform.position, new Quaternion(), 0);
        }
        else if (other.CompareTag("Shield"))
        {
            print("hit on shield");
            //Apply damage to the shield.
            //Damageable damageScript = other.GetComponent<Damageable>();
            //if (damageScript != null) damageScript.TakeDamage(damage);
            //Instantiate new explosion.
            //GameObject newExplosion = PhotonNetwork.Instantiate(explosion.name, this.transform.position, new Quaternion(), 0);
        }
        else if (other.CompareTag("Spell"))
        {
            print("hit on spell");
            //Get the point between the two spells
            //Vector3 midpoint = this.transform.position + ((other.transform.position - this.transform.position) * 0.5f);

            //Instantiate new explosion.
            //GameObject newExplosion = PhotonNetwork.Instantiate(explosion.name, this.transform.position, new Quaternion(), 0);



        }
    }
    IEnumerator Default()
    {
        yield return new WaitForSeconds(1f);
    }

}
