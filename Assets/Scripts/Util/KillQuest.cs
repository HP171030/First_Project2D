using System;

[Serializable]
public class KillQuest
{
    public string monsterName;
    public int targetCount;
    public string questName;
    public string questContents;
    public bool isCompleted; 

    public KillQuest(string questName,string monsterName, int targetCount,string contents )
    {
        this.questName = questName;
        this.monsterName = monsterName;
        this.targetCount = targetCount;
        this.isCompleted = false;
        questContents = contents;
    }
}
