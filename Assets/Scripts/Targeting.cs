using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeting : MonoBehaviour {

    [HideInInspector]
    public Transform result;
    [HideInInspector]
    public Transform result2;
    private TargetablePlayer targetableScript;
    public Transform pointer;
    public float range;
    public LayerMask layers;
    public LayerMask blessing_layers;
    [HideInInspector]
    public RaycastHit hit;
    [HideInInspector]
    public RaycastHit hit_blessing;
    SpellcastingGestureRecognition spellCast;

	// Use this for initialization
	void Start () 
	{
        spellCast = GetComponent<SpellcastingGestureRecognition>();
	}
	
	// Update is called once per frame
	void Update () {
        Target();
    }
    private void Target()
    {
        

        //Disable back faces so it doesn't collide with itself.
        Physics.queriesHitBackfaces = false;
        Physics.queriesHitTriggers = true;

        //Debug.DrawRay(pointer.position, pointer.forward * range, Color.red, 0.01f);
        //Get raycast results.
        if (Physics.Raycast (pointer.position, pointer.forward, out hit, range, layers))
        {
            if(hit.transform.parent==spellCast.avatar)
            {
                return;
            }
			//print(hit.collider);
			//Return if target is the same, and turn off the previous indicator if it's not.
			if (result != null)
            {
				if (result == hit.collider.transform)
                {
					return;
				}
                else
                {
                   // Reset targetable script.

                    if (targetableScript != null)
                        targetableScript.SetIndicator(false);
                    targetableScript = null;

                    //Reset result.
                    result = null;
				}
			}

			//Check if it has a Player tag.
			switch (hit.collider.tag)
            {
			case "Player":
                    //Assign resulting collider to target.
				result = hit.collider.transform;

                    //Try to get the targetable script. Turn it on if it's valid.
				targetableScript = result.GetComponent<TargetablePlayer> ();
				if (targetableScript != null)
					targetableScript.SetIndicator (true);
				break;
			case "BluePlatform":
			case "RedPlatform":
            case "GrayPlatform":
                case "Curse":
                    //Assign resulting collider to target.
				result = hit.collider.transform;
				break;

			}
		}
        else 
		{
			result = null;
            if (targetableScript != null)
            { 
                targetableScript.SetIndicator(false);
                targetableScript = null;
                }
        }
        Physics.queriesHitTriggers = true;
        if (Physics.Raycast(pointer.position, pointer.forward, out hit_blessing, range, blessing_layers))
        {
            result2 = hit_blessing.collider.transform;
        }
        else
        {
            result2 = null;
        }
    }
}
