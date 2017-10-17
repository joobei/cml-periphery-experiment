using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;


public class HapticInjection : HapticClassScript {
	
	//Generic Haptic Functions
	private GenericFunctionsClass myGenericFunctionsClassScript;

    //Workspace Update Value
    float[] workspaceUpdateValue = new float[1];

    private float myDopLimit;
	
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
				
			//Set the lenght of the syringue needle to penetrate inside the tissue
			PluginImport.SetMaximumPunctureLenght(maxPenetration);

			//Set the touchable face(s)
			PluginImport.SetTouchableFace(ConverterClass.ConvertStringToByteToIntPtr(TouchableFace));

			
		}
		else
			Debug.Log("Haptic Device cannot be launched");


		/***************************************************************/
		//Set Environmental Haptic Effect
		/***************************************************************/

			// Constant Force Example - We use this environmental force effect to simulate the weight of the cursor
			//myGenericFunctionsClassScript.SetEnvironmentConstantForce();
			
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

		//For the Puncture Mode effect
		if(PluginImport.GetMode() == 3)
		{
			//Debug.Log ("Contact state is set to " + PluginImport.GetContact());
			//Debug.Log ("Penetration State " + PluginImport.GetPenetrationRatio());

			double[] myScp = new double[3];
			myScp = ConverterClass.ConvertIntPtrToDouble3(PluginImport.GetFirstScpPt());
			//Debug.Log (" SCP " + myScp[0] + " " + myScp[1] + " " + myScp[2]);

			Vector3 posInjectionHole;
			posInjectionHole = ConverterClass.ConvertDouble3ToVector3(myScp);
			GameObject.Find ("InjectionMarker").transform.position = posInjectionHole;

			/*double[] myProx = new double[3];
			myProx = ConverterClass.ConvertIntPtrToDouble3(PluginImport.GetProxyPosition());
			
			Vector3 posProx;
			posProx = ConverterClass.ConvertDouble3ToVector3(myProx);
			GameObject.Find ("ProxyTipMarker").transform.position = posProx;*/

			/*double[] myDev = new double[3];
			myDev = ConverterClass.ConvertIntPtrToDouble3(PluginImport.GetDevicePosition());

			Vector3 posDevice;
			posDevice = ConverterClass.ConvertDouble3ToVector3(myDev);
			GameObject.Find ("DeviceTipMarker").transform.position = posDevice;*/

			double[] myPinch = new double[3];
			myPinch = ConverterClass.ConvertIntPtrToDouble3(PluginImport.GetPunctureDirection());

			Vector3 start = new Vector3();
			start = ConverterClass.ConvertDouble3ToVector3(myScp);
			Vector3 end = new Vector3();
			end = ConverterClass.ConvertDouble3ToVector3(myPinch);
			end.Normalize();

			Debug.DrawLine(start,start+end * maxPenetration, Color.green);

			//Ray Cast so we can determine the limitation of the puncture
			RaycastHit[] hits;
			hits = Physics.RaycastAll(start, end , maxPenetration);

			if(hits.Length != 0)
			{
				//Declare a float array to store the tissue layer
				float[] tissueLayers = new float[hits.Length];
				//Declare a string array to store the name of the tissue layer
				string[] punctObjects = new string[hits.Length];
				int nbLayer = 0;

				for (int i = 0; i < hits.Length; i++) 
				{
					RaycastHit hit = hits[i];

					//Only if the object is declared as touchable
					if(hit.collider.gameObject.tag == "Touchable")
					{
						tissueLayers[nbLayer] = hit.distance;
						punctObjects[nbLayer] = hit.collider.name;
						nbLayer++;
					}
				}

				/*Declaration of the Puncture Stack
				 * Additionally, on the basis of the puncture stack components, the plugin setup a penetration restriction
				 * - due to the fact that Proxy Method along such constraint line is not accurate - most probably due to the fact
				 * that device position and proxy position differ because the constraint applies forces onto the device.
				 * So, the plugin impedes the proxy to penetrate in underlying layer when their popthrough values is null
				 */
				SetPunctureStack(nbLayer, punctObjects, tissueLayers);
			}
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


	void SetPunctureStack(int nbLayer ,string[] name, float[] array)
	{
		IntPtr[] objname = new IntPtr[nbLayer];
		//Assign object encounter along puncture vector to the Object array
		for (int i = 0; i < nbLayer; i++)
			objname[i] = ConverterClass.ConvertStringToByteToIntPtr(name[i]);

		PluginImport.SetPunctureLayers(nbLayer, objname,ConverterClass.ConvertFloatArrayToIntPtr(array));

	}
}
