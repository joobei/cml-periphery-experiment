using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class NodeEditor : EditorWindow
{
    string fileName = "Trials";
    bool groupEnabled;
    bool isOrthographic = false;
    bool isOrthographicLast = false; //for comparison
    float myFloat = 1.23f;

    List<Node> nodes = new List<Node>();
    private float nodeWidth = 30;
    private float nodeHeight = 30;
    private List<int> angles = new List<int>() { -20, -10, 0, 10,20 };
    private List<float> depths = new List<float>() { 0.382f, 0.618f, 0.854f };
    List<Connection> connections = new List<Connection>();
    private GUIStyle nodeStyle;
    bool connecting = false;
    //To check if 
    public Vector2 tempDragPosition;
    private Node fromNode, toNode;
    private float zoomAmount = 1;

    private Vector2 origin;

    private void OnEnable()
    {
        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        nodeStyle.border = new RectOffset(12, 12, 12, 12);
        Vector3[,] positions = GeneratePositions(angles, depths);

        //where all the nodes are drawn from
        origin = new Vector2(EditorWindow.GetWindow(typeof(NodeEditor)).position.width / 2, EditorWindow.GetWindow(typeof(NodeEditor)).position.height * 0.95f);

        //offset of the last array element, i.e. the rightmost rect
        Vector2 maxOffset = Offset(new Vector2(0.4f * (angles.Count-1), 0.4f));
        Vector2 change = new Vector2(0.5f * (maxOffset.x - origin.x), 200);
        for (int i = 0; i < angles.Count; i++)
        {
            for (int j = 0; j < depths.Count; j++)
            {
                Vector2 nodePos = new Vector2(positions[i, j].x, positions[i, j].z);
                Vector2 nodePosOrth = new Vector2(0.4f * i, 0.4f * j);

                //                                v---- rect will be transformed for display but coordinates remain original
                Node tempNode = new Node(positions[i, j], Offset(nodePos), Offset(nodePosOrth) - change, nodeWidth, nodeHeight, nodeStyle);
                nodes.Add(tempNode);
            }
        }
    }

    private Vector2 Offset(Vector2 position)
    {
        position *= 300;  //to expand
        position.y *= -1;  //y axis is negative in GUI windows
        position += origin; //to bring to middle of window

        return position;
    }

    [MenuItem("Window/Node Editor")]
    public static void OpenWindow()
    {
        EditorWindow.GetWindow(typeof(NodeEditor)).Show();
    }

    public static Vector3[,] GeneratePositions(List<int> angles, List<float> depths)
    {
        Vector3[,] positions = new Vector3[angles.Count, depths.Count];

        //generate positions by looping through distance and depth arrays.
        foreach (int angle in angles)
        {
            foreach (float depth in depths)
            {
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


    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        fileName = EditorGUILayout.TextField("Serialization Filename", fileName);
        if (GUILayout.Button("Load from json"))
        {
            //TODO: Try to load connections from fileName.json
        }
        if (GUILayout.Button("Save to json"))
        {
            WriteTrialsToJson();
        }
        //TODO: editable list view of angles and depths
        //if (GUILayout.Button("Create nodes from angles and depths"))
        //{
        //    //TODO: create em
        //}

        groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        isOrthographic = EditorGUILayout.Toggle("Orthographic", isOrthographic);
        if (isOrthographic != isOrthographicLast) //has the selection changed?
        {
            isOrthographicLast = isOrthographic;
            Debug.Log("Switching between rects (orthographic: " + isOrthographic + ")");
            foreach (Node node in nodes)
            {
                node.SetRect(isOrthographic);
            }
        }
        myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup();

        GUIUtility.ScaleAroundPivot(new Vector2(zoomAmount,zoomAmount), origin);
        GUI.Box(new Rect(origin.x, origin.y, 40, 40), "Origin", nodeStyle);
        DrawNodes();
        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);

        if (connecting)
        {
            DrawNodeCurve(fromNode.rect, tempDragPosition);
        }

        if (GUI.changed)
            Repaint();
    }

    private void WriteTrialsToJson()
    {
        Trial[] trials = new Trial[connections.Count];
        for (int i = 0; i < connections.Count; i++)
        {
            //TODO: Fix this to also save eccentricity
            trials[i] = new Trial(connections[i].From.coordinates, connections[i].To.coordinates);
        }
        string json = JsonHelper.ToJson(trials, true);
        File.WriteAllText(Application.dataPath + "/../" + fileName + ".json", json);
        Debug.Log(json);
    }

    private void DrawNodes()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].Draw();
        }
        foreach (Connection conn in connections)
        {
            DrawNodeCurve(conn.From.rect, conn.To.rect);
        }
    }

    private void Zoom(Vector2 delta)
    {
        zoomAmount += delta.y/30;
        //Debug.Log("Delta amount: " + delta.x + " " + delta.y);
        Repaint();
    }

    private void ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.ScrollWheel:
                Zoom(e.delta);
                break;
            case EventType.MouseDown:
                if (e.button == 1)
                {   //menu to add node - not really used
                    //ProcessContextMenu(e.mousePosition);
                }
                else if (e.button == 0) //to create connections
                {
                    connecting = false;
                    fromNode = null;
                    foreach (Node node in nodes)
                    {
                        if (node.rect.Contains(e.mousePosition))
                        {
                            connecting = true;
                            fromNode = node;
                            tempDragPosition = e.mousePosition;
                            GUI.changed = true;
                        }
                    }
                    GUI.changed = true;
                }
                break;

            case EventType.MouseUp:
                connecting = false;
                foreach (Node node in nodes)
                {
                    if (node.rect.Contains(e.mousePosition))
                    {
                        Connection conn = new Connection(fromNode, node);
                        //only add if it's not already there and not self
                        if (fromNode != null && fromNode != node && !connections.Contains(conn))
                        {
                            connections.Add(conn);
                        }
                    }
                }
                GUI.changed = true;
                break;
            case EventType.MouseDrag:
                if (e.button == 0 && connecting && fromNode != null)
                {
                    tempDragPosition += e.delta;
                    e.Use();
                    GUI.changed = true;
                }
                break;
        }
    }


    private void ProcessNodeEvents(Event e)
    {
        if (nodes != null)
        {
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = nodes[i].ProcessEvents(e);

                if (guiChanged)
                {
                    GUI.changed = true;
                }
            }
        }
    }

    //private void ProcessContextMenu(Vector2 mousePosition)
    //{
    //    GenericMenu genericMenu = new GenericMenu();
    //    genericMenu.AddItem(new GUIContent("Add new Node"), false, () => OnClickAddNode(mousePosition));
    //    genericMenu.ShowAsContext();
    //}

    //while dragging
    void DrawNodeCurve(Rect start, Vector2 end)
    {
        Vector3 startPos = new Vector3(start.x + start.width / 2, start.y, 0);
        Vector3 endPos = new Vector3(end.x, end.y, 0);
        Vector3 startTan = startPos + Vector3.down * 50;
        Vector3 endTan = endPos + Vector3.up * 50;
        Color shadowCol = new Color(0, 0, 0, 0.06f);

        for (int i = 0; i < 3; i++)
        {// Draw a shadow
            Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
        }

        Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
    }

    //for established connections
    void DrawNodeCurve(Rect start, Rect end)
    {
        Vector3 startPos = new Vector3(start.x + start.width / 2, start.y, 0);
        Vector3 endPos = new Vector3(end.x + end.width / 2, end.y + end.height, 0);
        Vector3 startTan = startPos + Vector3.down * 50;
        Vector3 endTan = endPos + Vector3.up * 50;
        Color shadowCol = new Color(0, 0, 0, 0.06f);

        for (int i = 0; i < 3; i++)
        {// Draw a shadow
            Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
        }

        Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
    }
}
