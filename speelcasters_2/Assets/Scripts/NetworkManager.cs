using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ExitGames.Client.Photon;

public class NetworkManager : Photon.PunBehaviour
{
    [Tooltip("The maximum number of players per room")]
    public byte maxPlayersPerRoom = 4;

    public GameObject hat;

    private GameObject hat1;
    private GameObject hat2;
    private GameObject hat3;
    private GameObject hat4;
    private GameObject hat5;
    private GameObject hat6;
    public GameObject avatar;
    public GameObject scoreboard;
    public GlyphGuide guide;
    private GameObject cameraRig;
    public GameObject CameraRig
    {
        get
        {
            return cameraRig;
        }
    }
    public GameObject Avatar
    {
        get
        {
            return avatar;
        }
    }
    public Transform[] hatSpawns;

    public string roomName;

    PhotonView photonView;

    bool isConnecting;
    private int blues = 0;
    private int reds = 0;
    private int temp = 0;
    string _gameVersion = "1";
    public GameObject roundMan;
    public GameObject powerupManager;
    private PunTeams pt;
    void Awake()
    {
        // #Critical
        // we don't join the lobby. There is no need to join a lobby to get the list of rooms.
        PhotonNetwork.autoJoinLobby = false;

        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.automaticallySyncScene = true;



        Connect();
    }

