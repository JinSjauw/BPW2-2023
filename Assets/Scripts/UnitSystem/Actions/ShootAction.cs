using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu]
public class ShootAction : BaseAction
{
    public event EventHandler OnShoot;
    
    [SerializeField] private Transform projectilePrefab;
    
    private Unit targetUnit;
    [SerializeField] private float delay;
    private float timer;
    private int damage;
    
    private void Shoot()
    {
        Vector3 unitPosition = unit.transform.position;
        Vector3 projectileOrigin = new Vector3(unitPosition.x, unitPosition.y + 1.5f, unitPosition.z);
        Transform projectileTransform = Instantiate(projectilePrefab, projectileOrigin, Quaternion.identity);
        Projectile projectile = projectileTransform.GetComponent<Projectile>();

        Vector3 shootAtTarget = targetUnit.GetWorldPosition();
        shootAtTarget.y += 1.5f;
       
        projectile.Init(targetUnit, shootAtTarget, damage);
    }

    public void Unsubscribe()
    {
        OnShoot = null;
    }

    public override void TakeAction(GridPosition _targetPosition, Action _onActionComplete, Action _onActionFail)
    {
        timer = delay;
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(_targetPosition);
        damage = unit.GetUnitData().rangeDamage;

        if (unit.IsEnemy())
        {
            if (Vector3.Distance(unit.transform.position, targetUnit.transform.position) > unitData.attackRange)
            {
                targetUnit = null;
                _onActionFail();

                return;
            }
        }
        OnShoot?.Invoke(this, EventArgs.Empty);
        ActionStart(_onActionComplete);
    }

    public override List<GridPosition> GetValidActionPositionsList()
    {
        Vector3 unitPosition = LevelGrid.Instance.GetWorldPosition(unit.GetGridPosition());
        
        List<GridPosition> validPositions = new List<GridPosition>();
        List<GridPosition> tempPositions = new List<GridPosition>();
        tempPositions = LevelGrid.Instance.GetTilesInCircle(unitPosition, unitData.attackRange);

        if (tempPositions.Count <= 0)
        {
            return null;
        }
        
        foreach (GridPosition position in tempPositions)
        {
            if (position == LevelGrid.Instance.GetGridPosition(unitPosition))
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

    public override void Update()
    {
        if (!isActive)
        {
            return;
        }

        //Rotate towards target
        Vector3 direction = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
        unit.transform.rotation = Quaternion.RotateTowards(unit.transform.rotation, 
            Quaternion.LookRotation(direction), Time.deltaTime * unitData.rotateSpeed);
        
        //Delay 
        timer -= Time.deltaTime;

        //Shoot projectile
        if (timer < 0)
        {
            timer = delay;
            Shoot();
            ActionComplete();
        }
        

    }
    
    public override string GetActionName()
    {
        return "Shoot";
    }
}
