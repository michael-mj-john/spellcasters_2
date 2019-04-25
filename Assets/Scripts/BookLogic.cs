using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class BookLogic : MonoBehaviour
{
    int glyphRowCap = 3;
    float glyphGap = 3f;
    float glyphVertGap = 2f;
    float glyphStartX = 3f;
    float glyphStartY = 0.05f;
    float glyphStartZ = 0.2f;
    private PlayerStatus playerStatus; 
	private PlayerClass playerClass;

    [HideInInspector]
    public SpellcastingGestureRecognition spellcast;

    GameObject page;
    public GameObject leftPage;
    public Material[] pages;
    public Material[] pagesAttack;
    public Material[] pagesSupport;
    public Material[] pagesHealer;

    public GameObject[] glyphs;
    public GameObject[] glyphsAttack;
    public GameObject[] glyphsSupport;
    public GameObject[] glyphsHealer;

    Renderer rend;
    Animator animator;

    SteamVR_TrackedObject trackedObj;
    SteamVR_Controller.Device device;

	public int attackTop = 2;
	public int attackBottom = 0;

	public int supportBottom = 3;
	public int supportTop = 5;

	public int healBottom = 6;
	public int healTop = 9;

	bool flipped;
	bool isOculus;

    float trackpadPos;
    float startPressPos;
    float swipeThresh = 0.3f;
    public int index = 0;
    public string currentGlyph = "";
    public GlyphGuide guide;

    private void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // Use this for initialization
    void Start()
    {
		if (VRDevice.model.ToLower().Contains("oculus"))
		{
			isOculus = true;
		}

		playerStatus = transform.parent.parent.GetComponentInChildren<PlayerStatus> ();
		//print ("PLAYER" + playerStatus);

        if (transform.GetChild(1)!= null)
        {
					
            page = this.gameObject.transform.GetChild(1).gameObject;
            rend = page.GetComponent<Renderer>();
                

			rend.material = pages [pages.Length-1];
            currentGlyph = pages[pages.Length - 1].name;
            if(guide != null) guide.UpdateGlyphTexture(currentGlyph);
            animator = GetComponent<Animator>();
        }
        //page.Set
    }

    // Update is called once per frame
    void Update()
    {
        trackpadPos = Input.GetAxis("TrackpadHoriz");

		if (isOculus)
		{
			if (trackpadPos > 0.5f)
			{
				if (flipped == false)
				{
					FlipRight ();
					flipped = true;
				}
			} 
			else if (trackpadPos < -0.5f)
			{
				if (flipped == false)
				{
					FlipLeft ();
					flipped = true;
				}
			} 
			else
			{
				flipped = false;
			}
		}

		if (Input.GetKeyDown("joystick button 16") && !isOculus)
        {
            startPressPos = trackpadPos;
            //if (trackpadPos < -0.05f)
            //{
            //    FlipLeft();
            //}
            //else if (trackpadPos > 0.05f)
            //{
            //    FlipRight();
            //}
        }

		if (Input.GetKeyUp("joystick button 16") && !isOculus)
        {
            if (trackpadPos > startPressPos + swipeThresh )
            {
                FlipRight();
            }

            else if (trackpadPos < startPressPos - swipeThresh)
            {
                FlipLeft();
            }

        }


            //if (device.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
            //{
            //    Vector2 touchpad = (device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0));
            //    print("Pressing Touchpad");

            //    if (touchpad.y > 0.7f)
            //    {
            //        print("Moving Up");
            //    }

            //    else if (touchpad.y < -0.7f)
            //    {
            //        print("Moving Down");
            //    }

            //    if (touchpad.x > 0.7f)
            //    {
            //        print("Moving Right");
            //        FlipRight();

            //    }

            //    else if (touchpad.x < -0.7f)
            //    {
            //        print("Moving left");
            //        FlipLeft();
            //    }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            FlipLeft();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //if (SwipeLeft)
                FlipRight();
        }

        if(Input.GetKeyDown("joystick button 8"))
        {
            guide.ToggleVisibility();
        }


    }

    private void FixedUpdate()
    {
        // device = SteamVR_Controller.Input((int)trackedObj.index);
    }

    void FlipRight()
    {

        if (playerClass == PlayerClass.attack) 
		{
            if (index > 0)
            {
                index -= 1;
            }
            else
            {
                index = pagesAttack.Length - 1;
            }
        }

		else if (playerClass == PlayerClass.support)
		{
            if (index > 0)
            {
                index -= 1;
            }
            else
            {
                index = pagesSupport.Length - 1;
            }
        }

		else if (playerClass == PlayerClass.heal) 
		{
            if (index > 0)
            {
                index -= 1;
            }
            else
            {
                index = pagesHealer.Length - 1;
            }
        }

		else if (playerClass == PlayerClass.all) 
		{
        if (index > 0)
        {
            index -= 1;
        }
        else
        {
            index = pages.Length - 2;
        }
		}
        if (animator)
            animator.SetTrigger("FlipRight");
        //  UpdateUI();
        if (spellcast!= null)
            SteamVR_Controller.Input(spellcast.leftControllerIndex).TriggerHapticPulse(500);
    }

    void FlipLeft()
    { 

        if (playerClass == PlayerClass.attack) 
		{
            if (index < (pagesAttack.Length - 1))
            {
                index += 1;
            }
            else
            {
                index = 0;
            }
        }

		else if (playerClass == PlayerClass.support)
		{
            if (index < (pagesSupport.Length - 1))
            {
                index += 1;
            }
            else
            {
                index = 0;
            }
        }

		else if (playerClass == PlayerClass.heal) 
		{
            if (index < (pagesHealer.Length - 1))
            {
                index += 1;
            }
            else
            {
                index = 0;
            }
        }

		else if (playerClass == PlayerClass.all) 
		{
			if (index < (pages.Length - 2))
			{
				index += 1;
			}
			else
			{
				index = 0;
			}
		}
			
        if(animator)
            animator.SetTrigger("FlipLeft");
        // UpdateUI();
        if(spellcast!=null)
            SteamVR_Controller.Input(spellcast.leftControllerIndex).TriggerHapticPulse(500);
    }

    public void UpdateUI()
    {
        if (playerStatus == null)
        {
            playerStatus = transform.parent.parent.GetComponentInChildren<PlayerStatus>();
        }
        if (playerStatus != null)
        {
            playerClass = playerStatus.playerClass;
        }
        else
        {
            print("playerStatus is null");
        }

        if (transform.GetChild(1) != null)
        {

            page = this.gameObject.transform.GetChild(1).gameObject;
            rend = page.GetComponent<Renderer>();


            //rend.material = pages[pages.Length - 1];
            animator = GetComponent<Animator>();

            if (playerClass == PlayerClass.attack)
            {
                if (index >= pagesAttack.Length)
                    index = 0;

                rend.material = pagesAttack[index];
                currentGlyph = pagesAttack[index].name;
                //print(currentGlyph);
            }
            else if (playerClass == PlayerClass.support)
            {
                if (index >= pagesSupport.Length)
                    index = 0;

                rend.material = pagesSupport[index];
                currentGlyph = pagesSupport[index].name;
                //print(currentGlyph);
            }
            else if (playerClass == PlayerClass.heal)
            {
                if (index >= pagesHealer.Length)
                    index = 0;

                rend.material = pagesHealer[index];
                currentGlyph = pagesHealer[index].name;
                //print(currentGlyph);
            }
            else if (playerClass == PlayerClass.all)
            {
                if (index >= pages.Length)
                    index = 0;
                rend.material = pages[index];
                currentGlyph = pages[index].name;
                //print(currentGlyph);
            }
        }

        

		if (playerClass == PlayerClass.none) 
		{
			rend.material = pages[pages.Length-1];
            currentGlyph = pages[pages.Length - 1].name;
		}

        //		else if (playerClass == PlayerClass.attack) 
        //		{
        //			rend.material = pages[0];
        //		}
        //
        //		else if (playerClass == PlayerClass.support)
        //		{
        //			rend.material = pages[2];
        //		}
        //
        //		else if (playerClass == PlayerClass.heal) 
        //		{
        //			rend.material = pages[1];
        //		}
        if (guide == null)
        {
            return;
        }
        guide.UpdateGlyphTexture(currentGlyph);
    }

    public void UpdateHotbar()
    {
        //print("updating hotbar");
        GameObject emptyObj;

        GameObject[] emptyObjs;

        emptyObjs = GameObject.FindGameObjectsWithTag("glyph");
        foreach (GameObject empty in emptyObjs)
        {
            Destroy(empty);
        }


        if (playerClass == PlayerClass.attack)
        {
            int i = 0;
            int j = 0;
            foreach (GameObject glyph in glyphsAttack)
            {
                emptyObj = Instantiate(glyph);

                //SpriteRenderer sr = emptyObj.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
                //sr.sprite = glyph;
                if (i >= glyphRowCap)
                    j = (int)Mathf.Floor(i / glyphRowCap);

                emptyObj.transform.SetParent(leftPage.transform);
                emptyObj.transform.localRotation = Quaternion.Euler(-90, 0, 0);
                //emptyObj.transform.localScale = new Vector3(-.9f, .9f, .9f);
                emptyObj.transform.localPosition = new Vector3(glyphStartX - ((i - (j * glyphRowCap)) * glyphGap), glyphStartY, glyphStartZ + (glyphVertGap * j));
                i += 1;

            }
        }
        else if (playerClass == PlayerClass.support)
        {
            int i = 0;
            int j = 0;
            foreach (GameObject glyph in glyphsSupport)
            {
                emptyObj = Instantiate(glyph);
                //SpriteRenderer sr = emptyObj.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
                //sr.sprite = glyph;
                if (i >= glyphRowCap)
                    j = (int)Mathf.Floor(i / glyphRowCap);

                emptyObj.transform.SetParent(leftPage.transform);
                emptyObj.transform.localRotation = Quaternion.Euler(-90, 0, 0);
                //emptyObj.transform.localScale = new Vector3(-.9f, .9f, .9f);
                emptyObj.transform.localPosition = new Vector3(glyphStartX - ((i - (j * glyphRowCap)) * glyphGap), glyphStartY, glyphStartZ + (glyphVertGap * j));
                i += 1;

            }
        }
        else if (playerClass == PlayerClass.heal)
        {
            int i = 0;
            int j = 0;
            foreach (GameObject glyph in glyphsHealer)
            {
                emptyObj = Instantiate(glyph);
                //SpriteRenderer sr = emptyObj.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
                //sr.sprite = glyph;
                if (i >= glyphRowCap)
                    j = (int)Mathf.Floor(i / glyphRowCap);

                emptyObj.transform.SetParent(leftPage.transform);
                emptyObj.transform.localRotation = Quaternion.Euler(-90, 0, 0);
                //emptyObj.transform.localScale = new Vector3(-.9f, .9f, .9f);
                emptyObj.transform.localPosition = new Vector3(glyphStartX - ((i - (j * glyphRowCap)) * glyphGap), glyphStartY, glyphStartZ + (glyphVertGap * j));
                i += 1;

            }
        }
        else if (playerClass == PlayerClass.all)
        {
            int i = 0;
            int j = 0;
            foreach (GameObject glyph in glyphs)
            {
                emptyObj = Instantiate(glyph);
                //SpriteRenderer sr = emptyObj.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
                //sr.sprite = glyph;
                if (i >= glyphRowCap)
                    j = (int)Mathf.Floor(i / glyphRowCap);

                emptyObj.transform.SetParent(leftPage.transform);
                emptyObj.transform.localRotation = Quaternion.Euler(-90,0, 0);
               // emptyObj.transform.localScale = new Vector3(-.9f, .9f, .9f);
                emptyObj.transform.localPosition = new Vector3(glyphStartX - ((i - (j * glyphRowCap)) * glyphGap), glyphStartY, glyphStartZ + (glyphVertGap * j));
                i += 1;

            }
        }
    }
}