using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;


public class CustomForceEffect : HapticClassScript {

	//Generic Haptic Functions
	private GenericFunctionsClass myGenericFunctionsClassScript;

    //Workspace Update Value
    float[] workspaceUpdateValue = new float[1];

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
			// Viscous Force Example
			myGenericFunctionsClassScript.SetEnvironmentViscosity ();

			// Constant Force Example - We use this environmental force effect to simulate the weight of the cursor
			myGenericFunctionsClassScript.SetEnvironmentConstantForce();

			//Custom Force Effect Vibration Motor
			myGenericFunctionsClassScript.SetVibrationMotor();

			//Custom Force Effect Vibration at Contact//Good for p1ulsation
			myGenericFunctionsClassScript.SetVibrationContact();

			//Custom Tangential Force corresponding to that of a rotating power tool (e.g. Drill, Polisher, Grinder)
			// if tool is angled set direction to 0,1,0 otherwise it does not matter (Tool will be straight)
			myGenericFunctionsClassScript.SetTangentialForce();
		
		
		/***************************************************************/
		//Setup the Haptic Geometry in the OpenGL context
		/***************************************************************/
		myGenericFunctionsClassScript.SetHapticGeometry();

		//Get the Number of Haptic Object
		//Debug.Log ("Total Number of Haptic Objects: " + PluginImport.GetHapticObjectCount());

		/***************************************************************/
		//Launch the Haptic Event for all different haptic objects
		/***************************************************************/
		PluginImport.LaunchHapticEvent();
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
		myGenericFunctionsClassScript.GetProxyValues();	
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
