using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationManager : MonoBehaviour {

    TextMesh notification;
    float timer = 101;

	// Use this for initialization
	void Start () {
        if (this.transform.Find("NotificationText") != null)
        {
            notification = this.transform.Find("NotificationText").GetComponent<TextMesh>();
            Clear();
        }
        else
        {
            Debug.Log("NotificationManager.cs : Start() : Could not find child \"NotificationText\"");
        }
	}
	
	// Update is called once per frame
	void Update () {
		if (timer <= 0)
        {
            Clear();
        }
        else if (timer == 101)
        {
            // do nothing, keep notification displayed indefinitely
        }
        else
        {
            timer -= Time.deltaTime;
        }
	}

    public void SetNotification(string text, float duration = 1)
    {
        if (notification == null)
        {
            return;
        }

        notification.text = text;
        timer = duration;
    }

    public void Clear()
    {
        notification.text = "";
    }
}
