using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(fileName = "Monster", menuName = "monsterData/data")]
public class MonsterData : ScriptableObject
{
    public int id;
    public new string name;
    public int hp;
    public int range;
    public int atk;
    public float speed;
    public float attackRange;
    public float atkDelay;
    public event UnityAction<string> monsterOnDied;
    public Item dropItem;
    

    [Header("Sound Clip")]
    public AudioClip soundPlayerDamaged;
    public AudioClip soundMonsterDamaged;
    public AudioClip soundAttack;
    public AudioClip soundMonsterDead;

    public void OnDiedEvent(string name)
    {
        monsterOnDied?.Invoke(name);
    }

}
