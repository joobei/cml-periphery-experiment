using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HapticTouchingState : HapticBaseState
{
    public GameObject optiTarget;
    public GameObject decisionCanvas;

    //For making the decision if bigger or smaller
    //opti cursor is between optitarget and this
    private Transform forDeciding;

    private enum Phase
    {
        touching, deciding
    }

    private Phase phase = Phase.touching;

    public HapticTouchingState()
    {
        stateName = "Touching";

    }

    protected override void triggerPressed()
    {
        if (phase == Phase.touching)
        {
            phase = Phase.deciding;
         
            //Move forDeciding so that optiCursor is
            //in the middle between optiTarget and forDeciding


        }
        else if (phase == Phase.deciding)
        {
            advance();
        }
    }

    public override void OnEnable()
    {
        base.OnEnable(); //do parent class enable stuff

        decisionCanvas.SetActive(true);

        //TODO Init size pair list etc
    }

    //Next size pair in list (haptic vs real)
    private void advance()
    {
        //Measure to which (of optiTarget and forDeciding) optiCursor is closer
        //and save decision (to file)

        //Either switch phase to touching
        phase = Phase.touching;

        //or move on to next state
    }


}
