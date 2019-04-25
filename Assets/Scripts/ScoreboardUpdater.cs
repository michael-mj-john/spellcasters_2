using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *                                      ***     !!!!!!     IMPORTANT   !!!!!!   ***
 *                                      
 *                                       Make sure GameObject Scoreboard has tag 'Scoreboard' 
 *                  
 *                                       AND NOTHING ELSE SHOULD BE TAGGED WITH SCOREBOARD
 *                                                          
 *                                      because script PlayerStatus looks for it!
 *  
 *                                      ***     !!!!!!     IMPORTANT   !!!!!!     ***
 *  
 *  
 *  Used by NetworkManager (to instantiate once in scene [in masterclient]) and PlayerStatus (to update scoreboard when a player is defeated)
 */

public class ScoreboardUpdater : MonoBehaviour, IPunObservable {

    //public GameObject red_score_for_red_view;
    //public GameObject blue_score_for_red_view;
    //   public GameObject red_score_for_blue_view;
    //   public GameObject blue_score_for_blue_view;

    public GameObject scoreboardBlue;
    public GameObject scoreboardRed;

    public int red_score = 0;
	public int blue_score = 0;

	public bool roundOver = false;

    private PhotonView pv;

    private GameObject[] redHeartsB = new GameObject[30];
    private GameObject[] blueHeartsB = new GameObject[30];
    private GameObject[] redHeartsR = new GameObject[30];
    private GameObject[] blueHeartsR = new GameObject[30];


    int heartRowCap = 5;
    float heartGap = .65f;
    float heartVertGap = -.7f;
    Vector3 bHeartStart = new Vector3(.5f, -.05f, -0.1f);
    Vector3 rHeartStart = new Vector3(-.5f, -.05f, -0.1f);
    Vector3 heartScale = new Vector3(.5f, 0.5f, 6);
    GameObject heartObj;

    public Sprite blueHeart;
    public Sprite redHeart;
    public Sprite dead;

    [HideInInspector]
    public int maximumScore = 12;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        //red_score_for_red_view.GetComponent<TextMesh>().text = "0";
        //blue_score_for_red_view.GetComponent<TextMesh>().text = "0";
        //red_score_for_blue_view.GetComponent<TextMesh>().text = "0";
        //blue_score_for_blue_view.GetComponent<TextMesh>().text = "0";

        
    
    }
