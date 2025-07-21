using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeView : UnityEditor.Experimental.GraphView.Node, ISelectable
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


        SetPosition(new Rect(node.position.x,node.position.y, 200,150));

        input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(bool));
        output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
        
        input.portName = $"";
        output.portName = $"";


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
                SetBoarderColor(Color.cyan, 5);
                break;
            case Node.NodeState.Default:
                SetBoarderColor(Color.black);
                break;
            case Node.NodeState.Root:
                SetBoarderColor(Color.magenta);
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

public enum PortStatus
{

}




