using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMain : MonoBehaviour
{

    public bool isBlue;
    public string currentColor;
    private string originalColor;

    private bool isFlipped = false; //Flag to tell if tile has been flipped, and used to activate timer.
    private float resetDuration = 30f; //How long will the tile stay flipped.
    private float resetTimer = 0; //

    GameObject camera;
    GameObject countText;
    TextMesh countTextMesh;

    public Material blueMaterial;
    public string blueTag = "BluePlatform";
    public string blueLayer = "BluePlatform";

    public Material redMaterial;
    public string redTag = "RedPlatform";
    public string redLayer = "RedPlatform";

    public Material grayMaterial;
    public string grayTag = "GrayPlatform";
    public string grayLayer = "GrayPlatform";

    // Use this for initialization
    void Start()
    {
        originalColor = currentColor;
        countText = new GameObject();
        countText.AddComponent<TextMesh>();
        countTextMesh = countText.GetComponent<TextMesh>();
        countText.GetComponent<Renderer>().material = Resources.Load("3D_OneSided_White") as Material;
        countTextMesh.font = Resources.Load("Luminari-Regular") as Font;
        //countText.transform.SetParent(this.transform);
        countText.transform.position = transform.position;
        countText.transform.position += new Vector3(0,0f,0);// transform.position;
        //countText.transform.localPosition = new Vector3(0, 0, 0);
        countTextMesh.fontSize = 100;
        countTextMesh.text = "swag";
        countTextMesh.alignment = TextAlignment.Center;
        countTextMesh.anchor = TextAnchor.MiddleCenter;
        countText.transform.localScale = new Vector3(0.1f, 0.1f, 0.01f);
        countText.SetActive(false);
        countText.name = "PlatformTimerText";
    }

    // Update is called once per frame
    void Update()
    {

        //Only check timer if tile has been flipped.
        if (isFlipped)
        {
            if (resetTimer > 0)
            {
                //Decrease timer by passed time.
                resetTimer -= Time.deltaTime;
                countTextMesh.text = ""+ Mathf.Floor(resetTimer);

                if (Vector3.Distance(countText.transform.position, camera.transform.position) > 2)
                {
                    if (countText.transform.localScale.x != 0.1f)
                        countText.transform.localScale = new Vector3(0.1f, 0.1f, 0.01f);

                    countText.transform.LookAt(camera.transform);
                    countText.transform.eulerAngles += new Vector3(0, 180, 0);
                }

                else
                {
                    if (countText.transform.localScale.x != 0.05f)
                        countText.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

                    if(isBlue)
                    {
                        countText.transform.eulerAngles = new Vector3(countText.transform.eulerAngles.x, 180, countText.transform.eulerAngles.z);
                    }

                    else
                    {
                        countText.transform.eulerAngles = new Vector3(countText.transform.eulerAngles.x, 0, countText.transform.eulerAngles.z);
                    }
                }

                countText.transform.eulerAngles = new Vector3(90, countText.transform.eulerAngles.y, countText.transform.eulerAngles.z);
            }
            else
            {
                //Change color to its original one.
                ChangeColor(originalColor);
            }
        }

    }

    [PunRPC]
    public void ChangeColor()
    {
        ChangeColor(isBlue ? "red" : "blue");
    }

    [PunRPC]
    public void ChangeColorTo(bool blue)
    {
        ChangeColor(blue ? "red" : "blue");
    }

    public void ChangeColor(string color)
    {

        if (currentColor == color)
        {
            return;
        }
        else if (color == originalColor)
        {
            //Reset to unflipped.
            isFlipped = false;
            countText.SetActive(false);
            //Cancel timer.
            resetTimer = 0;
        }
        else
        {
            isFlipped = true;

            //Start timer to reset to the original color.
            resetTimer = resetDuration;
            countText.SetActive(true);
            camera = Camera.main.gameObject;
        }

        currentColor = color;

        switch (color)
        {

            case "blue":
                this.GetComponent<Renderer>().material = blueMaterial;
                this.gameObject.layer = LayerMask.NameToLayer(blueLayer);
                this.tag = blueTag;
                isBlue = true;
                GetComponent<PlatformNeighbors>().layerSave = LayerMask.NameToLayer(blueLayer);
                break;
            case "red":
                this.GetComponent<Renderer>().material = redMaterial;
                this.gameObject.layer = LayerMask.NameToLayer(redLayer);
                this.tag = redTag;
                isBlue = false;
                GetComponent<PlatformNeighbors>().layerSave = LayerMask.NameToLayer(redLayer);
                break;
            case "gray":
                this.GetComponent<Renderer>().material = grayMaterial;
                this.gameObject.layer = LayerMask.NameToLayer(grayLayer);
                this.tag = grayTag;
                isBlue = false;
                GetComponent<PlatformNeighbors>().layerSave = LayerMask.NameToLayer(grayLayer);
                break;
        }
    }

    void SetPlatform()
    {

    }
}