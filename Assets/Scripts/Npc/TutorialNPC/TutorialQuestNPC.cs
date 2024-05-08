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

       


        if ( curQuestProcess == 2&& quest [curQuestLevel].isCompleted )  //if(������ ����(2)����, �Ϸ��ߴ���,
        {
            Debug.Log("complete Quest");
            curQuestProcess = 0;                                            // �Ϸ᷹��(0) ��
        }



        if ( enterNPC )
        {
          

            tryEnter.enabled = false;
            if ( curNpc == NPCState.Quest )
            {


                #region ����Ʈ ����
                switch ( curQuestProcess )
                {
                    case 1:                                     // ����
                        TalkProcess(questDic [curQuestLevel].startQuest);
                        Manager.Quest.AddQuest(questDic [curQuestLevel]);
                        StartCoroutine(SpawnMimicsRoutine());           //Ʃ�丮��npc ����
                        curQuestProcess++;
                        foreach ( Quest quest in Manager.Quest.QuestLists )
                        {
                            if ( quest == questDic [curQuestLevel] )
                            {
                                Debug.Log("save the questProcess,Level");
                                quest.npcID = NPCID;
                                quest.npcCurQuestLevel = curQuestLevel;         // ó�� : 0
                                quest.npcCurQuestProcess++;                     // ó�� : 2
                                break;
                            }
                            else
                            {
                                Debug.Log("Error");
                            }
                        }

                        break;


                    case 2:                                     //  ���

                        TalkProcess(questDic [curQuestLevel].WaitQuest);
                        Debug.Log("isWait");
                        break;



                    case 0:                                        // �Ϸ�



                        Manager.Quest.ClearQuest(questDic [curQuestLevel]);
                        Debug.Log($"Remove => {questDic [curQuestLevel].questName}");
                        Last.SetActive(true);                               // Ʃ�丮��npc ����

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
                            curNpc = NPCState.Talk;             //�������Ʈ �Ϸ� => talk��
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
