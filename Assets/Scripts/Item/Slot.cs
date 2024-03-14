using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Slot : MonoBehaviour,IPointerUpHandler,IPointerEnterHandler,IPointerExitHandler
{
    public int slotID;  
    public Item item;
    public Image itemIcon;
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
        itemIcon.sprite = item.itemImage;

        itemIcon.gameObject.SetActive(true);
    }

    public void RemoveSlot()
    {
        item = null;
        itemIcon.gameObject.SetActive(false);
    }

    public void OnPointerUp( PointerEventData eventData )
    {
        if(item.itemType == ItemType.Consumable && item.itemImage !=null )
        {
            bool isUse = item.Use();
    
                 if ( isUse )
                 {
                     
                     inventoryManager.Ins.RemoveItem(slotID);
                 }
        }
        else
        {
            Debug.Log("this is not Consumable");
        }
        
    }

    public void OnPointerEnter( PointerEventData eventData )
    {
        if ( item!=null)
        {
        detailImage.sprite = item.itemImage;
        text.text = item.itemDescription;
            titleText.text = item.itemName;
            titleText.color = item.itemColor;

        gameUI.SetActive(true);
        
        }
        else
        {
            Debug.Log("isNull");
        }
    }

    public void OnPointerExit( PointerEventData eventData )
    {
        gameUI.SetActive(false);
    }
}
