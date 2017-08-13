using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Trial
{
    public Vector3 start;
    public Vector3 end;
    public int startAngle, endAngle;

    public Transferfunction transferFunction;
    public float startDepth, endDepth;

    public Trial(float sDepth, float eDepth, int sAngle, int eAngle)
    {
        this.startAngle = sAngle;
        this.startDepth = sDepth;
        this.endAngle = eAngle;
        this.endDepth = eDepth;
        transferFunction = Transferfunction.closed;
    }

    public Trial(Trial trial)
    {
        this.start = new Vector3(trial.start.x,trial.start.y,trial.start.z);
        this.end = new Vector3(trial.end.x, trial.end.y, trial.end.z);
        this.startDepth = trial.startDepth;
        this.endDepth = trial.endDepth;
        this.startAngle = trial.startAngle;
        this.endAngle = trial.endAngle;
        this.transferFunction = (Transferfunction)((int)trial.transferFunction);
    }

    public override string ToString()
    {
        return "From " + start.ToString("F4") + " to " + end.ToString("F4") + "Transferfunction: " + transferFunction.ToString() ;
    }
}
