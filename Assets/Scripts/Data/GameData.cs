using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public string sceneName;
    public Vector3 playerPos;
    public int Hp;
    public int maxHp;
    public int Mp;
    public int maxMp;
    public int gold;
    public List<KillQuest> questList;
    public bool thisisNew;

    public GameData()
    {
        sceneName = "GameScene";
        thisisNew = true;
        playerPos = new Vector3(9.25f, 12.71321f,0);
        Hp = 150;
        maxHp  = 150;
        Mp = 150;
        maxMp = 150;
        gold = 0;
        questList = new List<KillQuest>();

     
    }
}