using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetablePlayer : MonoBehaviour
{
    public Material headBaseMaterial;
    public Material torsoBaseMaterial;
    public Material selectedMaterial;
    Renderer materialRenderer;
    Texture texture;

    public GameObject head;
    public GameObject torso;
   

    void Start()
    {
        materialRenderer = GetComponent<Renderer>();
        torsoBaseMaterial = materialRenderer.material;
        headBaseMaterial = head.GetComponent<Renderer>().material;
    }

    public void SetIndicator(bool on)
    {
        //Change shaders
        if (on)
        {
            head.GetComponent<Renderer>().material = selectedMaterial;
            head.GetComponent<Renderer>().material.SetFloat("_OutlineVal", 0.125f);
            head.GetComponent<Renderer>().material.SetColor("_OutlineCol", Color.cyan);

            torso.GetComponent<Renderer>().material = selectedMaterial;
            torso.GetComponent<Renderer>().material.SetFloat("_OutlineVal", 0.125f);
            torso.GetComponent<Renderer>().material.SetColor("_OutlineCol", Color.cyan);
         
        }
        else
        {
            head.GetComponent<Renderer>().material = headBaseMaterial;
            torso.GetComponent<Renderer>().material = torsoBaseMaterial;
            
        }
    }

    public void UpdateMaterials(Material mat)
    {
        torsoBaseMaterial = mat;
    }
}