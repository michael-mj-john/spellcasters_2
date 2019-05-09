using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
/*
 *      !!!!!!!!!   IMPORTANT   !!!!!!!!!
 *      Broke up with my 
 *      
 * 
 * */


public class PlayerStatus : MonoBehaviour, IPunObservable
{
    public GameObject DeathSprite;
    public PlayerClass playerClass;
    public bool kill_spells = true;
    private GameObject[] hats;
    private GameObject[] players;

    private BookLogic bookLogic;

    private Transform respawnPt;
    private Transform timeOutPt;
    public PhotonView photonView;
    public PlayerSoundManager psm;
    public GameObject cameraRig;
    private TextMesh deadText;

    // Invulnerability frames
    private float startTime;
    float invulnerableFrames = 0.5f;
    public bool dead = false;
    public bool pregame = true;
    private float deathTime = 0f;
    public float respawnLength = 2f;
    private bool waitingForNextRound = false;
    bool rotated = false;
    bool isOculus = false;
    [HideInInspector]
    public bool bubbled = false;

    ScoreboardUpdater myScoreboard;

    PhotonView self_photonview;
    private GameObject rightHand;
    private bool set = false;
    VRTK.VRTK_StraightPointerRenderer vrtk_spr;

    public GameObject hat;
    public bool onTeleporter = false;
    PenaltyManager pm;

    public float max_health = 100;
    //  [HideInInspector]
    public float current_health = 100;
    //   public int hp = 100;

    float rightAnalogueHoriz;
    float oculusGrip;

    float deltaTime;
    float fps;

