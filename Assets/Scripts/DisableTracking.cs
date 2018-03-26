using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableTracking : MonoBehaviour {

    public Transform orientationOffset;
    public Transform positionOffset;
	
	// Update is called once per frame
	void Update () {
        orientationOffset.localRotation = Quaternion.Inverse(UnityEngine.XR.InputTracking.GetLocalRotation(UnityEngine.XR.XRNode.Head));
        positionOffset.localPosition = -UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.Head);
	}
}
