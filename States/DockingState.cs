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
    private List<Trial> deferredTrials;

    //TrialNico currentTrial;
    //private List<TrialNico> trials;
    private DockingStateType dockingStateType = DockingStateType.toStart;
    public PupilGazeTracker gazeTracker;
    public bool enforceGaze;

    //to keep the distance between target and cursor
    float distance;

    public GameObject target, rightController, cursor;

    string logPath;
    string toStartString;

    int trialCount = 1;
    float timeLast;
    float deltaTimeFailMin = 1f;
    float lastFailTime;

    public DockingState()
    {
        stateName = "Docking";
    }

    protected override void triggerPressed()
    {
        //only advance if we are within the threshold
        //if in unity editor i.e. debug then use trigger
        //if not only advance using click.
        //if (Application.isEditor && (distance < 0.05f))
        //{
        //    advance();
        //}
        
        gazeTracker.calibrateFixation();
    }

    protected override void mousePressed()
    {
        base.mousePressed();
        advance();
    }

    public override void OnEnable()
    {
        base.OnEnable(); //do parent class enable stuff

        deferredTrials = new List<Trial>();

        string json = System.IO.File.ReadAllText(Application.dataPath + "/../Trials.json");
        trials = new List<Trial>(JsonHelper.FromJson<Trial>(json));
        Debug.Log("Loaded " + trials.Count + " trials.");

        //generate positions
        foreach (Trial trial in trials) { 
            Vector3 tempVector;

            //create point along Z axis
            tempVector = new Vector3(0, 0, trial.startDepth*armLength);
            //rotate point by angle about Y axis
            tempVector = Quaternion.AngleAxis(trial.startAngle, new Vector3(0, 1, 0)) * tempVector;
            trial.start = tempVector;

            //create point along Z axis
            tempVector = new Vector3(0, 0, trial.endDepth*armLength);
            //rotate point by angle about Y axis
            tempVector = Quaternion.AngleAxis(trial.endAngle, new Vector3(0, 1, 0)) * tempVector;
            trial.end = tempVector;
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

        tempList = new List<Trial>();
        foreach (Trial trial in trials)
        {
            Trial tempTrial = new Trial(trial);
            tempList.Add(tempTrial);
        }
        foreach (Trial trial in tempList)
        {
            trials.Add(trial);
        }

        Debug.Log("No of trials: " + trials.Count);

        //tempList = new List<Trial>();

        //Debug.Log("No of trials: " + trials.Count);

        Util.Shuffle(trials);
        
        dockingStateType = DockingStateType.toStart;
        currentTrial = trials[0];
        
        trials.RemoveAt(0);
        target.transform.localPosition = currentTrial.start;
        timeLast = Time.time;

        logPath = Application.dataPath + "/../log.csv";
        //string line = ""; //blank line (if file already exists)
        //if (!File.Exists(logPath)) //write header
        //    line = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8},{9}",
        //        "trial_number", "time_in_milliseconds", "cursor.x", "cursor.y",
        //        "cursor.z", "target.x", "target.y", "target.z", "state");
        //using (StreamWriter sw = File.AppendText(logPath))
        //{
        //    sw.WriteLine(line);
        //}
    }

    protected override void Update()
    {
        base.Update();
        distance = Vector3.Distance(target.transform.position, cursor.transform.position);

        if (!gazeTracker.checkEyeTrackingThreshold(0.25f) && enforceGaze && (Time.time - lastFailTime >= deltaTimeFailMin))
        {
            lastFailTime = Time.time;
            resetTrial();
        }
    }

    private void resetTrial()
    {
        
        
        switch (dockingStateType)
        {
            case DockingStateType.toStart:
                currentTrial.timesStartFailed++;
                Debug.Log("currentTrial.timesStartFailed: " + currentTrial.timesStartFailed);
                playSound("Error");
                break;
            case DockingStateType.toEnd:
                currentTrial.timesEndFailed++;
                Debug.Log("currentTrial.timesEndFailed: " + currentTrial.timesEndFailed);
                playSound("Error");


                //save it for later if they failed because of
                //saccade allows them to remember position

                deferredTrials.Add(currentTrial);
                deferredTrials.Shuffle();

                //Fetch new one
                if (trials.Count == 0 && deferredTrials.Count == 0)
                {
                    advanceState();

                }
                if (trials.Count == 0 && deferredTrials.Count > 0)
                {
                    currentTrial = deferredTrials[0];
                    deferredTrials.RemoveAt(0);

                    Debug.Log("Advanced, remaining : " + trials.Count + deferredTrials.Count);
                    //move target to new position
                    target.transform.localPosition = currentTrial.start;
                    //target.transform.localPosition = currentTrial.translation.start;
                    dockingStateType = DockingStateType.toStart;
                    cursor.GetComponent<MeshRenderer>().enabled = true;
                    //trialCount++;
                }
                if (trials.Count > 0)
                {
                    currentTrial = trials[0];
                    trials.RemoveAt(0);

                    Debug.Log("Advanced, remaining : " + trials.Count);
                    Debug.Log("Deferred remaining : " + deferredTrials.Count);
                    //move target to new position
                    target.transform.localPosition = currentTrial.start;
                    //target.transform.localPosition = currentTrial.translation.start;
                    dockingStateType = DockingStateType.toStart;
                    cursor.GetComponent<MeshRenderer>().enabled = true;
                    //trialCount++;
                }
                break;
        }
        cursor.GetComponent<MeshRenderer>().enabled = true;
        timeLast = Time.time;
    }

    private void advance()
    {
        
        switch (dockingStateType)
        {
            case DockingStateType.toStart:
                
                //Save log string but don't log it until later when trial is complete
                toStartString = formLogLine();

                if (distance < 0.05)
                {
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

                }
                break;
            case DockingStateType.toEnd:
                LogTrial();
                playSound("toot");
                if (trials.Count ==0 && deferredTrials.Count == 0)
                {
                    advanceState();

                }
                if (trials.Count == 0 && deferredTrials.Count>0)
                {
                    currentTrial = deferredTrials[0];
                    deferredTrials.RemoveAt(0);

                    Debug.Log("Advanced, remaining : " + trials.Count + deferredTrials.Count);
                    //move target to new position
                    target.transform.localPosition = currentTrial.start;
                    //target.transform.localPosition = currentTrial.translation.start;
                    dockingStateType = DockingStateType.toStart;
                    cursor.GetComponent<MeshRenderer>().enabled = true;
                    trialCount++;
                }
                if (trials.Count > 0)
                {
                    currentTrial = trials[0];
                    trials.RemoveAt(0);

                    Debug.Log("Advanced, remaining : " + trials.Count);
                    Debug.Log("Deferred remaining : " + deferredTrials.Count);
                    //move target to new position
                    target.transform.localPosition = currentTrial.start;
                    //target.transform.localPosition = currentTrial.translation.start;
                    dockingStateType = DockingStateType.toStart;
                    cursor.GetComponent<MeshRenderer>().enabled = true;
                    trialCount++;
                }
                
                break;
        }

       
    }

    private String formLogLine()
    {
        float deltaTime = Time.time - timeLast;
        timeLast = Time.time;
        Vector3 targetPos = target.transform.localPosition;

        Transform rightCtrlParent = rightController.transform.parent;
        rightController.transform.SetParent(target.transform.parent);
        Vector3 cursorPos = rightController.transform.localPosition + cursor.transform.localPosition;
        rightController.transform.SetParent(rightCtrlParent);

        int currentTargetAngle;
        float currentTargetDepth;

        int timesFailed = 0;

        if (dockingStateType == DockingStateType.toStart)
        {
            currentTargetAngle = currentTrial.startAngle;
            currentTargetDepth = currentTrial.startDepth;
            timesFailed = currentTrial.timesStartFailed;
        }
        else
        {
            currentTargetAngle = currentTrial.endAngle;
            currentTargetDepth = currentTrial.endDepth;
            timesFailed = currentTrial.timesEndFailed;
        }

        string line = "";
        line += participant + ",";
        line += trialCount + ",";
        line += distance + ",";
        line += deltaTime + ",";
        line += cursorPos.x + ",";
        line += cursorPos.y + ",";
        line += cursorPos.z + ",";
        line += targetPos.x + ",";
        line += targetPos.y + ",";
        line += targetPos.z + ",";
        line += currentTrial.transferFunction.ToString() + ",";
        line += currentTargetAngle + ",";
        line += currentTargetDepth + ",";
        line += dockingStateType.ToString() +",";
        line += timesFailed;
        line += Environment.NewLine;

        return line;

    }

    private void LogTrial()
    {
        //form the line from this trial (toEnd)
        string line = formLogLine();

        File.AppendAllText(logPath, toStartString);
        File.AppendAllText(logPath, line);

        //StreamWriter sw;
        //using (sw = File.AppendText(logPath))
        //{
        //    //Log the start trial
        //    sw.WriteLine(toStartString);
        //    //Log the end trial
        //    sw.WriteLine(line);
        //}
        //sw.Dispose();
        
        //reset toStartString to gibberish so that we can debug
        toStartString = "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!";
    }

   
}
