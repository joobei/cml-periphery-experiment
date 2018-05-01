using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownState : ExperimentState
{
    public String[] introTextArray;
    short textIndex = 0;
    public GameObject canvasObject;
    public Text textPane;
    float countDownTimer = 1f;

    public override void OnEnable()
    {
        base.OnEnable();
        textPane.text = introTextArray[0];
        textPane.fontSize = 33;
    }

    protected override void Update()
    {
        countDownTimer -= Time.deltaTime;

        if (countDownTimer <0) {
            textIndex++;
            if (textIndex >= introTextArray.Length)
            {
                textIndex = 0;
                textPane.fontSize = 13;
                advanceState();
            }
            else {
                textPane.text = introTextArray[textIndex];
                countDownTimer = 1f;
            }
        }
    }

    protected override void triggerPressed()
    {
        Debug.Log("Wait for it");
    }
}
