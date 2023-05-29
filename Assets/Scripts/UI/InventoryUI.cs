using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform itemSlotContainer;
    [SerializeField] private Transform itemSlotTemplate;
    private Inventory inventory;

    private void Awake()
    {
        itemSlotContainer = transform.Find("ItemSlotContainer");
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
        Debug.Log(unit.name);
        SetInventory(unit.GetInventory());
    }


    private void UnitActionManager_RequestInventory(object sender, InventoryEventArgs e)
    {
        //Open inventory;
        RefreshInventory();
    }

    public void SetInventory(Inventory _inventory)
    {
        inventory = _inventory;
        Debug.Log("Items count: " + _inventory.GetItemList().Count);
        RefreshInventory();
    }

    private void RefreshInventory()
    {
        int x = 0;
        int y = 0;

        float itemSlotCellSize = 80f;
        
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
