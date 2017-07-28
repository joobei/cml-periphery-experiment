using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;



public class ExperimentController : MonoBehaviour
{
    protected ExperimentState currentState;
    public ExperimentState startingState;

    //Use this for initialization
    void Start()
    {
        currentState = startingState;
        currentState.enabled = true;
    }
}