using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class NodeRunnerFactory
{

    static NodeRunnerFactory _instance;
    public static NodeRunnerFactory Instance
    {
        get
        {
            if ( _instance == null )
            {
                _instance = new NodeRunnerFactory();
            }
            return _instance;
        }
    }

    Dictionary<Type, ConstructorInfo> _constructors = new();
    public bool Complete = false;
    public NodeRunnerFactory()
    {
        RegistAll();
        Complete = false;
    }
    void RegistAll()
    {
        var assembly = Assembly.GetExecutingAssembly();

        var runnerTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(NodeRunner).IsAssignableFrom(t));

        foreach ( var runnerType in runnerTypes )
        {
            var attr = runnerType.GetCustomAttribute<NodeRunnerForAttribute>();
            if ( attr == null ) continue;

            var ctor = runnerType.GetConstructor(new Type [] { attr.NodeType, typeof(Monster) });
            if ( ctor == null )
            {
                Debug.LogWarning($"{attr.NodeType.Name} arg1 {runnerType.Name} : ctor 없음");
                continue;
            }

            _constructors [attr.NodeType] = ctor;
        }

        Complete = true;
    }
    public NodeRunner Create( Node node, Monster monster )
    {

        var nodeType = node.GetType();
        if ( _constructors.TryGetValue(nodeType, out var ctor) )
        {
            return ( NodeRunner )ctor.Invoke(new object [] { node, monster });
        }
        throw new Exception($"러너에 없는 노드타입임 :  {nodeType.Name}");
    }
}
[AttributeUsage(AttributeTargets.Class)]
public class NodeRunnerForAttribute : Attribute
{
    public Type NodeType { get; }
    public NodeRunnerForAttribute( Type nodeType )
    {
        if ( nodeType == null || !typeof(Node).IsAssignableFrom(nodeType) )
        {
            Debug.LogError($"Except : {nodeType?.Name ?? "null"} 타입은 노드 상속해야함");
            throw new ArgumentException($"NodeType must inherit from Node. Invalid type: {nodeType?.Name}", nameof(nodeType));
        }

        NodeType = nodeType;
    }
}