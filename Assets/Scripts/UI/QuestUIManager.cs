using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestUIManager : Singleton<QuestUIManager>
{
    public bool questClose;
    [SerializeField] public QuestUIManager questUI;


    public List<Quest> QuestLists = new List<Quest>();



    public List<Quest> completedQuestList = new List<Quest>();



    public event UnityAction questSlotEvent;

    public QuestSlot [] questSlots;
    public Transform holder;
    [SerializeField] GameObject questContents;

    public void OnEnable()
    {
        if(QuestLists.Count == 0 )
        {
            questContents.SetActive(false);
        }
        else if (QuestLists.Count >0)
        {
            questContents.SetActive(true);
        }
    }
    public void questSlotChan()
    {
        for ( int i = 0; i < questSlots.Length; i++ )
        {
            questSlots [i].gameObject.SetActive(false);
            questSlots [i].RemoveQuest();
        }
        for ( int i = 0; i < questSlots.Length; i++ )
        {
            if ( i < QuestLists.Count )
            {
                questSlots [i].quest = QuestLists [i];

                questSlots [i].gameObject.SetActive(true);
                questSlots [i].LoadQuest();
            }
            else
            {
                questSlots [i].gameObject.SetActive(false);
                questSlots [i].RemoveQuest();
            }

        }
    }

    public void OpenQuest()
    {

        if ( !questClose )
        {
            questUI.gameObject.SetActive(false);
            questClose = true;

        }
        else if ( questClose )
        {

            questUI.gameObject.SetActive(true);
            questClose = false;

        }


    }
    protected override void Awake()
    {


        if ( instance == null )
        {
            instance = this;

        }
        else
        {
            Destroy(gameObject);
        }

    }
    private void Start()
    {
        questSlotEvent += questSlotChan;

        questSlots = holder.GetComponentsInChildren<QuestSlot>();
        questClose = true;
        questUI.gameObject.SetActive(false);
        questSlotEvent?.Invoke();
    }

    public void AddQuest( Quest quest )
    {
        QuestLists.Add(quest);
        questSlotEvent?.Invoke();
    }
    public void HandleNewData()
    {
        QuestLists.Clear();
        questSlotEvent?.Invoke();
    }
    public void ClearQuest( Quest quest )
    {
        for ( int i = QuestLists.Count - 1; i >= 0; i-- )
        {
            if ( QuestLists [i].questName == quest.questName )
            {
                completedQuestList.Add(QuestLists [i]);
                QuestLists.RemoveAt(i);
                Debug.Log($"{QuestLists.Count} is QuestListCount");
                Debug.Log($"{completedQuestList.Count} is CompleteList");
                questSlotEvent?.Invoke();
                break; // 퀘스트를 찾았으면 반복문 종료
            }
        }
        /* foreach(Quest quests in QuestLists )
         {
             if(quests.questName == quest.questName )
             {
                 completedQuestList.Add(quests);
                 QuestLists.Remove(quests);
                 Debug.Log($"{QuestLists.Count} is QuestListCount");
                 Debug.Log($"{completedQuestList.Count} is CompleteList");

                 questSlotEvent?.Invoke();


             }
         }*/

    }

    public void HandleMonsterDied( string monsterName )
    {
        foreach ( Quest quest in QuestLists )
        {
            if ( quest.targetName == monsterName && !quest.isCompleted )
            {
              //  Debug.Log($"Count : {quest.Count}  targetCount : {quest.targetCount}");
                quest.Count++;
                if ( quest.Count >= quest.targetCount )
                {
                   
                    quest.isCompleted = true;
                   
                    Debug.Log("Complete");
                }
                UpdateQuestUIText(quest, monsterName);
            }
        }
    }
    public void HandleGatherItem(string itemName )
    {
        foreach( Quest quest in QuestLists )
        {
            if(quest.targetName == itemName && !quest.isCompleted )
            {
                quest.Count++;
                if( quest.Count >=quest.targetCount )
                {
                    quest.isCompleted = true;
                   
                }

                UpdateQuestUIText(quest,itemName);
            }
        }
    }
    public void UpdateQuestUIText(Quest quest,string targetName)
    {
       // Debug.Log($"target : {targetName},questName : {quest.questName}");
        
        if ( questUI != null && questSlotEvent != null)                                     // 열때 텍스트 업데이트
        {
         
            string updatedText = $" {targetName}   {quest.Count}/{quest.targetCount} ";
            if ( quest.isCompleted )
            {
                updatedText += " (Complete)";
            }
            foreach ( QuestSlot slot in questSlots )
            {
               if ( slot.quest == quest )
               {
                   slot.UpdateQuestUIText(updatedText);
                  
                   break;
               }
             }
        }
        else
        {
            Debug.Log($"ui : {questUI.name}, slot : {questSlotEvent}");
        }
        
    }
}
