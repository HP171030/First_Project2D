using System;
using UnityEngine;
using System.Collections.Generic;

public class QuestManager : Singleton<QuestManager>
{
  
  [SerializeField] public List<KillQuest> killQuests = new List<KillQuest>();
    MonsterData monsterData;


    public void AddKillQuest( KillQuest quest )
    {
        killQuests.Add(quest);
    }

    public void RemoveKillQuest( KillQuest quest )
    {
        killQuests.Remove(quest);
    }

    public void HandleMonsterDied( int monsterId )
    {
        foreach ( KillQuest quest in killQuests )
        {
            if ( quest.monsterId == monsterId && !quest.isCompleted )
            {
                quest.targetCount--;
                if ( quest.targetCount <= 0 )
                {
                    quest.isCompleted = true;
                    Debug.Log("Complete");
                }
            }
        }
    }

}
