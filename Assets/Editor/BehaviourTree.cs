using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTree : MonoBehaviour
{
    // Start is called before the first frame update
    public static void Traverse( Node node, System.Action<Node> visiter )
    {
        if (node != null)
        {
            visiter.Invoke(node);
            var children = GetChildren(node);
            children.ForEach(( n ) => Traverse(n, visiter));
        }
    }

    public static List<Node> GetChildren( Node parent )
    {
        List<Node> children = new List<Node>();

        if ( parent is DecoratorNode decorator && decorator.child != null )
        {
            children.Add(decorator.child);
        }

        if ( parent is RootNode rootNode && rootNode.child != null )
        {
            children.Add(rootNode.child);
        }

        if ( parent is CompositeNode composite )
        {
            return composite.children;
        }

        return children;
    }
}
