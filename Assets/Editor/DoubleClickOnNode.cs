using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static MonsterEditor;

public class DoubleClickOnNode : MouseManipulator
{
    double time;
    double validInterval = 0.3f;
    public DoubleClickOnNode()
    {
        time = EditorApplication.timeSinceStartup;


    }
    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseDownEvent>(OnMouseDown);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.RegisterCallback<MouseDownEvent>(OnMouseDown);
    }

    void OnMouseDown(MouseDownEvent evt )
    {
        Debug.Log("Mouse Down");
        var nodeView = target as NodeView;
        if ( nodeView == null )
            return;

        double duration = EditorApplication.timeSinceStartup - time;
        if ( duration < validInterval )
        {
            Debug.Log("Double Click");
            SelectChildren(evt);
        }

        time = EditorApplication.timeSinceStartup;
    }

    private void SelectChildren( MouseDownEvent evt )
    {
        var graphView = target as BehaviourTreeView;
        if ( graphView == null )
            return;

        if ( !CanStopManipulation(evt) )
            return;

        NodeView clickedElement = evt.target as NodeView;
        if ( clickedElement == null )
        {
            var ve = evt.target as VisualElement;
            clickedElement = ve.GetFirstAncestorOfType<NodeView>();
            if ( clickedElement == null )
                return;
        }
        
        
         BehaviourTree.Traverse(clickedElement.node, node => {
            var view = graphView.FindNodeView(node);
            graphView.AddToSelection(view);
        });
    }
}
