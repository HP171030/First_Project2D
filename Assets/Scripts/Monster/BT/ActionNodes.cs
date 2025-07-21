using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionNodes
{

}


public class AttackNode
{
    private Monster _monster;

    /*    public AttackNode( SequenceNode node, Monster monster ) : base(node, monster)
        {
            _monster = monster;
        }*/

    /*    public override IEnumerator Execute()
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
        }*/


}


public class Node : ScriptableObject
{
    NodeState _state = NodeState.Default;
    Action<NodeState> onChangeNodeState;

    [HideInInspector] public NodeState State { get => _state; set => SetStatus(value); }
    [HideInInspector] public NodeType Type;
    [HideInInspector] public string guid = Guid.NewGuid().ToString();
    [HideInInspector] public Vector2 position = Vector2.zero;
    [HideInInspector] public Dictionary<string, Node> Children = new();
    [HideInInspector] public string nodeName;

    public enum NodeState
    {
        Run,
        Fail,
        Success,
        Selected,
        Default,
        Root
    }
    public enum NodeType
    {
        Selector,
        Sequence,
        Decorator,
        Condition,
        Action
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

public abstract class FlowNodeRunner : NodeRunner
{
    Node _node;
    public FlowNodeRunner( Node node, Monster monster ) : base(node)
    {
        _node = node;
    }
}
public abstract class LogicRunner : NodeRunner
{
    Node _node;
    public LogicRunner( Node node, Monster monster ) : base(node)
    {
        _node = node;
    }
}
[NodeRunnerFor(typeof(ActionNode))]
public class ActionNodeRunner : LogicRunner
{
    Node _node;
    public ActionNodeRunner( Node node, Monster monster ) : base(node, monster)
    {
        _node = node;
    }
    public override IEnumerator<bool> Execute()
    {
        Debug.Log($"Action {_node.nodeName} Excute ");
        yield return false;
    }
}
[NodeRunnerFor(typeof(ConditionNode))]
public class ConditionNodeRunner : LogicRunner
{
    Node _node;
    public ConditionNodeRunner( Node node, Monster monster ) : base(node, monster)
    {
        _node = node;
    }
    public override IEnumerator<bool> Execute()
    {
        Debug.Log($"Condition {_node.nodeName} Excute ");
        yield return false;
    }
}
[NodeRunnerFor(typeof(SelectorNode))]
public class SelectorNodeRunner : FlowNodeRunner
{
    Node _node;
    public SelectorNodeRunner( Node node, Monster monster ) : base(node, monster)
    {
        _node = node;
    }
    public override IEnumerator<bool> Execute()
    {
        Debug.Log($"Selector {_node.nodeName} Excute ");
        yield return false;
    }
}
[NodeRunnerFor(typeof(SequenceNode))]
public class SequenceRunner : FlowNodeRunner
{
    Node _node;
    public SequenceRunner( Node node, Monster monster ) : base(node, monster)
    {
        _node = node;
    }
    public override IEnumerator<bool> Execute()
    {
        Debug.Log($"Sequence {_node.nodeName} Excute ");
        yield return false;
    }

}
[NodeRunnerFor(typeof(DecoratorNode))]
public class DecoratorNodeRunner : FlowNodeRunner
{
    Node _node;
    public DecoratorNodeRunner( Node node, Monster monster ) : base(node, monster)
    {
        _node = node;
    }
    public override IEnumerator<bool> Execute()
    {
        Debug.Log($"Decorator {_node.nodeName} Excute ");
        yield return false;
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

    public abstract IEnumerator<bool> Execute();
}
