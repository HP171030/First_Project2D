using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Object = UnityEngine.Object;

public static class Extension
{
    public static bool Contain( this LayerMask layerMask, int layer )
    {
        return ( ( 1 << layer ) & layerMask ) != 0;
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

    public static List<T> LoadAssets<T>() where T : Object
    {
        var guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
        List<T> assets = new();
        foreach ( var guid in guids )
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var result = AssetDatabase.LoadAssetAtPath<T>(path);
            assets.Add(result);
        }
        return assets;
    }
    public static List<T> LoadAssets<T>( string _path ) where T : Object
    {
        var guids = AssetDatabase.FindAssets($"t:{typeof(T)}", new [] { _path });
        List<T> assets = new();
        foreach ( var guid in guids )
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var result = AssetDatabase.LoadAssetAtPath<T>(path);
            assets.Add(result);
        }
        return assets;
    }


}
