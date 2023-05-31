using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class UnitData
{
    public float moveSpeed;
    public float rotateSpeed;
    public float stoppingDistance; 
    public float moveDistance;
    public float attackRange;
    public int meleeDamage;
    public int rangeDamage;
    public int protection;
}

public class Unit : MonoBehaviour
{
    
    public enum UnitState
    {
        IDLE,
        COMBAT,
    }
    
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;
    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitAlert;
    public event EventHandler EquippedWeapon;
    public event EventHandler UnequippedWeapon;
    public event EventHandler isHit;
    public Pathfinding pathfinding;

    private GridPosition gridPosition;
    private HealthSystem healthSystem;
    private UnitState unitState;
    private Inventory inventory;
    
    [SerializeField] private BaseAction[] actionArray;
    [SerializeField] private bool isEnemy;
    [SerializeField] private int maxActionPoints = 3;
    [SerializeField] private int actionPoints = 3;
    [SerializeField] private BehaviourTree BTree;
    [SerializeField] private UnitData unitData;
    
    [SerializeField] private Transform sword2h;
    [SerializeField] private Transform helmet;
    [SerializeField] private Transform[] chest;
    [SerializeField] private Transform[] pants;
    
    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        pathfinding = new Pathfinding();
        
        foreach (var action in actionArray)
        {
            action.SetUnit(this);
        }
        if(isEnemy)
        {
            BTree = BTree.Clone(this);
            BTree.Bind(this);
        }

        unitState = UnitState.IDLE;
    }

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        EnemyManager.OnCombatEnd += EnemyManager_OnCombatEnd;
        healthSystem.OnDeath += HealthSystem_OnDeath;
        
        if (!isEnemy)
        {
            inventory = new Inventory(UseItem, UnequipItem);
            Debug.Log("Inventory: " + inventory + "from: " + gameObject.name);
        }
        
        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
        
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

    private void UseItem(Item _item)
    {
        switch (_item.itemType)
        {
            case(Item.ItemType.Sword):
                sword2h.gameObject.SetActive(true);
                unitData.meleeDamage += 60;
                EquippedWeapon?.Invoke(this, EventArgs.Empty);
                break;
            case(Item.ItemType.Helmet):
                helmet.gameObject.SetActive(true);
                unitData.protection += 5;
                break;
            case(Item.ItemType.Chest):
                foreach (Transform part in chest)
                {
                    part.gameObject.SetActive(true);
                }
                unitData.protection += 10;
                break;
            case(Item.ItemType.Pants):
                foreach (Transform part in pants)
                {
                    part.gameObject.SetActive(true);
                }
                unitData.protection += 8;
                break;
            case(Item.ItemType.RedPotion):
                healthSystem.Heal(45);
                break;
            case(Item.ItemType.YellowPotion):
                actionPoints += 1;
                OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
                break;
        }
    }

    private void UnequipItem(Item _item)
    {
        switch (_item.itemType)
        {
            case(Item.ItemType.Sword):
                sword2h.gameObject.SetActive(false);
                unitData.meleeDamage -= 60;
                UnequippedWeapon?.Invoke(this, EventArgs.Empty);
                break;
            case(Item.ItemType.Helmet):
                helmet.gameObject.SetActive(false);
                unitData.protection -= 5;
                break;
            case(Item.ItemType.Chest):
                foreach (Transform part in chest)
                {
                    part.gameObject.SetActive(false);
                }
                unitData.protection -= 10;
                break;
            case(Item.ItemType.Pants):
                foreach (Transform part in pants)
                {
                    part.gameObject.SetActive(false);
                }
                unitData.protection -= 8;
                break;
            default:
                Debug.Log("Invalid Item");
                break;
        }
    }
    
    private void EnemyManager_OnCombatEnd(object sender, EventArgs e)
    {
        actionPoints = maxActionPoints;
        unitState = UnitState.IDLE;
    }
    
    private void HealthSystem_OnDeath(object _sender, EventArgs _e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition);
        gameObject.SetActive(false);
        Debug.Log("Dead");
        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }
    
    private void TurnSystem_OnTurnChanged(object _sender, EventArgs e)
    {
        if (IsEnemy() && !TurnSystem.Instance.IsPlayerTurn() ||
            !IsEnemy() && TurnSystem.Instance.IsPlayerTurn())
        {
            actionPoints = maxActionPoints;
            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
            //UpdateMoveDistance();
        }
    }

    public Inventory GetInventory()
    {
        return inventory;
    }
    
    public BehaviourTree GetTree()
    {
        return BTree;
    }
    
    public BehaviourNode.BehaviourState RunTree()
    {
        return BTree.Update();
    }

    public void SetTreeState(BehaviourNode.BehaviourState behaviourState)
    {
        BTree.rootNode.behaviourState = behaviourState;
    }
    
    public void Damage(int _amount)
    {
        isHit?.Invoke(this, EventArgs.Empty);
        _amount -= unitData.protection;
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

    public BaseAction GetDefaultAction()
    {
        return actionArray[0];
    }

    public bool TryTakeAction(BaseAction _action)
    {
        if (unitState == UnitState.IDLE)
        {
            return true;
        }
        
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

    public void SpendActionPoints(int _amount)
    {
        actionPoints -= _amount;
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        Debug.Log($"Spent {_amount} AP : {actionPoints} AP left");
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

    public void Alert()
    {
        if (unitState == UnitState.IDLE)
        {
            OnAnyUnitAlert?.Invoke(this, EventArgs.Empty);
        }
    }

    public UnitState GetState()
    {
        return unitState;
    }
    
    public void SetState(UnitState _state)
    {
        unitState = _state;
    }
}
