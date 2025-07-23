
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DoubleClickOnNode : MouseManipulator
{
    double time;
    double validInterval = 0.3f;
    Action _onDoubleClick;
    public DoubleClickOnNode( System.Action onDoubleClick )
    {
        time = EditorApplication.timeSinceStartup;
        _onDoubleClick = onDoubleClick;
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

        foreach ( var child in target.Children() )
        {
            Debug.Log($"{child.name} : {child.GetType()}");
        }


        double duration = EditorApplication.timeSinceStartup - time;
        if ( duration < validInterval )
        {
            _onDoubleClick.Invoke();

        }

        time = EditorApplication.timeSinceStartup;
    }
}
