using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;


public class ApplyTransformation : MonoBehaviour {

    //Generic Haptic Functions
    public GenericFunctionsClass myGenericFunctionsClassScript;

    public Transform optitrackCursor;
    public Transform optitrackTarget;
    public Transform hapticCursor;
    public Transform hapticTarget;

    //Workspace Update Value
    float[] workspaceUpdateValue = new float[1];

    public CoordsysTransform coordsysTransform;

    /*****************************************************************************/

    internal void DoTransformation()
    {
        coordsysTransform.SavePositionPair(optitrackCursor.position, hapticCursor.position);
        if (coordsysTransform.SavedPosCnt == 3)
        {
            coordsysTransform.CreateTransformation();
            //Apply transformation to the touchable object (aka haptic target)
            hapticTarget.position = coordsysTransform.ApplyTransformationTo(optitrackTarget.position);

            //Also, scale unity target accordingly
            optitrackTarget.localScale = Vector3.one / coordsysTransform.scaling;
            //optitrackTarget.GetComponent<MeshRenderer>().enabled = true;
        }
    }
}
