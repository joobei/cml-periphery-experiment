﻿using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

public class DualShapeContactMod : HapticClassScript {

    //Generic Haptic Functions
    private GenericFunctionsClass myGenericFunctionsClassScript;

    //Workspace Update Value
    float[] workspaceUpdateValue = new float[2];

    private Vector3 referencePointCursorLeft;
    private Vector3 referencePointCursorRight;
    private float lastTime;
    private bool isStartPosCalibrated = false;

    /*****************************************************************************/

    void Awake()
    {
        myGenericFunctionsClassScript = transform.GetComponent<GenericFunctionsClass>();
    }

    void Start()
    {

        if (PluginImport.InitTwoHapticDevices(ConverterClass.ConvertStringToByteToIntPtr(device1Name), ConverterClass.ConvertStringToByteToIntPtr(device2Name)))
        {
            Debug.Log("OpenGL Context Launched");
            Debug.Log("Haptic Device Launched");

            myGenericFunctionsClassScript.SetTwoHapticWorkSpaces();
            myGenericFunctionsClassScript.GetTwoHapticWorkSpaces();

            //Update two Workspaces as function of camera for each
            //PluginImport.UpdateTwoWorkspaces(myHapticCamera.transform.rotation.eulerAngles.y, myHapticCamera.transform.rotation.eulerAngles.y);//To be Deprecated

            //Update the Workspace as function of camera - Note that two different references can be used to update each workspace
            for (int i = 0; i < workspaceUpdateValue.Length; i++)
                workspaceUpdateValue[i] = myHapticCamera.transform.rotation.eulerAngles.y;

            PluginImport.UpdateHapticWorkspace(ConverterClass.ConvertFloatArrayToIntPtr(workspaceUpdateValue));
            
            //Set Mode of Interaction
            /*
             * Mode = 0 Contact
             * Mode = 1 Manipulation - So objects will have a mass when handling them
             * Mode = 2 Custom Effect - So the haptic device simulate vibration and tangential forces as power tools
             * Mode = 3 Puncture - So the haptic device is a needle that puncture inside a geometry
             */
            PluginImport.SetMode(ModeIndex);
            //Show a text descrition of the mode
            myGenericFunctionsClassScript.IndicateMode();

            //Set the touchable face(s)
            PluginImport.SetTouchableFace(ConverterClass.ConvertStringToByteToIntPtr(TouchableFace));

        }
        else
            Debug.Log("Haptic Device cannot be launched");

        /***************************************************************/
        //Set Environmental Haptic Effect
        /***************************************************************/
        // Constant Force Example - We use this environmental force effect to simulate the weight of the cursor
        myGenericFunctionsClassScript.SetEnvironmentConstantForce();
        // Viscous Force Example 
        //myGenericFunctionsClassScript.SetEnvironmentViscosity();
        // Friction Force Example 
        //myGenericFunctionsClassScript.SetEnvironmentFriction();
        // Spring Force Example 
        //myGenericFunctionsClassScript.SetEnvironmentSpring();


        /***************************************************************/
        //Setup the Haptic Geometry in the OpenGL context 
        //And read haptic characteristics
        /***************************************************************/
        myGenericFunctionsClassScript.SetHapticGeometry();

        //Get the Number of Haptic Object
        Debug.Log ("Total Number of Haptic Objects: " + PluginImport.GetHapticObjectCount());

        /***************************************************************/
        //Launch the Haptic Event for all different haptic objects
        /***************************************************************/
        PluginImport.LaunchHapticEvent();
        lastTime = Time.time;
    }


