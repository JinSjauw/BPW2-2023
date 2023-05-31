using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlotUI : MonoBehaviour, IDropHandler
{

    [SerializeField] private Item.ItemType slotType;
    
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            RectTransform draggedObject = eventData.pointerDrag.GetComponent<RectTransform>();
            RectTransform equipmentSlot = GetComponent<RectTransform>();

            ItemSlotUI itemSlotUI = draggedObject.GetComponent<ItemSlotUI>();
            if (itemSlotUI.itemType == slotType)
            {
                draggedObject.position = equipmentSlot.position;
                draggedObject.SetParent(draggedObject.transform.parent.parent);
                itemSlotUI.OnEquipFunc();
            }
            else
            {
                itemSlotUI.Reject();
            }
        }
    }
    
    
}
