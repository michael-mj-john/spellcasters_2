using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class TeamManager : MonoBehaviour {
    private GameObject[] redSquares;
    private GameObject[] blueSquares;
    private Transform avatar;
    private Transform torso;
    private Transform head;
    private Transform hat;
    private GameObject rightHand;
    private GameObject cameraRig;
    private bool set = false;
    private VRTK.VRTK_StraightPointerRenderer vrtk_spr;
    private GameObject roundManager;
    public Material blue_mat;
    public Material red_mat;
	public bool blue = false;
    [HideInInspector]
   
    public PhotonView photonView;
	[HideInInspector]
	private void Awake()
    {

        photonView = GetComponent<PhotonView>();
    }
    // Use this for initialization
    void Start () {
        redSquares = GameObject.FindGameObjectsWithTag("RedPlatform");
        blueSquares = GameObject.FindGameObjectsWithTag("BluePlatform");
        cameraRig = GameObject.FindGameObjectWithTag("CameraRig");
        //roundManager = GameObject.FindGameObjectWithTag("RoundManager");
        //if(roundManager && photonView.isMine)
        //{
        //    Debug.Log("Assigning Team");
        //    roundManager.GetComponent<RoundManager>().AssignTeam(gameObject);
        //}
        //else if(photonView.isMine)
        //{
        //    Debug.Log("RoundManager not found");
        //    SetBlue();
        //}
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        //if(rightHand != null && !set && photonView.isMine)
        //{
        //    set = true;
        //    vrtk_spr = rightHand.GetComponent<VRTK.VRTK_StraightPointerRenderer>();
        //    if(blue && vrtk_spr != null)
        //    {
        //        vrtk_spr.blue = true;
        //    }
        //    else if(!blue && vrtk_spr != null)
        //    {
        //        vrtk_spr.blue = false;
        //    }
        //}
        //else if(!set)
        //{
        //    rightHand = GameObject.Find("RightController");
        //}
    }
    
    [PunRPC]
    public void SetBlue()
    {
       
            Debug.Log("Set Blue + " + Time.time);

            blue = true;

            //Respawn();

            TeamSetter[] children = GetComponentsInChildren<TeamSetter>();
        //Debug.Log("NUMBER OF TEAM SETTERS: " + children.Length);
            foreach (TeamSetter ts in children)
            {
                ts.SetBlue();
            }
            rightHand = GameObject.Find("RightController");
            if (rightHand && photonView.isMine)
            {
                set = true;
                vrtk_spr = rightHand.GetComponent<VRTK.VRTK_StraightPointerRenderer>();
                vrtk_spr.blue = true;
            }
        
    }

    [PunRPC]
    public void SetRed()
    {
        
            Debug.Log("Set Red + " + Time.time);

            blue = false;

            //Respawn();
            TeamSetter[] children = GetComponentsInChildren<TeamSetter>();
            foreach (TeamSetter ts in children)
            {
                ts.SetRed();
            }
            rightHand = GameObject.Find("RightController");
            if (rightHand && photonView.isMine)
            {
                set = true;
                vrtk_spr = rightHand.GetComponent<VRTK.VRTK_StraightPointerRenderer>();
                vrtk_spr.blue = false;
            }
        
    }

    public void Respawn()
    {
        cameraRig = GameObject.FindGameObjectWithTag("CameraRig");
        redSquares = GameObject.FindGameObjectsWithTag("RedPlatform");
        blueSquares = GameObject.FindGameObjectsWithTag("BluePlatform");
        if (blue)
        {
            Transform randPlatform = blueSquares[Random.Range(0, blueSquares.Length - 1)].transform;
            while(randPlatform == null || randPlatform.GetComponent<PlatformNeighbors>().hasPlayer || randPlatform.GetComponent<PlatformNeighbors>().hasVines || randPlatform.GetComponent<PlatformNeighbors>().hasPowerup)
            {
                blueSquares = GameObject.FindGameObjectsWithTag("BluePlatform");
                randPlatform = blueSquares[Random.Range(0, blueSquares.Length - 1)].transform;
            }
            if (!VRDevice.model.ToLower().Contains("oculus"))
            {
                //print("Vive Rotation");
                cameraRig.GetComponent<PlatformController>().SetPlatform(randPlatform,
                    Quaternion.Euler(0, cameraRig.transform.eulerAngles.y + (180 - Camera.main.transform.eulerAngles.y), 0));
            }
            else
            {
                //print("Oculus Rotation");
                cameraRig.GetComponent<PlatformController>().SetPlatform(randPlatform,
                    Quaternion.Euler(0, 180, 0));
            }
            //Camera.main.transform.rotation = Quaternion.Euler(0, 180, 0);
            
        }
        else
        {
            Transform randPlatform = redSquares[Random.Range(0, redSquares.Length - 1)].transform;
            while (randPlatform == null || randPlatform.GetComponent<PlatformNeighbors>().hasPlayer || randPlatform.GetComponent<PlatformNeighbors>().hasVines || randPlatform.GetComponent<PlatformNeighbors>().hasPowerup)
            {
                redSquares = GameObject.FindGameObjectsWithTag("RedPlatform");
                randPlatform = redSquares[Random.Range(0, redSquares.Length - 1)].transform;
            }
            if (!VRDevice.model.ToLower().Contains("oculus"))
            {
                //print("Vive Rotation");
                cameraRig.GetComponent<PlatformController>().SetPlatform(randPlatform,
                Quaternion.Euler(0, cameraRig.transform.eulerAngles.y + (0 - Camera.main.transform.eulerAngles.y), 0));
            }
            else
            {
                //print("Oculus Rotation");
                cameraRig.GetComponent<PlatformController>().SetPlatform(randPlatform,
                Quaternion.Euler(0, 0, 0));
            }
            //Camera.main.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void SetAvatar(Transform _avatar)
    {
        avatar = _avatar;
        torso = avatar.Find("Torso");
        head = avatar.Find("Head");
    }

    public void OnDestroy()
    {
        //if(photonView.isMine)
        //    GameObject.FindWithTag("RoundManager").GetComponent<RoundManager>().Unsubscribe(avatar.gameObject, cameraRig);
    }
}