    void Update()
    {

        /***************************************************************/
        //Update two Workspaces as function of camera for each
        /***************************************************************/
        //PluginImport.UpdateTwoWorkspace(myHapticCamera.transform.rotation.eulerAngles.y, myHapticCamera.transform.rotation.eulerAngles.y);

        //Update the Workspace as function of camera - Note that two different reference can be used to update each workspace
        for (int i = 0; i < workspaceUpdateValue.Length; i++)
            workspaceUpdateValue[i] = myHapticCamera.transform.rotation.eulerAngles.y;

        PluginImport.UpdateHapticWorkspace(ConverterClass.ConvertFloatArrayToIntPtr(workspaceUpdateValue));

        /***************************************************************/
        //Update 2 cubes workspaces
        /***************************************************************/
        myGenericFunctionsClassScript.UpdateTwoGraphicalWorkspaces();

        /***************************************************************/
        //Haptic Rendering Loop
        /***************************************************************/
        PluginImport.RenderHaptic();

        /***************************************************************/
        //Update Haptic Object Transform
        /***************************************************************/
        myGenericFunctionsClassScript.UpdateHapticObjectMatrixTransform();

        //myGenericFunctionsClassScript.GetProxyValues();
        myGenericFunctionsClassScript.GetTwoProxyValues();

        //Rotate haptic cursors around centers of their workspace cubes so that their rears are facing
        hapticCursor.transform.RotateAround(new Vector3(myWorkSpacePosition[0], myWorkSpacePosition[1], myWorkSpacePosition[2]), Vector3.up, -90f);
        secondHapticCursor.transform.RotateAround(new Vector3(mySecondWorkSpacePosition[0], mySecondWorkSpacePosition[1], mySecondWorkSpacePosition[2]), Vector3.up, 90f);

        if (!isStartPosCalibrated && Time.time >= 1f)
            CalibrateStartPositions();
        DebugCursorPositions(1);

        //myGenericFunctionsClassScript.GetTouchedObject();

        /*Debug.Log("Device 1: Button 1: " + PluginImport.GetButtonState(1, 1));
        Debug.Log("Device 1: Button 2: " + PluginImport.GetButtonState(1, 2));
        Debug.Log("Device 2: Button 1: " + PluginImport.GetButtonState(2, 1));
        Debug.Log("Device 2: Button 2: " + PluginImport.GetButtonState(2, 2));*/

        /*if(PluginImport.GetHapticContact(1))
            Debug.Log("Device 1 touches: " + PluginImport.GetTouchedObjId(1) + " " + ConverterClass.ConvertIntPtrToByteToString(PluginImport.GetTouchedObjName(1)));
        if (PluginImport.GetHapticContact(2))
            Debug.Log("Device 2 touches: " + PluginImport.GetTouchedObjId(2) + " " + ConverterClass.ConvertIntPtrToByteToString(PluginImport.GetTouchedObjName(2)));*/

    }

    //Calibrate haptic cursor start positions
    private void CalibrateStartPositions()
    {
        referencePointCursorLeft = hapticCursor.transform.position;
        referencePointCursorRight = secondHapticCursor.transform.position;
        isStartPosCalibrated = true;
        Debug.Log("Start pos calibrated");
    }

    //Debug haptic cursor position.g delta (g = 0, 1, or 2, i.e. x, y, or z)
    private void DebugCursorPositions(int g)
    {
        if (isStartPosCalibrated && Time.time - lastTime > 0.75)
        {
            bool buttonPressed = PluginImport.GetButtonState(2, 2);
            if (buttonPressed)
            {
                referencePointCursorRight = secondHapticCursor.transform.position;
                Debug.Log("Updated right cursor's reference point");
            }

            Debug.LogFormat("Time: {2}, delta(pos[{3}], refPoint[{3}]): cursor1: {0}, cursor2: {1}",
                Mathf.Abs(hapticCursor.transform.position[g] - referencePointCursorLeft[g]).ToString("F2"),
                Mathf.Abs(secondHapticCursor.transform.position[g] - referencePointCursorRight[g]).ToString("F2"),
                Time.time.ToString("F1"), g);
            //Debug.LogFormat("Time: {2}, delta(pos, refPoint): cursor1: {0}, cursor2: {1}",
            //    Vector3.Distance(hapticCursor.transform.position, referencePointCursorLeft).ToString("F2"),
            //    Vector3.Distance(secondHapticCursor.transform.position, referencePointCursorRight).ToString("F2"),
            //    Time.time.ToString("F1"));
            lastTime = Time.time;
        }
    }

    void OnDisable()
    {
        if (PluginImport.HapticCleanUp())
        {
            Debug.Log("Haptic Context CleanUp");
            Debug.Log("Desactivate Device");
            Debug.Log("OpenGL Context CleanUp");
        }
    }
}
