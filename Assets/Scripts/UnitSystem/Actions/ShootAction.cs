using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ShootAction : BaseAction
{
    
    public event EventHandler<OnShootEventArgs> OnShoot;

    public class OnShootEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit shootingUnit;
    }
    
    public enum State
    {
        Aiming,
        Shooting,
        CoolOff,
    }
    
    private State state;
    private float stateTimer;
    private bool canShoot = true;

    private Unit targetUnit;
    
   
    public override void Update()
    {
        if (!isActive)
        {
            return;
        }

        stateTimer -= Time.deltaTime;
        
        switch (state)
        {
            case State.Aiming:
                break;
            case State.Shooting:
                if (canShoot)
                {               
                    OnShoot?.Invoke(this, new OnShootEventArgs
                    {
                        targetUnit = targetUnit,
                        shootingUnit = unit
                    });
                    Shoot();
                    canShoot = false;
                }
                break;
            case State.CoolOff:
                break;
        }
        
        //Rotate towards target
        Vector3 direction = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
        unit.transform.rotation = Quaternion.RotateTowards(unit.transform.rotation, 
            Quaternion.LookRotation(direction), Time.deltaTime * unitData.rotateSpeed);

        
        if (stateTimer <= 0f)
        {
            NextState();
        }
    }

    private void NextState()
    {
        switch (state)
        {
            case State.Aiming:
                state = State.Shooting;
                float shootingStateTime = 0.2f;
                stateTimer = shootingStateTime;
                break;
            case State.Shooting:
                state = State.CoolOff;
                float coolOffStateTime = 0.4f;
                stateTimer = coolOffStateTime;
                break;
            case State.CoolOff:
                ActionComplete();
                break;
        }
        
        Debug.Log(state);
    }

    private void Shoot()
    {
        targetUnit.Damage(27);
    }

    public override void TakeAction(GridPosition _targetPosition, Action _onActionComplete)
    {
        ActionStart(_onActionComplete);

        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(_targetPosition);
        canShoot = true;
        Debug.Log("Aiming");
        state = State.Aiming;
        float aimingStateTime = 1f;
        stateTimer = aimingStateTime;
    
    }

    public override List<GridPosition> GetValidActionPositionsList()
    {
        Vector3 unitPosition = LevelGrid.Instance.GetWorldPosition(unit.GetGridPosition());
        return GetValidActionPositionsList(unitPosition);
    }
    
    public List<GridPosition> GetValidActionPositionsList(Vector3 _position)
    {
        List<GridPosition> validPositions = new List<GridPosition>();
        List<GridPosition> tempPositions = new List<GridPosition>();
        tempPositions = LevelGrid.Instance.GetTilesInCircle(_position, unitData.attackRange);
        
        foreach (GridPosition position in tempPositions)
        {
            if (position == LevelGrid.Instance.GetGridPosition(_position))
            {
                continue;
            }
            
            if (!LevelGrid.Instance.HasAnyUnit(position))
            {
                continue;
            }

            Unit target = LevelGrid.Instance.GetUnitAtGridPosition(position);
            
            if (target.IsEnemy() == unit.IsEnemy())
            {
                //Check is units are of the same type
                continue;
            }
            
            validPositions.Add(position);
        }

        return validPositions;
    }

    /*public override EnemyAIAction GetEnemyAIAction(GridPosition _gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = _gridPosition,
            actionValue = 100,
        };
    }*/

    public int GetTargetCountAtPosition(GridPosition _gridPosition)
    {
        Vector3 position = LevelGrid.Instance.GetWorldPosition(_gridPosition);
        return GetValidActionPositionsList(position).Count;
    }
    
    public override string GetActionName()
    {
        return "Shoot";
    }
}
