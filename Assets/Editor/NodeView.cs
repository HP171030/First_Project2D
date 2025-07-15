using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    public Action<NodeView> onSelectedNode;

    public Node node;
    public Port input;
    public Port output;

    public NodeView( Node node ) : base(AssetDatabase.GetAssetPath(EditorSetting.GetOrCreateSettings().nodeXml))
    {
        this.node = node;
        this.node.name = node.GetType().Name;
        this.title = node.name.Replace("(Clone)", "").Replace("Node", "");
        this.viewDataKey = node.guid;

        style.left = node.position.x;
        style.top = node.position.y;

        CreateInputPorts();
        CreateOutputPorts();
        SetupClasses();
        SetupDataBinding();
    }

    private void SetupDataBinding()
    {
        throw new NotImplementedException();
    }

    private void SetupClasses()
    {
        throw new NotImplementedException();
    }

    private void CreateOutputPorts()
    {
        throw new NotImplementedException();
    }

    private void CreateInputPorts()
    {
        throw new NotImplementedException();
    }
}

[System.Serializable]
public class Node : ScriptableObject
{
    public string guid;
    public Vector2 position;
    public List<Node> children;
    public Node child;
}

public class DecoratorNode : Node
{
}

public class RootNode : Node
{

}
public class CompositeNode : Node
{

}
