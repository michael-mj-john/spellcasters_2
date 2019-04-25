using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Edwon.VR;
using Edwon.VR.Gesture;
using UnityEngine.VR;

public class SpellcastingGestureRecognition : MonoBehaviour
{

    public VRGestureRig gestureRig;

    public Gradient greenHighlight;

    PadTeleport padTeleport;
    public ParticleSystem drawEffect;

    public Gradient baseGradient;

    public GameObject fireball;
    public string fireballGesture;
    public Gradient fireballGradient;
    public float fireballCooldown = 2f;

    public GameObject shield;
    public string shieldGesture;
    public Gradient shieldGradient;
    public float shieldCooldown = 6f;

    public GameObject Bubble_shield;
    public string Bubble_shieldGesture;
    public Gradient Bubble_shieldGradient;
    public float Bubble_shieldCooldown = 6f;

    public GameObject heal;
    public string healGesture;
    public Gradient healGradient;
    public float healCooldown = 1f;

    public GameObject vines;
    public string vinesGesture;
    public Gradient vinesGradient;
    public float vinesCooldown = 2f;

    public GameObject iceball;
    public string iceballGesture;
    public Gradient iceballGradient;
    public float iceballCooldown = 2f;

    public GameObject meteor;
    public string meteorGesture;
    public Gradient meteorGradient;
    public float meteorCooldown = 2f;

    public GameObject pongShield;
    public string pongShieldGesture;
    public Gradient pongShieldGradient;
    public float pongShieldCooldown = 2f;

    public GameObject platformSteal;
    public string platformStealGesture;
    public Gradient platformStealGradient;
    public float platformStealCooldown = 2f;

    public GameObject lightBlade;
    public string lightBladeGesture;
    public Gradient lightBladeGradient;
    public float lightBladeCooldown = 2f;

    public GameObject hammer;
    public string hammerGesture;
    public Gradient hammerGradient;

    public GameObject disenchant;
    public string disenchantGesture;
    public Gradient disenchantGradient;
    public float disenchantCooldown = 2f;

    public GameObject swipeLeft;
    public GameObject swipeRight;
    public AudioClip cast_success;
    public AudioClip cast_failure;
    public AudioClip cast_cooldown;
    public Transform wand;
    public Transform book;
    //[HideInInspector]
    public Targeting target;
    public Transform avatar;
    public Transform torso;
    public Transform padHit;

    public PlayerStatus playerStatus;

    public bool blue = false;

    public bool noHats = true;
    //public AudioClip spell_deflected;
    //--->Private Vars<---//
    Camera mainCam;
    float triggerL;
    float triggerR;
    bool triggerUsed = false;
    private AudioSource audioSource;

    public bool hasSpell;
    public GameObject currentSpell;
    public string currentSpellName;
    public Gradient currentSpellGradient;
    public float spellCooldown = 3f;
    private float spellTimer = 0;
    [HideInInspector]
    public bool isCoolingDown = false;
    bool isOculus = false;

    // Variables for targeting platforms
    private BeamTrail beamTrail;
    public GameObject reticle;
    private LineRenderer lineRend;
    public Gradient accurateTarget;
    public Gradient inaccurateTarget;

    [HideInInspector]
    SpellCooldowns cooldowns;
    [HideInInspector]
    public float fireCD, iceCD, swordCD, meteorCD, shieldCD, pongCD, vinesCD, healCD, blessingCD, flipCD, hammerCD, bubbleCD;

    private bool iceball_cast;

    // Vibration variables
    [HideInInspector]
    public int leftControllerIndex;
    [HideInInspector]
    public int rightControllerIndex;
    bool vibrateLoop;
    float vibrateStart;
    ushort vibrateIntensity;
    float length;

    NotificationManager nm;

    //Oculus

    public LayerMask platformLayers;

    RaycastHit hit;

    float dt = 0.0f;

    private void Awake()
    {
        padTeleport = GetComponent<PadTeleport>();
    }

    private void Start()
    {
        if (VRDevice.model.ToLower().Contains("oculus"))
        {
            isOculus = true;
        }

        mainCam = Camera.main;
        audioSource = GetComponent<AudioSource>();
        target = GetComponent<Targeting>();
        if (gestureRig == null)
            gestureRig = this.GetComponent<VRGestureRig>();
        //print(target.pointer);
        if (target.pointer.Find("BeamTrail").gameObject.GetActive() == false)
            target.pointer.Find("BeamTrail").gameObject.SetActive(true);

        beamTrail = target.pointer.GetComponentInChildren<BeamTrail>();
        //print(beamTrail);
        lineRend = beamTrail.GetComponent<LineRenderer>();
        lineRend.colorGradient = inaccurateTarget;
        beamTrail.gameObject.SetActive(false);
        reticle.SetActive(false);
        cooldowns = GetComponent<SpellCooldowns>();
    }

