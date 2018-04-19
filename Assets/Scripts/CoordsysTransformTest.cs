using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordsysTransformTest : MonoBehaviour {

    public CoordsysTransform coordTransform;

    public Transform[] triangleFrom;
    public Transform[] triangleTo;

    // Use this for initialization
    void Start () {
		for (int i=0; i<3; i++)
        {
            coordTransform.SavePositionPair(triangleFrom[i].position, triangleTo[i].position);
        }

        coordTransform.CreateTransformation();

        foreach (Transform t in triangleFrom)
            t.position = coordTransform.ApplyTransformationTo(t.position);
    }
}
