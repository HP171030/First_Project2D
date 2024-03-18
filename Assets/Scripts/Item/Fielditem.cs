using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fielditem : MonoBehaviour
{
    public Item item;
    public SpriteRenderer image;

    public void SetItem(Item item )
    {
        this.item.itemName = item.itemName;
        this.item.itemImage = item.itemImage;
        this.item.itemType = item.itemType;
        this.item.effects = item.effects;
        this.item.itemColor = item.itemColor;   
        this.item.itemDescription = item.itemDescription;

        image.sprite = item.itemImage;
    }

    public Item GetItem()
    {
        return item;
    }

    public void DestroyItem()
    {
        Destroy(gameObject);
    }
   
}
