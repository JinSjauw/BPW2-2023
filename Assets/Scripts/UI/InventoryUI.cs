using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform itemSlotContainer;
    [SerializeField] private Transform itemSlotTemplate;
    [SerializeField] private Transform inventoryContainer;
    private Inventory inventory;
    private bool state = false;
    private void Awake()
    {
        inventoryContainer = transform.Find("InventoryContainer");
        itemSlotContainer = inventoryContainer.Find("ItemSlotContainer");
        itemSlotTemplate = itemSlotContainer.Find("ItemSlot");
    }

    private void Start()
    {
        //Sub to opening the inventory
        UnitActionManager.Instance.RequestInventory += UnitActionManager_RequestInventory;
        //Sub player spawning
        Unit.OnPlayerUnitSpawn += Unit_OnPlayerUnitSpawn;
    }

    private void Unit_OnPlayerUnitSpawn(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;
        Debug.Log("Inventory from: " + unit.name);
        SetInventory(unit.GetInventory());
    }
    
    private void UnitActionManager_RequestInventory(object sender, EventArgs e)
    {
        state = !state;
        //Open inventory;
        if (state)
        {
            RefreshInventory();
        }
        
        inventoryContainer.gameObject.SetActive(state);
    }

    public void SetInventory(Inventory _inventory)
    {
        inventory = _inventory;
        inventory.OnItemsListChanged += Inventory_OnItemsListChanged;
        Debug.Log("Items count: " + _inventory.GetItemList().Count);
        RefreshInventory();
    }

    private void Inventory_OnItemsListChanged(object sender, EventArgs e)
    {
        Debug.Log(inventory.GetItemList().Count);
        RefreshInventory();
    }

    private void RefreshInventory()
    {
        foreach (Transform child in itemSlotContainer)
        {
            if(child == itemSlotTemplate) { continue; }
            Destroy(child.gameObject);
        }
        
        int x = 0;
        int y = 0;

        float itemSlotCellSize = 80f;
        Debug.Log(inventory);
        foreach (var item in inventory.GetItemList())
        {
            RectTransform itemSlotRectTransform = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            //Debug.Log("Instantiated item");
            itemSlotRectTransform.gameObject.SetActive(true);
            itemSlotRectTransform.anchoredPosition = new Vector2(x * itemSlotCellSize, y * itemSlotCellSize);
            Image itemIcon = itemSlotRectTransform.Find("ItemIcon").GetComponent<Image>();
            itemIcon.sprite = item.itemSprite;
            x++;
            if (x > 4)
            {
                x = 0;
                y++;
            }
        }
    }
}
