using System.Collections.Generic;
using UnityEngine;

[NodeRunnerFor(typeof(ActionNode), nameof(ChaseNode))]
[NodeName(nameof(ChaseNode))]
public class ChaseNode : ActionNodeRunner
{
    public ChaseNode( Node node, Monster monster ) : base(node, monster)
    {
    }

    public override IEnumerator<NodeState> Execute()
    {
        var player = Physics2D.OverlapCircle(Monster.transform.position, Monster.monsterData.attackRange, Monster.playerLayer);

        if ( player == null )
        {
            yield return NodeState.True;
            yield break;
        }

        var targetDir = ( player.transform.position - Monster.transform.position ).normalized;
        Monster.transform.Translate(targetDir * Monster.monsterData.speed / 100f);

        yield return NodeState.Running;
    }


}
