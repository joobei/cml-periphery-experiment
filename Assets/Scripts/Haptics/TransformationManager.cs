using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;


public class TransformationManager: MonoBehaviour {

    public CoordsysTransform coordsysTransform;
    //Generic Haptic Functions
    public GenericFunctionsClass myGenericFunctionsClassScript;
    public GameObject experimentController;
    public GameObject transformedCursor;

    [Header("Transformation variables")]
    public Transform cursorFrom;
    public Transform cursorTo;
    public Transform targetFrom;
    public Transform targetTo;

    void OnEnable()
    {
        HapticManager.OnStylusButton += SavePositionPair; //Subscribe
    }

    private void SavePositionPair()
    {
        if (coordsysTransform.SavedPosCnt < 3)
        {
            coordsysTransform.SavePositionPair(cursorFrom.position, cursorTo.position);
            if (coordsysTransform.SavedPosCnt == 3)
            {
                targetTo.position = coordsysTransform.ApplyTransformationTo(targetFrom.position);
                //Disable Optitrack cursor mesh.
                //We're going to continuously update a different cursor (see Update).
                //(Optitrack cursor will be used to update ui cursor later.)
                cursorTo.GetComponent<MeshRenderer>().enabled = false;
                transformedCursor.SetActive(true);
                experimentController.SetActive(true);
                HapticManager.OnStylusButton -= SavePositionPair; //Unsubscribe
            }
        }
    }

    private void Update()
    {
        if (coordsysTransform.SavedPosCnt == 3)
        {
            transformedCursor.transform.position = coordsysTransform.ApplyTransformationTo(cursorFrom.position);
        }
    }
}
