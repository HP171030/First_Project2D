using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ArcanaSlot : MonoBehaviour,IPointerUpHandler,IPointerEnterHandler,IPointerExitHandler
{
    public int slotID;  
    public Arcana arcana;
    public Image arcanaIcon;
    public GameObject gameUI;
    public Image detailImage;
    public TMP_Text text;
    public TMP_Text titleText;

    private void Start()
    {
        gameUI.SetActive(false); 
    }

    public void UpdateSlotUI()
    {
       // arcanaIcon.sprite = arcanaIcon.itemImage;

        arcanaIcon.gameObject.SetActive(true);
    }

    public void RemoveSlot()
    {
        arcana = null;
        arcanaIcon.gameObject.SetActive(false);
    }

    public void OnPointerUp( PointerEventData eventData )
    {
        
        
    }

    public void OnPointerEnter( PointerEventData eventData )
    {
       
       
    }

    public void OnPointerExit( PointerEventData eventData )
    {
        gameUI.SetActive(false);
    }
}
