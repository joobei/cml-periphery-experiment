using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Inherit from Joutai.State? Or let HapticBaseState inherit from Joutai.State?
public class HapticCalibrationState : HapticBaseState
{
    public DualShapeContactExperiment dualShapeContactScript;
    public ScaleTarget scaleTarget;

    protected override void triggerPressed()
    {
        if (dualShapeContactScript.transRight != null && dualShapeContactScript.transRight.coordsysTransform.SavedPosCnt < 3)
        {
            dualShapeContactScript.transRight.DoTransformation();
        }
        else if (dualShapeContactScript.transLeft != null && dualShapeContactScript.transLeft.coordsysTransform.SavedPosCnt < 3)
        {
            dualShapeContactScript.transLeft.DoTransformation();

            //Are we done with both transformations?
            if (dualShapeContactScript.transLeft.coordsysTransform.SavedPosCnt == 3)
            {
                //Then update positions of our haptic targets in haptic space
                dualShapeContactScript.UpdateHapticObjectMatrixTransform();
                dualShapeContactScript.transLeft.targetFrom.GetComponent<MeshRenderer>().enabled = true;
                //and make the Optitrack target remember its initial scaling
                //(for use in scaleTarget.NextScale())
                scaleTarget.StoreInitialScaling();
            }
        }
        else
        {
            advanceState();
        }
    }
}
