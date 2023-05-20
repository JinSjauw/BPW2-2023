﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class MoveAction : BaseAction
{
    private Vector3 targetPosition;
    private Pathfinding pathfinding;
    private int moveIndex;
    [SerializeField] private List<Vector3> path;

    public event EventHandler OnMove;
    public event EventHandler OnStop; 

    protected override void Awake()
    {
        base.Awake();
        targetPosition = transform.position;
    }

    public override void TakeAction(GridPosition _targetPosition, Action _onActionComplete)
    {
        if (pathfinding == null)
        {
            pathfinding = new Pathfinding();
        }

        moveIndex = 0;
        path = pathfinding.GetPath(LevelGrid.Instance.GetGridPosition(transform.position),
           _targetPosition);
        OnMove?.Invoke(this, EventArgs.Empty);
        ActionStart(_onActionComplete);
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

        targetPosition = path[moveIndex];
        
        Vector3 moveDirection = (targetPosition - unit.transform.position).normalized;
        float distance = Vector3.Distance(targetPosition, unit.transform.position);

        if (distance > unitData.stoppingDistance)
        {
            unit.transform.position += moveDirection * unitData.moveSpeed * Time.deltaTime;
            unit.transform.rotation = Quaternion.RotateTowards(unit.transform.rotation,
                Quaternion.LookRotation(moveDirection), Time.deltaTime * unitData.rotateSpeed);
        }
        else if(distance <= unitData.stoppingDistance)
        {
            moveIndex++;
        }

        if (moveIndex >= path.Count)
        {
            OnStop?.Invoke(this, EventArgs.Empty);
            ActionComplete();
        }
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition _gridPosition)
    {
        int targetCount = unit.GetShootAction().GetTargetCountAtPosition(_gridPosition);
        
        return new EnemyAIAction
        {
            gridPosition = _gridPosition,
            actionValue = targetCount * 10,
        };
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