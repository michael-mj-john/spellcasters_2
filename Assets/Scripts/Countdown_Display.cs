using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Countdown_Display : MonoBehaviour {

    public TextMesh red_timer;
    public TextMesh blue_timer;
    public Vector3 location;
    public float countdown_timer_max = 5.0f;
    public float y_acceleration = 9.78f;
    private float y_vel = 0;
    private float countdown_timer = 0;
    private TextMesh countdown_red_text;
    private TextMesh countdown_blue_text;
    private bool countdown_flag = false;
    private bool countdownDone = false;
    bool already1 = false;
    bool already2 = false;
    bool already3 = false;
    bool already4 = false;
    bool already5 = false;
    bool already6 = false;
    bool already7 = false;
    bool already8 = false;
    bool already9 = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (countdown_flag)
        {
            Tick();
        }
    }

    void OnEnable()
    {
        this.transform.position = location;
        countdown_flag = true;
        countdown_timer = 0;

        GameObject.Find("Announcer").GetComponent<AnnouncerEvents>().PlaySound("countdown");
    }

    private void Tick()
    {
        // Update countdown
        countdown_timer += Time.deltaTime;
        if (countdown_timer > 1.25 * countdown_timer_max)
        {
            countdown_flag = false;
            countdownDone = false;
            already1 = false;
            already2 = false;
            already3 = false;
            already4 = false;
            already5 = false;
            already6 = false;
            already7 = false;
            already8 = false;
            already9 = false;
        }
        else if (countdown_timer > countdown_timer_max)
        {
            if (!countdownDone)
            {
                countdownDone = true;
                if (GameObject.Find("Announcer").GetComponent<AudioSource>().isPlaying == false)
                {
                    GameObject.Find("Announcer").GetComponent<AnnouncerEvents>().PlaySound("roundStart");
                }

                foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
                {
                    go.GetComponent<PhotonView>().RPC("SetSpellcastingEnabled", PhotonTargets.AllBuffered, true);
                }
            }
            y_vel += y_acceleration * Time.deltaTime;
            this.transform.position += new Vector3(0, y_vel, 0);
        }
        else
        {
            int temp = Mathf.CeilToInt(countdown_timer_max - countdown_timer);
            float scale = 1.5f * (Mathf.Ceil(countdown_timer) - countdown_timer);
            red_timer.text = "" + temp;
            red_timer.characterSize = scale;
            blue_timer.text = "" + temp;
            blue_timer.characterSize = scale;

            if (GameObject.Find("Announcer").GetComponent<AudioSource>().isPlaying == false)
            {
                AnnouncerEvents ae = GameObject.Find("Announcer").GetComponent<AnnouncerEvents>();
                switch (temp)
                {
                    case 1:
                        if (already1) { break; }
                        already1 = true;
                        ae.PlaySound("one");
                        break;
                    case 2:
                        if (already2) { break; }
                        already2 = true;
                        ae.PlaySound("two");
                        break;
                    case 3:
                        if (already3) { break; }
                        already3 = true;
                        ae.PlaySound("three");
                        break;
                    case 4:
                        if (already4) { break; }
                        already4 = true;
                        ae.PlaySound("four");
                        break;
                    case 5:
                        if (already5) { break; }
                        already5 = true;
                        ae.PlaySound("five");
                        break;
                    case 6:
                        if (already6) { break; }
                        already6 = true;
                        ae.PlaySound("six");
                        break;
                    case 7:
                        if (already7) { break; }
                        already7 = true;
                        ae.PlaySound("seven");
                        break;
                    case 8:
                        if (already8) { break; }
                        already8 = true;
                        ae.PlaySound("eight");
                        break;
                    case 9:
                        if (already9) { break; }
                        already9 = true;
                        ae.PlaySound("nine");
                        break;
                }
            }
        }
        this.gameObject.SetActive(countdown_flag);
    }
}
