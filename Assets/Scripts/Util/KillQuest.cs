public class KillQuest
{
    public int monsterId;
    public int targetCount;
    public string questName;
    public string questContents;
    public bool isCompleted; 

    public KillQuest(string questName,int monsterId, int targetCount,string contents )
    {
        this.questName = questName;
        this.monsterId = monsterId;
        this.targetCount = targetCount;
        this.isCompleted = false;
        questContents = contents;
    }
}
