using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordsysTransformTest : MonoBehaviour {

    public CoordsysTransform coordsysTransform;

    public Transform[] triangleFrom;
    public Transform[] triangleTo;

    public Transform[] test;

    // Use this for initialization
    void Start () {
		for (int i=0; i<3; i++)
        {
            coordsysTransform.SavePositionPair(triangleFrom[i].position, triangleTo[i].position);
        }

        foreach (Transform t in triangleFrom)
            t.position = coordsysTransform.ApplyTransformationTo(t.position);

        foreach (Transform t in test)
        {
            t.position = coordsysTransform.ApplyTransformationTo(t.position);
        }
    }
}