    // Use this for initialization
    void Start()
    {
        cameraRig = Camera.main.transform.parent.gameObject;
        bookLogic = transform.parent.GetComponentInChildren<BookLogic>();
        hats = GameObject.FindGameObjectsWithTag("Grabbable");
        self_photonview = GetComponent<PhotonView>();
        if (VRDevice.model.ToLower().Contains("oculus"))
        {
            isOculus = true;
        }

        //Get camera rig if this object belogns to the client.
        if (photonView.isMine)
        {
            //Gets the Camera (eyes) and navigates to the Camera Rig object.
            cameraRig = Camera.main.transform.parent.gameObject;
            deadText = Camera.main.transform.GetChild(0).GetComponent<TextMesh>();
        }

        //Get's the location where the player will respawn.
        timeOutPt = GameObject.FindGameObjectWithTag("TimeOut").transform;
        respawnPt = GameObject.FindGameObjectWithTag("RespawnDefault").transform;
//        myScoreboard = GameObject.FindGameObjectWithTag("Scoreboard").GetComponent<ScoreboardUpdater>();
        if (GameObject.FindGameObjectWithTag("Arena") != null && photonView.isMine)
        {
            waitingForNextRound = true;
            cameraRig.GetComponent<SpellcastingGestureRecognition>().enabled = false;
            cameraRig.GetComponent<PlatformController>().enabled = false;
            cameraRig.GetComponent<Edwon.VR.VRGestureRig>().enabled = false;
            cameraRig.GetComponent<PadTeleport>().enabled = false;
            //if (!VRDevice.model.ToLower().Contains("oculus"))
            //cameraRig.transform.rotation = Quaternion.Euler(0, cameraRig.transform.eulerAngles.y + (270 - Camera.main.transform.eulerAngles.y), 0);
            //cameraRig.GetComponent<VRTK.VRTK_BasicTeleport>().Teleport(timeOutPt, timeOutPt.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        fps = 1.0f / deltaTime;
        // print("FPS: " + fps);


        // Debug rotation for oculus
        if (isOculus)
        {

            rightAnalogueHoriz = Input.GetAxis("TrackpadHoriz2");
            oculusGrip = Input.GetAxis("OculusRightGrip");
            //print (rightAnalogueHoriz);
            // Rotate for oculus players
            if (rightAnalogueHoriz > .5f)
            {
                if (rotated == false && VRDevice.model.ToLower().Contains("oculus"))
                {
                    //cameraRig.transform.rotation = cameraRig.transform.rotation.eulerAngles + new Vector3 (0, 90, 0);
                    cameraRig.transform.eulerAngles += new Vector3(0, 45, 0);
                    rotated = true;
                }
            }
            else if (rightAnalogueHoriz < -.5f)
            {
                if (rotated == false && VRDevice.model.ToLower().Contains("oculus"))
                {
                    //cameraRig.transform.rotation = cameraRig.transform.rotation.eulerAngles + new Vector3 (0, 90, 0);
                    cameraRig.transform.eulerAngles += new Vector3(0, -45, 0);
                    rotated = true;
                }
            }

            else
            {
                rotated = false;
            }
        }

        // Debug rotation for vive
        if (!isOculus)
        {
            rightAnalogueHoriz = Input.GetAxis("TrackpadHoriz2");
            oculusGrip = Input.GetAxis("ViveRightGrip");
            //print (rightAnalogueHoriz);
            // Rotate for oculus players
            if (Input.GetKey("joystick button 17") && oculusGrip > 0.5f && rightAnalogueHoriz > .5f)
            {
                if (rotated == false)
                {
                    //cameraRig.transform.rotation = cameraRig.transform.rotation.eulerAngles + new Vector3 (0, 90, 0);
                    cameraRig.transform.eulerAngles += new Vector3(0, 45, 0);
                    rotated = true;
                }
            }
            else if (Input.GetKey("joystick button 17") && oculusGrip > 0.5f && rightAnalogueHoriz < -.5f)
            {
                if (rotated == false)
                {
                    //cameraRig.transform.rotation = cameraRig.transform.rotation.eulerAngles + new Vector3 (0, 90, 0);
                    cameraRig.transform.eulerAngles += new Vector3(0, -45, 0);
                    rotated = true;
                }
            }

            else
            {
                rotated = false;
            }
        }

        //Respawn Player when time out's done.
        if (dead == true && !waitingForNextRound)
        {
            if ((Time.time - deathTime) > respawnLength)
            {
                //self_photonview.RPC("Respawn", PhotonTargets.All, null);
                Respawn();
            }

            else
            {
                if (photonView.isMine)
                {
                    myScoreboard = GameObject.FindGameObjectWithTag("Scoreboard").GetComponent<ScoreboardUpdater>();

                    if (myScoreboard.roundOver == false)
                        deadText.text = "You were killed!\nRespawn in " + ((int)(respawnLength - (Time.time - deathTime)));

                    else
                    {
                        deadText.text = "The round is over.";
                        //cameraRig.transform.position = GameObject.FindGameObjectWithTag("HatRoom").transform.position;
                        //dead = false;
                    }
                }
            }
        }
        else if (waitingForNextRound)
        {

        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Die();
        }
        if (rightHand != null && !set)
        {
            set = true;
            vrtk_spr = rightHand.GetComponent<VRTK.VRTK_StraightPointerRenderer>();

        }
        else if (!set)
        {
            rightHand = GameObject.Find("RightController");
        }
    }

    public void heal(float healthAdded)
    {
        if (dead == false)
        {
            current_health += healthAdded;
        }

        if (current_health >= max_health)
        {
            current_health = max_health;
        }
    }
    void SendToPtForRound()
    {

    }



    [PunRPC]
    //Reduces the health by the damage received.
    public void TakeDamage(float damage)
    {
        if (current_health <= 0 || Time.time - startTime < invulnerableFrames || bubbled)
        {
            return;
        }

        if (dead == false && pregame == false)
        {
            startTime = Time.time;
            current_health -= damage;
            psm.PlayerHurt();
        }

        if (current_health <= 0)
        {
            if (playerClass != PlayerClass.none)
            {
                Die();


            }
        }
    }

    [PunRPC]
    public void set_BubbleShield(float t)
    {


        StartCoroutine(setBubble_shield(t));

    }

    [PunRPC]
    public void unset_BubbleShield()
    {
        bubbled = false;
        StopCoroutine("setBubble_shield");
    }

    public IEnumerator setBubble_shield(float TtimeCD)
    {
        bubbled = true;
        yield return new WaitForSeconds(TtimeCD);
        bubbled = false;
    }

    //Immobilizes the player.
    public void EnableMovement(bool isEnabled)
    {
        if (photonView.isMine)
        {
            // cameraRig.GetComponent<PlatformController>().canMove = isEnabled;
            if (!waitingForNextRound)
                cameraRig.GetComponent<PadTeleport>().enabled = isEnabled;
            // vrtk_spr.enabled = isEnabled;
        }
    }

    // On death, we warp the camera rig of the corresponding player
    [PunRPC]
    public void Die()
    {
        //Move Player to the time out are if it belongs to the client.
        if (photonView.isMine)
        {
            if (myScoreboard == null)
            {
                myScoreboard = GameObject.FindGameObjectWithTag("Scoreboard").GetComponent<ScoreboardUpdater>();
            }
            if (playerClass == PlayerClass.none || myScoreboard.roundOver)
            {

            }
            else
            {
                GameObject.Find("Announcer").GetComponent<AnnouncerEvents>().PlaySound("vanquished");
                self_photonview.RPC("deathSpriteActive", PhotonTargets.All, true);
                // Increment scoreboard
                bool blueScored = !this.transform.parent.GetComponent<TeamManager>().blue;
                self_photonview = GetComponent<PhotonView>();
                if (blueScored)
                {
                    //               photonView.RPC("UpdateScoreboard", PhotonTargets.All, blueScored);
                    //self_photonview.RPC("UpdateScoreboard", PhotonTargets.AllBuffered, true);
                    UpdateScoreboard(true);
                }
                else
                {
                    //                photonView.RPC("UpdateScoreboard", PhotonTargets.All, ! blueScored);
                    //self_photonview.RPC("UpdateScoreboard", PhotonTargets.AllBuffered, false);
                    UpdateScoreboard(false);
                }

                //cameraRig.GetComponent<PlatformController>().lerp = false;
                //vrtk_spr.enabled = false;
                cameraRig.GetComponent<PadTeleport>().enabled = false;
                cameraRig.GetComponent<Edwon.VR.VRGestureRig>().enabled = false;
                cameraRig.GetComponent<SpellcastingGestureRecognition>().enabled = false;

                StartCoroutine("DelayToPenaltyBox");
            }
        }
        else
        {
            GameObject.Find("Announcer").GetComponent<AnnouncerEvents>().PlaySound("knockOut");
        }

        deathTime = Time.time;
        dead = true;
        //current_health = max_health;
        //  Respawn();
    }

    IEnumerator DelayToPenaltyBox()
    {
        // very bad if this is true
        if (GameObject.FindGameObjectWithTag("Penalty") == null)
        {
            Debug.Log("PlayerStatus.cs : Die() : Could not find \"Penalty\" GameObject in the scene");
            yield break;
        }

        // also very bad
        pm = GameObject.FindGameObjectWithTag("Penalty").GetComponent<PenaltyManager>();
        if (pm == null)
        {
            Debug.Log("PlayerStatus.cs : Die() : Could not find \"PenaltyManager\" script on \"Penalty\" GameObject");
            yield break;
        }

        deadText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2.0f);

        // determines where the player is placed on the penalty box
        bool isBlue = this.transform.parent.GetComponent<TeamManager>().blue;
        Transform penalty = pm.GetPenaltyTransform(isBlue);
        if (!VRDevice.model.ToLower().Contains("oculus"))
        {
            cameraRig.transform.rotation = Quaternion.Euler(0, cameraRig.transform.eulerAngles.y + (270 - Camera.main.transform.eulerAngles.y), 0);
        }
        cameraRig.GetComponent<VRTK.VRTK_BasicTeleport>().Teleport(penalty, penalty.position);


        //cameraRig.transform.position = new Vector3(timeOutPt.position.x - Camera.main.transform.localPosition.x, timeOutPt.position.y, timeOutPt.position.z - Camera.main.transform.localPosition.z);

    }

