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

    Dictionary<(Type,string), ConstructorInfo> _constructors = new();
    public bool Complete = false;
    public NodeRunnerFactory()
    {
        Complete = false;
        RegistAll();
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
            var key = (attr.NodeType, attr.RunnerKey ?? "");
            _constructors [key] = ctor;

        }

        Complete = true;
    }
    public NodeRunner Create( Node node, Monster monster )
    {
        string runnerKey = node.RunnerType;

        var key = (node.GetType(), runnerKey);

        if ( !_constructors.TryGetValue(key, out var ctor) )
        {
            if ( string.IsNullOrEmpty(runnerKey))
            {
                var found = _constructors
                    .Where(kv => kv.Key.Item1 == node.GetType())
                    .ToList();

                if ( found.Count == 1 )
                {
                    ctor = found [0].Value;
                }
                else
                {
                    throw new Exception($"러너를 찾을 수 없습니다: NodeType={node.GetType().Name}, RunnerKey='{runnerKey}'");
                }
            }
            else
            {
                throw new Exception($"러너를 찾을 수 없습니다: NodeType={node.GetType().Name}, RunnerKey='{runnerKey}'");
            }
        }
        var runner = ( NodeRunner )ctor.Invoke(new object [] { node, monster });
        runner.ParseFieldToDictionary();
        return runner;

    }

}
[AttributeUsage(AttributeTargets.Class)]
public class NodeRunnerForAttribute : Attribute
{
    public Type NodeType { get; }
    public string RunnerKey { get; }
    public NodeRunnerForAttribute( Type nodeType ,string runnerKey ="")
    {
        if ( nodeType == null || !typeof(Node).IsAssignableFrom(nodeType) )
        {
            throw new ArgumentException($"NodeType must inherit from Node {nodeType?.Name}", nameof(nodeType));
        }

        NodeType = nodeType;
        RunnerKey = runnerKey ?? "";
    }
}

[AttributeUsage(AttributeTargets.Class,Inherited =false)]
public class NodeNameAttribute : Attribute
{
    public string Name { get; set; }
    public NodeNameAttribute(string name )
    {
        Name = name;
    }
}