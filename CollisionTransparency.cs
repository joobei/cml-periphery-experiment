using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTransparency : MonoBehaviour {

    public Material material;
    public float intersectedAlpha;
    private Color originalColor, transparentColor;

    void Start()
    {
        originalColor = material.color;
        originalColor.a = 1.0f;
        transparentColor = originalColor;
        transparentColor.a = intersectedAlpha;
        material.color = originalColor;
    }

    //When there is a collision make object transparent
    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Collision");
        material.color = transparentColor;
    }

    //return it to original color
    void OnTriggerExit(Collider other) 
    {
        material.color = originalColor;
    }

    void OnApplicationQuit()
    {
        material.color = originalColor;
    }
}
