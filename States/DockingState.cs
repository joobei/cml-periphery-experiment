using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockingState : ExperimentState
{
    Trial currentTrial;
    private List<Trial> trials;

    //to keep the distance between target and cursor
    float distance;

    //eccentricities and depths
    List<int> eccentricities = new List<int>() { -7, -3, 0, 3, 7 }; //Retinal eccentricity breaks (parafoveal 3, perifoveal 7) 
    List<float> depths = new List<float>() { 0.4f, .5f, .7f }; //depths at which to put the targets

    public GameObject target, cursor;

    protected override void Start()
    {
        base.Start();
        stateName = "Docking";
        //Generate positions from Util (static class)
        Vector3[,] positions = Util.generatePositions(eccentricities, depths);
        trials = Util.generateTrials(positions);
        currentTrial = trials[0];
    }

    public override void Activate()
    {
        base.Activate();
        //Debug.Log("State: " + this.name);
        target.transform.localPosition = currentTrial.translation.from;
    }

    protected override void Update()
    {
        base.Update();

        //grab distance between cursor and target
        distance = Vector3.Distance(target.transform.position, cursor.transform.position);

        if (distance < 0.01)
        {
            playSound("toot");
            target.transform.localPosition = currentTrial.translation.to;
        }

        if (Input.GetMouseButtonDown(0))
            base.advanceState();
    }

    private void advance()
    {
       


        //case StateType.toEnd:
        //    {
        //        if (distance < 0.05f) //only allow advancement if close enough (avoid accidental advancement)
        //        {
        //            if (trials.Count > 0)
        //            {
        //                wand.TriggerHapticPulse(900);
        //                _experimentState = StateType.toStart;
        //                currentTrial = trials[0];
        //                trials.RemoveAt(0);
        //                Debug.Log("Advanced, ramaining : " + trials.Count);
        //                //move target to new position + height
        //                target.transform.localPosition = currentTrial.translation.from;
        //            }

        //        }




    }
}
