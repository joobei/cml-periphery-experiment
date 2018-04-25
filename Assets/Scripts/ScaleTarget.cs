using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleTarget : MonoBehaviour {

    public GameObject[] transformations;

    private GenericFunctionsClass myGenericFunctionsClassScript;

    void Start()
    {
        myGenericFunctionsClassScript = transformations[0].GetComponent<ApplyTransformation>().myGenericFunctionsClassScript;
    }

    void Update () {
		if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            transform.localScale = transform.localScale + Vector3.one * 0.05f;
            ScaleHapticTargets();
        }
        else if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            transform.localScale = transform.localScale - Vector3.one * 0.05f;
            ScaleHapticTargets();
        }
    }

    private void ScaleHapticTargets()
    {
        foreach (GameObject gO in transformations)
        {
            float scaling = gO.GetComponent<CoordsysTransform>().scaling;
            ApplyTransformation aT = gO.GetComponent<ApplyTransformation>();
            aT.hapticTarget.localScale = transform.localScale * scaling;
        }
        myGenericFunctionsClassScript.UpdateHapticObjectMatrixTransform();
    }
}
