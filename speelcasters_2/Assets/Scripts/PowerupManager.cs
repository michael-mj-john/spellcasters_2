using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *      !!!!!!!!!!!!!!  IMPORTANT   !!!!!!!!!!!!!!
 *      
 *      This script relies on the scene having the platforms called "Blue_platform1 (1)"
 *                                                                  "Blue_platform1 (3)"
 *                                                                  "Blue_platform1"
 *                                                                  "Red_platform1 (8)"
 *                                                                  "Red_platform1 (14)"
 *                                                                  "Red_platform1 (7)"
 *                                                                  
 *      Terrible, I know.
 * 
 * */

public class PowerupManager : MonoBehaviour, IPunObservable {

    public GameObject[] redPlatforms;
    public GameObject[] bluePlatforms;
    private GameObject[] grayPlatforms;
    public float frequency;
    public GameObject powerupPrefab;
    public bool spawn_powerups = false;
    public float timer;
    bool[] redPowerups;
    bool[] bluePowerups;
    bool[] grayPowerups;
    public int numPowerups;
    public int max_powerups;

    // Use this for initialization
    void Start () {
        timer = frequency;
        redPlatforms = GameObject.FindGameObjectsWithTag("RedPlatform");
        bluePlatforms = GameObject.FindGameObjectsWithTag("BluePlatform");
        redPowerups = new bool[redPlatforms.Length];
        bluePowerups = new bool[bluePlatforms.Length];
        
        //bluePlatforms[0] = GameObject.Find("Blue_platform1 (1)");
        //bluePlatforms[1] = GameObject.Find("Blue_platform1 (3)");
        //bluePlatforms[2] = GameObject.Find("Blue_platform1");

        //redPlatforms[0] = GameObject.Find("Red_platform1 (8)");
        //redPlatforms[1] = GameObject.Find("Red_platform1 (14)");
        //redPlatforms[2] = GameObject.Find("Red_platform1 (7)");

        //if (bluePlatforms[0] == null)
        //{
        //    Debug.Log("PowerupManager.cs : Start() : Could not find \"Blue_platform1 (1)\"");
        //}
        //if (bluePlatforms[1] == null)
        //{
        //    Debug.Log("PowerupManager.cs : Start() : Could not find \"Blue_platform1 (3)\"");
        //}
        //if (bluePlatforms[2] == null)
        //{
        //    Debug.Log("PowerupManager.cs : Start() : Could not find \"Blue_platform1\"");
        //}
        //if (redPlatforms[0] == null)
        //{
        //    Debug.Log("PowerupManager.cs : Start() : Could not find \"Red_platform1 (8)\"");
        //}
        //if (redPlatforms[1] == null)
        //{
        //    Debug.Log("PowerupManager.cs : Start() : Could not find \"Red_platform1 (14)\"");
        //}
        //if (redPlatforms[2] == null)
        //{
        //    Debug.Log("PowerupManager.cs : Start() : Could not find \"Red_platform1 (7)\"");
        //}
    }

    private void FixedUpdate()
    {
        if(redPlatforms.Length != GameObject.FindGameObjectsWithTag("RedPlatform").Length && PhotonNetwork.isMasterClient)
        {
            redPlatforms = GameObject.FindGameObjectsWithTag("RedPlatform");
            bluePlatforms = GameObject.FindGameObjectsWithTag("BluePlatform");
            grayPlatforms = GameObject.FindGameObjectsWithTag("GrayPlatform");
            redPowerups = new bool[redPlatforms.Length];
            bluePowerups = new bool[bluePlatforms.Length];
            grayPowerups = new bool[grayPlatforms.Length];
            if (redPlatforms.Length == 0)
            {
                foreach(GameObject pu in GameObject.FindGameObjectsWithTag("Powerup"))
                {
                    PhotonNetwork.Destroy(pu.GetPhotonView());
                    numPowerups = 0;
                }
            }
        }
    }

