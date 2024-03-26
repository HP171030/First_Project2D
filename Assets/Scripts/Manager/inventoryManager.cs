
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class inventoryManager : Singleton<inventoryManager>
{

    public List<Item> itemsList = new List<Item>();
    public bool invenClose = false;
 
    [SerializeField] public InventoryUI inventoryUI;
  
    public event UnityAction onAddItem;
    
    public event UnityAction<int> slotEvent;

    public bool AddItem( Item item )
    {
        if ( itemsList.Count < Slot )
        {
            itemsList.Add(item);
            if ( onAddItem != null )
                onAddItem.Invoke();
            return true;
        }
        return false;
    }

    protected override void Awake()
    {
        
    {
        if ( instance == null )
        {
            instance = this;
           
        }
        else
        {
            Destroy(gameObject);
        }
    }
}



    private int slot;
    
    public int Slot
    {
        get { return slot; }
        set
        {
            slot = value;
            slotEvent.Invoke(value);
        }
    }
    public Slot [] slots;
    public Transform slotHolder;
    public void OpenInven()
    {
        if (!invenClose )
        {
            inventoryUI.gameObject.SetActive(false);
            invenClose = true;

        }
        else if (invenClose )
        {

            inventoryUI.gameObject.SetActive(true);
            invenClose = false;

        }
    }
   public void AddSlot()
    {
        Slot++;
       
    }

    public void SlotChange( int var )
    {
        
        for ( int i = 0; i < slots.Length; i++ )
        {
            slots [i].slotID = i;
            if ( i < Slot )
            {
                slots [i].GetComponent<Button>().interactable = true;
            }
            else
            {
                slots [i].GetComponent<Button>().interactable = false;
            }
        }
    }

    private void Start()
    {
        
        slotEvent += SlotChange;
        slots = slotHolder.GetComponentsInChildren<Slot>();
        onAddItem += inventoryReload;
        inventoryUI.gameObject.SetActive(false);
        invenClose = true;

        if ( slotEvent != null )
        {
            Slot = 4;
        }

    }
    public void inventoryReload() 
    { 
        for(int i = 0;i < slots.Length; i++ )
        {
            slots [i].RemoveSlot();

        }
        for(int i =0;i<itemsList.Count;i++ )
        {
            slots [i].item = itemsList [i];
            slots [i].UpdateSlotUI();
        }
    }
    public void RemoveItem(int slotId)
    {
        itemsList.RemoveAt (slotId);
        onAddItem.Invoke();
    }
    
}
