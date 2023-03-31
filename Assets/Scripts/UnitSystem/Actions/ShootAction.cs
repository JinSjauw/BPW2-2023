using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    
    public override void TakeAction(GridPosition _position, Action _onActionComplete)
    {
        onActionComplete = _onActionComplete;
        isActive = true;
    }

    public override List<GridPosition> GetValidActionPositionsList()
    {
        List<GridPosition> validPositions = new List<GridPosition>();
        List<GridPosition> tempPositions = new List<GridPosition>();
        tempPositions = LevelGrid.Instance.GetTilesInCircle(transform.position, unitData.moveDistance);
        
        foreach (GridPosition position in tempPositions)
        {
            if (!LevelGrid.Instance.HasAnyUnit(position))
            {
                validPositions.Add(position);
            }
        }

        return validPositions;
    }
    
    public override string GetActionName()
    {
        return "Shoot";
    }
    
    private void Update()
    {
        if (!isActive)
        {
            return;
        }
        
        if (unit == null)
        {
            Debug.Log($"MoveAction unit is null {this}");
        }

        /*if (distance > unitData.stoppingDistance)
        {
            isActive = true;
            isExecuting = true;
            unitData.unitAnimator.SetBool(moveType, true);
            unit.transform.position += moveDirection * unitData.moveSpeed * Time.deltaTime;
            unit.transform.rotation = Quaternion.RotateTowards(unit.transform.rotation,
                Quaternion.LookRotation(moveDirection), Time.deltaTime * unitData.rotateSpeed);
        }
        else
        {
            isActive = false;
            isExecuting = false;
            onActionComplete();
            unitData.unitAnimator.SetBool(moveType, false);
        }*/
    }
}
