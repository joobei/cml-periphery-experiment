using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class InputManager : MonoBehaviour
{
    public delegate void MouseAction();
    public static event MouseAction OnMouseButton;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (OnMouseButton != null)
                OnMouseButton();
        }
    }
}