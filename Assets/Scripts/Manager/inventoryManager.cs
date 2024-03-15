
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class inventoryManager : MonoBehaviour
{
#region singleton
    private static inventoryManager instance;
    public static inventoryManager Ins { get { return instance; } }

    private void Awake()
    {
        if ( instance == null )
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
    public List<Item> itemsList = new List<Item>();
    bool invenClose = false;
 
    [SerializeField] InventoryUI inventoryUI;
  
    public event UnityAction onAddItem;
    public Transform playerPos;
    public Animator animator;
    
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
