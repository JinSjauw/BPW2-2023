using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UnitActionManager : MonoBehaviour
{
    public static UnitActionManager Instance { get; private set; }
    public EventHandler SelectedActionChanged;
    public EventHandler OnActionStarted;
    public EventHandler OnActionComplete;

    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitLayer;

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
        SetSelectedAction(selectedUnit.GetDefaultAction());
    }

    private void Update()
    {
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

        HandleSelectedAction();
        
    }

    private void HandleSelectedAction()
    {
        if (Input.GetMouseButton(0))
        {
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
            selectedAction.TakeAction(mouseGridPosition, ClearRunning);
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
}
