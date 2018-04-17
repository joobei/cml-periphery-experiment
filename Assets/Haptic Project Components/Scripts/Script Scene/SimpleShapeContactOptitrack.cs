using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;


public class SimpleShapeContactOptitrack : HapticClassScript {

    public CoordsysTransform coordsysTransform;
    public Transform optitrackCursor;
    public Transform optitrackTarget;
    public Transform hapticTarget;


	//Generic Haptic Functions
	private GenericFunctionsClass myGenericFunctionsClassScript;

    //Workspace Update Value
    float[] workspaceUpdateValue = new float[1];

    [Tooltip("Interval in seconds for writing debug output")]
    public float buttonInterval;

    private float lastTime;
    private byte savedPairs = 0;

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
        //Update the Workspace as function of camera
        for (int i = 0; i < workspaceUpdateValue.Length; i++)
            workspaceUpdateValue[i] = myHapticCamera.transform.rotation.eulerAngles.y;

        PluginImport.UpdateHapticWorkspace(ConverterClass.ConvertFloatArrayToIntPtr(workspaceUpdateValue));

        /***************************************************************/
        //Update cube workspace
        /***************************************************************/
        myGenericFunctionsClassScript.UpdateGraphicalWorkspace();

        /***************************************************************/
        //Haptic Rendering Loop
        /***************************************************************/
        PluginImport.RenderHaptic ();

        //Associate the cursor object with the haptic proxy value  
        myGenericFunctionsClassScript.GetProxyValues();

        if ( (PluginImport.GetButtonState(1, 1) || PluginImport.GetButtonState(1, 2)) 
            && Time.time - lastTime > buttonInterval
            && coordsysTransform.SavedPosCnt < 3)
        {
            coordsysTransform.SavePositionPair(optitrackCursor.position, hapticCursor.transform.position);
            if (coordsysTransform.SavedPosCnt == 3)
            {
                coordsysTransform.CreateTransformation();
                //Apply transformation to the touchable object (aka haptic target)
                hapticTarget.position = coordsysTransform.ApplyTransformationTo(optitrackTarget.position);

                //and update its position in haptic space
                myGenericFunctionsClassScript.UpdateHapticObjectMatrixTransform();

                //Also, scale unity target accordingly
                optitrackTarget.localScale = Vector3.one / coordsysTransform.scaling;
            }

            lastTime = Time.time;
        }
    }


    public void PrintCursorPosition() 
    {
        Debug.LogFormat("Time: {0}, cursor pos: {1}", Time.time.ToString("F1"),
            hapticCursor.transform.position.ToString("F2"));
    }

    void OnDisable()
	{
		if (PluginImport.HapticCleanUp())
		{
			Debug.Log("Haptic Context CleanUp");
			Debug.Log("Deactivate Device");
			Debug.Log("OpenGL Context CleanUp");
		}
	}
}
