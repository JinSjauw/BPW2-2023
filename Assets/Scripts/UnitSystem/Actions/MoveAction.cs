using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    private Vector3 targetPosition;

    public event EventHandler OnMove;
    public event EventHandler OnStop; 

    protected override void Awake()
    {
        base.Awake();
        targetPosition = transform.position;
    }

    public override void TakeAction(GridPosition _position, Action _onActionComplete)
    {
        targetPosition = LevelGrid.Instance.GetWorldPosition(_position);
        ActionStart(_onActionComplete);
        OnMove?.Invoke(this, EventArgs.Empty);
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
        
        //Move & look at targetPosition
        Vector3 moveDirection = (targetPosition - unit.transform.position).normalized;
        float distance = Vector3.Distance(targetPosition, unit.transform.position);

        if (distance > unitData.stoppingDistance)
        {
            unit.transform.position += moveDirection * unitData.moveSpeed * Time.deltaTime;
            unit.transform.rotation = Quaternion.RotateTowards(unit.transform.rotation,
                Quaternion.LookRotation(moveDirection), Time.deltaTime * unitData.rotateSpeed);
        }
        else
        {
            OnStop?.Invoke(this, EventArgs.Empty);
            ActionComplete();
        }
    }

    public override int GetActionPointsCost()
    {
        return actionCost;
    }

    public override string GetActionName()
    {
        return "Move";
    }
}
