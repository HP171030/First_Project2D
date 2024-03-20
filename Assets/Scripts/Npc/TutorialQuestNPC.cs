using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class TutorialQuestNPC : NPCScript
{
    [SerializeField] Transform mimicRegen2;
    [SerializeField] float mimicSpawnInterval;
    [SerializeField] GameObject monsterMimic;
    [SerializeField] int curQuestNum = 0;
    [SerializeField] KillQuest mimicQuest;
    [SerializeField] KillQuest test;
    [SerializeField] KillQuest test2;
    [SerializeField] GameObject Last;




    protected override void Start()
    {
        base.Start();
      
        AddQuest();
    }
    public void AddQuest()
    {
        quests.Add(0, new string [] { "please Kill the Mimic", "u can find them", "Kill 5 mimics", "Script is done" });
        quests.Add(1, new string [] { "Come on", "you must kill them" });
        quests.Add(2, new string [] { "you did it!",
            "i can help you something" });
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
    protected override void StartQuest()
    {
        base.StartQuest();
        Manager.UICanvas.dialogue.enabled = true;
        PlayerControll playerCon = FindObjectOfType<PlayerControll>();
        PlayerInput player = playerCon.GetComponent<PlayerInput>();
        player.enabled = false;
        Manager.UICanvas.text.enabled = true;
        enterNPC = false;
        Queue<string> strings = new Queue<string>(quests [curQuestNum].Length);
        for ( int i = 0; i < quests [curQuestNum].Length; i++ )
        {

            strings.Enqueue(quests[curQuestNum] [i]);
        }

        StartCoroutine(WaitSpace());
        Manager.UICanvas.text.text = strings.Dequeue();
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
            if ( strings.Count > 0 )
            {
                if ( Input.GetKeyDown(KeyCode.Space) )
                {
                    Manager.UICanvas.text.text = strings.Dequeue();
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

        if ( enterNPC )
        {
            tryEnter.enabled = false;
            if ( quests.Count > 0 )
            {
                StartQuest();
            }
            else
            {
                ShowDialogue();
            }
        switch ( curQuestNum )
        {
            case 0: curQuestNum++;
                    StartCoroutine(SpawnMimicsRoutine());
                    mimicQuest = new KillQuest("mimicQuest",1, 5, $"Kill 5 mimic \n asdfasdfsf"); 
                    Manager.Quest.AddKillQuest(mimicQuest);
                    
                    

                    break;
            case 1:
                    if (mimicQuest.isCompleted)
                    {
                        curQuestNum++;
                    }
                    else
                    {
                        
                        test = new KillQuest("TestCase", 1, 5, $"testest 5 mimic \n\n\n asdfasdfsf");
                        Manager.Quest.AddKillQuest(test);
                    }
                    
                
                break;
            case 2:
                    Manager.Quest.RemoveKillQuest(mimicQuest);
                    curQuestNum++;
                    Last.SetActive(true);
                    quests.Clear();
                    
                break;
                case 3:
                    test2 = new KillQuest("test2", 1, 5, $"baodfa");
                    Manager.Quest.AddKillQuest(test2);
                    
                    break;


            }
        }
        
    }

    
}
