using UnityEngine;
using System.Collections;

public class BlackBoard : MonoBehaviour {

    public Texture2D boardTexture;

	// Use this for initialization
	void Awake () {

        boardTexture = (Texture2D)this.GetComponent<Renderer>().material.mainTexture;
	}

}
