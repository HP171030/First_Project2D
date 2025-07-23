using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[NodeRunnerFor(typeof(DecoratorNode), nameof(InRangeCondition))]
[NodeName(nameof(InRangeCondition))]
public class InRangeCondition : DecoratorNodeRunner
{
    Monster _monster;

    //에디터에 해당 필드 이름으로 라벨을 표기함 / 실제 값은 사용자가 블랙보드에 등록한 키로 조회
    public float range;

    public InRangeCondition( Node node, Monster monster ) : base(node, monster)
    {
        _monster = monster;
    }
    public override bool CheckCondition()
    {
        Collider2D player = Physics2D.OverlapCircle(
             _monster.transform.position,
             _monster.monsterData.Blackboard.Get<float>(_Node.BlackBoardKey),
             _monster.playerLayer);

        if ( player != null )
        {
            _monster.SetTarget(player.gameObject);
            Debug.Log($"Range : {_monster.monsterData.Blackboard.Get<float>(_Node.BlackBoardKey)}");
            return true;
        }
        return false;
    }

}