    void OnEnable()
    {
        GestureRecognizer.GestureDetectedEvent += OnGestureDetected;
        GestureRecognizer.GestureRejectedEvent += OnGestureRejected;
    }

    void OnDisable()
    {
        GestureRecognizer.GestureDetectedEvent -= OnGestureDetected;
        GestureRecognizer.GestureRejectedEvent -= OnGestureRejected;
        wand.Find("tip").Find("flames").gameObject.GetComponent<ParticleSystem>().Stop();
        currentSpell = null;
        hasSpell = false;
        currentSpellName = "";
    }


    private void Update()
    {
        if (isOculus)
        {
            triggerR = Input.GetAxis("OculusRightTrigger");
        }

        //Check if we're cooling down.
        if (isCoolingDown)
        {
            //If fireball timer is still active.
            //if (spellTimer > 0)
            //{
            //    spellTimer -= Time.deltaTime;
            //}

            if (fireCD > 0)
            {
                fireCD -= Time.deltaTime;
            }

            if (iceCD > 0)
            {
                iceCD -= Time.deltaTime;
            }

            if (swordCD > 0)
            {
                swordCD -= Time.deltaTime;
            }

            if (meteorCD > 0)
            {
                meteorCD -= Time.deltaTime;
            }

            if (shieldCD > 0)
            {
                shieldCD -= Time.deltaTime;
            }

            if (bubbleCD > 0)
            {
                bubbleCD -= Time.deltaTime;
            }

            if (pongCD > 0)
            {
                pongCD -= Time.deltaTime;
            }

            if (vinesCD > 0)
            {
                vinesCD -= Time.deltaTime;
            }

            if (healCD > 0)
            {
                healCD -= Time.deltaTime;
            }

            if (blessingCD > 0)
            {
                blessingCD -= Time.deltaTime;
            }

            if (flipCD > 0)
            {
                flipCD -= Time.deltaTime;
            }

            if (hammerCD > 0)
            {
                hammerCD -= Time.deltaTime;
            }

            if (fireCD <= 0 && iceCD <= 0 && swordCD <= 0 && meteorCD <= 0 && shieldCD <= 0 && pongCD <= 0 && vinesCD <= 0 && healCD <= 0 && blessingCD <= 0 && flipCD <= 0 && hammerCD <= 0 && bubbleCD <= 0)
            {
                isCoolingDown = false;
                //GetComponent<VRGestureRig>().enabled = true;
                if (wand != null)
                {
                    wand.Find("tip").Find("spark").GetComponent<ParticleSystem>().Play();
                    audioSource.PlayOneShot(cast_success);

                    //wand.Find("tip").Find("spark").Find("dust").GetComponent<ParticleSystem>().Play();
                }

            }
        }
        if ((Input.GetKeyDown("joystick button 15") && !isOculus) || (triggerR > 0.35f && isOculus))
        {
            if (hasSpell)
                CastSpell();
            else if (!isCoolingDown && !iceball_cast)
                drawEffect.Play();
        }
        if (Input.GetKeyUp("joystick button 15"))
        {
            if (iceball_cast)
            {
                iceball_cast = false;
            }
            else
            {
                drawEffect.Stop();
                GetComponent<VRGestureRig>().enabled = true;
                if (!hasSpell && !isCoolingDown && wand != null)
                {
                    wand.Find("tip").Find("flames").gameObject.GetComponent<ParticleSystem>().Stop();

                }
            }
        }

        // Check if targeting platform
        if (currentSpellName == "vines" || currentSpellName == "platformSteal")
        {
            if (beamTrail.gameObject.active == false)
                beamTrail.gameObject.SetActive(true);

            //Physics.queriesHitTriggers = false;
            if (Physics.Raycast(target.pointer.position, target.pointer.forward, out hit, 1000, platformLayers))
            {
                beamTrail.destination = (hit.point);
                reticle.SetActive(true);
                reticle.transform.position = hit.point;
                if (padHit == null || padHit != hit.transform)
                {
                    if (padHit != null && padHit.childCount > 0)
                    {
                        disableHighlight(padHit, !blue);
                    }

                    padHit = hit.transform;

                    if (padHit.gameObject.tag == "GrayPlatform" || blue && padHit.gameObject.tag == "RedPlatform" || !blue && padHit.gameObject.tag == "BluePlatform")
                    {
                        AccurateTarget();
                        beamTrail.destination = hit.point;
                        enableHighlight(padHit, blue);
                    }
                    else
                    {
                        InaccurateTarget();
                    }
                }
            }
            else
            {
                hit.point = (target.pointer.position + target.pointer.forward * 25);
                InaccurateTarget();
            }
        }
        else if (currentSpellName == "disenchant")
        {
            //Physics.queriesHitTriggers = false;
            if (target.result2 != null)
            {
                if (target.result2.tag == "Curse")
                {
                    AccurateTargetBlessing();
                }
                else
                {
                    InaccurateTargetBlessing();
                }
            }
            else
            {
                InaccurateTargetBlessing();
            }

        }
        else
        {
            beamTrail.gameObject.SetActive(false);
            reticle.SetActive(false);
        }
    }

