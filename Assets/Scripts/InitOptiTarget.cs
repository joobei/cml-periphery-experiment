using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitOptiTarget: MonoBehaviour {

    public Transform cursorOptiLeft;
    public Transform cursorOptiRight;
    public Transform optiTarget;

    private bool initialized = false;

	// Update is called once per frame
	void Update ()
    {
        if (!initialized && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            optiTarget.position = cursorOptiRight.position + (cursorOptiLeft.position - cursorOptiRight.position) / 2;
            optiTarget.rotation = Quaternion.identity; //aligned with world axes
            Debug.Log("Placed Optitrack target between Optitrack cursors.");
            initialized = true;
        }
    }
}
