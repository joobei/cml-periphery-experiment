using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectVisibility : MonoBehaviour {

    public GameObject optiCursorLeft;
    public GameObject optiCursorRight;
    public GameObject unityTarget;

    public GameObject hapticCursorLeft;
    private MeshRenderer hCLMR;

    public GameObject hapticCursorRight;
    private MeshRenderer hCRMR;

    public GameObject hapticTargetLeft;
    private MeshRenderer hTLMR;

    public GameObject hapticTargetRight;
    private MeshRenderer hTRMR;

    public GameObject inkwellLeft;
    private MeshRenderer iLMR;

    public GameObject inkwellRight;
    private MeshRenderer iRMR;

    public MeshRenderer workspaceLeft;
    public MeshRenderer workspaceRight;

    void Start()
    {
        hCLMR = hapticCursorLeft.GetComponent<MeshRenderer>();
        hCRMR = hapticCursorRight.GetComponent<MeshRenderer>();
        hTLMR = hapticTargetLeft.GetComponent<MeshRenderer>();
        hTRMR = hapticTargetRight.GetComponent<MeshRenderer>();
        iLMR = inkwellLeft.GetComponent<MeshRenderer>();
        iRMR = inkwellRight.GetComponent<MeshRenderer>();
    }

    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            ToggleVisibilityHaptics();
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleVisibilityInkwells();
        }
        //else if (Input.GetKeyDown(KeyCode.Backspace))
        //{
        //    ToggleObjectsAll();
        //}
    }

    //Toggle MeshRenderer component of haptic objects
    public void ToggleVisibilityHaptics()
    {
        //First, make the states consistent
        hCRMR.enabled = hCLMR.enabled;
        hTLMR.enabled = hCLMR.enabled;
        hTRMR.enabled = hCLMR.enabled;
        workspaceLeft.enabled = hCLMR.enabled;
        workspaceRight.enabled = hCLMR.enabled;

        hCLMR.enabled = !hCLMR.enabled;
        hCRMR.enabled = !hCRMR.enabled;
        hTLMR.enabled = !hTLMR.enabled;
        hTRMR.enabled = !hTRMR.enabled;
        workspaceLeft.enabled = !workspaceLeft.enabled;
        workspaceRight.enabled = !workspaceRight.enabled;
        Debug.Log("Turned haptic visibility " + hTRMR.enabled);
    }

    //Toggle MeshRenderer component of inkwells
    public void ToggleVisibilityInkwells()
    {
        iLMR.enabled = iRMR.enabled;

        iLMR.enabled = !iLMR.enabled;
        iRMR.enabled = !iRMR.enabled;
        Debug.Log("Turned inkwell visibility " + iLMR.enabled);
    }

    public void ToggleObjectsAll()
    {
        hapticCursorLeft.SetActive(!hapticCursorLeft.activeSelf);
        hapticTargetLeft.SetActive(!hapticTargetLeft.activeSelf);
        hapticCursorRight.SetActive(!hapticCursorRight.activeSelf);
        hapticTargetRight.SetActive(!hapticTargetRight.activeSelf);
        inkwellLeft.SetActive(!inkwellLeft.activeSelf);
        inkwellRight.SetActive(!inkwellRight.activeSelf);
    }

    //Toggle gameobjects based on key combinations
    public void ToggleObjects()
    {
        //Optitrack (Unity)
        if (Input.GetKey(KeyCode.O))
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                optiCursorLeft.SetActive(!optiCursorLeft.activeSelf);
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                optiCursorRight.SetActive(!optiCursorRight.activeSelf);
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                unityTarget.SetActive(!unityTarget.activeSelf);
            }
        }
        //Haptic
        else if (Input.GetKey(KeyCode.H))
        {
            if (Input.GetKey(KeyCode.L))
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    hapticCursorLeft.SetActive(!hapticCursorLeft.activeSelf);
                }
                else if (Input.GetKeyDown(KeyCode.T))
                {
                    hapticTargetLeft.SetActive(!hapticTargetLeft.activeSelf);
                }
            }
            else if (Input.GetKey(KeyCode.R))
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    hapticCursorRight.SetActive(!hapticCursorRight.activeSelf);
                }
                else if (Input.GetKeyDown(KeyCode.T))
                {
                    hapticTargetRight.SetActive(!hapticTargetRight.activeSelf);
                }
            }
        }
        //Inkwell
        else if (Input.GetKey(KeyCode.I))
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                inkwellLeft.SetActive(!inkwellLeft.activeSelf);
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                inkwellRight.SetActive(!inkwellRight.activeSelf);
            }
        }
    }
}
