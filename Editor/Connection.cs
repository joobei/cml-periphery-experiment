using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection {
    public Node From { get; private set; }
    public Node To { get; private set; }

    public Connection(Node from, Node to)
    {
        From = from;
        To = to;
    }

    public override bool Equals(object obj)
    {
        return obj is Connection
            && From.Equals(((Connection)obj).From)
            && To.Equals(((Connection)obj).To);
    }

    public override string ToString()
    {
        return From.ToString() + ",\n" + To.ToString();
    }
}
