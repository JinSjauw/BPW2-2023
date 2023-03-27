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
    public bool isActive;
}

public class Unit : MonoBehaviour
{
    private GridPosition gridPosition;
    
    [SerializeField] private UnitData unitData;

    private Dictionary<string, BaseAction> actionList;
    private BaseAction currentAction;

    private Inventory inventory;
    
    private void Awake() {
        actionList = new Dictionary<string, BaseAction>();
        inventory = new Inventory();
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetUnitAtGridObject(gridPosition, this);
        LevelGrid.Instance.SetUnitAtGridPosition(gridPosition, this);

        //Temp adding actions manually
        MoveAction moveAction = new MoveAction();
        AddAction("Move", moveAction);

        TargetAction targetAction = new TargetAction();
        AddAction("Target", targetAction);
    }

    private void Update()
    {
        //Execute Action
        if(unitData.isActive)
        {
          currentAction.Execute();
        }
        
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
    
    //Move Action
    public void TakeAction(string _actionName, object _obj)
    {
        if (actionList.ContainsKey(_actionName) && !unitData.isActive)
        {
            unitData.isActive = true;
            currentAction = actionList[_actionName];
            currentAction.SetUnit(this, _obj);
        }
        else
        {
            Debug.Log($"Unit {this.name} does not have {_actionName} action available");
        }
    }
    
    public void AddAction(string _actionName, BaseAction _action)
    {
        actionList.Add(_actionName, _action);
    }
}
