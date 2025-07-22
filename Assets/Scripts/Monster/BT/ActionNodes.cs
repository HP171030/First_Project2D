using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
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
    [HideInInspector] public string nodeName;

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
[NodeRunnerFor(typeof(ConditionNode))]
public abstract class ConditionNodeRunner : LogicRunner
{
    protected Monster Monster;
    public ConditionNodeRunner( Node node, Monster monster ) : base(node, monster)
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
        Queue<NodeRunner> nodeRunnerQueue = new Queue<NodeRunner>();

        foreach ( Node childNode in Node.Children.Values )
        {
            NodeRunner childRunner = NodeRunnerFactory.Instance.Create(childNode, _monster); 
            if ( childRunner != null )
            {
                nodeRunnerQueue.Enqueue(childRunner);
            }
        }

        while ( nodeRunnerQueue.Count > 0 )
        {
            var currentRunner = nodeRunnerQueue.Dequeue();

            IEnumerator<NodeState> childExecution = currentRunner.Execute();

            while ( childExecution.MoveNext() )
            {
                switch ( childExecution.Current ) 
                {
                    case NodeState.True:
                        yield return NodeState.True;
                        yield break;
                    case NodeState.Fail:
                        break;
                    case NodeState.Running:
                        yield return NodeState.Running;
                        break;
                }
                yield return NodeState.Fail;
            }
        }

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
        Queue<NodeRunner> queue = new Queue<NodeRunner>();
        foreach(var child in Node.Children.Values )
        {
           var runner = NodeRunnerFactory.Instance.Create(child, _monster);
            if(runner != null)
                queue.Enqueue( runner );

            while(queue.Count > 0 )
            {
                var target = queue.Dequeue();
                Debug.Log($"{target.Node.nodeName} 시작");
                var evaluate = target.Execute();

                while ( evaluate.MoveNext() )
                {
                    switch ( evaluate.Current )
                    {
                        case NodeState.Fail :
                            Debug.Log($"{target.Node.nodeName} 실패");
                            yield return NodeState.Fail;
                            yield break;
                        case NodeState.True:
                            Debug.Log($"{target.Node.nodeName} 성공");
                            foreach ( var item in target.Node.Children.Values )
                            {
                                var childRunner = NodeRunnerFactory.Instance.Create(item, _monster);
                                queue.Enqueue(childRunner);
                            }
                            
                            break;
                        case NodeState.Running:
                            Debug.Log($"{target.Node.nodeName} 유지");
                            yield return NodeState.Running;
                            break;
                    }
                }
            }
        }

        yield return NodeState.Fail;
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
    public override IEnumerator<NodeState> Execute()
    {
        Debug.Log($"Decorator {Node.nodeName} Excute ");
        yield return NodeState.Fail;
    }

}
public abstract class NodeRunner
{
    public enum NodeState
    {
        True,
        Fail,
        Running
    }
    public Node Node { get; private set; }

    public NodeRunner(Node node )
    {
        Node = node;
    }

    public void ParseFieldToDictionary()
    {
/*        Node.blackboard.SetValue()*/
    }
    public abstract IEnumerator<NodeState> Execute();
}
