using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderTest : MonoBehaviour {

    Shader defaultShader;
    Shader selectedShader;
    Texture texture;
    Renderer materialRenderer;

	// Use this for initialization
	void Start () {
        materialRenderer = this.GetComponent<Renderer>();
        defaultShader = materialRenderer.material.shader;
        selectedShader = Shader.Find("Custom/Outline and ScreenSpace texture");
        texture = materialRenderer.material.mainTexture;
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.A))
        {
            materialRenderer.material.shader = defaultShader;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            materialRenderer.material.shader = selectedShader;
            materialRenderer.material.SetFloat("_OutlineVal", 0.125f);
            materialRenderer.material.SetColor("_OutlineCol", Color.cyan);
        }

        materialRenderer.material.mainTexture = texture;
	}
}
