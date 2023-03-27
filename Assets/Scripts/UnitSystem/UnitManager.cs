using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }
    public EventHandler SelectedUnitChanged;
    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitLayer;

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

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (TryHandleUnitSelection()) { return; }
            //Get valid grid position
            selectedUnit.TakeAction("Move", Mouse.GetPosition());
        }
    }

    private bool TryHandleUnitSelection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayer))
        {
            if(raycastHit.collider.TryGetComponent<Unit>(out Unit _unit))
            {
                SetSelectedUnit(_unit);
                return true;
            }
        }
        return false;
    }

    private void SetSelectedUnit(Unit _unit)
    {
        selectedUnit = _unit;
        SelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }
}
