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

    public float attackRange;
}

public class Unit : MonoBehaviour
{
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;
    public static event EventHandler OnAnyActionPointsChanged;
    
    private GridPosition gridPosition;
    private BaseAction[] actionArray;
    private HealthSystem healthSystem;

    [SerializeField] private bool isEnemy;
    [SerializeField] private int maxActionPoints = 3;
    [SerializeField] private int actionPoints = 3;
    [SerializeField] private BehaviourTree BTree;
    [SerializeField] private UnitData unitData;
    
    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        if (!isEnemy)
        {
            actionArray = GetComponents<BaseAction>();
        }
        else
        {
            BTree = BTree.Clone();
            BTree.Bind(this);
        }
        unitData.moveDistance = unitData.moveSpeed * actionPoints + .5f;
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetUnitAtGridObject(gridPosition, this);
        LevelGrid.Instance.SetUnitAtGridPosition(gridPosition, this);

        healthSystem.OnDeath += HealthSystem_OnDeath;
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        
        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
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
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        Debug.Log($"Spent {_amount} AP : {actionPoints} AP left");
    }

    private void TurnSystem_OnTurnChanged(object _sender, EventArgs e)
    {
        if (IsEnemy() && !TurnSystem.Instance.IsPlayerTurn() ||
            !IsEnemy() && TurnSystem.Instance.IsPlayerTurn())
        {
            actionPoints = maxActionPoints;
        
            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public BehaviourNode.State RunTree()
    {
        return BTree.Update();
    }
    
    public void Damage(int _amount)
    {
        healthSystem.Damage(_amount);
    }
    
    public bool IsEnemy()
    {
        return isEnemy;
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

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public Vector3 GetWorldPosition()
    {
        return new Vector3(gridPosition.x * 2, 0, gridPosition.z * 2);
    }

    private void HealthSystem_OnDeath(object _sender, EventArgs _e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition);
        Destroy(gameObject);
        Debug.Log("Dead");
        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }
}
