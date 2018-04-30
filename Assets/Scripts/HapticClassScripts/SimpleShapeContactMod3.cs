using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;


public class SimpleShapeContactMod3 : HapticClassScript {

	//Generic Haptic Functions
	private GenericFunctionsClass myGenericFunctionsClassScript;

    //Workspace Update Value
    float[] workspaceUpdateValue = new float[1];

    public GameObject cursorOptitrack;
    private Vector3 startPosCursorOptitrack;
    private bool savedStartPositions = false;
    private Vector3 vOptitrackChange;
    //position in Unity that corresponds to the stylus resting in the ink well
    private Vector3 posInkwell;
    public GameObject[] touchables;
    //Initial positions of touchables
    private Vector3[] posTouchables;
    //vector from ink well to haptic cursor's current position
    private Vector3 vCursorChange;
    public Transform unityTarget;

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
			
			//Update Workspace as function of camera
            //PluginImport.UpdateWorkspace(myHapticCamera.transform.rotation.eulerAngles.y);//To be deprecated

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

        //myGenericFunctionsClassScript.SetVibrationMotor();

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

        touchables[0].transform.position = unityTarget.position;
        posTouchables = new Vector3[touchables.Length];
        for (int i = 0; i < posTouchables.Length; i++)
        {
            posTouchables[i] = touchables[i].transform.position;
        }
	}


	void Update()
	{
		/***************************************************************/
		//Update Workspace as function of camera
		/***************************************************************/
		//PluginImport.UpdateWorkspace(myHapticCamera.transform.rotation.eulerAngles.y);//To be deprecated

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

        //if (Time.time >= 2f)
        //{
            bool touched = PluginImport.GetHapticContact(1);
            if (!touched)
            {
                touchables[0].transform.position = hapticCursor.transform.position + unityTarget.position - cursorOptitrack.transform.position;
                UpdateTargets();
            }
            else
                Debug.Log("Touch!");

            myGenericFunctionsClassScript.GetProxyValues();
            //if (!savedStartPositions)
            //{
            //    startPosCursorOptitrack = cursorOptitrack.transform.position;
            //    //Assume the stylus is in the ink well
            //    posInkwell = hapticCursor.transform.position;
            //    savedStartPositions = true;

            //}
            //vCursorChange = hapticCursor.transform.position - posInkwell;
            //vOptitrackChange = cursorOptitrack.transform.position - startPosCursorOptitrack;

            //Scale Optitrack position delta to try and make it more significant
            //vOptitrackChange.Scale(new Vector3(10f, 10f, 10f));

            //Subtract out haptic cursor delta by moving the touchable target as much
            //as the haptic cursor has moved away from the ink well.
            //We can't rely on the position from the haptic cursor (which is why we use Optitrack)
            //touchables[0].transform.position = unityTarget.position + vCursorChange;

            //Move the target
            //touchables[0].transform.position += vOptitrackChange;

            //if (myGenericFunctionsClassScript.Get)
        //}

        //myGenericFunctionsClassScript.GetProxyValues2();
        //myGenericFunctionsClassScript.UpdateHapticObjectMatrixTransform();

        //myGenericFunctionsClassScript.GetTouchedObject();

        //Debug.Log ("Button 1: " + PluginImport.GetButton1State()); // To be deprecated
        //Debug.Log ("Button 2: " + PluginImport.GetButton2State()); // To be deprecated

        //Debug.Log("Device 1: Button 1: " + PluginImport.GetButtonState(1, 1));
        //Debug.Log("Device 1: Button 2: " + PluginImport.GetButtonState(1, 2));
    }

    private void UpdateTargets()
    {
        ////current cursor pos
        //Vector3 posHapticCursor = myGenericFunctionsClassScript.GetProxyPosition();
        //for (int i = 0; i < touchables.Length; i++)
        //{
        //    //Vector from ink well to current position of haptic cursor 
        //    Vector3 vCursor = posInkWell + posHapticCursor;
        //    touchables[i].transform.position = posTouchables[i] - vCursor;
        //}

        myGenericFunctionsClassScript.UpdateHapticObjectMatrixTransform();
    }

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.cyan;
    //    foreach (GameObject touchable in touchables)
    //        Gizmos.DrawLine(posInkwell, touchable.transform.position);
    //    Gizmos.DrawLine(posInkwell, hapticCursor.transform.position);
    //    Gizmos.DrawLine(startPosCursorOptitrack, cursorOptitrack.transform.position);
    //}


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
