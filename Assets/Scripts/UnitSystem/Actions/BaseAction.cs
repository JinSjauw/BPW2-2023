using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
   protected bool isActive = false;
   protected Unit unit;
   protected Action onActionComplete;

   protected virtual void Awake()
   {
      unit = GetComponent<Unit>();
   }
   //public abstract void SetUnit(Unit _unit, object _obj);
   //public abstract void Execute();
   public abstract void TakeAction(GridPosition _position, Action _onActionComplete);

   public virtual bool IsValidActionGridPosition(GridPosition _gridPosition)
   {
      List<GridPosition> validGridPositions = GetValidActionPositionsList();
      return validGridPositions.Contains(_gridPosition);
   }
   
   public abstract List<GridPosition> GetValidActionPositionsList();
   public abstract string GetActionName();
}
