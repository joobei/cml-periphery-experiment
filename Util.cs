using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum DockingStateType
{
    toStart,
    toEnd
};

public enum Transferfunction
{
    open,
    closed
    //visuoHaptic
};

public struct Translation
{
    public Vector3 start;
    public Vector3 end;
}

public struct TrialNico
{
    public Translation translation;
    public Transferfunction transferFunction;
}

public static class Util
{
    public static Vector3[,] generatePositions(List<int> angles, List<float> depths)
    {
        Vector3[,] positions = new Vector3[angles.Count,depths.Count];
        
        //generate positions by looping throuh distance and depth arrays.
        foreach (int angle in angles) {
            foreach (float depth in depths) {
                Vector3 target;
                //create point along Z axis
                target = new Vector3(0, 0, depth);
                //rotate point by angle about Y axis
                target = Quaternion.AngleAxis(angle, new Vector3(0, 1, 0)) * target;
                //put vector in correct position in 2D array
                positions[angles.IndexOf(angle), depths.IndexOf(depth)] = target;
            }
        }
        Debug.Log("Positions Generated: " + positions.Length);
        return positions;
    }

    public static List<TrialNico> generateTrials(Vector3[,] positions)
    {
        List<TrialNico> trials = new List<TrialNico>();

        var transferFunctions = Enum.GetValues(typeof(Transferfunction));
        foreach (Transferfunction transferFunction in transferFunctions)
        {
            for (int x = 0; x < positions.GetLength(0); ++x)
            {
                for (int y = positions.GetLength(1); y >= 0; --y)
                {
                    Debug.Log("x: " + x + ", y: " + y);
                    TrialNico newTrial = new TrialNico();
                    newTrial.translation.end = positions[x, y];
                    newTrial.translation.start = positions[x, y];
                    trials.Add(newTrial);
                }
            }
        }
        Debug.Log("Trials Generated: " + trials.Count);
        return trials;
    }

    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}