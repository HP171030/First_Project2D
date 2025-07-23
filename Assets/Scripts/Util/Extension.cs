using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class Extension
{
    public static bool Contain( this LayerMask layerMask, int layer )
    {
        return ( ( 1 << layer ) & layerMask ) != 0;
    }

    public static List<Type> GetTypeList( this Type type )
    {
        List<Type> results = new();

        var types = type.Assembly.GetTypes();
        foreach ( var element in types )
        {
            if ( !element.IsAbstract && element.IsClass && element.IsSubclassOf(type) )
            {
                results.Add(element);
            }
        }
        return results;
    }

    public static string GetTypeNodeName(this Type type )
    {

        var attr = type.GetCustomAttribute<NodeNameAttribute>();
        return attr?.Name ?? type.Name;

    }
    public static List<Node> GetSortedChildren(this Node node )
    {
        var sorted = node.Children.Values
            .OrderBy(child => child.position.y)
            .ThenBy(child => child.position.x)
            .ToList();

        return sorted;
    }

}
public static class RunnerTypeRegistry
{
    public static Dictionary<string, Type> DisplayNameToType = new();

    [RuntimeInitializeOnLoadMethod]
    public static void Initialize()
    {
        var types = typeof(LogicRunner).Assembly.GetTypes()
            .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(LogicRunner)));

        foreach ( var type in types )
        {
            var attr = type.GetCustomAttribute<NodeNameAttribute>();
            var displayName = attr?.Name ?? type.Name;
            DisplayNameToType [displayName] = type;
        }
    }

    public static Type GetRunnerType( this string name )
    {
        return DisplayNameToType.TryGetValue(name, out var type) ? type : null;
    }
}
[AttributeUsage(AttributeTargets.Field)]
public class BlackBoardFieldAttribute : Attribute
{
    public string keyName;

    public BlackBoardFieldAttribute(string keyName )
    {
        this.keyName = keyName;
    }
}