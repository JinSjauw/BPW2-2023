using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlotUI : MonoBehaviour, IDropHandler
{

    [SerializeField] private Item.ItemType slotType;
    private ItemSlotUI itemInSlot = null;
    
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            RectTransform draggedObject = eventData.pointerDrag.GetComponent<RectTransform>();
            RectTransform equipmentSlot = GetComponent<RectTransform>();

            ItemSlotUI itemSlotUI = draggedObject.GetComponent<ItemSlotUI>();
            if (itemSlotUI.itemType == slotType && itemInSlot == null)
            {
                draggedObject.position = equipmentSlot.position;
                draggedObject.SetParent(draggedObject.transform.parent.parent);
                itemInSlot = itemSlotUI;
                itemInSlot.OnEquipFunc();
            }
            else if(itemInSlot != null && itemSlotUI.itemType == slotType)
            {
                Debug.Log("Switching Equipment!");
                itemInSlot.ToInventoryFunc();
                itemSlotUI.OnEquipFunc();
                itemInSlot = itemSlotUI;
            }
            else
            {
                itemSlotUI.Reject();
            }
        }
    }
}
