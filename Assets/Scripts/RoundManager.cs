using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
public class RoundManager : MonoBehaviour {


    public Transform hatRoom;
    public float roundTime;
    public bool isTimeBased = false;
    public int maxScore;
    public bool isScoreBased = true;
    private float timeElapsed;
    private bool inBattlefield = true;
    private bool hatsSelected = false;
    private List<GameObject> playerRigs = new List<GameObject>();
    private List<GameObject> players = new List<GameObject>();
    ScoreboardUpdater scoreboard;
    int score;
    private int blueMemb;
    private int redMemb;

    public GameObject practiceRoom;
    public GameObject[] arenas;
    public int arenaNum;
    //
//    public GameObject countdown_display;
    public GameObject restart_display;
    public GameObject countdown_display;
    private GameObject arena2;
    public GameObject scoreboard_prefab;
    
    //TODO score ssystem, if you want it to end the round
    // Use this for initialization
    void Start() {
/*        if (GameObject.FindGameObjectWithTag("Scoreboard")) {
            scoreboard = GameObject.FindGameObjectWithTag("Scoreboard").GetComponent<ScoreboardUpdater>();
        } else {
            print("COULD NOT FIND SCOREBOARD");
        }
*/
        hatRoom = GameObject.FindGameObjectWithTag("HatRoom").GetComponent<Transform>();

        if (GameObject.FindGameObjectWithTag("Pregame"))
        {
            practiceRoom = GameObject.FindGameObjectWithTag("Pregame");
            if (GameObject.FindGameObjectWithTag("Arena") != null)
            {
                practiceRoom.SetActive(false);
            }
            else
            {
                practiceRoom.SetActive(true);
            }
        }
    }

    // Update is called once per frame
    void Update() {

    }

    [PunRPC]
    public void Display_Countdown()
    {
        //Debug.Log("RoundManager.cs : Display_Countdown() : Inside");
        countdown_display.SetActive(true);
    }

