using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MeleeAction : BaseAction
{
    public event EventHandler OnMelee;
    public float delay = .4f;

    private float timer;
    private Unit targetUnit;
    private int damage = 40;
    
    public override BaseAction Clone()
    {
        MeleeAction meleeAction = Instantiate(this);
        return meleeAction;
    }
    
    public override void TakeAction(GridPosition _targetPosition, Action _onActionComplete, Action _onActionFail)
    {
        timer = delay;
        damage = unit.GetUnitData().meleeDamage;
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(_targetPosition);
        if (unit.IsEnemy())
        {
            Debug.Log($"Unit: {unit.name} : Distance " + unit.GetGridPosition().Distance(targetUnit.GetGridPosition()));
            
            if (Vector3.Distance(unit.transform.position, targetUnit.transform.position) > unitData.attackRange)
            {
                targetUnit = null;
                _onActionFail();

                return;
            }
        }

        ActionStart(_onActionComplete);
        OnMelee?.Invoke(this, EventArgs.Empty);
    }

    public override List<GridPosition> GetValidActionPositionsList()
    {
        //Show available Grid positions for melee attack
        List<GridPosition> validPositions = new List<GridPosition>();
        List<GridPosition> tempPositions = new List<GridPosition>();
        tempPositions = LevelGrid.Instance.GetTilesInCircle(unit.transform.position, unitData.attackRange);
        
        if(tempPositions.Count <= 0)
        {
            Debug.Log("No valid targets");
            return null;
        }

        if (unit.IsEnemy())
        {
            if (tempPositions.Contains(targetUnit.GetGridPosition()))
            {
                return tempPositions;
            }
        }

        foreach (GridPosition position in tempPositions)
        {
            Unit target = LevelGrid.Instance.GetUnitAtGridPosition(position);
            if (target == null)
            {
                continue;
            }
            
            if (target.IsEnemy())
            {
                validPositions.Add(position);
            }
        }

        return validPositions;
    }

    public override string GetActionName()
    {
        return "Melee";
    }

    public override void Update()
    {
        if (!isActive)
        {
            return;
        }

        if (targetUnit == null)
        {
            return;
        }
        
        Vector3 direction = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
        unit.transform.rotation = Quaternion.RotateTowards(unit.transform.rotation, 
            Quaternion.LookRotation(direction), Time.deltaTime * unitData.rotateSpeed);

        timer -= Time.deltaTime;

        if (timer < 0)
        {
            targetUnit.Damage(damage);
            
            ActionComplete();
        }
    }
}
