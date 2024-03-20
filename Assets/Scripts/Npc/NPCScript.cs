
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
    [SerializeField]protected SpriteRenderer tryEnter;
    [SerializeField] string [] dialogueLine;
   protected bool enterNPC = false;
    [SerializeField] protected TMP_Text text;




    [Header("Quest")]
    protected Dictionary<int, string []> quests = new Dictionary<int, string []> ();


    protected virtual void Start()
    {
       
       
       
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
        Manager.UICanvas.dialogue.enabled = true;
       PlayerControll playerCon = FindObjectOfType<PlayerControll>();
        PlayerInput player = playerCon.GetComponent<PlayerInput>(); 
        player.enabled = false;
        Manager.UICanvas.text.enabled = true;
        enterNPC = false;
        Queue<string> strings = new Queue<string>(dialogueLine.Length);
        for (int i =0;  i < dialogueLine.Length; i++)
        {
           
            strings.Enqueue(dialogueLine [i]);
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
        protected virtual void StartQuest()
        {

        }
    

}
