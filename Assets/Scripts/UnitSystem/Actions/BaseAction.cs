using System;
using System.Collections.Generic;using UnityEngine;

public abstract class BaseAction : ScriptableObject
{
   protected bool isActive = false;
   protected Unit unit;
   protected UnitData unitData;
   protected Action onActionComplete;
   protected int actionCost = 1;

   public virtual void SetUnit(Unit _unit)
   {
      unit = _unit;
      unitData = unit.GetUnitData();
   }

   public virtual BaseAction Clone()
   {
      return Instantiate(this);
   }
   
   public abstract void TakeAction(GridPosition _targetPosition, Action _onActionComplete);

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

   public bool IsActive()
   {
      return isActive;
   }

   public abstract void Update();
   
   protected void ActionStart(Action _onActiomComplete)
   {
      isActive = true;
      onActionComplete = _onActiomComplete;
   }

   protected void ActionComplete()
   {
      isActive = false;
      onActionComplete();
   }
}
