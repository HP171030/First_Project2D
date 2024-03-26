public class QuestData
{
    public string questName;
    public int [] npcId;

    public QuestData( string name, int [] npcId )
    {
        questName = name;
        this.npcId = npcId;
    }
}
