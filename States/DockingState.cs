using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DockingState : ExperimentState
{
    public string participant;

    Trial currentTrial;
    private List<Trial> trials;
    //TrialNico currentTrial;
    //private List<TrialNico> trials;
    private DockingStateType dockingStateType = DockingStateType.toStart;
    public PupilGazeTracker gazeTracker;
    public bool enforceGaze;

    //to keep the distance between target and cursor
    float distance;

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
        //if in unity editor i.e. debug then use trigger
        //if not only advance using click.
        if (Application.isEditor && (distance < 0.05f))
        {
            advance();
        }
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

        string line = ""; //blank line (if file already exists)
        logPath = Application.dataPath + "/../log.csv";
        if (!File.Exists(logPath)) //write header
            line = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8},{9}",
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
                break;
        }
        cursor.GetComponent<MeshRenderer>().enabled = true;
        timeLast = Time.time;
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
                if (trials.Count > 0)
                {
                    currentTrial = trials[0];
                   
                    trials.RemoveAt(0);

                    Debug.Log("Advanced, remaining : " + trials.Count);
                    //move target to new position
                    target.transform.localPosition = currentTrial.start;
                    //target.transform.localPosition = currentTrial.translation.start;
                    dockingStateType = DockingStateType.toStart;
                    cursor.GetComponent<MeshRenderer>().enabled = true;
                    trialCount++;
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(logPath))
                    {
                        sw.Close();
                    }
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

        float currentTargetAngle;
        if (dockingStateType == DockingStateType.toStart)
        {
            currentTargetAngle = Vector3.SignedAngle(new Vector3(0, 0, 0.5f), currentTrial.start,new Vector3(0,1,0));
            Debug.Log("Angle :" + currentTargetAngle);
        }
        else
        {
            currentTargetAngle = Vector3.SignedAngle(new Vector3(0, 0, 0.5f), currentTrial.end, new Vector3(0, 1, 0));
            Debug.Log("Angle :" + currentTargetAngle);

        }


        string line = "";
        line += participant + ",";
        line += trialCount + ",";
        line += distance + ",";
        line += deltaTime + ",";
        line += cursorPos.x.ToString("F4") + ",";
        line += cursorPos.y.ToString("F4") + ",";
        line += cursorPos.z.ToString("F4") + ",";
        line += targetPos.x.ToString("F4") + ",";
        line += targetPos.z.ToString("F4") + ",";
        line += currentTrial.transferFunction.ToString()+",";
        line += (int)currentTargetAngle + ",";
        line += dockingStateType.ToString();

        using (StreamWriter sw = File.AppendText(logPath))
        {
            sw.WriteLine(line);
        }
    }

   
}