    //Sets spell properties.
    void SetSpell(GameObject spell, string spellName, Gradient spellGradient)
    {
        currentSpell = spell;
        currentSpellName = spellName;
        currentSpellGradient = spellGradient;

        hasSpell = true;

        IgniteFlame(currentSpellGradient);

        //GetComponent<VRGestureRig>().enabled = false;
        audioSource.PlayOneShot(cast_success);

    }

    public void SetRandomSpell()
    {
        int random = Random.Range(0, 9);

        switch (random)
        {
            case 0:
                currentSpell = fireball;
                currentSpellName = "fireball";
                currentSpellGradient = fireballGradient;
                break;
            case 1:
                currentSpell = shield;
                currentSpellName = "shield";
                currentSpellGradient = shieldGradient;
                break;
            case 2:
                currentSpell = heal;
                currentSpellName = "heal";
                currentSpellGradient = healGradient;
                break;
            case 3:
                currentSpell = vines;
                currentSpellName = "vines";
                currentSpellGradient = vinesGradient;
                break;
            case 4:
                currentSpell = iceball;
                currentSpellName = "iceball";
                currentSpellGradient = iceballGradient;
                break;
            case 5:
                currentSpell = meteor;
                currentSpellName = "meteor";
                currentSpellGradient = meteorGradient;
                break;
            case 6:
                currentSpell = pongShield;
                currentSpellName = "pongShield";
                currentSpellGradient = pongShieldGradient;
                break;
            case 7:
                currentSpell = platformSteal;

                currentSpellName = "platformSteal";
                currentSpellGradient = platformStealGradient;
                break;
            case 8:
                currentSpell = lightBlade;
                currentSpellName = "lightBlade";
                currentSpellGradient = lightBladeGradient;
                break;
            case 9:
                currentSpell = Bubble_shield;
                currentSpellName = "Bubble_shield";
                currentSpellGradient = Bubble_shieldGradient;
                break;
            default:
                break;
        }

        hasSpell = true;

        IgniteFlame(currentSpellGradient);

        //GetComponent<VRGestureRig>().enabled = false;
        audioSource.PlayOneShot(cast_success);
    }
    public void SetRandomSuperSpell()
    {
        
        currentSpell = lightBlade;
        currentSpellName = "lightBlade";
        currentSpellGradient = lightBladeGradient;
                

        hasSpell = true;

        IgniteFlame(currentSpellGradient);

        //GetComponent<VRGestureRig>().enabled = false;
        //audioSource.PlayOneShot(cast_success);
    }

