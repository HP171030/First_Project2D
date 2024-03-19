using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public Vector3 playerPos;
    public int Hp;
    public int maxHp;
    public int Mp;
    public int maxMp;
    public int gold;
    public List<KillQuest> questList;

    public GameData()
    {
        playerPos = new Vector3(9.25f, 12.71321f,0);
        Hp = 100;
        maxHp  = 100;
        Mp = 100;
        maxMp = 100;
        gold = 0;
        questList = new List<KillQuest>();

     
    }
}