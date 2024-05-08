
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using System.Collections;
using DG.Tweening;

public class NPCScript : MonoBehaviour
{
    public int NPCID;

    public enum NPCState{Talk,Quest}
    public NPCState curNpc;

  


    [SerializeField]protected SpriteRenderer tryEnter;

   protected bool enterNPC = false;
    [SerializeField] protected TMP_Text text;


    [SerializeField] protected NpcTalkData npcTalkData ;



    protected virtual void Start()
    {
        
        curNpc = NPCState.Talk;
        tryEnter.enabled = false;
    }
    private void OnTriggerEnter2D( Collider2D collision )
    {
        
        tryEnter.enabled = true;
        enterNPC = true;
    }
    private void OnTriggerExit2D( Collider2D collision )
    {
        
        tryEnter.enabled = false;
        enterNPC = false;
    }

    protected virtual void OnSpc(InputValue value )
    {
        if(enterNPC)
        {
            tryEnter.enabled = false;
            if( curNpc == NPCState.Talk)
            {
                ShowDialogue();
                
            }

        }
        
    }

    protected virtual void ShowDialogue()
    {
        Manager.UICanvas.dialogue.enabled = true;
       PlayerControll playerCon = FindObjectOfType<PlayerControll>();
        PlayerInput player = playerCon.GetComponent<PlayerInput>(); 
        player.enabled = false;
        Manager.UICanvas.text.enabled = true;
        enterNPC = false;
        Queue<string> strings = new Queue<string>(npcTalkData.dialogueLine.Length);
        for (int i =0;  i < npcTalkData.dialogueLine.Length; i++)
        {
           
            strings.Enqueue(npcTalkData.dialogueLine [i]);
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
            if ( strings.Count > 0)
            {
                if ( Input.GetKeyDown(KeyCode.Space) )
                {
                    Manager.UICanvas.text.text = strings.Dequeue();
                    DoTweenText.DoText(Manager.UICanvas.text, 0.2f);
                    Debug.Log(strings.Count);   
                StartCoroutine(WaitSpace());
                }
            }
                else 
                {
                    Debug.Log("LastOn");
                Manager.UICanvas.text.DOFade(0, 0.5f);
                Manager.UICanvas.dialogue.DOFade(0, 0.5f).OnComplete(() =>
                {
                    Manager.UICanvas.dialogue.DOFade(1, 0f);
                    Manager.UICanvas.dialogue.enabled = false;

                    Manager.UICanvas.text.enabled = false;
                    player.enabled = true;
                    return;

                });
                }
        }
    }
    

}
