using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;


public class SimpleShapeContactWriting : HapticClassScript {

	//Generic Haptic Functions
	private GenericFunctionsClass myGenericFunctionsClassScript;

	private Writing myWritingScript;
	private GameObject myResetButton;
	private Color[] buttonResetColors = {new Color(1.0f, 1.0f, 1.0f), new Color(0.25f, 0.97f, 0.37f)};

    //Workspace Update Value
    float[] workspaceUpdateValue = new float[1];

    /*****************************************************************************/

    void Awake()
	{
		myGenericFunctionsClassScript = transform.GetComponent<GenericFunctionsClass>();

		myResetButton = GameObject.Find("reset");
		myResetButton.GetComponent<Renderer>().material.color = buttonResetColors[0];
		myWritingScript = GameObject.Find("Black_Board").GetComponent<Writing>();
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
			
		}
		else
			Debug.Log("Haptic Device cannot be launched");

		/***************************************************************/
		//Set Environmental Haptic Effect
		/***************************************************************/
		// Constant Force Example - We use this environmental force effect to simulate the weight of the cursor
		myGenericFunctionsClassScript.SetEnvironmentConstantForce();


		/***************************************************************/
		//Setup the Haptic Geometry in the OpenGL context 
		//And read haptic characteristics
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
		
		//myGenericFunctionsClassScript.GetTouchedObject();

        //Reset the writing on the board
        //if(ConverterClass.ConvertIntPtrToByteToString( PluginImport.GetTouchedObjectName()) == "reset") // GetTouchedObjectName - To be deprecated
        if (ConverterClass.ConvertIntPtrToByteToString(PluginImport.GetTouchedObjName(1)) == "reset")
        {
			myWritingScript.cleanBoard();

			//Change the Color of the button material
			myResetButton.GetComponent<Renderer>().material.color = buttonResetColors[1];
		}
		else
			myResetButton.GetComponent<Renderer>().material.color = buttonResetColors[0];

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
