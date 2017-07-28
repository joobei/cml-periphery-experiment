using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class OutroState : ExperimentState {

    public String[] introTextArray;
    short textIndex = 0;
    public Text textPane;

    public OutroState()
    {
        stateName = "Debriefing";
    }

    public override void OnEnable()
    {
        base.OnEnable();
        textPane.text = introTextArray[textIndex];
    }

    protected override void triggerPressed()
    {
        Debug.Log(stateName + " triggerPressed");
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
