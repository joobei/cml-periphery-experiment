/*  This file is part of Joutai.

    Joutai is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Joutai is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Joutai.  If not, see <http://www.gnu.org/licenses/>. 
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Joutai;

public class InstructionState : State
{
    [SerializeField]
    private String[] instructionTextArray;

    //this textIndex should ideally be
    //replaced by something more intelligent
    private short textIndex = 0;

    protected Text textPane;
    public GameObject canvas;

    public override void OnEnable()
    {
        base.OnEnable();
        GameObject mainCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0];
        canvas.SetActive(true);
        textPane = (UnityEngine.UI.Text)canvas.GetComponentInChildren(typeof(Text));
        if (instructionTextArray.Length > 0)
        {
            textPane.text = instructionTextArray[textIndex];
        }
        InputManager.OnMouseButton += LocalAdvance;
        HapticManager.OnStylusButton += LocalAdvance;
    }


    //advance to next text instruction
    private void LocalAdvance()
    {
        textIndex++;
        if (textIndex >= instructionTextArray.Length)
        {
            textIndex = 0;
            AdvanceState();
        }
        else
        {
            textPane.text = instructionTextArray[textIndex];
        }
    }

    GameObject CreateText(GameObject camera)
    {
        GameObject UICanvasGO = new GameObject("InstructionCanvas");

        RectTransform trans = UICanvasGO.AddComponent<RectTransform>();
        trans.anchoredPosition = new Vector2(0, 0);

        UICanvasGO.AddComponent<Canvas>();
        UICanvasGO.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        UICanvasGO.GetComponent<Canvas>().worldCamera = camera.GetComponent<Camera>();
        UICanvasGO.GetComponent<Canvas>().planeDistance = 100;

        UICanvasGO.AddComponent<CanvasScaler>();

        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(UICanvasGO.transform);
        textGO.AddComponent<RectTransform>();
        textGO.GetComponent<RectTransform>().position = Vector3.zero;
        textGO.GetComponent<RectTransform>().localPosition = Vector3.zero;
        textGO.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
        Text text = textGO.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        text.resizeTextForBestFit = true;


        return UICanvasGO;
    }

    private void OnDisable()
    {
        canvas.SetActive(false);
    }
}
