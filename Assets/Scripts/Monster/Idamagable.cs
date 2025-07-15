using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Idamagable
{
    public void TakeDamage( int damage );
    
}

public interface IOpenable
{
    public void OpenObject() { }
}

public interface ICloseable
{ 
    public void Close(); 
}
public interface IInfomation
{
    public void Info( string info );
}
