using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitData
{
    public Animator unitAnimator;
    public float moveSpeed;
    public float rotateSpeed;
    public float stoppingDistance; 
    public float moveDistance;
    //public bool isActive;
}

[RequireComponent(typeof(MoveAction))]
public class Unit : MonoBehaviour
{
    private GridPosition gridPosition;
    private BaseAction[] actionArray;
    [SerializeField] private int actionPoints = 3;
    
    [SerializeField] private UnitData unitData;
    
    private void Awake()
    {
        actionArray = GetComponents<BaseAction>();
        unitData.moveDistance = unitData.moveSpeed * actionPoints + .5f;

    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetUnitAtGridObject(gridPosition, this);
        LevelGrid.Instance.SetUnitAtGridPosition(gridPosition, this);
    }

    private void Update()
    {
        //Detect new GridPosition
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            //Changed grid position
            LevelGrid.Instance.UnitMovedGridPosition(this, gridPosition, newGridPosition);

            gridPosition = newGridPosition;
        }
    }

    private void SpendActionPoints(int _amount)
    {
        actionPoints -= _amount;
        Debug.Log($"Spent {_amount} AP : {actionPoints} AP left");
    }
    
    public UnitData GetUnitData()
    {
        return unitData;
    }

    public BaseAction[] GetActionArray()
    {
        return actionArray;
    }

    public MoveAction GetMoveAction()
    {
        return GetComponent<MoveAction>();
    }

    public bool TryTakeAction(BaseAction _action)
    {
        if (CanTakeAction(_action))
        {
            SpendActionPoints(_action.GetActionPointsCost());
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool CanTakeAction(BaseAction _action)
    {
        if (actionPoints >= _action.GetActionPointsCost())
        {
            return true;
        }
        
        return false;
    }

    public int GetActionPoints()
    {
        return actionPoints;
    }
    
}
