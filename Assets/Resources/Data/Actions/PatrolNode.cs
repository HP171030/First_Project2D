using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[NodeRunnerFor(typeof(ActionNode), nameof(PatrolNode))]
[NodeName(nameof(PatrolNode))]
public class PatrolNode : ActionNodeRunner
{
    public PatrolNode( Node node, Monster monster ) : base(node, monster)
    {
        Monster = monster;
    }

    public override IEnumerator<NodeState> Execute()
    {
        yield return NodeState.Fail;
    }
}
