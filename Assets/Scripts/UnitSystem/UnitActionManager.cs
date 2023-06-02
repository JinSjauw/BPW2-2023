using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UnitActionManager : MonoBehaviour
{
    public static UnitActionManager Instance { get; private set; }
    
    public EventHandler OnPlayerSpawn;
    public EventHandler SelectedActionChanged;
    public EventHandler OnActionStarted;
    public EventHandler OnActionComplete;
    public EventHandler RequestInventory;

    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitLayer;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float interactRadius;
    
    private BaseAction selectedAction;
    private bool isRunning = false;
    private bool CanAct = false;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("More than one instance of UnitActionManager");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (selectedUnit != null)
        {
            SetSelectedAction(selectedUnit.GetDefaultAction());   
        }

        GridSystemVisual.OnVisualActive += GridSystemVisual_OnVisualActive;
        GridSystemVisual.OnVisualOff += GridSystemVisual_OnVisualOff;
    }

    private void GridSystemVisual_OnVisualOff(object sender, EventArgs e)
    {
        CanAct = false;
    }

    private void GridSystemVisual_OnVisualActive(object sender, EventArgs e)
    {
        CanAct = true;
    }

    private void Update()
    {
        //Check for input for inventory
        if (Input.GetKeyDown(KeyCode.I))
        {
            //Invoke inventory event and pass the selected unit in the eventArgs
            OpenInventory();
        }

        if (isRunning)
        {
            if (selectedAction != null && selectedAction.IsActive())
            {
                selectedAction.Update();
            }
            
            return;
        }

        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }
        
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        
        HandleInput();
        
    }

    private void OpenInventory()
    {
        Debug.Log("Opening Inventory!");
        RequestInventory?.Invoke(this, EventArgs.Empty);
    }
    
    private void HandleInput()
    {
        if (Input.GetMouseButton(0))
        {
            PickUpItem();

            if (!CanAct)
            {
                return;
            }
            
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(Mouse.GetPosition());

            if (!selectedAction.IsValidActionGridPosition(mouseGridPosition))
            {
               return;
            }
            if (!selectedUnit.TryTakeAction(selectedAction))
            {
                return;
            }
            SetRunning();
            selectedAction.TakeAction(mouseGridPosition, ClearRunning, null);
        }
    }

    private void PickUpItem()
    {
        //Detect if mouse is over Loot
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000, LayerMask.GetMask("Loot")))
        {
            //Detect if mouse position is close enough to the player;
            Vector3 itemPosition = hit.collider.transform.position;
            Vector3 unitPosition = selectedUnit.transform.position;
            float distance = Vector2.Distance(new Vector2(itemPosition.x, itemPosition.z), 
                new Vector2(unitPosition.x, unitPosition.z));
        
            Debug.Log(distance);
            if (distance > interactRadius)
            {
                return;
            }
            
            
            ItemWorld worldItem = hit.collider.GetComponentInParent<ItemWorld>();
            Debug.Log(selectedUnit.name + " :" + selectedUnit.GetInventory());
            selectedUnit.GetInventory().AddItem(worldItem.GetItem());
            Destroy(worldItem.transform.gameObject);
        }
    }
    
    private void SetRunning()
    {
        isRunning = true;
        OnActionStarted?.Invoke(this, EventArgs.Empty);
    }

    private void ClearRunning()
    {
        isRunning = false;
        OnActionComplete?.Invoke(this, EventArgs.Empty);
    }

    public void SetSelectedAction(BaseAction _action)
    {
        selectedAction = _action;
        SelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }

    public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }

    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }

    public void SetSelectedUnit(Unit _unit)
    {
        selectedUnit = _unit;
        SetSelectedAction(selectedUnit.GetDefaultAction());
        cameraTransform.position = selectedUnit.transform.position;
        OnPlayerSpawn?.Invoke(this, EventArgs.Empty);
    }
}
