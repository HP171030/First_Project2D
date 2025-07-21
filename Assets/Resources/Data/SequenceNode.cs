using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceNode : Node
{
    public List<Node> children;
    public SequenceNode()
    {
        Type = NodeType.Sequence;
    }

}