// Use this for initialization
    void Start()
    {
        int j = 0;
        for (int i = 0; i < maximumScore; i++)
        {
            // Blue scoreboard

            heartObj = new GameObject();
            SpriteRenderer sr = heartObj.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
            sr.sprite = redHeart;
            //SpriteRenderer sr = emptyObj.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
            //sr.sprite = glyph;
            if (i >= heartRowCap)
                j = (int)Mathf.Floor(i / heartRowCap);

            heartObj.transform.SetParent(this.transform);
            heartObj.transform.localRotation = Quaternion.Euler(0, 0, 0);
            heartObj.transform.localScale = heartScale;
            heartObj.transform.localPosition = scoreboardBlue.transform.position + new Vector3(rHeartStart.x - ((i - (j * heartRowCap)) * heartGap), rHeartStart.y + (heartVertGap * j), -rHeartStart.z);
            redHeartsB[i] = heartObj;

            heartObj = new GameObject();
            SpriteRenderer sr2 = heartObj.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
            sr2.sprite = blueHeart;
            //SpriteRenderer sr = emptyObj.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
            //sr.sprite = glyph;
            if (i >= heartRowCap)
                j = (int)Mathf.Floor(i / heartRowCap);

            heartObj.transform.SetParent(this.transform);
            heartObj.transform.localRotation = Quaternion.Euler(0, 0, 0);
            heartObj.transform.localScale = heartScale;
            heartObj.transform.localPosition = scoreboardBlue.transform.position + new Vector3(bHeartStart.x + ((i - (j * heartRowCap)) * heartGap), bHeartStart.y + (heartVertGap * j), -bHeartStart.z);
            blueHeartsB[i] = heartObj;

            // Red scoreboard
            heartObj = new GameObject();
            SpriteRenderer sr3 = heartObj.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
            sr3.sprite = redHeart;
            //SpriteRenderer sr = emptyObj.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
            //sr.sprite = glyph;
            if (i >= heartRowCap)
                j = (int)Mathf.Floor(i / heartRowCap);

            heartObj.transform.SetParent(this.transform);
            heartObj.transform.localRotation = Quaternion.Euler(0, 0, 0);
            heartObj.transform.localScale = heartScale;
            heartObj.transform.localPosition = scoreboardRed.transform.position + new Vector3(bHeartStart.x + ((i - (j * heartRowCap)) * heartGap), bHeartStart.y + (heartVertGap * j), bHeartStart.z);
            redHeartsR[i] = heartObj;

            heartObj = new GameObject();
            SpriteRenderer sr4 = heartObj.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
            sr4.sprite = blueHeart;
            //SpriteRenderer sr = emptyObj.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
            //sr.sprite = glyph;
            if (i >= heartRowCap)
                j = (int)Mathf.Floor(i / heartRowCap);

            heartObj.transform.SetParent(this.transform);
            heartObj.transform.localRotation = Quaternion.Euler(0, 0, 0);
            heartObj.transform.localScale = heartScale;
            heartObj.transform.localPosition = scoreboardRed.transform.position + new Vector3(rHeartStart.x - ((i - (j * heartRowCap)) * heartGap), rHeartStart.y + (heartVertGap * j), rHeartStart.z);
            blueHeartsR[i] = heartObj;

        }
//        SetVisible(false);
    }
		
	
	// Update is called once per frame
	void Update () {
        if (PhotonNetwork.isMasterClient && (red_score >= maximumScore || blue_score >= maximumScore) && !roundOver)
        {
            GameObject rm = GameObject.Find("Round Manager(Clone)");
            Debug.Log("ScoreboardUpdater.cs : Update() : blueWon = " + (blue_score > red_score) + ", blue_score = " + blue_score + ", red_score = " + red_score);
            rm.GetComponent<PhotonView>().RPC("Display_Restart", PhotonTargets.All, blue_score > red_score, blue_score, red_score);
//            ResetScoreboard();
        }
	}

    private void OnEnable()
    {
//        ResetScoreboard();
    }

    public void SetVisible(bool isVisible)
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).gameObject.SetActive(isVisible);
        }
        //scoreboardBlue.SetActive(isVisible);
        //scoreboardRed.SetActive(isVisible);
    }

    public void ResetScoreboard()
    {
        pv.RPC("Reset2", PhotonTargets.AllBuffered, null);
    }
    public void IncrementRedScore()
    {
        pv.RPC("IncrementRedScore2", PhotonTargets.AllBuffered, null);
    }
    public void IncrementBlueScore()
    {
        pv.RPC("IncrementBlueScore2", PhotonTargets.AllBuffered, null);
    }
    [PunRPC]
    public void Reset2()
	{
        Debug.Log("RPC Scoreboard Reset");

        //red_score_for_red_view.GetComponent<TextMesh>().text = "0";
        //    blue_score_for_red_view.GetComponent<TextMesh>().text = "0";
        //    red_score_for_blue_view.GetComponent<TextMesh>().text = "0";
        //    blue_score_for_blue_view.GetComponent<TextMesh>().text = "0";

        red_score = 0;
        blue_score = 0;
        roundOver = true;
        updateUI();
    }

    [PunRPC]
    public void IncrementRedScore2()
	{
        ++red_score;
        //red_score_for_red_view.GetComponent<TextMesh>().text = "" + red_score;
        //red_score_for_blue_view.GetComponent<TextMesh>().text = "" + red_score;
        Debug.Log("RPC RED SCORED: " + red_score);
        updateUI();
    }

    void updateUI()
    {
        for (int i = 0; i<maximumScore; i++)
        {
            if (i >= red_score)
            {
                blueHeartsR[i].GetComponent<SpriteRenderer>().sprite = blueHeart;
                blueHeartsB[i].GetComponent<SpriteRenderer>().sprite = blueHeart;
            }
            else
            {
                blueHeartsR[i].GetComponent<SpriteRenderer>().sprite = dead;
                blueHeartsB[i].GetComponent<SpriteRenderer>().sprite = dead;
            }

            if (i >= blue_score)
            {
                redHeartsR[i].GetComponent<SpriteRenderer>().sprite = redHeart;
                redHeartsB[i].GetComponent<SpriteRenderer>().sprite = redHeart;
            }
            else
            {
                redHeartsR[i].GetComponent<SpriteRenderer>().sprite = dead;
                redHeartsB[i].GetComponent<SpriteRenderer>().sprite = dead;
            }
        }
    }

    [PunRPC]
    public void IncrementBlueScore2()
	{
        ++blue_score;
            //blue_score_for_red_view.GetComponent<TextMesh>().text = "" + blue_score;
            //blue_score_for_blue_view.GetComponent<TextMesh>().text = "" + blue_score;
        Debug.Log("RPC BLUE SCORED: " + blue_score);
        updateUI();
    }

    private void SetScores()
    {
        //blue_score_for_red_view.GetComponent<TextMesh>().text = "" + blue_score;
        //blue_score_for_blue_view.GetComponent<TextMesh>().text = "" + blue_score;
        //red_score_for_red_view.GetComponent<TextMesh>().text = "" + red_score;
        //red_score_for_blue_view.GetComponent<TextMesh>().text = "" + red_score;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
        
        // If you own the game object
        if (stream.isWriting)
        {

            // Sync all instances of health according to my health
            stream.SendNext(red_score);
            stream.SendNext(blue_score);
        }
        // If you dont own the game object
        else
        {
            // Sync the avatar's health according to the owner of the avatar.
            red_score = (int)(stream.ReceiveNext());
            blue_score = (int)(stream.ReceiveNext());
        }
    }
}
