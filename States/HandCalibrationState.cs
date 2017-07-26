using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandCalibrationState : ExperimentState
{
    public String[] introTextArray;
    short textIndex = 0;
    public Text textPane;

    public HandCalibrationState()
    {
        stateName = "Arm Length Calibration";
    }

    public override void Activate()
    {
        base.Activate();
        textPane.text = introTextArray[textIndex];
    }

    protected override void triggerPressed() {
        
        textIndex++;
        if (textIndex >= introTextArray.Length)
        {
            textIndex = 0;
            advanceState();
        }
        else
        {
            textPane.text = introTextArray[textIndex];
        }
        
    }
}
