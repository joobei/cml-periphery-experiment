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
        HapticManager.OnStylusButton += DisplayChoiceUI;
        ButtonInteract.OnUIButton += Choose;
        touchTimerPrivate = touchTimer;
    }

    private void DisplayChoiceUI()
    {
        HapticManager.OnStylusButton -= DisplayChoiceUI;
        //Stop sphere callback
        HapticManager.disableSphere();
        //Deactivate cursor and target
        SetActiveNeededObjects(false); 
        decisionCanvas.SetActive(true); //Display UI
    }

    private void Choose(ButtonInteract.Decision decision)
    {
        //Hide UI and activate cursor and target
        decisionCanvas.SetActive(false);
        SetActiveNeededObjects(true);

        //Reactivate sphere callback
        HapticManager.enableSphere();

        //TODO: Write decision to log file

        HapticManager.OnStylusButton += DisplayChoiceUI;
        //Set initial value again and thereby start timer again
        touchTimerPrivate = touchTimer;
    }

    private void OnDisable()
    {
        HapticManager.OnStylusButton -= DisplayChoiceUI;
    }

    private void Update()
    {
        if (touchTimerPrivate == 0)
            return;

        touchTimerPrivate -= Time.deltaTime;

        if (touchTimerPrivate <= 0)
        {
            touchTimerPrivate = 0;
            DisplayChoiceUI();
        }
    }
}
