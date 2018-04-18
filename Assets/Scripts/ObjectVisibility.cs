using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectVisibility : MonoBehaviour {

    public GameObject optiCursorLeft;
    public GameObject optiCursorRight;
    public GameObject unityTarget;

    public GameObject hapticCursorLeft;
    public GameObject hapticCursorRight;
    public GameObject hapticTargetLeft;
    public GameObject hapticTargetRight;

    public GameObject inkwellLeft;
    public GameObject inkwellRight;

    // Update is called once per frame
    void Update () {
        //Optitrack (Unity)
        if (Input.GetKey(KeyCode.O))
        {
            Debug.Log("O");
            if (Input.GetKeyDown(KeyCode.L))
            {
                Debug.Log("L");
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
