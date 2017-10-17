using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;


public class SimpleShapeManipulationAndPhysicsBlock : HapticClassScript {


	//Generic Haptic Functions
	private GenericFunctionsClass myGenericFunctionsClassScript;
	
	public GameObject block;
	const int height = 8;
	
	private GameObject[] blocks;

    //Workspace Update Value
    float[] workspaceUpdateValue = new float[1];

    /*****************************************************************************/

    void Awake()
	{
		myGenericFunctionsClassScript = transform.GetComponent<GenericFunctionsClass>();
	}
	
	void makeJengaBlocks()
	{
		float xDistPercentage = 0.5f;
		float yDistPercentage = 0.4f;
		int i = 0;
		for (int y=0; y<height; y++)
		{
			for (int x=-1; x<=1; x++)
			{
				if (blocks[i]!=null) GameObject.Destroy(blocks[i]);
				if ((y & 1) == 0)
					blocks[i] = (GameObject)GameObject.Instantiate(block, new Vector3(x * xDistPercentage, y * yDistPercentage, 0.0f), Quaternion.identity);
				else
					blocks[i] = (GameObject)GameObject.Instantiate(block, new Vector3(0.0f, y * yDistPercentage, x * xDistPercentage), Quaternion.Euler(0.0f, 90.0f, 0.0f));

				blocks[i].GetComponent<Renderer>().enabled = true;
				blocks[i].name = "Block(" + (x + 1) + "," + y + ")";
				i++;
			}
		}
	}

	void Start()
	{
		blocks = new GameObject[height * 3];
		makeJengaBlocks();

		if(PluginImport.InitHapticDevice())
		{
			Debug.Log("OpenGL Context Launched");
			Debug.Log("Haptic Device Launched");
			
			myGenericFunctionsClassScript.SetHapticWorkSpace();
			myGenericFunctionsClassScript.GetHapticWorkSpace();

            //Update Workspace as function of camera
            //PluginImport.UpdateWorkspace(myHapticCamera.transform.rotation.eulerAngles.y);  //To be deprecated

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
		//Debug.Log ("Total Number of Haptic Objects: " + PluginImport.GetHapticObjectCount());

		/***************************************************************/
		//Launch the Haptic Event for all different haptic objects
		/***************************************************************/
		PluginImport.LaunchHapticEvent();
	}
	

	void Update()
	{

		/***************************************************************/
		//Act on the rigid body of the Manipulated object
		// if Mode = Manipulation Mode
		/***************************************************************/
		if(PluginImport.GetMode() == 1)
			ActivatingGrabbedObjectPropperties();

        /***************************************************************/
        //Update Workspace as function of camera
        /***************************************************************/
        //PluginImport.UpdateWorkspace(myHapticCamera.transform.rotation.eulerAngles.y);  //To be deprecated

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

		myGenericFunctionsClassScript.GetTouchedObject();

        //Debug.Log ("Button 1: " + PluginImport.GetButton1State()); // To be deprecated
        //Debug.Log ("Button 2: " + PluginImport.GetButton2State()); // To be deprecated

        //Debug.Log("Device 1: Button 1: " + PluginImport.GetButtonState(1, 1));
        //Debug.Log("Device 1: Button 2: " + PluginImport.GetButtonState(1, 2));

        if (PluginImport.GetButtonState(1, 2))
		{
			makeJengaBlocks();
		}
		
		const float rotSpeed = 1.0f;
		
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
		{
			myHapticCamera.transform.RotateAround(Vector3.zero, new Vector3(0.0f, 1.0f, 0.0f), rotSpeed);
		}
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
		{
			myHapticCamera.transform.RotateAround(Vector3.zero, new Vector3(0.0f, 1.0f, 0.0f), -rotSpeed);
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

	/*****************************************************************************************/
	/* Act on Characteristics of Rigid Body of the manipulated object*/
	/*****************************************************************************************/
	
	//Deactivating gravity and enabled kinematics when a object is grabbed
	bool previousButtonState = false;
	string grabbedObjectName = "";
	
	void ActivatingGrabbedObjectPropperties(){
		
		GameObject grabbedObject;
		string myObjStringName;
		
		if (!previousButtonState && PluginImport.GetButtonState(1, 1))
		{
            //If the object is grabbed, the gravity is deactivated and kinematic is enabled
            //myObjStringName = ConverterClass.ConvertIntPtrToByteToString(PluginImport.GetTouchedObjectName());//GetTouchedObjectName() - To be deprecated
            myObjStringName = ConverterClass.ConvertIntPtrToByteToString(PluginImport.GetTouchedObjName(1));

            if (!myObjStringName.Equals("null")){
				grabbedObject = GameObject.Find(myObjStringName);
				
				//If there is a rigid body
				if(grabbedObject.GetComponent<Rigidbody>() != null)
				{
					grabbedObject.GetComponent<Rigidbody>().isKinematic=true;
					grabbedObject.GetComponent<Rigidbody>().useGravity=false;
				}
				grabbedObjectName = myObjStringName;
			}
			previousButtonState = true;
		}
		
		else if (previousButtonState && !PluginImport.GetButtonState(1, 1))
		{
			//If the object is dropped, the grabity is enabled again and kinematic is deactivated
			if(!grabbedObjectName.Equals("")){
				grabbedObject = GameObject.Find(grabbedObjectName);
				
				//If there is a rigid body
				if(grabbedObject.GetComponent<Rigidbody>() != null)
				{
					grabbedObject.GetComponent<Rigidbody>().isKinematic=false;
					grabbedObject.GetComponent<Rigidbody>().useGravity=true;
				}
				grabbedObjectName = "";
			}
			previousButtonState = false;
		}
		
	}

}
