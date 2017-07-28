using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroductionState : ExperimentState
{
    public String[] introTextArray;
    short textIndex = 0;
    public Text textPane;

   public IntroductionState() { stateName = "Introduction"; }

    public override void OnEnable()
    {
        base.OnEnable();
        textPane.text = introTextArray[textIndex];
    }

    protected override void triggerPressed()
    {
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
