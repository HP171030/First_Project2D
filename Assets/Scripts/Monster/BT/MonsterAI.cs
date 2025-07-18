using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    Monster _monster;
    Node _rootNode;

    NodeRunner _nodeRunner;
    NodeRunnerFactory _nodeRunnerFactory;

    private void Awake()
    {
        _nodeRunnerFactory = new NodeRunnerFactory();
        _monster = GetComponent<Monster>();

    }
    public void Start()
    {
        if ( _monster == null )
        {
            Debug.LogError("Monster Component is null");
            return;
        }

        _rootNode = _monster.monsterData.BehaviorTreeRootNode;

        if ( _rootNode == null )
        {
            Debug.LogWarning($"{_monster.monsterData.name} data is null ");
            return;

        }
        _nodeRunner = _nodeRunnerFactory.Create(_rootNode,_monster);
        StartCoroutine(_nodeRunner.Execute());
        
    }

}
public class NodeRunnerFactory
{
    Dictionary<Type, ConstructorInfo> _constructors = new();

    public NodeRunnerFactory()
    {
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
                Debug.LogWarning($"{runnerType.Name} : 컨스트럭터가 없음");
                continue;
            }

            _constructors [attr.NodeType] = ctor;
        }
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
        NodeType = nodeType;
    }
}
