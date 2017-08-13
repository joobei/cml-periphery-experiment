using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandCalibrationState : ExperimentState
{
    public TrainingDockingState trainingDockingstate;
    public DockingState dockingState;

    public String[] introTextArray;
    short textIndex = 0;
    public Text textPane;

    public GameObject cursor, hmd;

    public HandCalibrationState()
    {
        stateName = "Arm Length Calibration";
    }

    public override void OnEnable()
    {
        base.OnEnable();
        textPane.text = introTextArray[textIndex];
    }

    protected override void triggerPressed()
    {
        float tempLength = Vector3.Distance(cursor.transform.position, hmd.transform.position);
        trainingDockingstate.armLength = tempLength;
        dockingState.armLength = tempLength;
        Debug.Log("Arm Length :" + armLength);
        advanceState();
    }
}
