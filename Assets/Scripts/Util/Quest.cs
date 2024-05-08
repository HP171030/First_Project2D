
using System;
using System.Reflection;
using UnityEngine;
[Serializable]
public class Quest 
{
    public enum QuestType
    {
        Kill,
        Gather
    }
    [Header("Quest Contents")]
    public QuestType Type;
    public string questName;
    public string questContents;
    public string targetName;
    public bool isCompleted;
    public int Count;
    public int targetCount;

    [Header("Save&Load")]
    public int npcID;
    public int npcCurQuestLevel = 0;
    public int npcCurQuestProcess = 1;


    [Header("Quest Talk String")]
    public string [] startQuest;
    public string [] WaitQuest;
    public string [] ClearQuest;


}