    [PunRPC]
    void UpdateScoreboard(bool blueScored)
    {
        if (photonView.isMine)
        {
            //Why are we assigning this on runtime? It could be assigned through the NetworkManager.
            ScoreboardUpdater scoreboard = GameObject.FindGameObjectWithTag("Scoreboard").GetComponent<ScoreboardUpdater>();

            // Debug.Log(GameObject.FindGameObjectWithTag("Scoreboard").name);

            if (scoreboard == null)
            {
                Debug.Log("SCOREBOARD UPDATER IS NULL!");
            }

            // Debug.Log("INSIDE RPC: BLUE SCORED " + blueScored);

            if (blueScored)
            {
                scoreboard.IncrementBlueScore();
            }
            else
            {
                scoreboard.IncrementRedScore();
            }
        }

    }

    //	[PunRPC]
    void ResetScoreboard()
    {
        //Why are we assigning this on runtime? It could be assigned through the NetworkManager.
        ScoreboardUpdater scoreboard = GameObject.FindGameObjectWithTag("Scoreboard").GetComponent<ScoreboardUpdater>();

        //Debug.Log(GameObject.FindGameObjectWithTag("Scoreboard").name);

        if (scoreboard == null)
        {

        }

        scoreboard.ResetScoreboard();
    }

    //Reset health and move Player to respawn area.
    [PunRPC]
    public void Respawn()
    {


        //Move Player to respawn area if it belongs to the client.
        if (photonView.isMine)
        {
            //print(1);
            //dead = false;
            current_health = max_health;
            self_photonview.RPC("deathSpriteActive", PhotonTargets.All, false);

            //print(2);
            myScoreboard = GameObject.FindGameObjectWithTag("Scoreboard").GetComponent<ScoreboardUpdater>();

            // cameraRig.transform.position = respawnPt.position;
            //			if (myScoreboard.roundOver == false && playerClass != PlayerClass.none) {
            if (playerClass != PlayerClass.none)
            {
                this.transform.parent.GetComponent<TeamManager>().Respawn();
                //cameraRig.GetComponent<PlatformController>().lerp = true;
                //cameraRig.GetComponent<PlatformController>().canMove = true;
                // vrtk_spr.enabled = true;
                cameraRig.GetComponent<PadTeleport>().enabled = true;
                cameraRig.GetComponent<Edwon.VR.VRGestureRig>().enabled = true;
                cameraRig.GetComponent<SpellcastingGestureRecognition>().enabled = true;
            }
            else
            {
                //                             self_photonview.RPC("RestartRound", PhotonTargets.AllBuffered, null);
            }
            //print(3);
            deadText.gameObject.SetActive(false);
        }
    }

