using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private List<Item> itemList;

    public Inventory()
    {
        itemList = new List<Item>();

        AddItem(new Item(Item.ItemType.Sword));
        AddItem(new Item(Item.ItemType.Chest));
        AddItem(new Item(Item.ItemType.Helmet));
        
        Debug.Log("Items: " + itemList.Count);
    }

    public void AddItem(Item _item)
    {
        itemList.Add(_item);
    }

    public List<Item> GetItemList()
    {
        return itemList;
    }
}
