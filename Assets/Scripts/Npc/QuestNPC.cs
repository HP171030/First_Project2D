using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEditor.PackageManager.Requests;

public class QuestNPC : NPCScript
{

  public int curQuestLevel = 0;
  protected  Dictionary<int, Quest> questDic = new Dictionary<int, Quest>();                 // 해당 npc의 퀘스트 틀 <차례,해당 퀘스트>
 [SerializeField] protected Quest [] quest;                                                //해당 npc의 퀘스트 내용
   [SerializeField] protected int curQuestProcess;
    private void Awake()
    {
        
    }
    protected override void Start()
    {
       
        foreach(Quest quests in quest )
        {
            quests.npcID = NPCID;
        }
        base.Start();
        curNpc = NPCState.Quest;
        curQuestProcess = 1;
        AddQuest();
        Debug.Log("AddIn");
     




    }


    ///<summary>
    /// 특정 npc에게 활용되는 함수
    ///</summary>
    protected virtual void SpecificNPCFunc()
    {

    }
    public void InitQuest()
    {

        Debug.Log("startInit");
        Debug.Log(Manager.Quest.completedQuestList.Count);
        for ( int i = 0; i < Manager.Quest.completedQuestList.Count; i++ )
        {
            if ( Manager.Quest.completedQuestList [i].npcID == NPCID )
            {
                Debug.Log($"load : clear quest {Manager.Quest.completedQuestList [i].questName}");
                curQuestLevel++;
            }
            else
            {

                Debug.Log($"completeQuest is none - current num : {Manager.Quest.completedQuestList[i].npcID}");
                
            }
        }
        foreach ( Quest quest in Manager.Quest.QuestLists )
        {
            if ( quest.npcID == NPCID )
            {
      
                curQuestProcess = quest.npcCurQuestProcess;
                Debug.Log($"curLevel : {curQuestLevel}");
                this.quest [curQuestLevel] = quest;
            }
            else
            {
                Debug.Log("isNot NPCID2");
            }
        }
        
        if ( quest.Length-1 < curQuestLevel)
        {
           
            curNpc = NPCState.Talk;
        }
        SpecificNPCFunc();
    }

    public void AddQuest()
    {
        for ( int i = 0; i < quest.Length; i++ )
        {
            questDic.Add(i, quest [i]);
        }
    }



    private Queue<string> TalkDataEnqueue( string [] talkData )
    {
        Queue<string> strings = new Queue<string>(talkData.Length);
        for ( int i = 0; i < talkData.Length; i++ )
        {
            strings.Enqueue(talkData [i]);
        }
        return strings;
    }


    public void TalkProcess( string [] talkStrings )
    {


        Manager.UICanvas.dialogue.enabled = true;
        PlayerControll playerCon = FindObjectOfType<PlayerControll>();
        PlayerInput player = playerCon.GetComponent<PlayerInput>();
        player.enabled = false;
        Manager.UICanvas.text.enabled = true;
        enterNPC = false;
        Queue<string> talk = TalkDataEnqueue(talkStrings);

        StartCoroutine(WaitSpace());
        Manager.UICanvas.text.text = talk.Dequeue();
        DoTweenText.DoText(Manager.UICanvas.text, 0.2f);
        IEnumerator WaitSpace()
        {
            while ( true )
            {
                yield return null;

                if ( Input.GetKeyDown(KeyCode.Space) )
                {

                    Next();
                    yield break;
                }
            }
        }
        void Next()
        {

            if ( talk.Count > 0 )
            {
                if ( Input.GetKeyDown(KeyCode.Space) )
                {
                    Manager.UICanvas.text.text = talk.Dequeue();
                    DoTweenText.DoText(Manager.UICanvas.text, 0.2f);

                    StartCoroutine(WaitSpace());
                }
            }
            else
            {

                Manager.UICanvas.dialogue.enabled = false;
                Manager.UICanvas.text.enabled = false;
                player.enabled = true;
                return;
            }
        }
    }


    protected override void OnSpc( InputValue value )
    {
       



        if ( curQuestProcess == 2 && quest [curQuestLevel].isCompleted )  //if(진행중 레벨(2)인지, 완료했는지,
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
                        
                        curQuestProcess++;
                        foreach ( Quest quest in Manager.Quest.QuestLists )
                        {
                            if ( quest == questDic [curQuestLevel] )
                            {
                                Debug.Log("save the quest");
                                quest.npcID = NPCID;
                                quest.npcCurQuestLevel = curQuestLevel;         // 처음 : 0
                                quest.npcCurQuestProcess++;                     // 처음 : 2
                                break;
                            }
                            else
                            {
                                
                            }
                        }

                        break;


                    case 2:                                     //  대기

                        TalkProcess(questDic [curQuestLevel].WaitQuest);
                        Debug.Log("isWait");
                        break;



                    case 0:                                        // 완료



                        Manager.Quest.ClearQuest(questDic [curQuestLevel]);            //안지워짐 잘 살펴볼것



                        TalkProcess(questDic [curQuestLevel].ClearQuest);
                        Debug.Log("isClear");
                        if ( questDic.ContainsKey(curQuestLevel+1))
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
