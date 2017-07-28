using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeCalibrationState : ExperimentState
{
    protected override void triggerPressed()
    {
        advanceState();
    }
}
