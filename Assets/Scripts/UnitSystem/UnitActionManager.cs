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

    private BaseAction selectedAction;
    private bool isRunning = false;
    
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
            //Detect if mouse is over Loot
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000, LayerMask.GetMask("Loot")))
            {
                ItemWorld worldItem = hit.collider.GetComponent<ItemWorld>();
                selectedUnit.GetInventory().AddItem(worldItem.GetItem());
                Destroy(worldItem.gameObject);
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

public class InventoryEventArgs : EventArgs
{
    public Unit unit;
    
    public InventoryEventArgs(Unit _unit)
    {
        unit = _unit;
    }
}
