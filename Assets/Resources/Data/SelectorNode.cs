using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorNode : Node
{
    public List<Node> children;

    public SelectorNode()
    {
        Type = NodeType.Selector;
    }
}