    //Updates the color of the wand flame and restarts it.
    void IgniteFlame(Gradient flameGradient)
    {
        if (wand != null)
        {
            ParticleSystem wandParticle = wand.Find("tip").Find("flames").gameObject.GetComponent<ParticleSystem>();
            wandParticle.Stop();
            var wandParticleModule = wandParticle.colorOverLifetime;
            wandParticleModule.color = flameGradient;
            wandParticle.Play();
        }
    }
    void OnGestureDetected(string gestureName, double confidence, Handedness hand, bool isDouble)
    {
        Color gestureStartColor = Color.red;
        Color gestureEndColor = Color.red;

        switch (gestureName)
        {
        case "Jay":
            if ((playerStatus.playerClass == PlayerClass.attack || playerStatus.playerClass == PlayerClass.all || noHats == true) && fireCD <= 0)
            {
                SetSpell(fireball, "fire", fireballGradient);
                gestureStartColor = Color.green;
                gestureEndColor = Color.green;
            }
            else if (fireCD > 0)
            {
                gestureStartColor = Color.blue;
                gestureEndColor = Color.blue;
                audioSource.PlayOneShot(cast_cooldown);
                    Notify_Cooldown();
                }
            break;
        case "Shield":
            if ((playerStatus.playerClass == PlayerClass.support || playerStatus.playerClass == PlayerClass.all || noHats == true) && shieldCD <= 0)
            {
                SetSpell(shield, "shield", shieldGradient);
                gestureStartColor = Color.green;
				gestureEndColor = Color.green;
            }
            else if (shieldCD > 0)
            {
                gestureStartColor = Color.blue;
                gestureEndColor = Color.blue;
				audioSource.PlayOneShot(cast_cooldown);
                    Notify_Cooldown();
                }
            break;
        case "Elle":
            if ((playerStatus.playerClass == PlayerClass.support || playerStatus.playerClass == PlayerClass.all || noHats == true) && bubbleCD <= 0)
            {
                SetSpell(Bubble_shield, "Bubble_shield", Bubble_shieldGradient);
                gestureStartColor = Color.green;
                gestureEndColor = Color.green;
            }
            else if (bubbleCD > 0)
            {
                gestureStartColor = Color.blue;
                gestureEndColor = Color.blue;
				audioSource.PlayOneShot(cast_cooldown);
                    Notify_Cooldown();
                }
            break;
        case "Heal":
            if ((playerStatus.playerClass == PlayerClass.heal || playerStatus.playerClass == PlayerClass.all || noHats == true) && healCD <= 0)
            {
                SetSpell(heal, "heal", healGradient);
                gestureStartColor = Color.green;
                gestureEndColor = Color.green;
            }
            else if (healCD > 0)
            {
                gestureStartColor = Color.blue;
                gestureEndColor = Color.blue;
                audioSource.PlayOneShot(cast_cooldown);
                    Notify_Cooldown();
                }
            break;
        case "Spring":
            if ((playerStatus.playerClass == PlayerClass.heal || playerStatus.playerClass == PlayerClass.all || noHats == true) && vinesCD <= 0)
            {
                SetSpell(vines, "vines", vinesGradient);
                gestureStartColor = Color.green;
                gestureEndColor = Color.green;
            }
            else if (vinesCD > 0)
            {
                gestureStartColor = Color.blue;
                gestureEndColor = Color.blue;
                audioSource.PlayOneShot(cast_cooldown);
                    Notify_Cooldown();
                }
            break;
        case "Bolt":
            if ((playerStatus.playerClass == PlayerClass.attack || playerStatus.playerClass == PlayerClass.all || noHats == true) && iceCD <= 0)
            {
                SetSpell(iceball, "iceball", iceballGradient);
                gestureStartColor = Color.green;
                gestureEndColor = Color.green;
            }
            else if (iceCD > 0)
            {
                gestureStartColor = Color.blue;
                gestureEndColor = Color.blue;
                audioSource.PlayOneShot(cast_cooldown);
                    Notify_Cooldown();
                }
            break;
        case "Wave":
            if ((playerStatus.playerClass == PlayerClass.attack || playerStatus.playerClass == PlayerClass.all || noHats == true) && meteorCD <= 0)
            {
                SetSpell(meteor, "meteor", meteorGradient);
                gestureStartColor = Color.green;
                gestureEndColor = Color.green;
            }
            else if (meteorCD > 0)
            {
                gestureStartColor = Color.blue;
                gestureEndColor = Color.blue;
                audioSource.PlayOneShot(cast_cooldown);
                    Notify_Cooldown();
                }
            break;
            case "OpenFrame":
                if ((playerStatus.playerClass == PlayerClass.support|| playerStatus.playerClass == PlayerClass.all || noHats == true) && hammerCD <= 0)
                {
                    SetSpell(hammer, "hammer", hammerGradient);
                    gestureStartColor = Color.green;
                    gestureEndColor = Color.green;
                }
                else if (hammerCD > 0)
                {
                    gestureStartColor = Color.blue;
                    gestureEndColor = Color.blue;
                    audioSource.PlayOneShot(cast_cooldown);
                    Notify_Cooldown();
                }
                break;
            case "Star":
            if ((playerStatus.playerClass == PlayerClass.heal || playerStatus.playerClass == PlayerClass.all || noHats == true) && flipCD <= 0)
            {
                SetSpell(platformSteal, "platformSteal", platformStealGradient);
                gestureStartColor = Color.green;
                gestureEndColor = Color.green;
            }
            else if (flipCD > 0)
            {
                gestureStartColor = Color.blue;
                gestureEndColor = Color.blue;
                audioSource.PlayOneShot(cast_cooldown);
                    Notify_Cooldown();
                }
            break;
        //case "Zed":
        //    if ((playerStatus.playerClass == PlayerClass.attack || playerStatus.playerClass == PlayerClass.all || noHats == true) && swordCD <= 0)
        //    {
        //        SetSpell(lightBlade, "lightBlade", lightBladeGradient);
        //        gestureStartColor = Color.green;
        //        gestureEndColor = Color.green;
        //    }
        //    else if (swordCD > 0)
        //    {
        //        gestureStartColor = Color.blue;
        //        gestureEndColor = Color.blue;
        //        audioSource.PlayOneShot(cast_failure);
        //            Notify_Cooldown();
        //        }
        //    break;
        /*case "Zed":
            if ((playerStatus.playerClass == PlayerClass.attack || playerStatus.playerClass == PlayerClass.all || noHats == true) && swordCD <= 0)
            {
                SetSpell(lightBlade, "lightBlade", lightBladeGradient);
                gestureStartColor = Color.green;
                gestureEndColor = Color.green;
            }
            else if (swordCD > 0)
            {
                gestureStartColor = Color.blue;
                gestureEndColor = Color.blue;
                audioSource.PlayOneShot(cast_failure);
                    Notify_Cooldown();
                }
            break;
          */
    //    case "Hourglass":
    //        if ((playerStatus.playerClass == PlayerClass.heal || playerStatus.playerClass == PlayerClass.all || noHats == true) && swordCD <= 0)
    //        {
    //            SetSpell(disenchant, "disenchant", disenchantGradient);
    //            gestureStartColor = Color.green;
    //            gestureEndColor = Color.green;
    //        }
    //        else if (swordCD > 0)
    //        {
    //            gestureStartColor = Color.blue;
    //            gestureEndColor = Color.blue;
    //            audioSource.PlayOneShot(cast_failure);
				//Notify_Cooldown ();
    //        }
    //        break;
        }

        //Set gesture as successful.
        if (gestureRig.rightCapture.myTrail != null) gestureRig.rightCapture.myTrail.UpdateRenderer(gestureStartColor, gestureEndColor, gestureRig.gestureMaterial);
    }

