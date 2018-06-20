using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdoptHapticScale : MonoBehaviour {

    public HapticManager hapticManager;

    // Use this for initialization
    void Start()
    {
        float radiusMeters = hapticManager.radius * 0.001f; //convert form mm to m
        transform.localScale = Vector3.one * radiusMeters * 2;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = hapticManager.targetSphere.transform.localScale;
    }
}
