using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EyeCalibrationState : ExperimentState
{
    public PupilCalibMarker calibrationMarker;

    public override void OnEnable()
    {
        base.OnEnable();
        calibrationMarker.startCalibrate();
    }

    protected override void triggerPressed()
    {
        //advanceState();
    }

    protected override void Update()
    {
        base.Update();
        if (calibrationMarker._finished)
        {
            advanceState();
        }
    }

   
}
