using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEditor.Profiling.RawFrameDataView;
using UnityEngine.Events;

public class QuestUIManager : MonoBehaviour
{
    #region singleton
    private static QuestUIManager instance;
    public static QuestUIManager Ins { get { return instance; } }

    private void Awake()
    {
        if ( instance == null )
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
    bool questClose;
   [SerializeField] QuestUIManager questUI;


    List<KillQuest> killQuestLists = new List<KillQuest>();
    public event UnityAction questSlotEvent;

    public QuestSlot [] questSlots;
    public Transform holder;

    public void questSlotChan()
    {
        for(int i = 0; i < questSlots.Length; i++)
        {
            questSlots [i].gameObject.SetActive(false);
            questSlots [i].RemoveQuest();
        }
        for(int i = 0; i < questSlots.Length; i++)
        {
            if(i < killQuestLists.Count)
            {
            questSlots [i].quest = killQuestLists [i];
                
            questSlots [i].gameObject.SetActive (true);
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
    private void Start()
    {
        questSlotEvent += questSlotChan;

        questSlots = holder.GetComponentsInChildren<QuestSlot>();
        questClose=true;
        questUI.gameObject.SetActive(false);
        questSlotEvent?.Invoke();
    }
    public void AddKillQuest( KillQuest quest )
    {
        killQuestLists.Add(quest);
        questSlotEvent?.Invoke();
    }

    public void RemoveKillQuest( KillQuest quest )
    {
        killQuestLists.Remove(quest);
        questSlotEvent?.Invoke();
    }

    public void HandleMonsterDied( int monsterId )
    {
        foreach ( KillQuest quest in killQuestLists )
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
