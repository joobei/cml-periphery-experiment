using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkspaceRotationNPosDelta : MonoBehaviour {

    public HapticClassScript hCS;

    public GameObject hapticTip1;
    public GameObject hapticTip2;
    private Vector3 referencePosCursorLeft;
    private Vector3 referencePosCursorRight;
    private float lastTime;
    private bool areStartPosCalibrated = false;

	
	void Update () {
        Vector3 workSpacePosLeft = new Vector3(hCS.myWorkSpacePosition[0], hCS.myWorkSpacePosition[1], hCS.myWorkSpacePosition[2]);
        Vector3 workSpacePosRight = new Vector3(hCS.mySecondWorkSpacePosition[0], hCS.mySecondWorkSpacePosition[1], hCS.mySecondWorkSpacePosition[2]);

        //Rotate workspaces so that they're not rotated in comparison to the cursors
        hCS.workSpaceObj.transform.RotateAround(workSpacePosLeft, Vector3.up, 90f);
        hCS.secondWorkSpaceObj.transform.RotateAround(workSpacePosLeft, Vector3.up, -90f);

        //Rotate haptic cursors around centers of their workspace cubes so that their rears are facing
        hCS.hapticCursor.transform.RotateAround(workSpacePosLeft, Vector3.up, -90f);
        hCS.secondHapticCursor.transform.RotateAround(workSpacePosRight, Vector3.up, 90f);

        if (!areStartPosCalibrated && Time.time >= 1f)
        {
            CalibrateReferencePos(true, true); //set start positions
            areStartPosCalibrated = true;
            Debug.Log("Start pos calibrated");
        }

        if (areStartPosCalibrated && Time.time - lastTime > 0.75)
        {
            CalibrateReferencePos(PluginImport.GetButtonState(1, 2), PluginImport.GetButtonState(2, 2));
            PrintCursorPositionDelta();
            lastTime = Time.time;
        }
    }

    //Calibrate cursor reference positions
    private void CalibrateReferencePos(bool cursorLeft, bool cursorRight)
    {
        if (cursorLeft)
        {
            //referencePosCursorLeft = hapticCursor.transform.position;
            referencePosCursorLeft = hapticTip1.transform.position;
            Debug.Log("Updated left cursor's reference pos");
        }
        if (cursorRight)
        {
            referencePosCursorRight = hCS.secondHapticCursor.transform.position;
            //referencePosCursorRight = hapticTip2.transform.position;
            Debug.Log("Updated right cursor's reference pos");
        }
    }


    /// <summary>
    /// Print haptic cursors' transform.position.g delta from their reference position
    /// </summary>
    /// <param name="g"> 0, 1, or 2. (i.e. x, y, or z) </param>
    private void PrintCursorPositionDelta(int g)
    {
        Debug.LogFormat("Time: {2}, delta(pos[{3}], refPoint[{3}]): cursor1: {0}, cursor2: {1}",
            /*Mathf.Abs*/(hCS.hapticCursor.transform.position[g] - referencePosCursorLeft[g]).ToString("F2"),
            ///*Mathf.Abs*/(hapticTip1.transform.position[g] - referencePosCursorLeft[g]).ToString("F2"),
            /*Mathf.Abs*/(hCS.secondHapticCursor.transform.position[g] - referencePosCursorRight[g]).ToString("F2"),
            ///*Mathf.Abs*/(hapticTip2.transform.position[g] - referencePosCursorRight[g]).ToString("F2"),
            Time.time.ToString("F1"), g);
    }

    private void PrintCursorPositionDelta()
    {
        //TODO Calculate / calibrate scale factor
        //Also: Later, incorporate scaling factor in the workspace dimensions and don't apply it here anymore
        float scaleFactor = 0.69f;
        //float deltaCursor1 = Vector3.Distance(hapticCursor.transform.position, referencePosCursorLeft);
        float deltaCursor1 = Vector3.Distance(hapticTip1.transform.position, referencePosCursorLeft) / scaleFactor;
        float deltaCursor2 = Vector3.Distance(hCS.secondHapticCursor.transform.position, referencePosCursorRight) / scaleFactor;
        //float deltaCursor2 = Vector3.Distance(hapticTip2.transform.position, referencePosCursorRight) / scaleFactor;
        Debug.LogFormat("Time: {2}, delta(pos, refPoint): cursor1: {0}, cursor2: {1}",
            deltaCursor1.ToString("F2"), deltaCursor2.ToString("F2"), Time.time.ToString("F1"));
    }
}
