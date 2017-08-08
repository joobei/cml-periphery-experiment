using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Trial
{
    public Vector3 start;
    public Vector3 end;

    public Trial(Vector3 start, Vector3 end)
    {
        this.start = start;
        this.end = end;
    }

    public override string ToString()
    {
        return "From " + start.ToString() + " to " + end.ToString();
    }
}
