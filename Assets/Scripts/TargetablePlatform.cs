using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetablePlatform : MonoBehaviour
{

    public Shader baseShader;
    public Shader selectedShader;
    Renderer materialRenderer;
    Texture texture;

    public GameObject platform;
  
    void Start()
    {
        materialRenderer = GetComponent<Renderer>();
        baseShader = materialRenderer.material.shader;
        selectedShader = Shader.Find("Custom/Outline and ScreenSpace texture");
        texture = materialRenderer.material.mainTexture;
    }
    public void SetIndicator(bool on)
    {
        //Change shaders
        if (on)
        {
            platform.GetComponent<Renderer>().material.shader = selectedShader;
            platform.GetComponent<Renderer>().material.SetFloat("_OutlineVal", 0.125f);
            platform.GetComponent<Renderer>().material.SetColor("_OutlineCol", Color.cyan);


        }
        else
        {
            platform.GetComponent<Renderer>().material.shader = baseShader;
            
        }
        materialRenderer.material.mainTexture = texture;
    }
}