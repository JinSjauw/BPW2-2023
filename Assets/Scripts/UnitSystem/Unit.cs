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
    
    [SerializeField] private UnitData unitData;
    
    //private Dictionary<string, BaseAction> actionList;
    private void Awake()
    {
        actionArray = GetComponents<BaseAction>();
        //Temp adding actions manually
        /*MoveAction moveAction = new MoveAction();
        AddAction(moveAction.GetActionName(), moveAction);

        TargetAction targetAction = new TargetAction();
        AddAction(targetAction.GetActionName(), targetAction);*/
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
}
