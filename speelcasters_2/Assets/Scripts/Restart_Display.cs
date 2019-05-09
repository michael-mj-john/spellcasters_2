using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restart_Display : MonoBehaviour {

    public GameObject blue_wins;
    public GameObject red_wins;
    public GameObject red_score;
    public GameObject blue_score;
    public GameObject timer;
    public Vector3 location;

    public float restart_timer_max;
    private float restart_timer;
    private bool timer_flag = false;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (timer_flag)
        {
            Tick();
        }
    }

    void OnEnable()
    {
        this.transform.position = location;
        timer_flag = true;
        restart_timer = 0;
    }

    public void SetWinner(bool blueWon)
    {
        blue_wins.SetActive(blueWon);
        red_wins.SetActive(!blueWon);
    }

    public void SetScore(int red, int blue)
    {
        red_score.transform.Find("red_view").GetComponent<TextMesh>().text = "" + red;
        red_score.transform.Find("blue_view").GetComponent<TextMesh>().text = "" + red;

        blue_score.transform.Find("red_view").GetComponent<TextMesh>().text = "" + blue;
        blue_score.transform.Find("blue_view").GetComponent<TextMesh>().text = "" + blue;
    }

    void Tick()
    {
        restart_timer += Time.deltaTime;
        if (restart_timer > restart_timer_max)
        {
            restart_timer = 0;
            timer_flag = false;
            GameObject rm = GameObject.Find("Round Manager(Clone)");
            rm.GetComponent<PhotonView>().RPC("EndRound", PhotonTargets.All, null);
            this.gameObject.SetActive(false);
        }
        else
        {
            int temp = Mathf.CeilToInt(restart_timer_max - restart_timer);
            float scale = 1.5f * (Mathf.Ceil(restart_timer) - restart_timer);

            TextMesh red_view = timer.transform.Find("red_view").GetComponent<TextMesh>();
            TextMesh blue_view = timer.transform.Find("blue_view").GetComponent<TextMesh>();
            red_view.text = "" + temp;
//            red_view.characterSize = scale;
            blue_view.text = "" + temp;
//            blue_view.characterSize = scale;
        }
        
    }
}
