using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Joutai;

public class TrialsState : State
{
    public GameObject decisionCanvas;

    public override void OnEnable()
    {
        base.OnEnable();
        HapticManager.OnStylusButton += AdvanceTrial;
        Interact.OnUIButton += Choose;
    }

    //Advance to next trial
    private void AdvanceTrial()
    {
        HapticManager.OnStylusButton -= AdvanceTrial;
        //Deactivate cursor and target and display UI
        SetActiveNeededObjects(false); 
        //Display decision UI
        decisionCanvas.SetActive(true);
    }

    private void Choose(Interact.Decision decision)
    {
        //Hide UI and activate cursor and target
        decisionCanvas.SetActive(false);
        SetActiveNeededObjects(true);

        //TODO: Write decision to log file

        HapticManager.OnStylusButton += AdvanceTrial;
    }

    private void OnDisable()
    {
        HapticManager.OnStylusButton -= AdvanceTrial;
    }
}
