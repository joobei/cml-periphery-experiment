using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FixationCalibrationState : ExperimentState
{
    public PupilGazeTracker gazeTracker;

    public override void OnEnable()
    {
        base.OnEnable();
        
    }

    protected override void triggerPressed()
    {
        //Debug.Log("Arm Length: "+armLength.ToString());
        gazeTracker.calibrateFixation();
        advanceState();
    }

}
