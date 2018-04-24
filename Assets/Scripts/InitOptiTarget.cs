using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitOptiTarget: MonoBehaviour {

    public Transform cursorOptiLeft;
    public Transform cursorOptiRight;
    public Transform optiTarget;

	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            optiTarget.position = cursorOptiRight.position + (cursorOptiLeft.position - cursorOptiRight.position) / 2;
            optiTarget.GetComponent<MeshRenderer>().enabled = false;
            Debug.Log("Placed Optitrack target between Optitrack cursors.");
        }
    }
}
