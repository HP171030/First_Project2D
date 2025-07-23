using AYellowpaper.SerializedCollections;
using System;
using UnityEngine;

[Serializable]
public class Blackboard
{
    public SerializedDictionary<string, BlackboardValue> data = new();

    public void Set<T>( string key, T value )
    {
        if ( !data.TryGetValue(key, out var bbValue) )
        {
            bbValue = new BlackboardValue();
            data [key] = bbValue;
        }

        if ( typeof(T) == typeof(int) )
        {
            bbValue.type = BlackboardValueType.Int;
            bbValue.intValue = Convert.ToInt32(value);
        }
        else if ( typeof(T) == typeof(float) )
        {
            bbValue.type = BlackboardValueType.Float;
            bbValue.floatValue = Convert.ToSingle(value);
        }
        else if ( typeof(T) == typeof(bool) )
        {
            bbValue.type = BlackboardValueType.Bool;
            bbValue.boolValue = Convert.ToBoolean(value);
        }
        else if ( typeof(T) == typeof(string) )
        {
            bbValue.type = BlackboardValueType.String;
            bbValue.stringValue = Convert.ToString(value);
        }
        else if ( typeof(T) == typeof(Vector3) )
        {
            bbValue.type = BlackboardValueType.Vector3;
            bbValue.vector3Value = ( Vector3 )( object )value;
        }

    }
    public bool Contain( string key )
    {
        if ( key == null )
            return false;

        return data.ContainsKey(key);

    }
    public T Get<T>( string key )
    {
        if ( data.TryGetValue(key, out var bbValue) )
        {
            return ( T )bbValue.GetValue();
        }
        return default;
    }
}



public enum BlackboardValueType { Int, Float, Bool, String, Vector3 }

[Serializable]
public class BlackboardValue
{
    public BlackboardValueType type;

    public int intValue;
    public float floatValue;
    public bool boolValue;
    public string stringValue;
    public Vector3 vector3Value;

    public object GetValue()
    {
        switch ( type )
        {
            case BlackboardValueType.Int: return intValue;
            case BlackboardValueType.Float: return floatValue;
            case BlackboardValueType.Bool: return boolValue;
            case BlackboardValueType.String: return stringValue;
            case BlackboardValueType.Vector3: return vector3Value;
        }
        return null;
    }
}
