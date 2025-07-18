using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionNodes
{

}


[NodeRunnerFor(typeof(SequenceNode))]
public class AttackNode : ActionNodeRunner
{
    private Monster _monster;

    public AttackNode( SequenceNode node, Monster monster ) : base(node, monster)
    {
        _monster = monster;
    }

    public override IEnumerator Execute()
    {
        _monster.MoveOn = false;

        if ( !_monster.atkDelayOn )
        {
            Vector2 prePos = _monster.transform.position;
            Vector2 targetPos = prePos + _monster.atkDir * 2f;
            float t = 0f;
            float duration = 0.5f;

            _monster.atkDelayOn = true;
            Manager.Sound.PlaySFX(_monster.monsterData.soundAttack);
            _monster.animator.Play("AttackMonster");

            while ( t < 1f )
            {
                _monster.transform.position = Vector2.Lerp(prePos, targetPos, t);
                t += Time.deltaTime / duration;
                yield return null;
            }

            Collider2D player = Physics2D.OverlapCircle(_monster.transform.position, _monster.monsterData.attackRange, _monster.playerLayer);
            if ( player != null )
            {
                SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
                Manager.Game.HpEvent -= _monster.monsterData.atk;
                Manager.Game.ShakeCam();
                spriteRenderer.material.color = Color.red;
                spriteRenderer.material.DOColor(Color.white, 1f);
                Manager.Sound.PlaySFX(_monster.monsterData.soundPlayerDamaged);
            }

            _monster.ChangeState(Monster.MonsterState.Idle);
            _monster.animator.SetBool("Move", false);
            yield return new WaitForSeconds(_monster.monsterData.atkDelay);
            _monster.atkDelayOn = false;
        }
    }


}

public class MoveNode : ActionNodeRunner
{
    object target;
    public MoveNode( SequenceNode node, Monster target ) : base(node, target) { this.target = target; }

    public override IEnumerator Execute()
    {
        throw new System.NotImplementedException();
    }
}


public class Node : ScriptableObject
{
    NodeState _state = NodeState.Default;
    Action<NodeState> onChangeNodeState;

    public NodeState State { get => _state; set => SetStatus(value); }
    public NodeType Type;
    public string guid = Guid.NewGuid().ToString();
    public Vector2 position = Vector2.zero;
    public List<string> ChildrenGUIDs = new();
    public string nodeName;

    public enum NodeState
    {
        Run,
        Fail,
        Success,
        Selected,
        Default
    }
    public enum NodeType
    {
        Selector,
        Sequence,
        Decorator
    }


    public void AddEventFunc( Action<NodeState> eventFunc )
    {
        onChangeNodeState += eventFunc;
    }
    public void RemoveEventFunc( Action<NodeState> eventFunc )
    {
        onChangeNodeState -= eventFunc;
    }
    public void SetStatus( Node.NodeState State )
    {
        _state = State;
        onChangeNodeState.Invoke(State);

    }

}
public class SequenceNode : Node
{
    public List<Node> children;
    public SequenceNode()
    {
        Type = NodeType.Sequence;
    }

}
public class DecoratorNode : Node
{
    public Node child;

    public DecoratorNode()
    {
        Type = NodeType.Decorator;
    }

}
public class SelectorNode : Node
{
    public List<Node> children;

    public SelectorNode()
    {
        Type = NodeType.Selector;
    }
}

[NodeRunnerFor(typeof(SequenceNode))]
public abstract class ActionNodeRunner : NodeRunner
{
    SequenceNode _node;
    public ActionNodeRunner( SequenceNode node, Monster monster ) : base(node)
    {
        _node = node;
    }
}

[NodeRunnerFor(typeof(ConditionNodeRunner))]
public abstract class ConditionNodeRunner : NodeRunner
{
    SelectorNode _node;
    public ConditionNodeRunner( SelectorNode node, Monster monster ) : base(node)
    {
        _node = node;
    }

}

public abstract class NodeRunner
{
    public Node NodeData { get; private set; }
    public List<NodeRunner> Children { get; private set; } = new();

    public NodeRunner( Node node )
    {
        NodeData = node;
    }

    public abstract IEnumerator Execute();
}
