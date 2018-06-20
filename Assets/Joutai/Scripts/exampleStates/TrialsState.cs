using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Joutai;

public class TrialsState : State
{
    public GameObject decisionCanvas;
    [Tooltip("Time the participant has for feeling size of sphere before s/he has to decide")]
    public float touchTimer;
    private float touchTimerPrivate;

    public override void OnEnable()
    {
        base.OnEnable();
        HapticManager.OnStylusButton += AdvanceTrial;
        Interact.OnUIButton += Choose;
        touchTimerPrivate = touchTimer;
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
        //Set initial value again and thereby start timer again
        touchTimerPrivate = touchTimer;
    }

    private void OnDisable()
    {
        HapticManager.OnStylusButton -= AdvanceTrial;
    }

    private void Update()
    {
        if (touchTimerPrivate == 0)
            return;

        touchTimerPrivate -= Time.deltaTime;

        if (touchTimerPrivate <= 0)
        {
            touchTimerPrivate = 0;
            AdvanceTrial();
        }
    }
}
