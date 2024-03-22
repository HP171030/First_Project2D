using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UICanvasManager : Singleton<UICanvasManager>
{
    [SerializeField] public Image dash;
    [SerializeField] public Image slash;
    [SerializeField]public Image skill1;
    [SerializeField]public Image skill2;

    [SerializeField] public Image Skill1Cool;
    [SerializeField] public Image Skill2Cool;

    [SerializeField] public GameObject enemyUI;
    [SerializeField] public Image enemyHpUi;
    [SerializeField] public TMP_Text enemyNameUi;

    [SerializeField] public Image dialogue;
    [SerializeField] public TMP_Text text;
    public UnityEvent enemyDamagedEvent;
    float time;
    private void Start()
    {
        this.gameObject.SetActive(false);
        dialogue.enabled = false;
        text .enabled = false;

        enemyDamagedEvent.AddListener(EnemyHpBarUpdate);

    }
    private void Update()
    {
        if(time > 0 )
        {
            time -= Time.deltaTime;
         
        }
        else if(time > 5 )
        {
            time = 5;
        }
        else if (time <= 0 )
        {
            time = 0;
            Debug.Log("endTime");
           enemyUI.SetActive(false);
        }
    }
    public void EnemyHpBarUpdate()
    {
        time++;
        if ( time > 0 )
        enemyUI.SetActive(true);
       


    }


       
        
    
}
