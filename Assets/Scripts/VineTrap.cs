using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineTrap : MonoBehaviour {
    public float platform_stay = 20f;
    public float duration;
    public float durationTimer;
    public bool isSet;
    public bool isActivated;
    public Transform player;
    public PlayerStatus playerStatus;

    public float damagePerCycle;
    public float damageCycle;
    public float damageTimer;

    public Transform seed;
    public Transform explosion;
    public Transform body;

    PlatformNeighbors platform;

    private PhotonView player_photonView;
    private bool first = true;

    public bool blue;

    // Use this for initialization
    void Start() {
        SetUp();

    }

    // Update is called once per frame
    void Update() {
        if (isActivated)
        {
            /*
             * COMMENTED DUE TO CHANGE IN MECHANICS, VINE TRAP NOW CAN ONLY BE RID BY DISENCHANT SPELL *
            if (durationTimer > 0)
                durationTimer -= Time.deltaTime;
            else
                PhotonNetwork.Destroy(this.gameObject);
            */
            if (playerStatus.dead)
            {
                print(playerStatus.dead + " " + playerStatus.gameObject);
                playerStatus.EnableMovement(true);
                PhotonNetwork.Destroy(GetComponent<PhotonView>());
            }

            //Deals damage every time the timer reaches 0.
            if (damageTimer > 0)
                damageTimer -= Time.deltaTime;
            else
                DealDamage();
        }
    }
    void SetUp()
    {
        isSet = true;
        seed.gameObject.SetActive(true);
        StartCoroutine(DestroyAfterSeconds(platform_stay));
    }

    void Activate()
    {
        StopCoroutine(DestroyAfterSeconds(platform_stay));
        //Flag as activated.
        isActivated = true;
        //Disable seed particle.
        seed.gameObject.SetActive(false);

        //Enable explosion and body particles.
        explosion.gameObject.SetActive(true);
        body.gameObject.SetActive(true);

        //Disable player's movement.
        playerStatus.EnableMovement(false);

        //Start dealing damage.
        // 
        DealDamage();

        //Set duration timer.
        durationTimer = duration;
        StartCoroutine(DestroyAfterSeconds(duration));
    }

    [PunRPC]
    public void DestroyVines()
    {
        if (playerStatus != null)
            playerStatus.EnableMovement(true);
        if (GetComponent<PhotonView>().isMine)
            PhotonNetwork.Destroy(this.GetComponent<PhotonView>());
    }

    //[PunRPC]
    void DealDamage()
    {

        damageTimer = damageCycle;
        //Destroy if player dies.
        if (playerStatus != null)
            if (playerStatus.dead)
            {
                print(playerStatus.dead + " " + playerStatus.gameObject);
                //Enable movement before destroy itself.
                StopCoroutine(DestroyAfterSeconds(duration));
                isActivated = false;
                body.gameObject.SetActive(false);
                playerStatus.EnableMovement(true);
                if (GetComponent<PhotonView>().isMine)
                    PhotonNetwork.Destroy(GetComponent<PhotonView>());
                DestroyVines();
                GetComponent<PhotonView>().RPC("DestroyVines", PhotonTargets.AllBuffered, null);
                platform.hasVines = false;
                platform.GetComponent<PhotonView>().RPC("HasVines2", PhotonTargets.Others, false);
            }
    }

    private void OnTriggerEnter(Collider trigger)
    {
        if (isActivated) return;

        Transform other = trigger.transform;


        if (other.CompareTag("Player") && first)
        {
            first = false;
            player = other;
            playerStatus = player.GetComponent<PlayerStatus>();
            player_photonView = player.GetComponent<PlayerStatus>().photonView;
            Activate();
        }
    }
    IEnumerator DestroyAfterSeconds(float t)
    {
        yield return new WaitForSeconds(t);
        //DestroyVines();
        GetComponent<PhotonView>().RPC("DestroyVines", PhotonTargets.All, null);
    }

    public void SetPlatform(PlatformNeighbors pn)
    {
        platform = pn;
        platform.hasVines = true;
        platform.GetComponent<PhotonView>().RPC("HasVines2", PhotonTargets.Others, true);
    }

    [PunRPC]
    public void SetBlue(bool _blue)
    {
        this.blue = _blue;
    }
}
