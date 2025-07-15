using System;
using System.Collections.Generic;
using UnityEngine;

public static class Extension
{
    public static bool Contain(this LayerMask layerMask, int layer)
    {
        return ((1 << layer) & layerMask) != 0;
    }

    public static List<Type> GetTypeList( Type type )
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
}
