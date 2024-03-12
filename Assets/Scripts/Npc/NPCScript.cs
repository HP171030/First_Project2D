
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using System.Collections;
using Unity.VisualScripting;

public class NPCScript : MonoBehaviour
{
    [SerializeField]protected SpriteRenderer tryEnter;
    [SerializeField]protected Image dialogueOn;
    [SerializeField] string [] dialogueLine;
   protected bool enterNPC = false;
    [SerializeField] protected TMP_Text text;
   

    [Header("Quest")]
    protected Dictionary<int, string []> quests = new Dictionary<int, string []> ();


    protected virtual void Start()
    {
        tryEnter.enabled = false;
        text.enabled = false;
        dialogueOn.enabled = false;
        
    }
    private void OnTriggerEnter2D( Collider2D collision )
    {
        Debug.Log("enter");
        tryEnter.enabled = true;
        enterNPC = true;
    }
    private void OnTriggerExit2D( Collider2D collision )
    {
        Debug.Log("exit");
        tryEnter.enabled = false;
        enterNPC = false;
    }

    protected virtual void OnSpc(InputValue value )
    {
        if(enterNPC)
        {
            tryEnter.enabled = false;
            if( quests.Count > 0 )
            {
                StartQuest();
            }
            else
            {
                ShowDialogue();
            }
        
        }
        
    }

    protected virtual void ShowDialogue()
    {
        dialogueOn.enabled = true;
       PlayerControll playerCon = FindObjectOfType<PlayerControll>();
        PlayerInput player = playerCon.GetComponent<PlayerInput>(); 
        player.enabled = false;
        text.enabled = true;
        enterNPC = false;
        Queue<string> strings = new Queue<string>(dialogueLine.Length);
        for (int i =0;  i < dialogueLine.Length; i++)
        {
           
            strings.Enqueue(dialogueLine [i]);
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
            if ( strings.Count > 0)
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
        protected virtual void StartQuest()
        {

        }
    

}
