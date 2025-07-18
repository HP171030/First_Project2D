using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class EditorExtension 
{
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

    public static List<string> GetAssetPaths<T>() where T : UnityEngine.Object
    {
        string [] assetIds = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
        List<string> paths = new List<string>();
        foreach ( var assetId in assetIds )
        {
            string path = AssetDatabase.GUIDToAssetPath(assetId);
            paths.Add(path);
        }
        return paths;
    }
}
