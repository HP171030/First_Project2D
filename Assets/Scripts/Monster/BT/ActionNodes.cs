using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionNodes
{

}


public class Node : ScriptableObject
{
    NodeViewState _state = NodeViewState.Default;
    Action<NodeViewState> onChangeNodeState;

    [HideInInspector] public NodeViewState State { get => _state; set => SetStatus(value); }
    [HideInInspector] public NodeType Type;
    [HideInInspector] public string guid = Guid.NewGuid().ToString();
    [HideInInspector] public Vector2 position = Vector2.zero;
    public string nodeName;

    [SerializedDictionary]
    public SerializedDictionary<string, Node> Children = new();

    public string BlackBoardKey;

    public string RunnerType;


    public enum NodeViewState
    {
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


    public IEnumerable<Node> DFSProcess(bool includeSelf)
    {
        if(includeSelf)
        yield return this;

        foreach ( var child in this.GetSortedChildren() )
        {

            foreach ( var node in child.DFSProcess(true) )
                yield return node;
        }
    }




    public void AddEventFunc( Action<NodeViewState> eventFunc )
    {
        onChangeNodeState += eventFunc;
    }
    public void RemoveEventFunc( Action<NodeViewState> eventFunc )
    {
        onChangeNodeState -= eventFunc;
    }
    public void SetStatus( Node.NodeViewState State )
    {
        _state = State;
        onChangeNodeState.Invoke(State);

    }

}

public abstract class FlowNodeRunner : NodeRunner
{
    public FlowNodeRunner( Node node, Monster monster ) : base(node)
    {
    }
}
public abstract class LogicRunner : NodeRunner
{
    public LogicRunner( Node node, Monster monster ) : base(node)
    {
    }
}

public abstract class ActionNodeRunner : LogicRunner
{
    protected Monster Monster;
    public ActionNodeRunner( Node node, Monster monster ) : base(node, monster)
    {
        Monster = monster;
    }
}

[NodeRunnerFor(typeof(SelectorNode))]
public class SelectorNodeRunner : FlowNodeRunner
{
    Monster _monster;
    public SelectorNodeRunner( Node node, Monster monster ) : base(node, monster)
    {
        _monster = monster;
    }
    public override IEnumerator<NodeState> Execute()
    {


        foreach ( var node in _Node.GetSortedChildren())
        {
            Debug.Log($"{node.nodeName} : 셀렉터 시작");
            var runner = NodeRunnerFactory.Instance.Create(node, _monster);
            var eval = runner.Execute();

            while ( eval.MoveNext() )
            {
                var state = eval.Current;
                if ( state == NodeState.Success )
                {
                    Debug.Log($"{node.nodeName} 셀렉터 성공");
                    yield return NodeState.Success;
                    yield break;
                }
                else if ( state == NodeState.Running )
                {
                    Debug.Log($"{node.nodeName} 셀렉터 러닝");
                    yield return NodeState.Running;
                }
                else if ( state == NodeState.Fail )
                {
                    Debug.Log($"{node.nodeName} 셀렉터 실패");
                    break;
                }
            }
        }

        Debug.Log("셀렉터 최종 실패");
        yield return NodeState.Fail;
    }

}
[NodeRunnerFor(typeof(SequenceNode))]
public class SequenceRunner : FlowNodeRunner
{
    Monster _monster;

    public SequenceRunner( Node node, Monster monster ) : base(node, monster)
    {
        _monster = monster;
    }
    public override IEnumerator<NodeState> Execute()
    {
        Debug.Log("시퀀스 시작");

        foreach ( var node in _Node.GetSortedChildren() )
        {
            var runner = NodeRunnerFactory.Instance.Create(node, _monster);
            var eval = runner.Execute();

            while ( eval.MoveNext() )
            {
                var state = eval.Current;
                if ( state == NodeState.Success )
                {
                    Debug.Log($"{node.nodeName} 시퀀스 성공");
                    break;
                    
                }
                else if ( state == NodeState.Running )
                {
                    Debug.Log($"{node.nodeName} 시퀀스 러닝");
                    yield return NodeState.Running;
                }
                else if ( state == NodeState.Fail )
                {
                    Debug.Log($"{node.nodeName} 시퀀스 실패");
                    yield return NodeState.Fail;
                    yield break;
                    
                }
            }
        }

        Debug.Log("시퀀스 최종 성공");
        yield return NodeState.Success;
    }

}
[NodeRunnerFor(typeof(DecoratorNode))]
public class DecoratorNodeRunner : FlowNodeRunner
{
    Monster _monster;
    public DecoratorNodeRunner( Node node, Monster monster ) : base(node, monster)
    {
        
        _monster = monster;
    }
    public virtual bool CheckCondition()
    {
        Debug.Log("Check");
        return true;
    }
    public override IEnumerator<NodeState> Execute()
    {
        
        if ( !CheckCondition() )
        {
            Debug.Log($"{_Node.nodeName} : 데코 실패");
            yield return NodeState.Fail;
            yield break;
        }   

        //밑에 또 뭐 있으면 없으면 반환
        if ( _Node.Children.Count == 0 )
        {
            yield return NodeState.Success;
            yield break;
        }

        Debug.Log("데코레이터 컨디션 통과");
        //있으면 실행
        var childRunner = NodeRunnerFactory.Instance.Create(_Node.Children.First().Value, _monster);

        if ( childRunner == null )
        {
            yield return NodeState.Fail;
            yield break;
            throw new System.Exception($"올바르지 않은 노드형식 : {_Node.nodeName}");
        }

        var childExecute = childRunner.Execute();
        while ( childExecute.MoveNext() )
        {
            yield return childExecute.Current;
        }
    }

}
public abstract class NodeRunner
{
    public enum NodeState
    {
        Success,
        Fail,
        Running
    }
    public Node _Node { get; private set; }

    public NodeRunner( Node node )
    {
        _Node = node;
    }

    public void ParseFieldToDictionary()
    {
        /*        Node.blackboard.SetValue()*/
    }
    public abstract IEnumerator<NodeState> Execute();
}
