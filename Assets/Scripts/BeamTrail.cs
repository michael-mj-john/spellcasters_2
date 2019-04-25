using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamTrail : MonoBehaviour {

    public float beamSpeed = 2;
    public Vector3 destination;
    private LineRenderer lineRenderer;
    private Material[] lrMaterials;
	// Use this for initialization
	void Start () {
        lineRenderer = GetComponent<LineRenderer>();
        lrMaterials = lineRenderer.materials;
        lrMaterials[0].mainTextureScale = new Vector2(1, 1);
        lrMaterials[0].mainTextureOffset = new Vector2(0, 0);

        for (int i = 1; i < lrMaterials.Length; i++)
        {
            lrMaterials[i].mainTextureScale = new Vector2(10, 1);
        }
    }

    // Update is called once per frame
    void Update () {
        lineRenderer.SetPosition(0, transform.position);
		if (destination != null)
        	lineRenderer.SetPosition(1, destination);

        for(int i = 1; i < lrMaterials.Length; i++)
        {
            MoveTexture(lrMaterials[i], beamSpeed + i);
        }

    }

    void MoveTexture(Material lrMaterial, float speed)
    {
        float lrMaterialOffsetX = Mathf.Abs(lrMaterial.mainTextureOffset.x - speed * Time.deltaTime) % lrMaterial.mainTextureScale.x;
        lrMaterialOffsetX *= -1;

        lrMaterial.mainTextureOffset = new Vector2(lrMaterialOffsetX, lrMaterial.mainTextureOffset.y);
        //print(lrMaterial.mainTextureScale + " : " + lrMaterial.mainTextureOffset);
    }
}