    //public void RemoveHat()
    //{
    //    Transform head = this.transform.parent.Find("Head");

    //    if (head == null)
    //    {
    //        Debug.Log("PlayerStatus.cs : RemoveHat() : head is null");
    //        return;
    //    }

    //    transform hat = null;
    //    foreach (transform child in head)
    //    {
    //        if (child.tag == "grabbable")
    //        {
    //            hat = child;
    //            break;
    //        }
    //    }

    //    if (hat == null)
    //    {
    //        debug.log("playerstatus.cs : removehat() : hat is null");
    //        return;
    //    }

    //    Debug.Log("PlayerStatus.cs : RemoveHat() : removed hat");

    //    hat.GetComponent<HatLogic>().onHead = false;
    //    hat.GetComponent<HatLogic>().resettable = true;
    //    hat.GetComponent<HatLogic>().resetHat();

    //}

    [PunRPC]
    public void RestartRound()
    {
        //        self_photonview.RPC("ResetScoreboard", PhotonTargets.All, null);
        //ResetScoreboard();
        /*
                if (hats == null)
                {
                    hats = GameObject.FindGameObjectsWithTag("Grabbable");
                }

                if (hats != null)
                {
                    foreach (GameObject hat in hats)
                    {
                        hat.transform.SetParent(null);
                        hat.GetComponent<HatLogic>().resetHat();
                        hat.GetComponent<HatLogic>().onHead = false;
                        hat.GetComponent<HatLogic>().resettable = true;
                    }
                }
        */
        players = GameObject.FindGameObjectsWithTag("Player");
        waitingForNextRound = false;
        foreach (GameObject player in players)
        {
            PlayerStatus ps = player.transform.parent.GetComponentInChildren<PlayerStatus>();
            ps.playerClass = PlayerClass.none;
            ps.RemoveHat();
        }
        playerClass = PlayerClass.none;
        cameraRig = Camera.main.transform.parent.gameObject;
        if (kill_spells)
        {
            cameraRig.GetComponent<SpellcastingGestureRecognition>().enabled = false;
            cameraRig.GetComponent<PlatformController>().enabled = false;
            cameraRig.GetComponent<PadTeleport>().enabled = true;
            cameraRig.GetComponent<Edwon.VR.VRGestureRig>().enabled = false;
        }

        if (bookLogic == null)
        {
            bookLogic = transform.parent.GetComponentInChildren<BookLogic>();
        }
        if (bookLogic != null)
        {
            bookLogic.UpdateUI();
        }
        //cameraRig.transform.position = GameObject.FindGameObjectWithTag("HatRoom").transform.position;
        myScoreboard = GameObject.FindGameObjectWithTag("Scoreboard").GetComponent<ScoreboardUpdater>();
        myScoreboard.roundOver = false;
        //print ("Scoreboard " +  myScoreboard.roundOver);
        current_health = max_health;
    }
    [PunRPC]
    public void deathSpriteActive(bool b)
    {
        dead = b;
        DeathSprite.SetActive(b);
    }
    [PunRPC]
    public void SetClass(PlayerClass pc)
    {
        playerClass = pc;

        if (playerClass == PlayerClass.none)
        {

            if (photonView.isMine)
            {
                cameraRig.GetComponent<SpellcastingGestureRecognition>().enabled = false;
                cameraRig.GetComponent<Edwon.VR.VRGestureRig>().enabled = false;
                bookLogic.index = bookLogic.pages.Length - 1;
                bookLogic.UpdateUI();
            }
        }
        else
        {
            //Switch wands off.
            string wandStickPath = "Right Hand/MagicWand/stick/";
            this.transform.parent.Find(wandStickPath + "basic").gameObject.SetActive(false);
            this.transform.parent.Find(wandStickPath + "conjurer").gameObject.SetActive(false);
            this.transform.parent.Find(wandStickPath + "guardian").gameObject.SetActive(false);
            this.transform.parent.Find(wandStickPath + "shaman").gameObject.SetActive(false);
            this.transform.parent.Find("Head/HeadDress/helmet").gameObject.SetActive(false);
            this.transform.Find("armor").gameObject.SetActive(false);
            this.transform.Find("cape").gameObject.SetActive(false);
            this.transform.Find("scarf").gameObject.SetActive(false);

            bookLogic = transform.parent.GetComponentInChildren<BookLogic>();

            if (playerClass == PlayerClass.attack)
            {
                bookLogic.index = 0;

                //Switch corresponding wand on.
                this.transform.parent.Find(wandStickPath + "conjurer").gameObject.SetActive(true);
                this.transform.Find("cape").gameObject.SetActive(true);
            }
            if (playerClass == PlayerClass.support)
            {
                bookLogic.index = 0;

                //Switch corresponding wand on.
                this.transform.parent.Find(wandStickPath + "guardian").gameObject.SetActive(true);
                this.transform.Find("armor").gameObject.SetActive(true);
                this.transform.parent.Find("Head/HeadDress/helmet").gameObject.SetActive(true);
            }
            if (playerClass == PlayerClass.heal)
            {
                bookLogic.index = 0;

                //Switch corresponding wand on.
                this.transform.parent.Find(wandStickPath + "shaman").gameObject.SetActive(true);
                this.transform.Find("scarf").gameObject.SetActive(true);
            }


            if (photonView.isMine)
            {
                cameraRig.GetComponent<SpellcastingGestureRecognition>().enabled = true;
                cameraRig.GetComponent<Edwon.VR.VRGestureRig>().enabled = true;
                cameraRig.GetComponent<PlatformController>().enabled = true;
                bookLogic.UpdateUI();
                bookLogic.UpdateHotbar();
            }
        }
    }

