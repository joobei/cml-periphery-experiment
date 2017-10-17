using UnityEngine;
using System.Collections;

public class GuiTextureScript : MonoBehaviour {

	private float width;
	private float height;

	// Use this for initialization
	void Start () 
	{	
		width = GetComponent<GUITexture>().pixelInset.width;
		height = GetComponent<GUITexture>().pixelInset.height;

		GetComponent<GUITexture>().pixelInset = new Rect(width * 0.05f, Screen.height - height * 1.05f, width, height);
	}
}
