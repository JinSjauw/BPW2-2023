using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }
    public EventHandler SelectedUnitChanged;
    public EventHandler SelectedActionChanged;
    public EventHandler OnActionStarted;
    public EventHandler OnActionComplete;

    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitLayer;

    private BaseAction selectedAction;
    private bool isBusy = false;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("More than one instance of UnitManager");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        SetSelectedUnit(selectedUnit);
    }

    private void Update()
    {
        if (isBusy)
        {
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
        
        if (TryHandleUnitSelection())
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
            SetBusy();
            selectedAction.TakeAction(mouseGridPosition, ClearBusy);
        }
    }
    
    private void SetBusy()
    {
        isBusy = true;
        OnActionStarted?.Invoke(this, EventArgs.Empty);
    }

    private void ClearBusy()
    {
        isBusy = false;
        OnActionComplete?.Invoke(this, EventArgs.Empty);
    }
    
    private bool TryHandleUnitSelection()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayer))
            {
                if(raycastHit.collider.TryGetComponent<Unit>(out Unit _unit))
                {
                    if (_unit == selectedUnit)
                    {
                        return false;
                    }

                    if (_unit.IsEnemy())
                    {
                        return false;
                    }
                    
                    SetSelectedUnit(_unit);
                    return true;
                }
            }
        }
        return false;
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
    
    private void SetSelectedUnit(Unit _unit)
    {
        selectedUnit = _unit;
        SetSelectedAction(selectedUnit.GetMoveAction());
        SelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }
}
