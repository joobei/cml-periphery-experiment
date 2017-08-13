using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class Node
{
    public Rect rect; //points to the rect that is currently used (std or orthographic)
    Rect rectStd;
    Rect rectOrth;
    public Vector3 coordinates;
    public int angle;
    public float depth;
    
	public string title;
	public GUIStyle style;
	public bool isDragged;

	public Node(Vector3 coordinates, Vector2 rectPosition, Vector2 rectPositionOrth, float width, float height, GUIStyle nodeStyle, int angle, float depth)
	{
        rect = rectStd = new Rect(rectPosition.x, rectPosition.y, width, height);
        this.coordinates = coordinates;
        rectOrth = new Rect(rectPositionOrth.x, rectPositionOrth.y, width, height);
        style = nodeStyle;
        this.angle = angle;
        this.depth = depth;
	}

	public void Drag(Vector2 delta)
	{
		rect.position += delta;
	}

	public void Draw()
	{
        GUI.Box(rect, title, style);
    }

	public bool ProcessEvents(Event e)
	{
		return false;
	}

    public override string ToString()
    {
        return rect.ToString();
    }

    public void SetRect(bool isOrthographic)
    {
        if (isOrthographic)
            rect = rectOrth;
        else
            rect = rectStd;
    }
}
