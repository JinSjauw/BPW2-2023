using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Action ClickFunc;
    public Action RightClickFunc;
    public Action DragBeginFunc;
    public Action DraggingFunc;
    public Action DragEndFunc;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ClickFunc();
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            RightClickFunc();
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        DragBeginFunc();
    }

    public void OnDrag(PointerEventData eventData)
    {
        DraggingFunc();
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        DragEndFunc();
    }
    
}
