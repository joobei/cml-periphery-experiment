using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionState : ExperimentState
{
    public String[] introTextArray;
    short textIndex = 0;
    public GameObject canvasObject;
    public Text textPane;   

    public override void OnEnable()
    {
        base.OnEnable();
        textPane.text = introTextArray[textIndex];
    }

    protected override void mousePressed()
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

    protected override void triggerPressed()
    {
        throw new NotImplementedException();
    }
}
