using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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

    GraphView graphPanel;

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


    public NodeView( Node node ,GraphView graphPanel) : base()
    {
        AddToClassList("node-default");
        this.node = node;
        viewDataKey = node.guid;
        this.graphPanel = graphPanel;
        node.AddEventFunc(SetColorViewByState);


        SetPosition(new Rect(node.position.x,node.position.y, 200,150));

        input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(bool));
        output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
        
        input.portName = $"";
        output.portName = $"";



        CreateInputPorts();
        CreateOutputPorts();
        SetupDataBinding();
        RefreshExpandedState();


        AddEvent();
    }

    void AddEvent()
    {

        //TODO : 노드 더블클릭시
        //this.AddManipulator(new DoubleClickOnNode());

        if ( titleContainer != null )
        {
            var titleLabel = titleContainer.Q<Label>("title-label");
            if ( titleLabel != null )
            {
                titleContainer.AddManipulator(new DoubleClickOnNode(() =>
                {
                    StartRename(titleLabel);
                }));
            }
        }



        onSelectedNode = ( nodeView ) =>
        {
            node.SetStatus(Node.NodeViewState.Selected);
        };
        onUnSelectedNode = ( nodeView ) =>
        {
            node.SetStatus(Node.NodeViewState.Default);
        };
    }
    void StartRename( Label label )
    {
        var textField = new TextField { value = label.text };
        textField.style.flexGrow = 1;
        textField.RegisterCallback<FocusOutEvent>(( _ ) =>
        {
            label.text = textField.value;
            label.style.display = DisplayStyle.Flex;
            SetNodeName(label.text);
            textField.RemoveFromHierarchy();
            graphPanel.ClearSelection();
        });

        textField.RegisterCallback<KeyDownEvent>(( evt ) =>
        {
            if ( evt.keyCode == KeyCode.Return )
            {
                label.text = textField.value;
                label.style.display = DisplayStyle.Flex;
                SetNodeName(label.text);
                textField.RemoveFromHierarchy();
                graphPanel.ClearSelection();
            }
        });

        label.style.display = DisplayStyle.None;
        label.parent.Add(textField);
        textField.Focus();
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
        node.nodeName = nodeName;
        Debug.Log($"{node.nodeName}");
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
    void SetColorViewByState( Node.NodeViewState state )
    {
        switch ( state )
        {
            case Node.NodeViewState.Selected:
                SetBoarderColor(Color.cyan, 5);
                break;
            case Node.NodeViewState.Default:
                SetBoarderColor(Color.black);
                break;
            case Node.NodeViewState.Root:
                SetBoarderColor(Color.magenta);
                break;
        }
    }
    private void SetupDataBinding()
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




