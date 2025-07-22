using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NodeRunnerFor(typeof(ActionNode),nameof(IdleNode))]
[NodeName(nameof(IdleNode))]
public class IdleNode : ActionNodeRunner
{
    public IdleNode( Node node, Monster monster ) : base(node, monster)
    {

    }

    public override IEnumerator<NodeState> Execute()
    {
        float elapsed = 0f;
        Debug.Log("Idle Start");
        while ( elapsed < Monster.monsterData.idleTime )
        {
            elapsed += Time.deltaTime;
            yield return NodeState.Running;
        }
        Debug.Log("Idle Exit");
        yield return NodeState.True;
        yield break;
    }

    
}