    [PunRPC]
    public void Display_Restart(bool blueWon, int blue_score, int red_score)
    {
        Debug.Log("RoundManager.cs : Display_Restart() : blueWon = " + blueWon + ", red_score = " + red_score + ", blue_score = " + blue_score);
        scoreboard.roundOver = true;
        restart_display.SetActive(true);
        Restart_Display restart = restart_display.GetComponent<Restart_Display>();
        restart.SetWinner(blueWon);
        restart.SetScore(red_score, blue_score);
        GameObject.FindGameObjectWithTag("PowerUpManager").GetComponent<PowerupManager>().spawn_powerups = false;

        //Debug.Log("victory or defeat speech");
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("PCP"))
        {
            if (go.GetComponent<TeamManager>().blue == blueWon)
            {
                //Debug.Log("VICTORY SPEECH");
                GameObject.Find("Announcer").GetComponent<AnnouncerEvents>().PlaySound("victory");
            }
            else
            {
                //Debug.Log("DEFEAT SPEECH");
                GameObject.Find("Announcer").GetComponent<AnnouncerEvents>().PlaySound("defeat");
            }
        }
    }
  
    [PunRPC]
    public void EndRound()
    {
        practiceRoom.SetActive(true);
        //arena2.SetActive(false);

        Camera.main.transform.parent.position = GameObject.FindGameObjectWithTag("HatRoom").transform.position;

        print("ROUND ENDED, SHOULD HAVE TURNED OFF PLATFORMCONTROLLER");
        //        FindPlayers();

        foreach (GameObject playerRCP in GameObject.FindGameObjectsWithTag("Player"))
        {//TODO
            playerRCP.GetComponent<PlayerStatus>().RestartRound();
            playerRCP.GetComponent<PlayerStatus>().pregame = true;
        }
        foreach (GameObject curse in GameObject.FindGameObjectsWithTag("Curse"))
        {
            PhotonNetwork.Destroy(curse.GetPhotonView());
        }
        ChooseHats();
        //ShowFinalScoreboard();
        //       inBattlefield = false;
//        scoreboard.ResetScoreboard();
//        scoreboard.SetVisible(false);

        timeElapsed = 0;
        
        print ("END OF ENDROUND");
        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.Destroy(arena2.gameObject);
            PhotonNetwork.Destroy(scoreboard.gameObject);
        }
        GameObject.FindGameObjectWithTag("PowerUpManager").GetComponent<PowerupManager>().spawn_powerups = false;

        SetUnusedHatsVisible(true);
    }

    [PunRPC]
    void StartRound()
    {
        score = 0;
        timeElapsed = 0;
        inBattlefield = true;
        if (practiceRoom != null)
        {
            practiceRoom.SetActive(false);
            if(PhotonNetwork.isMasterClient)
            {
                arena2 = PhotonNetwork.InstantiateSceneObject(this.arenas[arenaNum].name, Vector3.zero, Quaternion.identity, 0, null);
                arenaNum = Random.Range(0, arenas.Length);

                scoreboard = PhotonNetwork.InstantiateSceneObject(this.scoreboard_prefab.name, new Vector3(0, 0, 0), Quaternion.identity, 0, null).GetComponent<ScoreboardUpdater>();
                scoreboard.maximumScore = maxScore;
            }
        }
//        scoreboard.SetVisible(true);
        print("Starting Round");
        Display_Countdown();
        foreach (GameObject playerRCP in GameObject.FindGameObjectsWithTag("Player"))
        {//TODO
            playerRCP.GetComponent<PlayerStatus>().pregame = false;
        }
        foreach (GameObject curse in GameObject.FindGameObjectsWithTag("Curse"))
        {
            PhotonNetwork.Destroy(curse.GetPhotonView());
        }

          

        Camera.main.transform.parent.GetComponent<SpellcastingGestureRecognition>().kill_spells();
        GameObject.FindGameObjectWithTag("PowerUpManager").GetComponent<PowerupManager>().spawn_powerups = true;

        SetUnusedHatsVisible(false);

        StartCoroutine(DelayAssignment());
    }

    IEnumerator DelayAssignment()
    {
        yield return new WaitForSeconds(1f);

        if (GameObject.FindGameObjectWithTag("Scoreboard") == null)
        {
            Debug.Log("COULD NOT FIND SCOREBOARD");
        }
        else
        {
            scoreboard = GameObject.FindGameObjectWithTag("Scoreboard").GetComponent<ScoreboardUpdater>();
            scoreboard.roundOver = false;
            arena2 = GameObject.FindGameObjectWithTag("Arena");
        }
    }

    void SetUnusedHatsVisible(bool isVisible)
    {
        //Debug.Log("SetHatsVisible is " + isVisible);
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Grabbable"))
        {
            HatLogic hat = go.GetComponent<HatLogic>();
            if (hat != null && hat.onHead == false)
            {
                hat.SetVisible(isVisible);
                //Debug.Log("Setting hat visible " + isVisible);
            }
        }
    }

    void ChooseHats()
    {
        foreach (GameObject player in playerRigs)
        {
            SendPlayerToHatRoom(player);                                                                  // UNCOMMENT

        }
        //foreach (GameObject playerRCP in GameObject.FindGameObjectsWithTag("Player"))
        //    //player.TakeOffHat();

        hatsSelected = false;

        if (GameObject.Find("RightController") == null)
        {
            return;
        }
    }

    void SendPlayerToHatRoom(GameObject player)
    {
        //Debug.Log("SENDPLAYERTOHATROOM CALLED");
        if (hatRoom)
        {
            Vector3 newPos = hatRoom.GetChild(Random.Range(0, hatRoom.childCount-1)).transform.position;
            if (!VRDevice.model.ToLower().Contains("oculus"))
            {
                player.transform.rotation = 
                Quaternion.Euler(0, player.transform.eulerAngles.y + (180 - Camera.main.transform.eulerAngles.y), 0);
            }
            else
            {
                player.transform.rotation = 
                Quaternion.Euler(0, 0, 0);
            }
            player.GetComponent<VRTK.VRTK_BasicTeleport>().ForceTeleport(newPos);
            
        }
        else
        {
            hatRoom = GameObject.FindGameObjectWithTag("HatRoom").GetComponent<Transform>();
            Vector3 newPos = hatRoom.GetChild(Random.Range(0, hatRoom.childCount - 1)).transform.position;
            player.GetComponent<VRTK.VRTK_BasicTeleport>().ForceTeleport(newPos, Quaternion.Euler(0,0,0));

        }

        //practiceRoom.SetActive(true);
        

    }
 //   [PunRPC]
 //   void UpdateScoreboard(bool blueScored)
 //   {
 //       //Why are we assigning this on runtime? It could be assigned through the NetworkManager.
 //       ScoreboardUpdater scoreboard = GameObject.FindGameObjectWithTag("Scoreboard").GetComponent<ScoreboardUpdater>();

 //       Debug.Log(GameObject.FindGameObjectWithTag("Scoreboard").name);

 //       if (scoreboard == null)
 //       {
 //           Debug.Log("SCOREBOARD UPDATER IS NULL!");
 //       }

 ////       Debug.Log("INSIDE RPC: BLUE SCORED " + blueScored);

 //       if (blueScored)
 //       {
 //           scoreboard.IncrementBlueScore();
 //       }
 //       else
 //       {
 //           scoreboard.IncrementRedScore();
 //       }
 //   }


    void ShowFinalScoreboard()
    {
        //TODO show whatever AG like to show in the end of round 
    }
    public void Subscribe(GameObject avatar, GameObject rig)
    {
        //Debug.Log(avatar.name + " subscribed");
        players.Add(avatar);
        playerRigs.Add(rig);
        //send 
//        SendPlayerToHatRoom(rig);
       
        
    }
    //public void AssignTeam(GameObject avatar)
    //{
    //    avatar.GetComponent<TeamManager>().SetAvatar(avatar.transform);
    //    setMembers();
    //    if (blueMemb >= redMemb)
    //    {
    //        avatar.GetComponent<TeamManager>().SetRed();

    //    }
    //    else
    //    {
    //        avatar.GetComponent<TeamManager>().SetBlue();
    //    }
    //}

    public void Unsubscribe(GameObject avatar, GameObject rig)
    {
        players.Remove(avatar);
        players.Remove(rig);
        //Debug.Log(avatar.name + " unsubscribed");
        if (avatar.GetComponent<TeamManager>().blue)
            blueMemb--;
        else
            redMemb--;
    }
    //private void setMembers()
    //{
    //    blueMemb = 0;
    //    redMemb = 0;
    //    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
    //    Debug.Log("Num players found: " + players.Length);
    //    foreach (GameObject p in players)
    //    {
    //        if(p.GetComponentInParent<TeamManager>().blue)
    //        {
    //            blueMemb++;   
    //        }
    //        else
    //        {
    //            redMemb++;
    //        }
    //    }
    //    Debug.Log("Red: " + redMemb + " Blue: " + blueMemb);
    //}
}
