using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory
{
    public event EventHandler OnItemsListChanged;
    
    private List<Item> itemList;

    public Inventory()
    {
        itemList = new List<Item>();

        Debug.Log("Items: " + itemList.Count);
    }

    public void AddItem(Item _item)
    {
        Debug.Log("Added Item: " + _item.worldPrefab.name);
        itemList.Add(_item);
        OnItemsListChanged?.Invoke(this, EventArgs.Empty);
    }

    public List<Item> GetItemList()
    {
        return itemList;
    }
}
