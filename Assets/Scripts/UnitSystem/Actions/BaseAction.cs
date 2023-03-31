using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
   protected bool isActive = false;
   protected Unit unit;
   protected UnitData unitData;
   protected Action onActionComplete;
   protected int actionCost = 1;
   protected virtual void Awake()
   {
      unit = GetComponent<Unit>();
      unitData = unit.GetUnitData();
   }
   public abstract void TakeAction(GridPosition _position, Action _onActionComplete);

   public virtual bool IsValidActionGridPosition(GridPosition _gridPosition)
   {
      List<GridPosition> validGridPositions = GetValidActionPositionsList();
      return validGridPositions.Contains(_gridPosition);
   }
   
   public abstract List<GridPosition> GetValidActionPositionsList();

   public virtual int GetActionPointsCost()
   {
      return actionCost;
   }
   public abstract string GetActionName();
}