	void Notify_Cooldown() {
		// look for notification manager if it isn't already set
		if (nm == null)
		{
			if (Camera.main == null)
			{
				Debug.Log ("SpellcastingGestureRecognition.cs : Notify_Cooldown() : Could not find Camera.main");
				return;
			}

			nm = Camera.main.GetComponent<NotificationManager> ();
			if (nm == null)
			{
				Debug.Log ("SpellcastingGestureRecognition.cs : Notify_Cooldown() : Could not find notification manager on Camera.main");
				return;
			}
		}

		//
		nm.SetNotification("Spell is not ready");
	}

    void OnGestureRejected(string error, string gestureName = null, double confidenceValue = 0)
    {
        audioSource.PlayOneShot(cast_failure);

        //Set gesture as failure.
        if (gestureRig.rightCapture.myTrail != null) gestureRig.rightCapture.myTrail.UpdateRenderer(Color.red, Color.red, gestureRig.gestureMaterial);
    }

    //Get avatar's wand and book.
    public void SetAvatar(Transform _avatar)
    {
        avatar = _avatar;
        torso = avatar.Find("Torso");
        wand = avatar.Find("Right Hand").Find("MagicWand");
        book = avatar.Find("Left Hand").Find("SpellBook");
        book.GetComponent<BookLogic>().spellcast = this;
        playerStatus = torso.GetComponent<PlayerStatus>();
    }

