using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;



public class ExperimentController : Singleton<ExperimentController>
{
    protected ExperimentState currentState;
    public ExperimentState startingState;

    public GameObject controller;
    private SteamVR_TrackedObject trackedObject;
    private SteamVR_Controller.Device wand;

    //Use this for initialization
    void Start() {
        //trackedObject = controller.GetComponentInChildren<SteamVR_TrackedObject>();
        currentState = startingState;
        currentState.Activate();
    }
       
    // Update is called once per frame
    //void Update () {
       

    //    wand = SteamVR_Controller.Input((int)trackedObject.index);

      

    //    if ((wand.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) || Input.GetMouseButtonDown(0)))
    //    { }
    //        //advance();
    //}
}