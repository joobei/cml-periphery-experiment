using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum Transferfunction
{
    open,
    closed,
    visuoHaptic
};
public struct Translation
{
    public Vector3 from;
    public Vector3 to;
}

public struct Trial
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
        return positions;
    }

    public static List<Trial> generateTrials(Vector3[,] positions) {
        List<Trial> trials = new List<Trial>();

        //if (positions.GetLength(0) % 2 == 0) { 
        //TODO: Make this fail proof
        int startColumn = positions.GetLength(0)/2;
        //}

        var transferFunctions = Enum.GetValues(typeof(Transferfunction));
        foreach (Transferfunction transferFunction in transferFunctions) { 
            for (int x =0; x<positions.GetLength(0); ++x)
            {
                for (int y = 0; y < positions.GetLength(1); ++y)
                {
                    if (x!=startColumn)
                    {
                        Trial newTrial = new Trial();
                        newTrial.translation.from = positions[x, startColumn];
                        newTrial.translation.to = positions[x, y];
                        newTrial.transferFunction = transferFunction;
                        trials.Add(newTrial);
                    }
                }
            }
        }

        return trials;
    }
}