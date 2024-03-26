using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonMonster : MonoBehaviour
{
    public GameObject SummonMonsterFunc( GameObject monster,Vector2 pos )
    {
        return Instantiate(monster, pos, Quaternion.identity);
    }
    
}