    // Use this for initialization
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        hat1 = hat;
        hat2 = hat;
        hat3 = hat;
        hat4 = hat;
        hat5 = hat;
        hat6 = hat;
    }

    // Update is called once per frame
    void Update()
    {

        //if (roundMan.==null)
        //{

        //    if (roundMan = GameObject.FindGameObjectWithTag("RoundManager"))
        //    {
        //        roundMan.GetComponent<RoundManager>().Subscribe(avatar, cameraRig);
        //    }
        //}
    }

    public void Connect()
    {
        isConnecting = true;

        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.connected)
        {
            Debug.Log("Joining Room...");
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnPhotonRandomJoinFailed() and we'll create one.
            PhotonNetwork.JoinRoom(roomName);
        }
        else
        {
            Debug.Log("Connecting...");

            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings(_gameVersion);
        }
    }

    /// <summary>
    /// Called after the connection to the master is established and authenticated but only when PhotonNetwork.autoJoinLobby is false.
    /// </summary>
    public override void OnConnectedToMaster()
    {

        //			Debug.Log("Region:"+PhotonNetwork.networkingPeer.CloudRegion);

        // we don't want to do anything if we are not attempting to join a room. 
        // this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
        // we don't want to do anything.
        if (isConnecting)
        {
            Debug.Log("DemoAnimator/Launcher: OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room.\n Calling: PhotonNetwork.JoinRandomRoom(); Operation will fail if no room found");

            // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnPhotonRandomJoinFailed()
            PhotonNetwork.JoinRoom(roomName);
        }
    }

    /// <summary>
    /// Called when a JoinRandom() call failed. The parameter provides ErrorCode and message.
    /// </summary>
    /// <remarks>
    /// Most likely all rooms are full or no rooms are available. <br/>
    /// </remarks>
    /// <param name="codeAndMsg">codeAndMsg[0] is short ErrorCode. codeAndMsg[1] is string debug msg.</param>
    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        Debug.Log("DemoAnimator/Launcher:OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = this.maxPlayersPerRoom }, null);
    }
    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        Debug.Log("Join failed, Creating Room...");
        PhotonNetwork.CreateRoom(roomName);
    }

    /// <summary>
    /// Called after disconnecting from the Photon server.
    /// </summary>
    /// <remarks>
    /// In some cases, other callbacks are called before OnDisconnectedFromPhoton is called.
    /// Examples: OnConnectionFail() and OnFailedToConnectToPhoton().
    /// </remarks>
    public override void OnDisconnectedFromPhoton()
    {
        Debug.LogError("DemoAnimator/Launcher:Disconnected");

        isConnecting = false;

    }

    /// <summary>
    /// Called when entering a room (by creating or joining it). Called on all clients (including the Master Client).
    /// </summary>
    /// <remarks>
    /// This method is commonly used to instantiate player characters.
    /// If a match has to be started "actively", you can call an [PunRPC](@ref PhotonView.RPC) triggered by a user's button-press or a timer.
    ///
    /// When this is called, you can usually already access the existing players in the room via PhotonNetwork.playerList.
    /// Also, all custom properties should be already available as Room.customProperties. Check Room..PlayerCount to find out if
    /// enough players are in the room to start playing.
    /// </remarks>
    public override void OnJoinedRoom()
    {
        Debug.Log("DemoAnimator/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.\nFrom here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage");

        avatar = PhotonNetwork.Instantiate(this.avatar.name, new Vector3(0, 0, 0), Quaternion.identity, 0);

        if (PhotonNetwork.isMasterClient)
        {
            scoreboard = PhotonNetwork.InstantiateSceneObject(this.scoreboard.name, new Vector3(0, 0, 0), Quaternion.identity, 0, null);
            HatSpawn();
        }

        //	cameraRig = Camera.main.transform.parent.gameObject;
        cameraRig = GameObject.FindWithTag("CameraRig");
        cameraRig.GetComponent<SpellcastingGestureRecognition>().SetAvatar(avatar.transform);
        cameraRig.GetComponent<PlatformController>().SetAvatar(avatar);
        BookLogic book = avatar.GetComponentInChildren<BookLogic>();
        book.guide = guide;
        guide.book = book;

        if (PunTeams.PlayersPerTeam[PunTeams.Team.blue].Count >= PunTeams.PlayersPerTeam[PunTeams.Team.red].Count)
        {
            //avatar.GetComponent<TeamManager>().SetRed();
            avatar.GetComponent<PhotonView>().RPC("SetRed", PhotonTargets.AllBuffered, null);
            PhotonNetwork.player.SetTeam(PunTeams.Team.red);
        }
        else
        {
            //avatar.GetComponent<TeamManager>().SetBlue();
            avatar.GetComponent<PhotonView>().RPC("SetBlue", PhotonTargets.AllBuffered, null);
            PhotonNetwork.player.SetTeam(PunTeams.Team.blue);
        }
        //        avatar.GetComponent<TeamManager>().SetAvatar(avatar.transform);
        //avatar.GetComponent<TeamSetter>().SetTeam();
        //Debug.Log(PhotonNetwork.room.PlayerCount);

        //        if (PhotonNetwork.room.PlayerCount % 2 == 0)
        //		{
        //			//photonView.RPC("SetBlue", PhotonTargets.AllBuffered, null);
        //			//localPlayer.GetComponentInParent<TeamManager>().SetBlue();
        ////			avatar.GetComponent<TeamManager>().SetBlue();
        //		}
        //		else
        //		{
        //			//photonView.RPC("SetRed", PhotonTargets.AllBuffered, null);
        //			//localPlayer.GetComponentInParent<TeamManager>().SetRed();
        ////			avatar.GetComponent<TeamManager>().SetRed();

        //		}

        //temp++;
        //localPlayer.GetComponentInParent<TeamManager>().SetRed();
        //localPlayer.GetComponent<SpellcastingGestureRecognition>().SetAvatar(avatar.transform);
        if (PhotonNetwork.isMasterClient)
        {
            roundMan = PhotonNetwork.InstantiateSceneObject(this.roundMan.name, new Vector3(0, 0, 0), Quaternion.identity, 0, null);
            roundMan.GetComponent<RoundManager>().Subscribe(avatar, cameraRig);
            scoreboard.GetComponent<ScoreboardUpdater>().maximumScore = roundMan.GetComponent<RoundManager>().maxScore;
         
            powerupManager = PhotonNetwork.InstantiateSceneObject(this.powerupManager.name, new Vector3(0, 0, 0), Quaternion.identity, 0, null);
        }
        else
            roundMan.GetComponent<RoundManager>().Subscribe(avatar, cameraRig);
    }

    /// <summary>
    /// Called when a Photon Player got connected. We need to then load a bigger scene.
    /// </summary>
    /// <param name="other">Other.</param>

    public override void OnPhotonPlayerConnected(PhotonPlayer other)
    {


        Debug.Log("OnPhotonPlayerConnected() " + other.NickName); // not seen if you're the player connecting
    }

    /// <summary>
    /// Called when a Photon Player got disconnected. We need to load a smaller scene.
    /// </summary>
    /// <param name="other">Other.</param>
    public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
    {
        Debug.Log("OnPhotonPlayerDisconnected() " + other.NickName); // seen when other disconnects
    }

    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        roundMan.GetComponent<RoundManager>().Unsubscribe(avatar, cameraRig);

    }

    public void SetMat()
    {
    }



    public void HatSpawn()
    {


        //		hat1 = PhotonNetwork.Instantiate(this.hat1.name, hatSpawns[0].position, Quaternion.identity, 0);
        //		hat1.GetComponent<HatLogic>().callSetClass(PlayerClass.attack);
        //
        //		hat2 = PhotonNetwork.Instantiate(this.hat2.name, hatSpawns[1].position, Quaternion.identity, 0);
        //		hat2.GetComponent<HatLogic>().callSetClass(PlayerClass.support);
        //
        //		hat3 = PhotonNetwork.Instantiate(this.hat3.name, hatSpawns[2].position, Quaternion.identity, 0);
        //		hat3.GetComponent<HatLogic>().callSetClass(PlayerClass.heal);
        //
        //		hat4 = PhotonNetwork.Instantiate(this.hat4.name, hatSpawns[3].position, Quaternion.identity, 0);
        //		hat4.GetComponent<HatLogic>().callSetClass(PlayerClass.attack);
        //
        //		hat5 = PhotonNetwork.Instantiate(this.hat5.name, hatSpawns[4].position, Quaternion.identity, 0);
        //		hat5.GetComponent<HatLogic>().callSetClass(PlayerClass.support);
        //
        //		hat6 = PhotonNetwork.Instantiate(this.hat6.name, hatSpawns[5].position, Quaternion.identity, 0);
        //		hat6.GetComponent<HatLogic>().callSetClass(PlayerClass.heal);	}

    }
}