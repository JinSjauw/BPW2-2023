using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Action ClickFunc;
    public Action RightClickFunc;
    public Action ToInventoryFunc;
    public Action OnEquipFunc;
    public Item.ItemType itemType;
    
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 lastPosition;

    private void Awake()
    {
        canvas = transform.root.GetComponent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && !eventData.dragging)
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
        lastPosition = rectTransform.position;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
        GetComponent<Image>().enabled = false;
        //DragBeginFunc();
    }

    public void OnDrag(PointerEventData eventData)
    {
        //DraggingFunc();
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        //DragEndFunc();
        List<RaycastResult> raycastHits = new List<RaycastResult>();
        List<RaycastResult> ignoreHits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastHits);
        for (int i = 0; i < raycastHits.Count; i++)
        {
            if (raycastHits[i].gameObject.GetComponent<EquipmentSlotUI>() != null ||
                raycastHits[i].gameObject.GetComponent<ItemSlotContainerUI>() != null ||
                raycastHits[i].gameObject.GetComponent<ItemSlotUI>())
            {
                ignoreHits.Add(raycastHits[i]);
            }
        }

        if (ignoreHits.Count < 1)
        {
            //Drop back to inventory;
            Debug.Log("Invalid Spot");
            rectTransform.position = lastPosition;
        }
        else
        {
            Debug.Log(ignoreHits[0].gameObject.name);
        }
        
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
        GetComponent<Image>().enabled = true;
    }

    public void Reject()
    {
        rectTransform.position = lastPosition;
    }
    
}
