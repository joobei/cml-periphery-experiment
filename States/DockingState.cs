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

    //to keep the distance between target and cursor
    float distance;

    //eccentricities and depths
    List<int> eccentricities = new List<int>() { -7, -3, 0, 3, 7 }; //Retinal eccentricity breaks (parafoveal 3, perifoveal 7) 
    List<float> depths = new List<float>() { 0.4f, .5f, .7f }; //depths at which to put the targets

    public GameObject target, cursor;

    public DockingState()
    {
        stateName = "Docking";
    }
    protected override void triggerPressed()
    {
        Debug.Log(stateName + " triggerPressed");
        if (dockingStateType == DockingStateType.toEnd && distance < 0.05f)
        { 
            advance();
        }
    }
    void Start()
    {
        //Generate positions from Util (static class)
        Vector3[,] positions = Util.generatePositions(eccentricities, depths);
        trials = Util.generateTrials(positions);
        currentTrial = trials[0];
        dockingStateType = DockingStateType.toStart;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        target.transform.localPosition = currentTrial.translation.from;
    }

    protected override void Update()
    {
        base.Update();

        //grab distance between cursor and target
        distance = Vector3.Distance(target.transform.position, cursor.transform.position);


        //move target toEnd position if close enough
        if (distance < 0.01 && dockingStateType == DockingStateType.toStart)
        {
            playSound("toot");
            target.transform.localPosition = currentTrial.translation.to;
            dockingStateType = DockingStateType.toEnd;
        }

        if (currentTrial.translation.from.Equals(currentTrial.translation.to))
        {
            advance();
        }
    }

    private void advance()
    {
        if (trials.Count > 0)
        {
            //todo: Log Trial!!

            //wand.TriggerHapticPulse(900);
            dockingStateType = DockingStateType.toStart;

            currentTrial = trials[0];
            trials.RemoveAt(0);
            //while (currentTrial.translation.from == currentTrial.translation.to)
            //{
            //    Debug.Log("skipped");
            //    
            //    currentTrial = trials[0];
            //}

            Debug.Log("Advanced, ramaining : " + trials.Count);
            //move target to new position
            target.transform.localPosition = currentTrial.translation.from;
        }
        else
        {
            advanceState();
        }



    }
}
