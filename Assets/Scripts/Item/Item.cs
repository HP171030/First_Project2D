using System;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{ 
    None,
    Equipment,
    Consumable,
    questItem
}
[Serializable]
public class Item
{
    public ItemType itemType;
    public string itemName;
    public string itemDescription;
    public Sprite itemImage;
    public ItemEffect effects;
    public Color itemColor;


    public bool Use()
    {
        return effects.eft();
    }

    
}