    // Update is called once per frame
    void Update () {
        if (PhotonNetwork.isMasterClient && spawn_powerups)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                graySpawn();
            }
            if(numPowerups < max_powerups)
            timer -= Time.deltaTime;
            if (timer <= 0 && numPowerups < max_powerups)
            {
                //Debug.Log(numPowerups);
                //int randomPlatform;
                //if (HasSpace(redPlatforms, redPowerups))
                //{
                //    randomPlatform = Random.Range(0, redPlatforms.Length);
                //    while (redPowerups[randomPlatform] || redPlatforms[randomPlatform].GetComponent<PlatformNeighbors>().hasPlayer)
                //    {
                //        randomPlatform = Random.Range(0, redPlatforms.Length);
                //    }
                //    Powerup pup = PhotonNetwork.Instantiate(powerupPrefab.name, redPlatforms[randomPlatform].transform.position + new Vector3(0, 1, 0), new Quaternion(45, 0, 45, 0), 0).GetComponent<Powerup>();
                //    pup.SetPowerupProperties(false, randomPlatform, redPlatforms[randomPlatform].GetComponent<PlatformNeighbors>());
                //    redPowerups[randomPlatform] = true;
                //    numPowerups++;
                //}

                //if (HasSpace(bluePlatforms, bluePowerups))
                //{
                //    randomPlatform = Random.Range(0, bluePlatforms.Length);
                //    while (bluePowerups[randomPlatform] || bluePlatforms[randomPlatform].GetComponent<PlatformNeighbors>().hasPlayer)
                //    {
                //        randomPlatform = Random.Range(0, bluePlatforms.Length);
                //    }
                //    Powerup pup = PhotonNetwork.Instantiate(powerupPrefab.name, bluePlatforms[randomPlatform].transform.position + new Vector3(0, 1, 0), new Quaternion(45, 0, 45, 0), 0).GetComponent<Powerup>();
                //    pup.SetPowerupProperties(true, randomPlatform, bluePlatforms[randomPlatform].GetComponent<PlatformNeighbors>());
                //    bluePowerups[randomPlatform] = true;
                //    numPowerups++;
                //}
                if (HasSpace(grayPlatforms, grayPowerups))
                {
                    graySpawn();
                }
                timer = frequency;
            }
        }
	}
    private void graySpawn()
    {
        int randomPlatform;
        randomPlatform = Random.Range(0, grayPlatforms.Length);
        while (grayPowerups[randomPlatform])
        {
            randomPlatform = Random.Range(0, grayPlatforms.Length);
        }
        Powerup pup = PhotonNetwork.Instantiate(powerupPrefab.name, grayPlatforms[randomPlatform].transform.position + new Vector3(0, 1, 0), new Quaternion(45, 0, 45, 0), 0).GetComponent<Powerup>();
        pup.SetPowerupProperties(true, randomPlatform, grayPlatforms[randomPlatform].GetComponent<PlatformNeighbors>());
        grayPowerups[randomPlatform] = true;
        numPowerups++;
        
    }
    public void FindGrays()
    {
        grayPlatforms = GameObject.FindGameObjectsWithTag("GrayPlatform");
    }
    bool HasSpace(GameObject[] platforms, bool[] powerups)
    {
        for (int i = 0; i < platforms.Length; i++)
        {
            if (platforms.Length != 0 && ! powerups[i])
            {
                return true;
            }
        }

        return false;
    }

    public void DecrementPowerUp(bool isBlue, int platformIndex)
    {
        //print("PlayerStatus.cs : DecrememntPowerUp() : " + isBlue + " " + platformIndex);

        numPowerups--;

        if (isBlue)
        {
            
            bluePowerups[platformIndex] = false;
        }
        else
        {
            
            redPowerups[platformIndex] = false;
        }
        grayPowerups[platformIndex] = false;
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // If you own the game object
        if (stream.isWriting)
        {
            // Sync all instances of health according to my health
            stream.SendNext(numPowerups);
        }
        // If you dont own the game object
        else
        {
            // Sync the avatar's health according to the owner of the avatar.
            numPowerups = (int)(stream.ReceiveNext());
        }
    }
}