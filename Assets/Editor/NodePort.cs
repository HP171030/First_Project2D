using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NodePort : Port
{
    public NodePort( Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type ) : base(portOrientation, portDirection, portCapacity, type)
    {
    }

    
}