    [PunRPC]
    void SetRandomSpell()
    {
        Camera.main.transform.parent.GetComponent<SpellcastingGestureRecognition>().SetRandomSuperSpell();
    }


    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // If you own the game object
        if (stream.isWriting)
        {
            // Sync all instances of health according to my health
            stream.SendNext(current_health);
        }
        // If you dont own the game object
        else
        {
            // Sync the avatar's health according to the owner of the avatar.
            current_health = (float)(stream.ReceiveNext());
        }
    }

    [PunRPC]
    public void Teleport(bool isBlue, Vector3 newLocation)
    {
        //        Debug.Log("PlayerStatus.cs : Teleport() : newLocation = " + newLocation);
        if (this.GetComponent<PhotonView>().isMine)
        {
            if (isBlue)
            {
                this.transform.parent.GetComponent<PhotonView>().RPC("SetBlue", PhotonTargets.AllBuffered, null);
            }
            else
            {
                this.transform.parent.GetComponent<PhotonView>().RPC("SetRed", PhotonTargets.AllBuffered, null);
            }

            //            Debug.Log("PlayerStatus.cs : Teleport() : Inside isMine");
            this.transform.parent.GetComponent<TeamManager>().Respawn();
            Camera.main.transform.parent.GetComponent<PadTeleport>().enabled = true;
            Camera.main.transform.parent.GetComponent<PadTeleport>().blue = isBlue;
            Camera.main.transform.parent.GetComponent<SpellcastingGestureRecognition>().blue = isBlue;
            Camera.main.transform.parent.GetComponent<Edwon.VR.VRGestureRig>().enabled = false;
            Camera.main.transform.parent.GetComponent<SpellcastingGestureRecognition>().enabled = false;
            //            GameObject.Find("Camera (eye)").transform.LookAt(new Vector3(0, 0, 0));
        }
    }

    [PunRPC]
    public void SetSpellcastingEnabled(bool isEnabled)
    {
        Camera.main.transform.parent.GetComponent<Edwon.VR.VRGestureRig>().enabled = isEnabled;
        Camera.main.transform.parent.GetComponent<SpellcastingGestureRecognition>().enabled = isEnabled;
    }

    public void RemoveHat()
    {
        if (hat != null)
        {
            hat.GetComponent<HatLogic>().resetHat();
            hat = null;
        }
    }

    [PunRPC]
    public void ResetOnTeleporter()
    {
        this.onTeleporter = false;
    }
}