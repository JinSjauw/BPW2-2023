using System;
using System.Collections.Generic;
using UnityEngine;

public class TargetAction : BaseAction
{
    private Unit targetUnit;

    protected override void Awake()
    {
        base.Awake();
        targetUnit = null;
    }
    
    /*public override void SetUnit(Unit _unit, object _obj)
    {
        unit = _unit;
        //targetUnit = (Unit)_obj;
        targetUnit = (Unit)LevelGrid.Instance.GetUnitAtGridPosition(LevelGrid.Instance.GetGridPosition((Vector3)_obj));
    }*/

    public override void TakeAction(GridPosition _position, Action _onActionComplete)
    {
        Debug.Log($"{GetActionName()} : ActionTaken!");
        isActive = true;
    }

    public override List<GridPosition> GetValidActionPositionsList()
    {
        return null;
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }
        
        if (targetUnit == null)
        {
            Debug.Log("No Unit Spotted!");
            isActive = false;
        }
        else
        {
            Debug.Log($" {unit} Spotted target! {targetUnit} location at {targetUnit.transform.position}!");
            isActive = false;
        }
        //unitData.isActive = false;
    }
    
    public override string GetActionName()
    {
        return "Target";
    }
}