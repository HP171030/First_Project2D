using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TutorialQuestNPC : NPCScript
{
    [SerializeField] Transform mimicRegen2;
    [SerializeField] float mimicSpawnInterval;
    [SerializeField] PooledObject monsterMimic;
    [SerializeField] int curQuestNum = 0;
 


    protected override void Start()
    {
        base.Start();
     //   StartCoroutine(SpawnMimicsRoutine());
        AddQuest();
    }
    public void AddQuest()
    {
        quests.Add(0, new string [] { "please Kill the Mimic", "u can find them","Kill 5 mimics","Script is done" });
        quests.Add(1,new string[] {"Come on","you must kill them" });
        quests.Add(2, new string [] { "you did it!", "i can help you something" });
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
                    Manager.Pool.GetPool(monsterMimic, newPosition, Quaternion.identity);

                }
            }
            else if ( limit >= 11 )
            {
                StopCoroutine("SpawnMimicsRoutine");
                Debug.Log("CheckStopCo");
            }
        }
    }
    protected override void StartQuest()
    {
        base.StartQuest();
        dialogueOn.enabled = true;
        PlayerControll playerCon = FindObjectOfType<PlayerControll>();
        PlayerInput player = playerCon.GetComponent<PlayerInput>();
        player.enabled = false;
        text.enabled = true;
        enterNPC = false;
        Queue<string> strings = new Queue<string>(quests [curQuestNum].Length);
        for ( int i = 0; i < quests [curQuestNum].Length; i++ )
        {

            strings.Enqueue(quests[curQuestNum] [i]);
        }

        StartCoroutine(WaitSpace());
        text.text = strings.Dequeue();
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
                    text.text = strings.Dequeue();
                    Debug.Log(strings.Count);
                    StartCoroutine(WaitSpace());
                }
            }
            else
            {
                Debug.Log("LastOn");
                dialogueOn.enabled = false;
                text.enabled = false;
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
                break;
            case 1:
              //조건 충족시 올리기
                
                break;
            case 2:
               
                quests.Clear();
                break;

        }
        }
        
    }

    
}
