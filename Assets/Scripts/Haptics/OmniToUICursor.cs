using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OmniToUICursor : MonoBehaviour
{

    public Transform cursor;
    public Transform OmniCursor;
    public float movementSpeed;

    private Vector3 OmniInitialPos;


    private void OnEnable()
    {
        OmniInitialPos = OmniCursor.localPosition * movementSpeed;
    }

    void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        Vector3 newPos = OmniCursor.localPosition;
        newPos *= movementSpeed;
        newPos -= OmniInitialPos;
        newPos.z = 0;
        cursor.localPosition = newPos;
    }
}
