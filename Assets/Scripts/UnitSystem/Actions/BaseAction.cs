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

   public EnemyAIAction GetBestAIAction()
   {
      List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>();

      List<GridPosition> validPositionsList = GetValidActionPositionsList();

      foreach (var gridPosition in validPositionsList)
      {
         EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPosition);
         enemyAIActionList.Add(enemyAIAction);
      }

      if (enemyAIActionList.Count > 0)
      {
         enemyAIActionList.Sort((EnemyAIAction a, EnemyAIAction b)=> b.actionValue - a.actionValue );
         return enemyAIActionList[0];
      }
      else
      {
         return null;
      }
   }

   public abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition);

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