    //Casts selected spell.
    private void CastSpell()
    {
        Vibrate(.05f, 3999);

        GameObject spellInstance = null;
        Transform wandTip = wand.Find("tip");
        Quaternion spellRotation;
        BaseSpellClass baseSpellClass;
        switch (currentSpellName)
        {
            case "fire":
                spellRotation = target.result != null && target.result.CompareTag("Player") ? Quaternion.LookRotation(target.result.position - wandTip.transform.position) : wandTip.rotation;
                spellInstance = PhotonNetwork.Instantiate(currentSpell.name, wandTip.position, spellRotation, 0);
                spellInstance.GetComponent<FireballNew>().SetBlue(blue);
                fireCD = cooldowns.fireCD;
                //spellTimer = fireballCooldown;
                //if (baseSpellClass = spellInstance.GetComponent<BaseSpellClass>())
                //{
                //    SetSpellOwner(baseSpellClass);
                //}
                break;
            case "iceball":
                spellRotation = wandTip.rotation;
                spellInstance = PhotonNetwork.Instantiate(currentSpell.name, wandTip.position, spellRotation, 0);
                spellInstance.GetComponent<IceBall_1>().blue = avatar.GetComponent<TeamManager>().blue;
                spellInstance.GetComponent<IceBall_1>().spellcast = this;
                iceCD = cooldowns.iceCD;
                iceball_cast = true;
                StartCoroutine(IceballCast());
                //spellTimer = iceballCooldown;
                break;
            case "shield":
                //spellInstance = PhotonNetwork.Instantiate(currentSpell.name, wandTip.position + wandTip.forward, Camera.main.transform.rotation, 0);
                //spellInstance = PhotonNetwork.Instantiate(currentSpell.name, wandTip.position + wandTip.forward, wandTip.rotation, 0);
                //spellInstance.transform.SetParent(wandTip);
                spellInstance = PhotonNetwork.Instantiate(currentSpell.name, book.position + book.forward, book.rotation, 0);
                spellInstance.GetComponent<Shield>().SetBook(book);
                spellInstance.GetComponent<Shield>().owner = torso;
                //spellInstance.GetComponent<Shield>().SetBlue(avatar.GetComponent<TeamManager>().blue);
                spellInstance.gameObject.GetPhotonView().RPC("SetBlue", PhotonTargets.All, avatar.GetComponent<TeamManager>().blue);

                shieldCD = cooldowns.shieldCD;
                //                spellInstance.transform.SetParent(book);
                //spellTimer = shieldCooldown;
                break;
            case "Bubble_shield":


                if (target.result != null && target.result.tag == "Player")
                {
                    spellInstance = PhotonNetwork.Instantiate(currentSpell.name, target.result.transform.position, target.result.transform.rotation, 0);
                    spellInstance.GetComponent<Bubble_shield>().SetTorso(target.result.transform);
                    spellInstance.gameObject.GetPhotonView().RPC("SetBlue", PhotonTargets.All, target.result.GetComponentInParent<TeamManager>().blue);
                    spellInstance.GetComponent<Bubble_shield>().owner = target.result.transform;
                }
                //else
                //{
                //    spellInstance = PhotonNetwork.Instantiate(currentSpell.name, torso.position, torso.rotation, 0);
                //    spellInstance.GetComponent<Bubble_shield>().SetTorso(torso);
                //    spellInstance.gameObject.GetPhotonView().RPC("SetBlue", PhotonTargets.All, avatar.GetComponent<TeamManager>().blue);
                //    spellInstance.GetComponent<Bubble_shield>().owner = torso;
                //}
               bubbleCD = cooldowns.bubbleCD;
                break;
            case "heal":
                // Heal others
                if (target.result != null)
                {
                    spellInstance = PhotonNetwork.Instantiate(currentSpell.name, target.result.transform.position + new Vector3(-1, 0, 0), currentSpell.transform.rotation, 0);
                }

                // Self heal
                else
                {
                    //print("self heal");
                    //print(avatar);
                    spellInstance = PhotonNetwork.Instantiate(currentSpell.name, torso.transform.position + new Vector3(-1, 0, 0), currentSpell.transform.rotation, 0);
                }
                healCD = cooldowns.healCD;
                //spellTimer = healCooldown;
                break;
            case "vines":
                //Check if target is a platform, otherwise don't do anything.
                if (target == null || target.result == null)
                {
                    return;
                }

                if (target.result.gameObject.layer == LayerMask.NameToLayer("BluePlatform") || target.result.gameObject.layer == LayerMask.NameToLayer("RedPlatform") || target.result.gameObject.layer == LayerMask.NameToLayer("GrayPlatform"))
                {
                    spellInstance = PhotonNetwork.Instantiate(vines.name, target.result.position, new Quaternion(), 0);
                    spellInstance.GetComponent<VineTrap>().SetPlatform(target.result.GetComponent<PlatformNeighbors>());
                    //spellInstance.GetComponent<VineTrap>().blue = avatar.GetComponent<TeamManager>().blue;
                    spellInstance.gameObject.GetPhotonView().RPC("SetBlue", PhotonTargets.All, avatar.GetComponent<TeamManager>().blue);
                    vinesCD = cooldowns.vinesCD;

                    if (padHit != null)
                    {
                        if (padHit.childCount > 0)
                        {
                            disableHighlight(padHit, !blue);
                        }
                        padHit = null;
                    }
                }
                else
                {
                    return;
                }
                break;
            //case "pongShield":
            //    spellRotation = new Quaternion();
            //    spellInstance = PhotonNetwork.Instantiate(currentSpell.name, wandTip.position, spellRotation, 0);
            //    spellInstance.GetComponent<Pong_Shield>().SetBlue(avatar.GetComponent<TeamManager>().blue);
            //    pongCD = cooldowns.pongCD;
            //    //spellTimer = pongShieldCooldown;
            //    break;
            case "meteor":
                spellRotation = wandTip.rotation;
                spellInstance = PhotonNetwork.Instantiate(currentSpell.name, wandTip.position, spellRotation, 0);
                spellInstance.GetComponent<MeteorSpell>().blue = avatar.GetComponent<TeamManager>().blue;
                spellInstance.GetComponent<MeteorSpell>().spellcast = this;
                meteorCD = cooldowns.meteorCD;
                // spellTimer = meteorCooldown;
                break;
            case "platformSteal":
                //Check if target is a platform, otherwise don't do anything.
                if (target == null || target.result == null)
                {
                    Debug.Log("target for platform steal is null");
                    return;
                }
                if (target.result.gameObject.layer == LayerMask.NameToLayer("BluePlatform") || target.result.gameObject.layer == LayerMask.NameToLayer("RedPlatform") || target.result.gameObject.layer == LayerMask.NameToLayer("GrayPlatform"))
                {
                    if (target.result.gameObject.layer == LayerMask.NameToLayer("GrayPlatform"))
                    {
                        target.result.GetComponent<PhotonView>().RPC("ChangeColorTo", PhotonTargets.AllBuffered, !blue);
                    }
                    else
                    {
                        target.result.GetComponent<PhotonView>().RPC("ChangeColor", PhotonTargets.AllBuffered, null);
                    }

                    spellInstance = PhotonNetwork.Instantiate(platformSteal.name, target.result.position, new Quaternion(), 0);
                    flipCD = cooldowns.flipCD;

                    if (padHit != null)
                    {
                        if (padHit.childCount > 0)
                        {
                            disableHighlight(padHit, !blue);
                        }
                        padHit = null;
                    }
                }
                else
                {
                    return;
                }

                break;
            case "lightBlade":
                spellInstance = PhotonNetwork.Instantiate(currentSpell.name, wandTip.position, wandTip.rotation, 0);
                spellInstance.GetComponent<LightBlade>().SetBlue(avatar.GetComponent<TeamManager>().blue);
                spellInstance.GetComponent<LightBlade>().SetWand(wandTip);

                swordCD = cooldowns.swordCD;
                spellTimer = lightBladeCooldown;
                break;

            case "hammer":
                spellInstance = PhotonNetwork.Instantiate(currentSpell.name, wandTip.position, wandTip.rotation, 0);
                //spellInstance.GetComponent<LightBlade>().SetBlue(avatar.GetComponent<TeamManager>().blue);
                //spellInstance.GetComponent<LightBlade>().SetWand(wandTip);
                spellInstance.gameObject.GetPhotonView().RPC("SetBlue", PhotonTargets.All, avatar.GetComponent<TeamManager>().blue);
                spellInstance.GetComponent<GlassHammer>().SetWand(wandTip);
                hammerCD = cooldowns.hammerCD;
                break;
            default:
                //spellTimer = spellCooldown;
                break;
        }

        if (wand != null)
        {
            wand.Find("tip").Find("flames").gameObject.GetComponent<ParticleSystem>().Stop();
            ParticleSystem ps = wand.Find("tip").Find("smoke").GetComponent<ParticleSystem>();
            ParticleSystem.MainModule main = ps.main;
            ps.Stop();
            main.duration = spellTimer;
            ps.Play();
        }

        isCoolingDown = true;
        hasSpell = false;
        currentSpell = null;
        //target.currentSpellName = "";
        currentSpellName = "";
        GetComponent<VRGestureRig>().enabled = false;

    }
    void SetSpellOwner(BaseSpellClass bsp)
    {
        if (playerStatus.photonView.isMine)
        {
            bsp.SetOwner(avatar.gameObject);
        }
    }
    public void kill_spells()
    {
        wand.Find("tip").Find("flames").gameObject.GetComponent<ParticleSystem>().Stop();
        currentSpell = null;
        hasSpell = false;
        currentSpellName = "";
    }
    // Successfully target a platform
    void AccurateTarget()
    {
        if (lineRend.colorGradient == accurateTarget)
            return;

        beamTrail.gameObject.SetActive(true);
        reticle.SetActive(true);
        lineRend.colorGradient = accurateTarget;
    }
    void AccurateTargetBlessing()
    {
        if (beamTrail.gameObject.activeSelf == true)
            if (lineRend.colorGradient == accurateTarget)
                return;

        Physics.queriesHitTriggers = true;

        if (beamTrail)
            beamTrail.gameObject.SetActive(true);
        reticle.SetActive(true);
        beamTrail.destination = target.hit_blessing.point;
        lineRend.colorGradient = accurateTarget;
        reticle.transform.position = target.hit_blessing.point;
    }

