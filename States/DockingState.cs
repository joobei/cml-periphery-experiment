using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DockingState : ExperimentState
{
    public enum DockingStateType
    {
        toStart,
        toEnd
    };

    Trial currentTrial;
    private List<Trial> trials;
    //TrialNico currentTrial;
    //private List<TrialNico> trials;
    private DockingStateType dockingStateType = DockingStateType.toStart;
    public PupilGazeTracker gazeTracker;
    public bool enforceGaze;

    //to keep the distance between target and cursor
    float distance;

    //eccentricities and depths
    public List<int> eccentricities = new List<int>(); //Retinal eccentricity breaks (parafoveal 3, perifoveal 7) 
    public List<float> depths = new List<float>(); //depths at which to put the targets

    public GameObject target, rightController, cursor;

    string logPath;
    int trialCount = 1;
    float timeLast;

    public DockingState()
    {
        stateName = "Docking";
    }

    protected override void triggerPressed()
    {
        //only advance if we are within the threshold
        distance = Vector3.Distance(target.transform.position, cursor.transform.position);
        if (distance < 0.05f)
            advance();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        //Generate positions from Util (static class)
        string json = System.IO.File.ReadAllText(Application.dataPath + "/../Trials.json");
        trials = new List<Trial>(JsonHelper.FromJson<Trial>(json));
        //Vector3[,] positions = Util.generatePositions(eccentricities, depths);
        //trials = Util.generateTrials(positions);
        //foreach (TrialNico trial in trials)
        foreach (Trial trial in trials)
            Debug.Log("trial " + trials.IndexOf(trial) + ": " + trial);
        dockingStateType = DockingStateType.toStart;
        currentTrial = trials[0];
        trials.RemoveAt(0);
        target.transform.localPosition = currentTrial.start;
        timeLast = Time.time;
        //target.transform.localPosition = currentTrial.translation.start;

        string line = ""; //blank line (if file already exists)
        logPath = Application.dataPath + "/../log.csv";
        if (!File.Exists(logPath)) //write header
            line = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}",
                "trial_number", "time_in_milliseconds", "cursor.x", "cursor.y",
                "cursor.z", "target.x", "target.y", "target.z", "state");
        using (StreamWriter sw = File.AppendText(logPath))
        {
            sw.WriteLine(line);
        }
    }

    protected override void Update()
    {
        base.Update();

        if (!gazeTracker.checkEyeTrackingThreshold(0.3f) && timeRepeat < 0 && enforceGaze)
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
        LogTrial();
        switch (dockingStateType)
        {
            case DockingStateType.toStart:

                playSound("toot");
                target.transform.localPosition = currentTrial.end;
                //target.transform.localPosition = currentTrial.translation.end;
                dockingStateType = DockingStateType.toEnd;

                break;
            case DockingStateType.toEnd:
                if (trials.Count > 0)
                {
                    currentTrial = trials[0];
                    trials.RemoveAt(0);

                    Debug.Log("Advanced, remaining : " + trials.Count);
                    //move target to new position
                    target.transform.localPosition = currentTrial.start;
                    //target.transform.localPosition = currentTrial.translation.start;
                    dockingStateType = DockingStateType.toStart;
                    trialCount++;
                }
                else
                {
                    advanceState();
                }
                break;
        }
    }

    private void LogTrial()
    {
        float deltaTime = Time.time - timeLast;
        timeLast = Time.time;
        Vector3 targetPos = target.transform.localPosition;

        Transform rightCtrlParent = rightController.transform.parent;
        rightController.transform.SetParent(target.transform.parent);
        Vector3 cursorPos = rightController.transform.localPosition + cursor.transform.localPosition;
        rightController.transform.SetParent(rightCtrlParent);

        string line
            = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}",
            trialCount, deltaTime, cursorPos.x, cursorPos.y, cursorPos.z, targetPos.x,
            targetPos.y, targetPos.z, dockingStateType);
        using (StreamWriter sw = File.AppendText(logPath))
        {
            sw.WriteLine(line);
        }
    }
}
