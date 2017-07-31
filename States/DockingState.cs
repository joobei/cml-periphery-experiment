using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockingState : ExperimentState
{
    public enum DockingStateType
    {
        toStart,
        toEnd
    };

    Trial currentTrial;
    private List<Trial> trials;
    private DockingStateType dockingStateType = DockingStateType.toStart;
    public PupilGazeTracker gazeTracker;
    public bool enforceGaze;

    //to keep the distance between target and cursor
    float distance;

    //eccentricities and depths
    public List<int> eccentricities = new List<int>(); //Retinal eccentricity breaks (parafoveal 3, perifoveal 7) 
    public List<float> depths = new List<float>(); //depths at which to put the targets

    public GameObject target, cursor;

    public DockingState()
    {
        stateName = "Docking";



    }

    protected override void triggerPressed()
    {
        //only advance if we are within the threshold

        advance();

    }

    public override void OnEnable()
    {
        base.OnEnable();
        //Generate positions from Util (static class)
        Vector3[,] positions = Util.generatePositions(eccentricities, depths);
        trials = Util.generateTrials(positions);
        dockingStateType = DockingStateType.toStart;
        currentTrial = trials[0];
        target.transform.localPosition = currentTrial.translation.start;
    }

    protected override void Update()
    {
        base.Update();

        if (gazeTracker.checkEyeTrackingThreshold(0.1f) && timeRepeat < 0)
        {
            resetTrial();
        }
    }

    private void resetTrial()
    {
        switch (dockingStateType)
        {
            case DockingStateType.toStart:
                playSound("Error");
                break;
            case DockingStateType.toEnd:
                playSound("Error");
                dockingStateType = DockingStateType.toStart;
                target.transform.localPosition = currentTrial.translation.start;
                break;
        }
    }

    private void advance()
    {
        //grab distance between cursor and target
        distance = Vector3.Distance(target.transform.position, cursor.transform.position);

        switch (dockingStateType)
        {
            case DockingStateType.toStart:

                playSound("toot");
                target.transform.localPosition = currentTrial.translation.end;
                dockingStateType = DockingStateType.toEnd;

                break;
            case DockingStateType.toEnd:


                if (distance < 0.05f)
                {
                    if (trials.Count > 0)
                    {
                        //todo: Log Trial!
                        dockingStateType = DockingStateType.toStart;

                        currentTrial = trials[0];
                        trials.RemoveAt(0);

                        Debug.Log("Advanced, ramaining : " + trials.Count);
                        //move target to new position
                        target.transform.localPosition = currentTrial.translation.start;
                    }
                    else
                    {
                        advanceState();
                    }
                }
                break;

        }
    }
}
