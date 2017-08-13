using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TrainingDockingState : ExperimentState
{

    Trial currentTrial;
    private List<Trial> trials;

    DockingStateType dockingStateType = DockingStateType.toStart;
    public PupilGazeTracker gazeTracker;
    public bool enforceGaze;

    //to keep the distance between target and cursor
    float distance;

    public GameObject target, rightController, cursor;

    int trialCount = 10;
    float timeLast;

    protected override void triggerPressed()
    {
       
        if (distance < 0.05f)
            advance();
    }

    public override void OnEnable()
    {
        base.OnEnable(); //do parent class enable stuff

        string json = System.IO.File.ReadAllText(Application.dataPath + "/../Trials.json");
        trials = new List<Trial>(JsonHelper.FromJson<Trial>(json));

        foreach (Trial trial in trials)
        {
            trial.transferFunction = Transferfunction.closed;
            Debug.Log("trial " + trials.IndexOf(trial) + ": " + trial);
        }

        //HACK to duplicate list of trials through another temporary list

        List<Trial> tempList = new List<Trial>();

        foreach (Trial trial in trials)
        {
            Trial tempTrial = new Trial(trial);
            tempTrial.transferFunction = Transferfunction.open;
            tempList.Add(tempTrial);
        }

        foreach (Trial trial in tempList)
        {
            trials.Add(trial);
        }
        //END HACK


        Util.Shuffle(trials);
        
        //scale Z by arm length
        if (armLength != 0)
        {
            foreach (Trial trial in trials)
            {
                trial.start.z *= armLength;
                trial.end.z *= armLength;
            }
        }

        dockingStateType = DockingStateType.toStart;

        currentTrial = trials[0];
        trials.RemoveAt(0);

        target.transform.localPosition = currentTrial.start;
        timeLast = Time.time;
    }

    protected override void Update()
    {
        base.Update();

        distance = Vector3.Distance(target.transform.position, cursor.transform.position);

        if (!gazeTracker.checkEyeTrackingThreshold(0.1f) && timeRepeat < 0 && enforceGaze)
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
                target.transform.localPosition = currentTrial.start;
                //target.transform.localPosition = currentTrial.translation.start;
                break;
        }
    }

    private void advance()
    {
        switch (dockingStateType)
        {
            case DockingStateType.toStart:
                playSound("toot");
                target.transform.localPosition = currentTrial.end;
                //target.transform.localPosition = currentTrial.translation.end;
                dockingStateType = DockingStateType.toEnd;
                if (currentTrial.transferFunction == Transferfunction.open)
                {
                    cursor.GetComponent<MeshRenderer>().enabled = false;
                }
                else
                {
                    cursor.GetComponent<MeshRenderer>().enabled = true;
                }

                break;
            case DockingStateType.toEnd:
                playSound("toot");
                if (trialCount > 0)
                {
                    currentTrial = trials[0];
                    trials.RemoveAt(0);
                    trialCount--;

                    //move target to new position
                    target.transform.localPosition = currentTrial.start;
                    //target.transform.localPosition = currentTrial.translation.start;
                    dockingStateType = DockingStateType.toStart;
                    cursor.GetComponent<MeshRenderer>().enabled = true;
                }
                else
                {
                    //enable renderer in preparation for docking state.
                    cursor.GetComponent<MeshRenderer>().enabled = true;
                    advanceState();
                }
                break;
        }

       
    }

   

   
}
