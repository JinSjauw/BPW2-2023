using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlotContainerUI : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            RectTransform draggedObject = eventData.pointerDrag.GetComponent<RectTransform>();
            ItemSlotUI itemSlotUI = draggedObject.GetComponent<ItemSlotUI>();
            if (itemSlotUI.transform.parent != transform)
            {
                itemSlotUI.ToInventoryFunc();
            }
            else
            {
                itemSlotUI.Reject();
            }
        }
    }
}
