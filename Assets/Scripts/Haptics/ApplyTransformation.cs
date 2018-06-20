using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;


public class ApplyTransformation : MonoBehaviour {

    //Generic Haptic Functions
    public GenericFunctionsClass myGenericFunctionsClassScript;

    public Transform cursorFrom;
    public Transform targetFrom;
    public Transform cursorTo;
    public Transform targetTo;

    public CoordsysTransform coordsysTransform;

    /*****************************************************************************/

    internal void DoTransformation()
    {
        coordsysTransform.SavePositionPair(cursorFrom.position, cursorTo.position);
        if (coordsysTransform.SavedPosCnt == 3)
        {
            //coordsysTransform.CreateTransformation();
            //Apply transformation to the touchable object (aka haptic target)
            targetTo.position = coordsysTransform.ApplyTransformationTo(targetFrom.position);

            //Also, scale unity target accordingly
            targetFrom.localScale = targetTo.localScale / coordsysTransform.scaling;
        }
    }
}
