using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterPlatform : MonoBehaviour {
//    [HideInInspector]
    public int numPlayersOnPlatform = 0;
    [HideInInspector]
    public List<GameObject> players = new List<GameObject>();

    NotificationManager nm;

    // Use this for initialization
    void Start () {
        SetNotificationManager();
    }
	
	// Update is called once per frame
	void Update () {

	}

    private void OnEnable()
    {
        numPlayersOnPlatform = 0;
        players.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        SetNotificationManager();

        //        Debug.Log("TeleporterPlatform.cs : OnTriggerEnter() : Collided with " + other.name + " with tag " + other.tag);
        if (other.tag == "Player")
        {
            PlayerStatus ps = other.GetComponent<PlayerStatus>();

//            Debug.Log("TeleporterPlatform.cs : OnTriggerEnter() : numPlayersOnPlatform : " + numPlayersOnPlatform);
            if (! players.Contains(other.gameObject))
            {
                numPlayersOnPlatform++;
                players.Add(other.gameObject);
            }

            // Notify player to get a hat if the player is on the teleporter without a hat
            if (other.GetComponent<PhotonView>().isMine && ps.playerClass == PlayerClass.none)
            {
                nm.SetNotification("Grab a hat!", 101);
            }

            other.GetComponent<PlayerStatus>().onTeleporter = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        SetNotificationManager();

        //        Debug.Log("TeleporterPlatform.cs : OnTriggerExit() : Collided with " + other.tag + " with tag " + other.tag);
        if (other.tag == "Player")
        {
            //Debug.Log("TeleporterPlatform.cs : OnTriggerEnter() : numPlayersOnPlatform : " + numPlayersOnPlatform);
            if (players.Contains(other.gameObject))
            {
                numPlayersOnPlatform--;
                players.Remove(other.gameObject);
            }

            // Clear notifications
            nm.Clear();

            other.GetComponent<PlayerStatus>().onTeleporter = false;
        }
    }

    void SetNotificationManager()
    {
        if (nm == null)
        {
            if (Camera.main == null)
            {
                //Debug.Log("TeleporterManager.cs : Start() : Could not find \"Camera.main\" GameObject");
            }
            else if (Camera.main.GetComponent<NotificationManager>() == null)
            {
                Debug.Log("TeleporterManager.cs : Start() : Could not find \"NotificationManager\" component");
            }
            else
            {
                nm = Camera.main.GetComponent<NotificationManager>();
            }
        }
    }
}
