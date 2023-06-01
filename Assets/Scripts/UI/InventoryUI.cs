using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class InventoryUI : MonoBehaviour
{
    public static event EventHandler OnOpenInventory;
    
    [SerializeField] private Transform itemSlotContainer;
    [SerializeField] private Transform itemSlotTemplate;
    [SerializeField] private Transform inventoryContainer;
    [SerializeField] private Transform equipmentContainer;
    private Inventory inventory;
    private Transform dropPoint;
    private bool state = false;
    private void Awake()
    {
        inventoryContainer = transform.Find("InventoryContainer");
        equipmentContainer = transform.Find("EquipmentContainer");
        itemSlotContainer = inventoryContainer.Find("ItemSlotContainer");
        itemSlotTemplate = itemSlotContainer.Find("ItemSlotTemplate");
        
        //Sub to opening the inventory
        UnitActionManager.Instance.RequestInventory += UnitActionManager_RequestInventory;
        //Sub player spawning
        Unit.OnAnyUnitSpawned += Unit_OnPlayerUnitSpawn;
    }

    private void Unit_OnPlayerUnitSpawn(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;
        //Debug.Log("Inventory from: " + unit.name);
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
            OnOpenInventory?.Invoke(this, EventArgs.Empty);
            RefreshInventory();
        }
        
        inventoryContainer.gameObject.SetActive(state);
        equipmentContainer.gameObject.SetActive(state);
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
            itemSlot.itemType = item.itemType;
            itemSlot.ClickFunc = () =>
            {
                Debug.Log("Left Click!!");
                //Use item
                Debug.Log(item.itemType);
                if (item.itemType == Item.ItemType.RedPotion || item.itemType == Item.ItemType.YellowPotion)
                {
                    inventory.UseItem(item);
                    inventory.RemoveItem(item);
                }
            };
            itemSlot.RightClickFunc = () =>
            {
                inventory.RemoveItem(item);
                DropItem(item);
                Destroy(itemSlot.gameObject);
            };
            itemSlot.ToInventoryFunc = () =>
            {
                //Unequip item;
                inventory.Unequip(item);
                inventory.AddItem(item);
                Destroy(itemSlot.gameObject);
            };
            itemSlot.OnEquipFunc = () =>
            {
                inventory.UseItem(item);
                inventory.RemoveItem(item);
            };

            x++;
            if (x > 2)
            {
                x = 0;
                y--;
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
        itemWorld.GetComponentInParent<Rigidbody>().AddForce(randomDirection * 5f, ForceMode.Impulse);

        return itemWorld;
    }
    
}
