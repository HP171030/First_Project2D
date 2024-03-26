using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestUIManager : Singleton<QuestUIManager>
{
    public bool questClose;
    [SerializeField] public QuestUIManager questUI;


    public List<KillQuest> killQuestLists = new List<KillQuest>();
    public event UnityAction questSlotEvent;

    public QuestSlot [] questSlots;
    public Transform holder;
    [SerializeField] GameObject questContents;

    public void OnEnable()
    {
        if(killQuestLists.Count == 0 )
        {
            questContents.SetActive(false);
        }
        else if ( killQuestLists.Count >0)
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
            if ( i < killQuestLists.Count )
            {
                questSlots [i].quest = killQuestLists [i];

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
    public void AddKillQuest( KillQuest quest )
    {
        killQuestLists.Add(quest);
        questSlotEvent?.Invoke();
    }
    public void HandleNewData()
    {
        killQuestLists.Clear();
        questSlotEvent?.Invoke();
    }
    public void RemoveKillQuest( KillQuest quest )
    {
        killQuestLists.Remove(quest);
        
        questSlotEvent?.Invoke();
    }

    public void HandleMonsterDied( string monsterName )
    {
        foreach ( KillQuest quest in killQuestLists )
        {
            if ( quest.monsterName == monsterName && !quest.isCompleted )
            {
                quest.targetCount--;
                if ( quest.targetCount <= 0 )
                {
                    quest.isCompleted = true;
                    Debug.Log("Complete");
                }
                UpdateQuestUIText(quest,monsterName);
            }
        }
    }

    private void UpdateQuestUIText( KillQuest quest,string monsterName )
    {
        // 퀘스트가 UI에 표시된 상태라면 텍스트를 업데이트
        if ( questUI != null && questSlotEvent != null)
        {
            // 새로운 텍스트 생성
            string updatedText = $" kill {quest.targetCount} {monsterName} ";
            foreach ( QuestSlot slot in questSlots )
        {
            if ( slot.quest == quest )
            {
                slot.UpdateQuestUIText(updatedText);
                break;
            }
        }
        }
        
    }
}
