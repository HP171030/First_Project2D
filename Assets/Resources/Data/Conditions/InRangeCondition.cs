using System.Collections.Generic;
using UnityEngine;

[NodeRunnerFor(typeof(ConditionNode), nameof(InRangeCondition))]
[NodeName(nameof(InRangeCondition))]
public class InRangeCondition : ConditionNodeRunner
{
    Monster _monster;

    public float range;

    public InRangeCondition( Node node, Monster monster ) : base(node, monster)
    {


        _monster = monster;
    }
    public override IEnumerator<NodeState> Execute()
    {
        Debug.Log($"Range : {_monster.monsterData.Blackboard.Get<float>(nameof(range))}");
        Collider2D player = Physics2D.OverlapCircle(_monster.transform.position, _monster.monsterData.Blackboard.Get<float>(nameof(range)), _monster.playerLayer);
        if ( player != null )
        {
            Debug.Log("InRange True");
            yield return NodeState.True;
            yield break;
        }
        yield return NodeState.Fail;
    }

}
