using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICursor : MonoBehaviour {

    public Transform cursor;
    public Transform optitrackCursor;
    public float movementSpeed;

    private Vector3 optitrackInitialPos;


    private void OnEnable()
    {
        optitrackInitialPos = optitrackCursor.localPosition * movementSpeed;
    }

    void Update ()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        Vector3 newPos = optitrackCursor.localPosition;
        newPos *= movementSpeed;
        newPos -= optitrackInitialPos;
        newPos.x = -newPos.z;
        newPos.z = 0;
        cursor.localPosition = newPos;
    }
}
