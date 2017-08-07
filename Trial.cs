using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Trial
{
    public Vector3 from;
    public Vector3 to;

    public Trial(Vector3 from, Vector3 to)
    {
        this.from = from;
        this.to = to;
    }

    public override string ToString()
    {
        return "From " + from.ToString() + " to " + to.ToString();
    }
}
