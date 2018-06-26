using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleTarget : MonoBehaviour {

    public GameObject[] transformations;


    public enum Scalings
    {
        three = 3, five = 5, seven = 7
    }
    [Space(20)]
    [Header("Scaling")]
    public float largest;
    public float smallest;

    [Tooltip("How many different scalings? ODD values only")]
    public Scalings noOfScalings;
    [Tooltip("The scale factor")]
    public float interval = 0.05f;

    private int scalings;
    private int level;

    //Placeholder value; will hold the initial, calibrated scaling
    //after call to StoreInitialScaling()
    private Vector3 vInitial = Vector3.forward;

    void Start()
    {
        scalings = (int)noOfScalings;
        if ((scalings % 2) == 0) //Only allow odd values
            noOfScalings++;
        level = (scalings - 1) / 2; //Start "in the middle"
    }

    public void StoreInitialScaling()
    {
        if (this.vInitial == Vector3.forward)
            this.vInitial = transform.localScale;
    }

    public void ScaleTargetsUp()
    {
        transform.localScale = transform.localScale + Vector3.one * interval;
        ScaleHapticTargets();
    }

    public void ScaleTargetsDown()
    {
        transform.localScale = transform.localScale - Vector3.one * interval;
        ScaleHapticTargets();
    }
    /// <summary>
    /// Scales the targets up.
    /// After the biggest scaling is applied
    /// to the targets, next time,
    /// the smallest will be applied.
    /// <para/>If, for instance, noOfScalings = 5,
    /// we have scalings #0 to #4 with 
    /// the initial scaling being #2
    /// (in the middle).
    /// #3 and #4 are larger scalings
    /// and #1 and #0 smaller scalings
    /// than the initial scaling #2.
    /// </summary>
    public void NextScale()
    {
        level++;
        level %= scalings;
        int factor = level - (scalings - 1) / 2;
        Debug.Log("LeveL: " + level + ", factor: " + factor);
        transform.localScale = vInitial + factor * Vector3.one * interval;
        ScaleHapticTargets();
    }

    private void ScaleHapticTargets()
    {
        foreach (GameObject gO in transformations)
        {
            float hapticScaleFactor = gO.GetComponent<CoordsysTransform>().scaling;
            ApplyTransformation aT = gO.GetComponent<ApplyTransformation>();
            aT.targetTo.localScale = transform.localScale * hapticScaleFactor;
        }
        myGenericFunctionsClassScript.UpdateHapticObjectMatrixTransform();
    }
}
