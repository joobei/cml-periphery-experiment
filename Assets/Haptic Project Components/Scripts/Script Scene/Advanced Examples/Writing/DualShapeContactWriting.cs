using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;


public class DualShapeContactWriting : HapticClassScript {
   
    //Generic Haptic Functions
    private GenericFunctionsClass myGenericFunctionsClassScript;

    //Workspace Update Value
    float[] workspaceUpdateValue = new float[2];

    /*****************************************************************************/

    private Writing myWritingScript;
    private GameObject myResetButton;
    private Color[] buttonResetColors = { new Color(1.0f, 1.0f, 1.0f), new Color(0.25f, 0.97f, 0.37f) };

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

        if (PluginImport.InitTwoHapticDevices(ConverterClass.ConvertStringToByteToIntPtr(device1Name), ConverterClass.ConvertStringToByteToIntPtr(device2Name)))
        {
            Debug.Log("OpenGL Context Launched");
            Debug.Log("Haptic Device Launched");


            myGenericFunctionsClassScript.SetTwoHapticWorkSpaces();
            myGenericFunctionsClassScript.GetTwoHapticWorkSpaces();

            //Update the Workspace as function of camera - Note that two different reference can be used to update each workspace
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
        Debug.Log ("Total Number of Haptic Objects: " + PluginImport.GetHapticObjectCount());

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

        //Associate the cursor object with the haptic proxy value  
        //myGenericFunctionsClassScript.GetProxyValues();
        myGenericFunctionsClassScript.GetTwoProxyValues();

        //myGenericFunctionsClassScript.GetTouchedObject();


        //Reset the writing on the board
        if (ConverterClass.ConvertIntPtrToByteToString(PluginImport.GetTouchedObjName(1)) == "reset" || ConverterClass.ConvertIntPtrToByteToString(PluginImport.GetTouchedObjName(2)) == "reset")
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

