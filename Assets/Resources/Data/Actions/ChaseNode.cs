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
        Debug.Log("Chase");
        var player = Physics2D.OverlapCircle(Monster.transform.position, Monster.monsterData.Blackboard.Get<float>("Chase"), Monster.playerLayer);

        if ( player == null )
        {
            yield return NodeState.Success;
            yield break;
        }


        var elapsed = 0f;
        var duration = Monster.monsterData.chaseDuration;
        var startPos = Monster.transform.position;
        var targetPos = player.transform.position;

        while ( elapsed < duration )
        {
            float t = elapsed / duration;
            Monster.transform.position = Vector2.Lerp(startPos, targetPos, t);
            elapsed += Time.deltaTime;
            yield return NodeState.Running;
        }

        yield return NodeState.Success;
        yield break;
    }


}
