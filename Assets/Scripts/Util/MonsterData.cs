using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//[CreateAssetMenu(fileName = "Monster", menuName = "monsterData/data")]
public class MonsterData : ScriptableObject
{
    public int id;
    public new string name = "NoName";
    public int hp;
    public int range;
    public int atk;
    public float speed;
    public float attackRange;
    public float atkDelay;
    public float moveDelay;
    public event UnityAction<string> monsterOnDied;
    public Item dropItem;
    public int dropGold;

    public List<AttackPattern> attackPatterns;


    [Header("Sound Clip")]
    public AudioClip soundPlayerDamaged;
    public AudioClip soundMonsterDamaged;
    public AudioClip soundAttack;
    public AudioClip soundMonsterDead;

    public void OnDiedEvent( string name )
    {
        monsterOnDied?.Invoke(name);
    }

    [Serializable]
    public class AttackPattern
    {
        public string patternName;
        
        public float cooldown;                        
        public int priority;                          
        public bool loop;
        [HideInInspector]
        public string patternJsonPath;               
    }
}
