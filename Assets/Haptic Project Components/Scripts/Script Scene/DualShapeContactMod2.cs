using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

public class DualShapeContactMod2 : HapticClassScript {

    public ApplyTransformation transLeft;
    public ApplyTransformation transRight;

    [Tooltip("Interval in seconds for writing debug output")]
    public float buttonInterval;

    //Generic Haptic Functions
    private GenericFunctionsClass myGenericFunctionsClassScript;

    //Workspace Update Value
    float[] workspaceUpdateValue = new float[2];

    private float lastTime;

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

        //if (PluginImport.GetButtonState(1, 1))
        //{
        //    Debug.Log("1, 1");
        //}
        //else if (PluginImport.GetButtonState(1, 2))
        //{
        //    Debug.Log("1, 2");
        //}
        //else if (PluginImport.GetButtonState(2, 1))
        //{
        //    Debug.Log("2, 1");
        //}
        //else if (PluginImport.GetButtonState(2, 2))
        //{
        //    Debug.Log("2, 2");
        //}


        //Using only the buttons on one stylus because the other's don't
        //seem to function. Creating and applying the transformation
        //for the right Omni first, then for the left one.
        if ((PluginImport.GetButtonState(1, 1) || PluginImport.GetButtonState(1, 2))
            && Time.time - lastTime > buttonInterval)
        {
            if (transRight != null && transRight.coordsysTransform.SavedPosCnt < 3)
            {
                transRight.DoTransformation();
            }
            else if (transLeft != null && transLeft.coordsysTransform.SavedPosCnt < 3)
            {
                transLeft.DoTransformation();
            }
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
