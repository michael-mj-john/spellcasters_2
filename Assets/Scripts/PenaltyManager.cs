using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenaltyManager : MonoBehaviour {

	public PenaltySpawn[] spawn_blue;
	public PenaltySpawn[] spawn_red;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public Transform GetPenaltyTransform(bool isBlue)
	{
		if (isBlue)
		{
			foreach(PenaltySpawn ps in spawn_blue)
			{
				if (ps.IsVacant ())
				{
					return ps.transform;
				}
			}

			// this should never be reached, return the first penalty spawn location just in case
			Debug.Log ("PenaltyManager.cs : GetPenaltyLocation() : spawn_blue has no vacant positions");
			return spawn_blue[0].transform;
		} else
		{
			foreach(PenaltySpawn ps in spawn_red)
			{
				if (!ps.IsVacant ())
				{
					return ps.transform;
				}
			}

			// this should never be reached, return the first penalty spawn location just in case
			Debug.Log ("PenaltyManager.cs : GetPenaltyLocation() : spawn_red has no vacant positions");
			return spawn_red[0].transform;
		}
	}
}
