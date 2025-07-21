using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoratorNode : Node
{
    public Node child;

    public DecoratorNode()
    {
        Type = NodeType.Decorator;
    }

}
