using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    public Action<NodeView> onSelectedNode;
    public Action<NodeView> onUnSelectedNode;
    public Node node;
    public Port input;
    public Port output;

    public NodeView NodeParent
    {
        get
        {
            using ( IEnumerator<Edge> iter = input.connections.GetEnumerator() )
            {
                iter.MoveNext();
                return iter.Current?.output.node as NodeView;
            }
        }
    }

    public List<NodeView> NodeChildren
    {
        get
        {
            List<NodeView> children = new List<NodeView>();
            foreach ( var edge in output.connections )
            {
                NodeView child = edge.output.node as NodeView;
                if ( child != null )
                {
                    children.Add(child);
                }
            }
            return children;
        }
    }


    public NodeView( Node node ) : base()
    {
        AddToClassList("node-default");
        this.node = node;
        viewDataKey = node.guid;

        node.AddEventFunc(SetColorByState);

        style.left = node.position.x;
        style.top = node.position.y;
        style.minWidth = new StyleLength(StyleKeyword.Auto);
        style.minHeight = new StyleLength(StyleKeyword.Auto);
        input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
        input.portColor = SetPortColor(input.connected);
        output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
        output.portName = $"{output.connected}";
        output.portColor = SetPortColor(output.connected);

        CreateInputPorts();
        CreateOutputPorts();
        SetupClasses();
        SetupDataBinding();
        RefreshExpandedState();


        AddEvent();
    }
    void AddEvent()
    {

        this.AddManipulator(new DoubleClickOnNode());


        onSelectedNode = ( nodeView ) =>
        {
            node.SetStatus(Node.NodeState.Selected);
        };
        onUnSelectedNode = ( nodeView ) =>
        {
            node.SetStatus(Node.NodeState.Default);
        };
    }
    public override void OnSelected()
    {
        base.OnSelected();
        if ( onSelectedNode != null )
        {
            onSelectedNode.Invoke(this);
            Debug.Log($"Selected node: {this.node.guid}");
        }
    }
    public override void OnUnselected()
    {
        base.OnUnselected();
        onUnSelectedNode.Invoke(this);

    }
    public void SetNodeName( string nodeName )
    {
        title = nodeName;
    }
    Color SetPortColor( bool connect )
    {
        if ( connect )
        {
            return Color.green;
        }
        else
        {
            return Color.yellow;
        }
    }
    public void SetContainerColor( Color color )
    {
        style.backgroundColor = color;
    }
    public void SetBoarderColor( Color color, int width = 2 )
    {
        style.borderTopColor = color;
        style.borderBottomColor = color;
        style.borderLeftColor = color;
        style.borderRightColor = color;
        style.borderTopWidth = width;
        style.borderBottomWidth = width;
        style.borderLeftWidth = width;
        style.borderRightWidth = width;
    }
    void SetColorByState( Node.NodeState state )
    {
        switch ( state )
        {
            case Node.NodeState.Run:
                SetBoarderColor(Color.green);
                break;
            case Node.NodeState.Fail:
                SetBoarderColor(Color.red);
                break;
            case Node.NodeState.Success:
                SetBoarderColor(Color.blue);
                break;
            case Node.NodeState.Selected:
                SetBoarderColor(Color.cyan, 10);
                break;
            case Node.NodeState.Default:
                SetBoarderColor(Color.black);
                break;
        }
    }
    void UpdateNodeData()
    {

    }
    private void SetupDataBinding()
    {

    }

    private void SetupClasses()
    {

    }

    private void CreateOutputPorts()
    {
        outputContainer.Add(output);

    }

    private void CreateInputPorts()
    {
        inputContainer.Add(input);
    }
}

[System.Serializable]
public class Node
{

    public enum NodeState
    {
        Run,
        Fail,
        Success,
        Selected,
        Default
    }
    public NodeState State { get => _state; set => SetStatus(value); }
    NodeState _state;
    public string guid;
    public Vector2 position;
    public List<Node> children;

    Action<NodeState> onChangeNodeState;

    public Node()
    {
        _state = NodeState.Default;
        guid = GUID.Generate().ToString();
        position = Vector2.zero;
        children = new List<Node>();



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

public enum PortStatus
{

}
public class DecoratorNode : Node
{
    public Node child;
}

public class RootNode : Node
{
    public Node child;
}
public class CompositeNode : Node
{
    public Node child;
}

public abstract class ActionNode : Node { }
public abstract class ConditionNode : Node { }

