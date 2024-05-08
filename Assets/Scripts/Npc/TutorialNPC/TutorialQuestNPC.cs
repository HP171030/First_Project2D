using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialQuestNPC : QuestNPC
{
    [SerializeField] Transform mimicRegen2;
    [SerializeField] float mimicSpawnInterval;
    [SerializeField] GameObject monsterMimic;
    [SerializeField] GameObject Last;
    protected override void SpecificNPCFunc()
    {
        Debug.Log("Load specific");
       
            
        if ( curNpc == NPCState.Talk )
        {
            Debug.Log("continue level is clear level, npc changed talk state");
            Last.SetActive(true);
        }
        else
        {
            if ( curQuestProcess == 1 )
            {
                StartCoroutine(SpawnMimicsRoutine());
            }
            else
            {
                Debug.Log("current Progress is not spawning level");
            }

            Debug.Log($"is not clear yet {curNpc}");
        }
    }
    private IEnumerator SpawnMimicsRoutine()
    {
        int limit = 0;
        while ( true )
        {
            yield return new WaitForSeconds(mimicSpawnInterval);

            if ( monsterMimic != null && mimicRegen2 != null && limit < 10 )
            {
                int ranSize = Random.Range(1, 4);
                limit += ranSize;
                for ( int i = 0; i < ranSize; i++ )
                {
                    Vector2 newPosition = mimicRegen2.position + Random.insideUnitSphere * 5f;
                    Instantiate(monsterMimic, newPosition, Quaternion.identity);

                }
            }
            else if ( limit >= 11 )
            {
                StopCoroutine("SpawnMimicsRoutine");

            }
        }
    }

    protected override void OnSpc( InputValue value )
    {

       


        if ( curQuestProcess == 2&& quest [curQuestLevel].isCompleted )  //if(진행중 레벨(2)인지, 완료했는지,
        {
            Debug.Log("complete Quest");
            curQuestProcess = 0;                                            // 완료레벨(0) 로
        }



        if ( enterNPC )
        {
          

            tryEnter.enabled = false;
            if ( curNpc == NPCState.Quest )
            {


                #region 퀘스트 절차
                switch ( curQuestProcess )
                {
                    case 1:                                     // 시작
                        TalkProcess(questDic [curQuestLevel].startQuest);
                        Manager.Quest.AddQuest(questDic [curQuestLevel]);
                        StartCoroutine(SpawnMimicsRoutine());           //튜토리얼npc 로직
                        curQuestProcess++;
                        foreach ( Quest quest in Manager.Quest.QuestLists )
                        {
                            if ( quest == questDic [curQuestLevel] )
                            {
                                Debug.Log("save the questProcess,Level");
                                quest.npcID = NPCID;
                                quest.npcCurQuestLevel = curQuestLevel;         // 처음 : 0
                                quest.npcCurQuestProcess++;                     // 처음 : 2
                                break;
                            }
                            else
                            {
                                Debug.Log("Error");
                            }
                        }

                        break;


                    case 2:                                     //  대기

                        TalkProcess(questDic [curQuestLevel].WaitQuest);
                        Debug.Log("isWait");
                        break;



                    case 0:                                        // 완료



                        Manager.Quest.ClearQuest(questDic [curQuestLevel]);
                        Debug.Log($"Remove => {questDic [curQuestLevel].questName}");
                        Last.SetActive(true);                               // 튜토리얼npc 로직

                        TalkProcess(questDic [curQuestLevel].ClearQuest);
                        Debug.Log("isClear");
                        if ( questDic.ContainsKey(curQuestLevel + 1) )
                        {
                            Debug.Log(questDic [curQuestLevel+1]);
                            curQuestLevel++;
                            curQuestProcess = 1;
                        }
                        else
                        {
                            curNpc = NPCState.Talk;             //모든퀘스트 완료 => talk로
                        }


                        break;


                }
                #endregion
            }
            else
            {
                ShowDialogue();
            }



        }

    }
}
