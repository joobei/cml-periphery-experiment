using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Trial
{
    public Vector3 start;
    public Vector3 end;

    public Transferfunction transferFunction;

    public Trial(Vector3 start, Vector3 end)
    {
        this.start = start;
        this.end = end;
    }

    public Trial(Trial trial)
    {
        this.start = new Vector3(trial.start.x,trial.start.y,trial.start.z);
        this.end = new Vector3(trial.end.x, trial.end.y, trial.end.z);
        this.transferFunction = (Transferfunction)((int)trial.transferFunction);
    }

    public override string ToString()
    {
        return "From " + start.ToString("F4") + " to " + end.ToString("F4") + "Transferfunction: " + transferFunction.ToString() ;
    }
}
