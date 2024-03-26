using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class QuestSlot : MonoBehaviour,IPointerUpHandler,IPointerExitHandler
{
  [SerializeField] public GameObject contentsUI;
  [SerializeField] public TMP_Text text;
  [SerializeField] public int targetAmount;
  [SerializeField] public TMP_Text contentsText;
  [SerializeField] public KillQuest quest;
   
   
    void Start()
    {
        contentsUI.SetActive(false);
        
    }

    public void LoadQuest()
    {
        
        targetAmount = quest.targetCount;
        text.text = quest.questName;
        contentsText.text = quest.questContents;
    }
    public void RemoveQuest()
    {
        
        quest = null;
       
        
    }
    public void UpdateQuestUIText( string newText )
    {
        // 퀘스트 슬롯의 UI 텍스트 업데이트
        contentsText.text = newText;
    }
    public void OnPointerUp( PointerEventData eventData )
    {
       
        contentsUI.SetActive(true);
    }

    public void OnPointerExit( PointerEventData eventData )
    {
       
        contentsUI.SetActive(false);
    }
}
