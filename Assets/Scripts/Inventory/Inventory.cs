using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory
{
    public event EventHandler OnItemsListChanged;
    
    private List<Item> itemList;
    private Action<Item> UseItemAction;
    private Action<Item> UnequipItemAction;
    
    public Inventory(Action<Item> _UseItemAction, Action<Item> _UnequipItemAction)
    {
        itemList = new List<Item>();
        UseItemAction = _UseItemAction;
        UnequipItemAction = _UnequipItemAction;
        Debug.Log("Items: " + itemList.Count);
    }

    public void AddItem(Item _item)
    {
        Debug.Log("Added Item: " + _item.worldPrefab.name);
        itemList.Add(_item);
        OnItemsListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void RemoveItem(Item _item)
    {
        //Debug.Log("Removed Item: " + _item.worldPrefab.name);
        itemList.Remove(_item);
        OnItemsListChanged?.Invoke(this, EventArgs.Empty);
    }
    
    public List<Item> GetItemList()
    {
        return itemList;
    }

    public void UseItem(Item _item)
    {
        UseItemAction(_item);
    }

    public void Unequip(Item _item)
    {
        UnequipItemAction(_item);
    }
}
