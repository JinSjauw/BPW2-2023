using UnityEngine;

public class TargetAction : BaseAction
{
    private Unit targetUnit; 
    
    public override void SetUnit(Unit _unit, object _obj)
    {
        unit = _unit;
        targetUnit = (Unit)_obj;
    }

    public override void Execute()
    {
        Debug.Log($" {unit} Spotted target! {targetUnit} location at {targetUnit.transform.position}!");
        UnitData unitData = unit.GetUnitData();
        unitData.isActive = false;
    }
}