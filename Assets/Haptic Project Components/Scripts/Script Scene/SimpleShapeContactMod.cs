using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;


public class SimpleShapeContactMod : HapticClassScript {

	//Generic Haptic Functions
	private GenericFunctionsClass myGenericFunctionsClassScript;

    //Workspace Update Value
    float[] workspaceUpdateValue = new float[1];

    public GameObject hapticTip;
    private Vector3 referencePosCursor;
    private float lastTime;
    private bool isStartPosCalibrated = false;

    /*****************************************************************************/

    void Awake()
	{
		myGenericFunctionsClassScript = transform.GetComponent<GenericFunctionsClass>();
    }

    void Start()
	{

		if(PluginImport.InitHapticDevice())
		{
			Debug.Log("OpenGL Context Launched");
			Debug.Log("Haptic Device Launched");
			
			myGenericFunctionsClassScript.SetHapticWorkSpace();
			myGenericFunctionsClassScript.GetHapticWorkSpace();

            //Update the Workspace as function of camera
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
	}


	void Update()
	{
		/***************************************************************/
		//Haptic Rendering Loop
		/***************************************************************/
		PluginImport.RenderHaptic ();

        //Associate the cursor object with the haptic proxy value  
        myGenericFunctionsClassScript.GetProxyValues();
		
        if (!isStartPosCalibrated && Time.time >= 1f)
        {
            CalibrateReferencePos(true); //set start positions
            isStartPosCalibrated = true;
            Debug.Log("Start pos calibrated");
        }

        if (isStartPosCalibrated && Time.time - lastTime > 0.75)
        {
            CalibrateReferencePos(PluginImport.GetButtonState(1, 2));
            PrintCursorPositionDelta();
            lastTime = Time.time;
        }
    }

    //Calibrate cursor reference positions
    private void CalibrateReferencePos(bool buttonPressed)
    {
        if (buttonPressed)
        {
            referencePosCursor = hapticCursor.transform.position;
            //referencePosCursor = hapticTip.transform.position;
            Debug.Log("Updated cursor's reference pos");
        }
    }


    /// <summary>
    /// Print haptic cursors' transform.position.g delta from their reference position
    /// </summary>
    /// <param name="g"> 0, 1, or 2. (i.e. x, y, or z) </param>
    private void PrintCursorPositionDelta(int g)
    {
        Debug.LogFormat("Time: {0}, cursor delta(pos[{1}], refPoint[{1}]): {2}", Time.time.ToString("F1"), g,
            /*Mathf.Abs*/(hapticCursor.transform.position[g] - referencePosCursor[g]).ToString("F2")
            ///*Mathf.Abs*/(hapticTip.transform.position[g] - referencePosCursorLeft[g]).ToString("F2")
            );
    }

    private void PrintCursorPositionDelta()
    {
        //TODO Calculate / calibrate scale factor
        //Also: Later, incorporate scaling factor in the workspace dimensions and don't apply it here anymore
        float scaleFactor = 0.69f;
        float deltaCursor = Vector3.Distance(hapticCursor.transform.position, referencePosCursor) / scaleFactor;
        //float deltaCursor = Vector3.Distance(hapticTip.transform.position, referencePosCursor) / scaleFactor;
        Debug.LogFormat("Time: {0}, cursor delta(pos, refPoint): {1}", Time.time.ToString("F1"), deltaCursor.ToString("F2"));
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
