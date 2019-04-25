using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {
    
    public   float yOffset = 1.8f;
    public GameObject owner;
    public GameObject bar;
    private Transform camera;

    public bool bookHealth = false;
    public Transform dummyCam;

    private Transform ownerTrans;
    public PlayerStatus ownerStat;
	// Use this for initialization
	void Start () 
	{
        camera = Camera.main.transform;

        
        ownerTrans = owner.transform;
//        ownerStat = owner.GetComponent<PlayerStatus>();
    }
	
	// Update is called once per frame
	void Update () {
       
        //       myTrans.position = new Vector3(owner.position.x, owner.position.y+yOffset,owner.position.z);

		if (bookHealth == false)
        {
			this.transform.position = new Vector3 (ownerTrans.position.x, ownerTrans.position.y + yOffset, ownerTrans.position.z);
			this.transform.forward = camera.forward;
		}
		else 
		{
           // this.transform.position = new Vector3(ownerTrans.position.x, ownerTrans.position.y, ownerTrans.position.z);
            
        }
        // Debug.Log(camera);
        if (ownerStat != null && (float)ownerStat.current_health >= 0)
        {
            //print("greater than 0 health");
            setHealthbarScale((float)ownerStat.current_health / (float)ownerStat.max_health);
        }
        else
        {
            //print("lesser than than 0 health");
            setHealthbarScale(0f);
        }

    }

    void setHealthbarScale(float maHealth)
    {
        //print("updatedHealth");
        // bar.transform.localScale = new Vector3(maHealth, bar.transform.localScale.y, bar.transform.localScale.z);
        bar.transform.localScale = new Vector3(Mathf.Lerp(bar.transform.localScale.x, maHealth, Time.deltaTime*5), bar.transform.localScale.y, bar.transform.localScale.z);
        
    }
}