    // Draw dotted line when not hitting platform
    void InaccurateTarget()
    {
        if (lineRend.colorGradient == inaccurateTarget)
            return;

        if (padHit != null)
        {
            if (padHit.childCount > 0)
                disableHighlight(padHit, !blue);

            padHit = null;
        }

        beamTrail.destination = (hit.point);
        reticle.SetActive(true);
        reticle.transform.position = hit.point;

        lineRend.colorGradient = inaccurateTarget;
    }
    void InaccurateTargetBlessing()
    {
        if (beamTrail.gameObject.activeSelf == true)
            if (lineRend.colorGradient == inaccurateTarget)
                return;

        Physics.queriesHitTriggers = true;
        beamTrail.gameObject.SetActive(true);
        RaycastHit hit;
        if (Physics.Raycast(target.pointer.position, target.pointer.forward, out hit, 1000, target.blessing_layers))
        {
            beamTrail.destination = (hit.point);
            lineRend.colorGradient = inaccurateTarget;
            reticle.SetActive(true);
            reticle.transform.position = hit.point;
        }
        else
        {
            beamTrail.destination = (target.pointer.position + target.pointer.forward * 25);
            reticle.SetActive(true);
            reticle.transform.position = (target.pointer.position + target.pointer.forward * 25);
        }

        lineRend.colorGradient = inaccurateTarget;
    }
    IEnumerator IceballCast()
    {
        yield return new WaitForSeconds(4);
        iceball_cast = false;
        drawEffect.Stop();
        GetComponent<VRGestureRig>().enabled = true;
        if (!hasSpell && !isCoolingDown && wand != null)
        {
            wand.Find("tip").Find("flames").gameObject.GetComponent<ParticleSystem>().Stop();
        }
    }

