using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class MoveAction : BaseAction
{
    private Vector3 targetPosition;
    private int moveIndex;
    private List<Vector3> path = new List<Vector3>();

    public event EventHandler OnMove;
    public event EventHandler OnStop;
    
    public override BaseAction Clone()
    {
        MoveAction moveAction = Instantiate(this);
        return moveAction;
    }

    public override void TakeAction(GridPosition _targetPosition, Action _onActionComplete, Action _onActionFail)
    {
        List<GridPosition> validPositionsList = GetValidActionPositionsList();
        GridPosition destination = _targetPosition;
        
        path.Clear();
        unit.pathfinding.SetGrid(validPositionsList);

        if (!validPositionsList.Contains(_targetPosition))
        {
            //Get closest node to the target position
            GridPosition closest = unit.GetGridPosition();
            foreach (var position in validPositionsList)
            {
                if (LevelGrid.Instance.GetGridObject(position).HasUnit())
                {
                    continue;
                }
                
                if (position.Distance(_targetPosition) < closest.Distance(_targetPosition))
                {
                    closest = position;
                }
            }
            
            destination = closest;
        }
        moveIndex = 0;

        List<GridPosition> foundPath = new List<GridPosition>();
        foundPath = unit.pathfinding.FindPath(LevelGrid.Instance.GetGridObject((unit.transform.position)),
           LevelGrid.Instance.GetGridObject(destination));

        foreach (var gridPosition in foundPath)
        {
            path.Add(LevelGrid.Instance.GetWorldPosition(gridPosition));
        }

        if (unit.IsEnemy())
        {
            path.RemoveAt(path.Count - 1);
        }
        
        OnMove?.Invoke(this, EventArgs.Empty);
        ActionStart(_onActionComplete);
    }

    public override List<GridPosition> GetValidActionPositionsList()
    {
        List<GridPosition> validPositions = new List<GridPosition>();
        List<GridPosition> tempPositions = new List<GridPosition>();
        List<GridPosition> inCirclePosition = new List<GridPosition>();
        //tempPositions = LevelGrid.Instance.GetWalkableTilesInCircle(unit.transform.position, unitData.moveDistance);
        
        tempPositions = unit.pathfinding.FindPossiblePaths(LevelGrid.Instance.GetWalkableList(),
            LevelGrid.Instance.GetGridPosition(unit.transform.position), unitData.moveDistance);

        foreach (var tile in tempPositions)
        {
            if (LevelGrid.Instance.insideCircle(unit.transform.position, 
                    LevelGrid.Instance.GetWorldPosition(tile), unitData.moveDistance))
            {
                inCirclePosition.Add(tile);
            }
        }

        //Debug.Log(tempPositions.Count);
        
        foreach (GridPosition position in inCirclePosition)
        {
            if (!LevelGrid.Instance.HasAnyUnit(position))
            {
                validPositions.Add(position);
            }
            else if (unit.IsEnemy() && !LevelGrid.Instance.GetUnitAtGridPosition(position).IsEnemy())
            {
                validPositions.Add(position);
            }
        }

        return validPositions;
    }

    public override void Update()
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
            //unit.UpdateMoveDistance();
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
