using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlyphGuide : MonoBehaviour {

    public Transform anchor;
    public BookLogic book;
    public float distanceRange = 2;
    private Renderer rend;
    public Dictionary<string, Vector2> glyphs = new Dictionary<string, Vector2>();


	// Use this for initialization
	void Start () {
        rend = this.GetComponent<Renderer>();

        //Set the offset equivalence.
        glyphs.Add("sb_fire", new Vector2(0, 0.5f));
        glyphs.Add("sb_heal", new Vector2(0.1f, 0.5f));
        glyphs.Add("sb_vines", new Vector2(0.2f, 0.5f));
        glyphs.Add("sb_meteor", new Vector2(0.3f, 0.5f));
        glyphs.Add("sb_hammer", new Vector2(0.4f, 0.5f));
        //glyphs.Add("sb_blessing", new Vector2(0.5f, 0.5f));
        glyphs.Add("sb_blade", new Vector2(0.6f, 0.5f));
        glyphs.Add("sb_iceball", new Vector2(0.7f, 0.5f));
        glyphs.Add("sb_shield", new Vector2(0.8f, 0.5f));
        glyphs.Add("sb_flip", new Vector2(0.9f, 0.5f));
        glyphs.Add("sb_bubble", new Vector2(0f, 0));
       // glyphs.Add("sb_hammer", new Vector2(0f, 0));

        rend.enabled = false;

    }

    // Update is called once per frame
    void Update () {
        if (rend == null)
            rend = this.GetComponent<Renderer>();
        
        /*
        float distanceFromAnchor = Vector3.Magnitude(anchor.position - this.transform.position);

        if (distanceFromAnchor > distanceRange || distanceFromAnchor < distanceRange)
        {
            this.transform.position = anchor.position + anchor.forward * distanceRange;
            Quaternion lookAtRotation = Quaternion.LookRotation(anchor.forward);
            this.transform.rotation = Quaternion.Euler(lookAtRotation.eulerAngles + new Vector3(90, 0, -180));
        }
        */

        if (book != null)
        {
            Quaternion lookAtRotation = Quaternion.LookRotation(anchor.forward);
            this.transform.rotation = Quaternion.Euler(lookAtRotation.eulerAngles + new Vector3(90, 0, -180));

            //Quaternion newGlyphRotation = Quaternion.Euler(book.transform.rotation.eulerAngles + new Vector3(90, 0, -180));
            //this.transform.SetPositionAndRotation(book.transform.position + new Vector3(0,0.25f) + book.transform.forward, newGlyphRotation);
            this.transform.position = book.transform.position + new Vector3(0, 0.25f) + book.transform.forward;
        }
    }

    public void UpdateGlyphTexture(string glyphName)
    {
        //print(glyphName);
        if (glyphName == "sb_blank")
            return;
        else
        {
            rend.material.mainTextureOffset = glyphs[glyphName];
        }
    }

    public void ToggleVisibility()
    {
        if(rend!=null)
            rend.enabled = !rend.enabled;
    }
}
