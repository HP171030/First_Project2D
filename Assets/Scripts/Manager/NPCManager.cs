using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : Singleton<NPCManager>
{
    Dictionary<int, string []> talkData;

    protected override void Awake()
    {
        base.Awake();

        talkData = new Dictionary<int, string []>();
        GenerateData();
    }

    void GenerateData()
    {
        talkData.Add(1000, new string [] { "It's Confirmed" });
    }

    public string GetTalk(int id,int talkIndex)
    {
        return talkData [id] [talkIndex];
    }
}
