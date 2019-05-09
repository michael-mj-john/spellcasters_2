using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Heal spell
// We need to heal every instance of player avatar.

public class HealSpell : MonoBehaviour
{
    // int times_hit = 0;
    public int healthAdded = 1000;
    public GameObject healParticle;

    // Use this for initialization
    void Start()
    {
        // Destroy(gameObject, 5);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        transform.position += new Vector3(.5f, 0, 0);
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject hit = collision.gameObject;

        if (hit.tag == "Player")
        {
            PlayerStatus statusScript = hit.GetComponent<PlayerStatus>();
            if (statusScript != null) statusScript.heal(healthAdded);
            PhotonNetwork.Instantiate(healParticle.name,transform.position,transform.rotation, 0);
        }
        Destroy(gameObject);
    }
}