    public void Vibrate(float _length, ushort _vibrateIntensity)
    {
        //SteamVR_Controller.Input(rightControllerIndex).TriggerHapticPulse(2000);
        length = _length;
        vibrateIntensity = _vibrateIntensity;
        vibrateStart = Time.time;
        InvokeRepeating("VibrateRepeat", 0, 0.05F);

    }

    void VibrateRepeat()
    {
        // Vibration caps at 3999
        SteamVR_Controller.Input(rightControllerIndex).TriggerHapticPulse(vibrateIntensity);
        if (Time.time >= vibrateStart + length)
        {
            CancelInvoke();
        }
    }

    public void disableHighlight(Transform highlighted, bool myBlue)
    {
        //print("disabling!!!");
        if (highlighted != null && (highlighted.gameObject.tag == "GrayPlatform" || highlighted.gameObject.tag == "BluePlatform" || highlighted.gameObject.tag == "RedPlatform" || highlighted.gameObject.tag == "PlatformTrigger"))
            if (highlighted.childCount > 1)
            {
                if (highlighted.GetChild(1).gameObject.activeSelf == false)
                    return;
            }

        if (highlighted != null && (highlighted.gameObject.tag == "GrayPlatform" || highlighted.gameObject.tag == "BluePlatform" || highlighted.gameObject.tag == "RedPlatform" || highlighted.gameObject.tag == "PlatformTrigger"))
        {
            if (highlighted.childCount > 1)
                highlighted.GetChild(1).gameObject.SetActive(false);
        }
    }

    public void enableHighlight(Transform highlighted, bool myBlue)
    {
        //print("ENABLING in spellcast");

        if (highlighted.childCount > 1)
        {
            if (highlighted.GetChild(1).gameObject.activeSelf == true)
            {
                return;
            }
        }

        if ((highlighted.gameObject.tag == "GrayPlatform" || (!blue && highlighted.gameObject.tag == "BluePlatform") || (blue && highlighted.gameObject.tag == "RedPlatform")))
        {
            if (highlighted.childCount > 1)
            {
                //highlighted.GetChild(1).gameObject.SetActive(false);
                var mainModule = highlighted.GetChild(1).gameObject.GetComponent<ParticleSystem>().main;
                mainModule.startColor = greenHighlight;
                highlighted.GetChild(1).gameObject.SetActive(true);
            }
        }
    }
}
