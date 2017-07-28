using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EyeCalibrationState : ExperimentState
{
    public PupilCalibMarker calibrationMarker;
    public String[] introTextArray;
    short textIndex = 0;
    public Text textPane;
     
    protected override void triggerPressed()
    {
        textIndex++;
        if (textIndex >= introTextArray.Length)
        {
            textIndex = 0;
            textPane.enabled = false;
            calibrationMarker.startCalibrate();
        }
        else
        {
            textPane.text = introTextArray[textIndex];
        }
    }
}
