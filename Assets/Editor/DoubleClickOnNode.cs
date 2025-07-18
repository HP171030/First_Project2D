
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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
        }

        time = EditorApplication.timeSinceStartup;
    }
}
