using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform itemSlotContainer;
    [SerializeField] private Transform itemSlotTemplate;
    [SerializeField] private Transform inventoryContainer;
    private Inventory inventory;
    private Transform dropPoint;
    private bool state = false;
    private void Awake()
    {
        inventoryContainer = transform.Find("InventoryContainer");
        itemSlotContainer = inventoryContainer.Find("ItemSlotContainer");
        itemSlotTemplate = itemSlotContainer.Find("ItemSlot");
        
        //Sub to opening the inventory
        UnitActionManager.Instance.RequestInventory += UnitActionManager_RequestInventory;
        //Sub player spawning
        Unit.OnAnyUnitSpawned += Unit_OnPlayerUnitSpawn;
    }

    private void Unit_OnPlayerUnitSpawn(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;
        Debug.Log("Inventory from: " + unit.name);
        if (!unit.IsEnemy())
        {
            dropPoint = unit.transform;
            SetInventory(unit.GetInventory());
        }
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
            itemSlotRectTransform.gameObject.SetActive(true);
            itemSlotRectTransform.anchoredPosition = new Vector2(x * itemSlotCellSize, y * itemSlotCellSize);
            Image itemIcon = itemSlotRectTransform.Find("ItemIcon").GetComponent<Image>();
            itemIcon.sprite = item.itemSprite;
            ItemSlotUI itemSlot = itemSlotRectTransform.GetComponent<ItemSlotUI>();
            itemSlot.ClickFunc = () =>
            {
                Debug.Log("Left Click!!");
                //Use item
                inventory.UseItem(item);
            };
            itemSlot.RightClickFunc = () =>
            {
                //Debug.Log("Right Click!!");
                //Remove item
                inventory.RemoveItem(item);
                DropItem(item);
            };
            itemSlot.DragBeginFunc = () =>
            {
                //Instantiate the sprite from itemIcon
            };
            itemSlot.DraggingFunc = () =>
            {
                Debug.Log("DRAGGING");
                // IDK
            };
            itemSlot.DragEndFunc = () =>
            {
                //Goes into a new slot or gets dropped;
                //Check where the player has dragged it;
            };
            x++;
            if (x > 3)
            {
                x = 0;
                y++;
            }
        }
    }

    private ItemWorld DropItem(Item item)
    {
        Vector3 dropPosition = dropPoint.position;
        Vector3 randomDirection = Random.onUnitSphere;
        randomDirection.y = Random.Range(1, 5);
        randomDirection.Normalize();
        ItemWorld itemWorld = ItemWorld.SpawnItemWorld(dropPosition + randomDirection * 2f, item);
        itemWorld.GetComponentInParent<Rigidbody>().AddForce(randomDirection * 15f, ForceMode.Impulse);

        return itemWorld;
    }
    
}
